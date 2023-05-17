using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Live
{
    public class Live
    {

        public Live()
        {
            //最优先队列

            TopQue = new();
            if (GModel.Conf.Live.ApiQueueLength > 0) ApiQue = new(GModel.Conf.Live.ApiQueueLength);
            else ApiQue = new();

            if (GModel.Conf.Live.PayQueueLength > 0) ApiQue = new(GModel.Conf.Live.PayQueueLength);
            else PayQue = new();

            FreeQue = new(GModel.Conf.Live.FreeQueueLength);

        }
        public LiveModel GetLatestQueue()
        {
            if (TopQue.Count > 0) return TopQue.Dequeue();
            if (ApiQue.Count > 0) return ApiQue.Dequeue();
            //if (PayQue.Count > 0) return PayQue.Dequeue();
            return ApiQue.Dequeue();
        }


        public static Queue<LiveModel> TopQue;
        /// <summary>
        /// API 队列
        /// </summary>
        public static Queue<LiveModel> ApiQue;
        /// <summary>
        /// 付费队列
        /// </summary>
        public static PriorityQueue<int, LiveModel> PayQue;
        /// <summary>
        /// 免费队列
        /// </summary>
        public static PriorityQueue<int, LiveModel> FreeQue;


    }
}
