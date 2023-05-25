using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GPT
{
    public class GPT
    {

        protected bool running = false;

        protected readonly bool _isDev;

        protected readonly GptConf _gptConf;

        public GPT(GptConf gptConf, bool isDev)
        {
            running = true;
            _isDev = isDev;
            _gptConf = gptConf;
        }

        /// <summary>
        /// 以调用方时间作为排序记录
        /// </summary>
        protected PriorityQueue<(string, string), DateTime> qaQueue = new();

        public (string, string)? GetLatestQueue()
        {
            if (qaQueue.Count > 0) return qaQueue.Dequeue();
            return null;
        }

        #region 配置 setting
        public class GptConf
        {
            public string Platform { get; set; }

            public ChatgptConf Chatgpt { get; set; }

            public Glm6bConf Glm6b { get; set; }

            public OtherConf Other { get; set; }
            public class ChatgptConf
            {
                public string key { get; set; }

                public string ProxyDomain { get; set; }

                public int MaxContext { get; set; }

                public string Model { get; set; }

                public string System { get; set; }
                public string User { get; set; }
                public string Assistant { get; set; }
                public string FirstAsk { get; set; }
            }

            public class Glm6bConf
            {

            }

            public class OtherConf
            {
                public string TransmitUrl { get; set; }

                [DataMember(Name = "q_name")]
                public string QName { get; set; }
            }
        }

        #endregion
    }
}
