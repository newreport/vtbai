using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Live
{
    public abstract class Live
    {
        protected bool running = false;

        protected readonly bool _isDev;

        protected readonly LiveConf _liveConf;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="liveConf">配置类</param>
        /// <param name="isDev">是否开发</param>
        public Live(LiveConf liveConf, bool isDev)
        {
            running = true;
            _isDev = isDev;
            _liveConf = liveConf;

            #region 初始化消息队列

            //最优先队列
            topQue = new();
            if (_liveConf.ApiQueueLength > 0) apiQue = new(_liveConf.ApiQueueLength);
            else apiQue = new();
            if (_liveConf.PayQueueLength > 0) payQue = new(_liveConf.PayQueueLength);
            else payQue = new();
            freeQue = new(_liveConf.FreeQueueLength);
            #endregion
        }


        #region 消息队列 messgae
        /// <summary>
        /// 出队
        /// </summary>
        /// <returns></returns>
        public LiveModel GetLatestQueue()
        {
            if (topQue.Count > 0) return topQue.Dequeue();
            if (apiQue.Count > 0) { return apiQue.Dequeue(); }
            if (payQue.Count > 0) { return payQue.Dequeue(); }
            if (freeQue.Count > 0) { return freeQue.Dequeue(); }
            return null;
        }



        protected Queue<LiveModel> topQue;
        /// <summary>
        /// API 队列
        /// </summary>
        protected Queue<LiveModel> apiQue;
        /// <summary>
        /// 付费队列
        /// </summary>
        protected PriorityQueue<LiveModel, int> payQue;
        /// <summary>
        /// 免费队列
        /// </summary>
        protected PriorityQueue<LiveModel, int> freeQue;
        /// <summary>
        /// 清理满载队列
        /// </summary>
        protected void CleanFullQueue()
        {
            if (freeQue.Count >= _liveConf.FreeQueueLength) freeQue.Dequeue();
            if (_liveConf.PayQueueLength != -1 && payQue.Count >= _liveConf.PayQueueLength) payQue.Dequeue();
            if (_liveConf.ApiQueueLength != -1 && apiQue.Count >= _liveConf.ApiQueueLength) apiQue.Dequeue();
        }

        public class LiveModel
        {
            /// <summary>
            /// 消息类型
            /// </summary>
            public LiveMessageType Type { get; set; }
            /// <summary>
            /// id
            /// </summary>
            public string Uid { get; set; }
            /// <summary>
            /// 昵称
            /// </summary>
            public string NickName { get; set; }
            /// <summary>
            /// 付费金额 付费/免费队列
            /// </summary>
            public decimal PayMoney { get; set; }

            /// <summary>
            /// 动作
            /// </summary>
            public string Action { get; set; }
            /// <summary>
            /// 礼物名称 
            /// </summary>
            public string GiftName { get; set; }
            /// <summary>
            /// 舰长类型
            /// </summary>
            public string GuardType { get; set; }
            /// <summary>
            /// 消息类型
            /// </summary>
            public string Message { get; set; }
        }

        public enum LiveMessageType
        {
            None,
            /// <summary>
            /// superchat   YTB | Bili
            /// </summary>
            SC,
            /// <summary>
            /// 礼物  All
            /// </summary>
            Gift,
            /// <summary>
            /// 舰长  Bili
            /// </summary>
            Guard,
            /// <summary>
            /// 弹幕
            /// </summary>
            Danmu,
        }
        #endregion

        #region 配置 setting

        public class LiveConf
        {
            public BiliConf Bili { get; set; }
            public DouyinConf Douyin { get; set; }

            public string Platform { get; set; }

            public int ApiQueueLength { get; set; }
            public int PayQueueLength { get; set; }
            public decimal PayThreshold { get; set; }
            public int FreeQueueLength { get; set; }

            public class BiliConf
            {
                public string Roomid { get; set; }
                public List<string> Topid { get; set; }

            }
            public class DouyinConf
            {
                public string Roomid { get; private set; }

            }
        }
        #endregion
    }
}
