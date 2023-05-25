using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace CommonHelper
{
    public static class PinyinHelper
    {

        /// <summary>
        /// 汉字转拼音
        /// Chinese characters to pinyin
        /// </summary>
        /// <param name="hans"></param>
        /// <returns></returns>
        public static string Hanzi2Pinyin(string hans) => TinyPinyin.PinyinHelper.GetPinyin(hans, "");

        /// <summary>
        /// 是否汉字
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsChinese(char c) => TinyPinyin.PinyinHelper.IsChinese(c);

    }
}
