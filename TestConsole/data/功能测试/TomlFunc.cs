using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tomlyn;

namespace TestConsole.data.功能测试
{
    internal class TomlFunc
    {
        public TomlFunc() {



            var toml = @"
global = ""this is a string""
# This is a comment of a table
[my_table]
key = 1 # Comment a key
value = true
list = [1,2,3]
[a]
    [a.b1]
    b2 = 3333
";

            var model = Toml.ToModel<MyModel>(toml);
            // Prints "this is a string"
            Console.WriteLine($"found global = \"{model.Global}\"");
            // Prints 1
            var key = model.MyTable!.Key;
            Console.WriteLine($"found key = {key}");
            var value = model.MyTable!.Value;
            Console.WriteLine($"found value = {value}");
            Console.WriteLine($"found value = {model.A.B1.B2}");
            Console.WriteLine($"found value = {string.Join(",", model.MyTable.List2)}");
        }
    }

// Check list
//var list = model.MyTable!.List;
//Console.WriteLine($"found list = {string.Join(", ", list)}");

// Simple model that maps the TOML string above
class MyModel
    {
        public string? Global { get; set; }

        public MyTable? MyTable { get; set; }

        public A? A { get; set; }
    }

    class A
    {
        public B? B1 { get; set; }

        public class B
        {
            public string B2 { get; set; }
        }

    }


    class MyTable
    {
        public MyTable()
        {
            //List2 = new List<int>();
        }

        public int Key { get; set; }

        public bool Value { get; set; }

        // The type can be an interface if it is pre-instantiated by this instance.
        //[DataMember(Name = "list")]
        //public IList<int> List { get; }

        [DataMember(Name = "list")]
        public List<int> List2 { get; set; }

        [IgnoreDataMember]
        public string? ThisPropertyIsIgnored { get; set; }
    }


}
