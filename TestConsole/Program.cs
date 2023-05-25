
using ConfuseCore;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using TestConsole.data.功能测试;
using TestConsole.data.性能测试;

//Conv c= new Conv(); 

Core core = new Core();
await core.Start();



//await Task.Delay(10000);
//await core.Stop();

//int a = 16;
//Console.WriteLine(a);
//var by = BitConverter.GetBytes(a);
//Console.WriteLine(by.Length);
//short b = 17;
//Console.WriteLine(b);
//var by2 = BitConverter.GetBytes(b);
//Console.WriteLine(by2.Length);
Console.ReadKey();


//Pinyin pinyin = new Pinyin();

//var str = FileHelper.ReadAllText("data/config/config.toml");
//////Console.WriteLine(str);
//var model = TomlHelper.ToModel(str)!;
//ConfigModel.RefreshConfig();
//var model = ConfigModel.NewM();
//Console.WriteLine(ConfigModel.ConfigToml.ApiListen);
//Console.WriteLine(ConfigModel.ConfigToml.Live.Platform);
//Console.WriteLine(ConfigModel.ConfigToml.Live.Douyin.Roomid);
//Console.WriteLine(ConfigModel.ConfigToml.Tts.Moegoe.ModelOnnx);
//Console.WriteLine(ConfigModel.ConfigToml.Tts.Moegoe.NoiseScaleW);
//Console.WriteLine(ConfigModel.ConfigToml.Tts.AutoDelWav);
//Console.WriteLine(cf.ApiListen);
//Console.WriteLine(ConfigModel.LiveConf.Douyin.Roomid);
//Console.WriteLine(model);
//Console.WriteLine(model["env"]);
//Console.WriteLine(model["live.bili"]);

////var tomlOut=Toml.FromModel(model);
////Console.WriteLine(tomlOut);

//TomlFunc tlf = new TomlFunc();
//PinyinPer pyp=new PinyinPer();  