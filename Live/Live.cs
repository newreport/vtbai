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

        protected bool _isDev;

        protected LiveConf _liveConf;

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
            TopQue = new();
            if (_liveConf.ApiQueueLength > 0) ApiQue = new(_liveConf.ApiQueueLength);
            else ApiQue = new();
            if (_liveConf.PayQueueLength > 0) PayQue = new(_liveConf.PayQueueLength);
            else PayQue = new();
            FreeQue = new(_liveConf.FreeQueueLength);
            #endregion
        }


        #region 消息队列 messgae
        /// <summary>
        /// 出队
        /// </summary>
        /// <returns></returns>
        public LiveModel? GetLatestQueue()
        {
            if (TopQue.Count > 0) return TopQue.Dequeue();
            if (ApiQue.Count > 0) return ApiQue.Dequeue();
            //if (PayQue.Count > 0) return PayQue.Dequeue();
            return null;
        }

        public Queue<LiveModel> TopQue;
        /// <summary>
        /// API 队列
        /// </summary>
        public Queue<LiveModel> ApiQue;
        /// <summary>
        /// 付费队列
        /// </summary>
        public PriorityQueue<int, LiveModel> PayQue;
        /// <summary>
        /// 免费队列
        /// </summary>
        public PriorityQueue<int, LiveModel> FreeQue;
        /// <summary>
        /// 清理满载队列
        /// </summary>
        protected void CleanFullQueue()
        {
            if (FreeQue.Count >= _liveConf.FreeQueueLength) FreeQue.Dequeue();
            if (_liveConf.PayQueueLength != -1 && PayQue.Count >= _liveConf.PayQueueLength) PayQue.Dequeue();
            if (_liveConf.ApiQueueLength != -1 && ApiQue.Count >= _liveConf.ApiQueueLength) ApiQue.Dequeue();
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
            /// 权重
            /// </summary>
            public int Weigth { get; set; }
            /// <summary>
            /// 动作
            /// </summary>
            public string Action { get; set; }
            /// <summary>
            /// 礼物名称 舰长/提督/总督
            /// </summary>
            public string GiftName { get; set; }
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
