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
        private readonly string _str = "《地火》讲述的是矿工之子刘欣为改变传统、落后的煤炭工业而进行的一场气化煤实验，憧憬无限美好，结局却悲恸天地。《地火》是一部典型的煤矿题材作，国内 大 带宽 服务器,爆品大促,物理服务器6折,云服务器2.5折,跨境CN2 GIA+500M大带宽不限流,BGP丰富带宽. 国内 大 带宽 服务器,7年品牌IDC服务商,香港美国新加坡";
    }
}
