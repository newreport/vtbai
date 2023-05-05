using CommonHelper;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Live
{
    public class Bili : ILive
    {

        private string _roomid { get; set; }

        async void ILive.Initialize(string roomid)
        {
            //获取真实ip
            var response = await HttpHelper.HttpGet("https://api.live.bilibili.com/room/v1/Room/get_info", new List<string>() { $"roomid={roomid}" });
            _roomid = response["data"]["room_id"].ToString();
        }
    }
}
