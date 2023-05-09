
using System.Net.WebSockets;
using System.Threading;
using System;
using System.Diagnostics;

namespace Live
{
    public class Bili : Live, ILive
    {

        private string _roomid { get; set; }
        private string _wsUrl { get; set; }
        public Bili()
        {
            var response = HttpHelper.HttpGet("https://api.live.bilibili.com/room/v1/Room/get_info", new Dictionary<string, string>() { { "room_id", ConfigModel.ConfigToml.Live.Bili.Roomid } });
            _roomid = response["data"]["room_id"].ToString();
            Log.WriteLine("bilibili 真实房间号  ::  " + _roomid);

            response = HttpHelper.HttpGet($"https://api.live.bilibili.com/room/v1/Danmu/getConf?room_id={_roomid}&platform=pc&player=web");
            //Console.WriteLine(response);
            _wsUrl = response["data"]["host_server_list"][0]["host"].ToString() + ":" + response["data"]["host_server_list"][0]["wss_port"].ToString()+"/sub";
            //_wsUrl = response["data"]["host"].ToString()+":"+ response["data"]["port"].ToString();
            Log.WriteLine("wobsocket 地址 ::  " + _wsUrl);

            //using SocketsHttpHandler handler = new();
            //using ClientWebSocket ws = new();
            //ws.ConnectAsync("wss://broadcastlv.chat.bilibili.com:2245/sub", new HttpMessageInvoker(handler), cancellationToken);
        }

        private ClientWebSocket _ws { get; set; }


        /// <summary>
        /// 心跳包
        /// </summary>
        private void Hearbet()
        {

            Task.Factory.StartNew(() =>
            {
                while (_ws != null && _ws.State == WebSocketState.Connecting)
                {

                    Thread.Sleep(30 * 1000);
                }
            });
        }

        public void Dispose()
        {
            _ws?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
