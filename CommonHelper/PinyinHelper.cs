using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace CommonHelper
{
    public class PinyinHelper
    {

        /// <summary>
        /// 汉字转拼音
        /// Chinese characters to pinyin
        /// </summary>
        /// <param name="hans"></param>
        /// <returns></returns>
        public static string Hanzi2Pinyin(string hans)
        {
            return TinyPinyin.PinyinHelper.GetPinyin(hans, "");
        }

        public static bool IsChinese(char c)
        {
            return TinyPinyin.PinyinHelper.IsChinese(c);
        }
    }
}
