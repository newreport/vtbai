using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonHelper
{
    public static class Log
    {
        public static void WriteLine(string str)
        {
            Console.WriteLine(DateTime.Now.ToString("hh:mm:ss:fff") + " " + str);
        }

        public static void WriteLine(string str1, string str2)
        {
            if (!string.IsNullOrWhiteSpace(str2)) Console.WriteLine(DateTime.Now.ToString("hh:mm:ss:fff") + " " + str1 + "  ::  " + str2);
            else WriteLine(str1);

        }

        public static void Error(string e)
        {
            Console.WriteLine(DateTime.Now.ToString("hh:mm:ss:fff") + " " + e);
        }

        public static void Error(string msg, string e)
        {
            if (!string.IsNullOrWhiteSpace(e)) Console.WriteLine(DateTime.Now.ToString("hh:mm:ss:fff") + " " + msg + "  ::  " + e);
            else Error(e);
        }
    }
}
