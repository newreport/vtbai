
using ConfuseCore;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using TestConsole.data.功能测试;
using TestConsole.data.性能测试;

//Conv c= new Conv(); 
string str = "{\"cmd\":\"INTERACT_WORD\",\"data\":{\"contribution\":{\"grade\":0},\"core_user_type\":0,\"dmscore\":18,\"fans_medal\":{\"anchor_roomid\":47867,\"guard_level\":0,\"icon_id\":0,\"is_lighted\":0,\"medal_color\":12478086,\"medal_color_border\":12632256,\"medal_color_end\":12632256,\"medal_color_start\":12632256,\"medal_level\":16,\"medal_name\":\"C酱\",\"score\":128751,\"special\":\"\",\"target_id\":67141},\"identities\":[1],\"is_spread\":0,\"msg_type\":1,\"privilege_type\":0,\"roomid\":47867,\"score\":1684311242720,\"spread_desc\":\"\",\"spread_info\":\"\",\"tail_icon\":0,\"timestamp\":1684311242,\"trigger_time\":1684311241649906200,\"uid\":281311118,\"uname\":\"被大叔吓死的熊哥\",\"uname_color\":\"\"}}";

JObject obj=JObject.Parse(str);
Console.WriteLine(obj["cmd"]);
Core core = new Core();

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