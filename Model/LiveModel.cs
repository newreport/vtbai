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
        public string NickName { get;set; }
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
}
