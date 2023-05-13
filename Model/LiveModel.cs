using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class LiveModel
    {
        /// <summary>
        /// 付费金额 付费/免费队列
        /// </summary>
        public decimal PayMoney { get; set; }

        public MessageType Type { get; set; }
        /// <summary>
        /// 权重
        /// </summary>
        public int Weigth { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// id
        /// </summary>
        public string Uid { get; set; }
    }



    public enum MessageType
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
        Guard

    }
}
