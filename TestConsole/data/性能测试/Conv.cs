
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole.data.性能测试
{
    internal class Conv : BasePerformanceTest
    {
        public Conv()
        {
            _watch.Restart();
            //ConversionHelper.StringToByte(base._str);
            _watch.Stop();
            Console.WriteLine(_watch.Elapsed);

            _watch.Restart();
            //ConversionHelper.strToToHexByte(base._str);
            _watch.Stop();
            Console.WriteLine(_watch.Elapsed);
        }
    }
}
