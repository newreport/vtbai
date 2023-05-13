using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole.data.性能测试
{
    internal class PinyinPer:BasePerformanceTest
    {
        public PinyinPer()
        {
            _watch.Restart();
            var str1 = PinyinHelper.Hanzi2Pinyin(_str);
            Console.WriteLine(str1);
            _watch.Stop();
            Console.WriteLine(_watch.Elapsed);


            Console.WriteLine(_str.Length);
        }
    }
}
