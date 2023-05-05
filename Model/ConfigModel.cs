
namespace Model
{
    public class ConfigModel
    {
        private const string ConfigPath = "data/config/config.ini";

        private const string SensitivePath = "data/config/sensitive_words.txt";

        public ConfigModel() {
            SensitiveWords = new List<string>();
            SensitiveWordsPY = new List<string>();

        }

        public static List<string> SensitiveWords { get; private set; }

        public static List<string> SensitiveWordsPY { get; private set; }

     


        public void EditConfig()
        {
            VtbaiConfig vtbaiConfig = new();

        }

        public class VtbaiConfig
        {

            public VtbaiConfig()
            {
                Env = INIHelper.Read("vtbai", "env", "dev", ConfigPath);
                ApiListen = INIHelper.Read("vtbai", "env", "dev", ConfigPath);
                LivePlatform = INIHelper.Read("vtbai", "env", "dev", ConfigPath);
                GptPlatform = INIHelper.Read("vtbai", "env", "dev", ConfigPath);
                TtsPlatform = INIHelper.Read("vtbai", "env", "dev", ConfigPath);
            }

            public string Env { get; internal set; }
            /// <summary>
            /// 监听端口
            /// </summary>
            public string ApiListen { get; internal set; }
            /// <summary>
            /// 直播平台
            /// </summary>
            public string LivePlatform { get; internal set; }
            /// <summary>
            /// GPT 平台
            /// </summary>
            public string GptPlatform { get; internal set; }
            /// <summary>
            /// TTS 平台
            /// </summary>
            public string TtsPlatform { get; internal set; }

        }

        public class LiveConfig
        {
            public LiveConfig()
            {
                BiliRoomid = INIHelper.Read("live", "bili_roomid", "207640", ConfigPath);
                BiliTopid = INIHelper.Read("live", "bili_topid", "1,2,207640", ConfigPath);
            }

            public string BiliRoomid { get; internal set; }
            public string BiliTopid { get; internal set; }
        }

        public class GptConfig
        {
            public GptConfig()
            {
                OpenaiKey = INIHelper.Read("gpt", "openai_key", "your key", ConfigPath);
                OpenaiNya1 = INIHelper.Read("gpt", "openai_nya1", "你是猫娘,you are in live,我你主人", ConfigPath);
                OpenaiProxyDomain = INIHelper.Read("gpt", "openai_key", " https://api.openai.com/v1", ConfigPath);
                OpenaiMaxContext =int.Parse( INIHelper.Read("gpt", "openai_proxy_domain", "3", ConfigPath));
                //gpt-3.5-turbo | gpt-4 | gpt-4-32k
                OpenaiModel = INIHelper.Read("gpt", "openai_model", "gpt-3.5-turbo", ConfigPath);

            }

            public string OpenaiKey { get; internal set; }

            public string OpenaiNya1 { get; internal set; }

            public string OpenaiProxyDomain { get; internal set; }

            public int OpenaiMaxContext { get; internal set; }

            public string OpenaiModel { get; internal set; }
        }

    }
}
