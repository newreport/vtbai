
using System.Net.WebSockets;
using System.Threading;
using System;
using System.Diagnostics;
using System.Text;
using System.Net.NetworkInformation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Channels;
using System.Buffers.Text;
using System.Net;
using System.IO.Compression;

namespace Live
{
    public class Bili : Live, ILive
    {

        Connect connect = new Connect();
        ClientWebSocket _ws => connect.WS;

        public Bili()
        {
            #region 获取房间id和token
            var response = HttpHelper.HttpGet("https://api.live.bilibili.com/room/v1/Room/get_info", new Dictionary<string, string>() { { "room_id", GModel.Conf.Live.Bili.Roomid } });
            connect.Roomid = response["data"]!["room_id"]!.ToString();
            Log.WriteLine("bilibili 真实房间号", connect.Roomid);

            response = HttpHelper.HttpGet($"https://api.live.bilibili.com/room/v1/Danmu/getConf?room_id={connect.Roomid}&platform=pc&player=web");
            var wsDomain = response["data"]!["host"]!.ToString();
            //var ipAddress = Dns.GetHostAddresses(wsDomain);
            //var ip = ipAddress[new Random().Next(0, ipAddress.Length)];
            connect.WsUrl = new Uri($"wss://{wsDomain}/sub");
            Log.WriteLine("wobsocket 地址", connect.WsUrl.Host.ToString());
            connect.Token = response["data"]!["token"]!.ToString();
            #endregion


            #region websocket 认证 & 心跳
            _ws.ConnectAsync(connect.WsUrl, connect.Cancellation).Wait();
            var packetModel = new { uid = 0, roomid = connect.Roomid, protover = 3, platform = "web", type = 2, key = connect.Token, };
            var playload = JsonConvert.SerializeObject(packetModel);
            SendSocketDataAsync(7, playload, "发送认证包").Wait();
            var recAuth = new byte[1024];
            _ws.ReceiveAsync(recAuth, connect.Cancellation).Wait();
            recAuth = recAuth.Skip(connect.headLength).ToArray();
            Log.WriteLine("接收认证包", Encoding.UTF8.GetString(recAuth));

            _ = HeartbeatLoop();
            _ = ReceiveMessageLoop();
            #endregion
        }

        async Task ReceiveMessageLoop()
        {
            var stableBuffer = new byte[16];
            var buffer = new byte[4096];
            while (connect.Connected)
            {
                await _ws.ReceiveAsync(stableBuffer, connect.Cancellation);
                Log.WriteLine("信息流", Encoding.UTF8.GetString(stableBuffer));
            }
        }



        async Task HeartbeatLoop()
        {
            try
            {
                while (connect.Connected)
                {
                    //每30秒发送一次 心跳
                    await SendHeartbeatAsync();
                    await Task.Delay(30000);
                }
            }
            catch (Exception e)
            {
                Log.Error($"{e}");
                this.Dispose();
            }

            // 发送ping包
            async Task SendHeartbeatAsync() => await SendSocketDataAsync(2);
        }

        async Task SendSocketDataAsync(int action, string body = "", string remarks = "")
        {
            //头大小 头部长度 心跳包 认证包/心跳包 自增 消息体
            await SendSocketDataAsync(0, connect.headLength, 1, action, 1, body, remarks);
        }

        /// <summary>
        /// 发送消息到bilibili
        /// https://github.com/SocialSisterYi/bilibili-API-collect/blob/master/docs/live/message_stream.md
        /// </summary>
        /// <param name="packetLength">总长度</param>
        /// <param name="headerLength">头部长度</param>
        /// <param name="version">协议版本
        /// 协议版本:
        ///0普通包正文不使用压缩
        ///1心跳及认证包正文不使用压缩
        ///2普通包正文使用zlib压缩
        ///3普通包正文使用brotli压缩,解压为一个带头部的协议0普通包
        /// </param>
        /// <param name="action">操作码
        /// 2	心跳包
        /// 3	心跳包回复（人气值）
        /// 5	普通包（命令）
        /// 7	认证包
        /// 8	认证包回复
        /// </param>
        /// <param name="sequence">每次发包时向上递增</param>
        /// <param name="body">消息体</param>
        /// <returns></returns>
        async Task SendSocketDataAsync(int packetLength, short headerLength, short version, int action, int sequence = 1, string body = "", string remarks = "")
        {
            var playload = Encoding.UTF8.GetBytes(body);
            if (packetLength == 0) packetLength = playload.Length + 16;
            var buffer = new byte[packetLength];
            using (var ms = new MemoryStream(buffer))
            {
                var b = ConversionHelper.GetBytes(buffer.Length);

                await ms.WriteAsync(b, 0, 4);
                b = ConversionHelper.GetBytes(headerLength);
                await ms.WriteAsync(b, 0, 2);
                b = ConversionHelper.GetBytes(version);
                await ms.WriteAsync(b, 0, 2);
                b = ConversionHelper.GetBytes(action);
                await ms.WriteAsync(b, 0, 4);
                b = ConversionHelper.GetBytes(sequence);
                await ms.WriteAsync(b, 0, 4);
                if (playload.Length > 0)
                {
                    await ms.WriteAsync(playload, 0, playload.Length);
                }
                if (!string.IsNullOrWhiteSpace(remarks))
                {
                    Log.WriteLine(remarks, Encoding.UTF8.GetString(buffer.Skip(connect.headLength).ToArray()));
                }
                if (GModel.Conf.Env == "dev")
                {
                    await Console.Out.WriteLineAsync(Convert.ToBase64String(buffer));
                    await Console.Out.WriteLineAsync(ConversionHelper.ByteToHexS(buffer));
                }

                await _ws.SendAsync(buffer, WebSocketMessageType.Binary, true, connect.Cancellation);
            }
        }

        private class Connect
        {
            public Connect()
            {
                WS = new ClientWebSocket();
            }
            public ClientWebSocket WS { get; set; }
            public string Roomid { get; set; }
            public Uri WsUrl { get; set; }
            public string Token { get; set; }
            public short headLength => 16;

            public CancellationToken Cancellation = new CancellationToken();

            public bool Connected => WS != null && (WS.State == WebSocketState.Connecting || WS.State == WebSocketState.Open);

            /// <summary>
            /// 接收缓存区
            /// </summary>
            public byte[] RecTemp { get; set; }


        }



        public void Dispose()
        {
            _ws?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
