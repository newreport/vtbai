
namespace Model
{
    public class ConfigModel
    {
        private const string ConfigPath = "data/config/config.toml";

        private const string SensitivePath = "data/config/sensitive_words.txt";

        public ConfigModel()
        {
            SensitiveWords = FileHelper.ReadAllLines(SensitivePath).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
            SensitiveWordsPY = new List<string>();
            foreach (var s in SensitiveWords)
            {
                SensitiveWordsPY.Add(PinyinHelper.Hanzi2Pinyin(s));
            }
        }
        public static VtbaiConfig VTBAIConfig { get; private set; } = new();

        public static LiveConfig LIVEConfig { get; private set; } = new();

        public static GptConfig GPTConfig { get; private set; } = new();

        public static TtsConfig TTSConfig { get; private set; } = new();


        public static List<string> SensitiveWords { get; private set; } = new();
        public static List<string> SensitiveWordsPY { get; private set; } = new();




        public void RefreshConfig()
        {
            VTBAIConfig = new();
            LIVEConfig = new();
            GPTConfig = new();
            TTSConfig = new();
            SensitiveWords = FileHelper.ReadAllLines(SensitivePath).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
            SensitiveWordsPY = new List<string>();
            foreach (var s in SensitiveWords)
            {
                SensitiveWordsPY.Add(PinyinHelper.Hanzi2Pinyin(s));
            }
        }



        public class VtbaiConfig
        {

            public VtbaiConfig()
            {
                Env = INIHelper.Read("vtbai", "env", "dev", ConfigModel.ConfigPath);
                ApiListen = INIHelper.Read("vtbai", "env", "dev", ConfigModel.ConfigPath);
                LivePlatform = INIHelper.Read("vtbai", "env", "dev", ConfigModel.ConfigPath);
                GptPlatform = INIHelper.Read("vtbai", "env", "dev", ConfigModel.ConfigPath);
                TtsPlatform = INIHelper.Read("vtbai", "env", "dev", ConfigModel.ConfigPath);
            }

            public string Env { get; private set; }
            /// <summary>
            /// 监听端口
            /// </summary>
            public string ApiListen { get; private set; }
            /// <summary>
            /// 直播平台
            /// </summary>
            public string LivePlatform { get; private set; }
            /// <summary>
            /// GPT 平台
            /// </summary>
            public string GptPlatform { get; private set; }
            /// <summary>
            /// TTS 平台
            /// </summary>
            public string TtsPlatform { get; private set; }

        }

        public class LiveConfig
        {
            public LiveConfig()
            {
                BiliRoomid = INIHelper.Read("live", "bili_roomid", "207640", ConfigModel.ConfigPath);
                BiliTopid = INIHelper.Read("live", "bili_topid", "1,2,207640", ConfigModel.ConfigPath);
            }

            public string BiliRoomid { get; private set; }
            public string BiliTopid { get; private set; }
        }

        public class GptConfig
        {
            public GptConfig()
            {
                OpenaiKey = INIHelper.Read("gpt", "openai_key", "your key", ConfigModel.ConfigPath);
                OpenaiNya1 = INIHelper.Read("gpt", "openai_nya1", "你是猫娘,you are in live,我你主人", ConfigModel.ConfigPath);
                OpenaiProxyDomain = INIHelper.Read("gpt", "openai_key", " https://api.openai.com/v1", ConfigModel.ConfigPath);
                OpenaiMaxContext = int.Parse(INIHelper.Read("gpt", "openai_proxy_domain", "3", ConfigModel.ConfigPath));
                //gpt-3.5-turbo | gpt-4 | gpt-4-32k
                OpenaiModel = INIHelper.Read("gpt", "openai_model", "gpt-3.5-turbo", ConfigModel.ConfigPath);

            }

            public string OpenaiKey { get; private set; }

            public string OpenaiNya1 { get; private set; }

            public string OpenaiProxyDomain { get; private set; }

            public int OpenaiMaxContext { get; private set; }

            public string OpenaiModel { get; private set; }
        }

        public class TtsConfig
        {

            public TtsConfig()
            {
                MaxTextLength = int.Parse(INIHelper.Read("tts", "max_text_length", "50", ConfigModel.ConfigPath));
                MaxTtsLength = int.Parse(INIHelper.Read("tts", "max_tts_length", "200", ConfigModel.ConfigPath));
                TextIntervalMs = int.Parse(INIHelper.Read("tts", "text_interval_ms", "100", ConfigModel.ConfigPath));
                MaxWavQueue = int.Parse(INIHelper.Read("tts", "max_wav_queue", "5", ConfigModel.ConfigPath));
                AutoDelWav = INIHelper.Read("tts", "auto_del_wav", "0", ConfigModel.ConfigPath) == "1";

                MoegoeModelOnxx = INIHelper.Read("tts", "moegoe_model_onnx", ".data/models/model.onnx", ConfigModel.ConfigPath);
                MoegoeModelConfig = INIHelper.Read("tts", "moegoe_model_config", ".data/models/config.json", ConfigModel.ConfigPath);
                MoegoeModelPth = INIHelper.Read("tts", "moegoe_model_pth", ".data/models/model.pth", ConfigModel.ConfigPath);
                MoegoeLengthScale = decimal.Parse(INIHelper.Read("tts", "moegoe_length_scale", "1", ConfigModel.ConfigPath));
                MoegoeNoiseScale = decimal.Parse(INIHelper.Read("tts", "moegoe_noise_scale", "0.667", ConfigModel.ConfigPath));
                MoegoeNoiseScaleW = decimal.Parse(INIHelper.Read("tts", "moegoe_noise_scale_w", "0.8", ConfigModel.ConfigPath));
                MoegoeSpeakerId = int.Parse(INIHelper.Read("tts", "moegoe_speaker_id", "0", ConfigModel.ConfigPath));
            }
            public int MaxTextLength { get; private set; }

            public int MaxTtsLength { get; private set; }
            public int TextIntervalMs { get; private set; }
            public int MaxWavQueue { get; private set; }
            public int IntervalMs { get; private set; }

            public bool AutoDelWav { get; private set; }

            #region MoeGoe

            public string MoegoeModelOnxx { get; private set; }
            public string MoegoeModelConfig { get; private set; }
            public string MoegoeModelPth { get; private set; }
            public decimal MoegoeLengthScale { get; private set; }
            public decimal MoegoeNoiseScale { get; private set; }
            public decimal MoegoeNoiseScaleW { get; private set; }
            public int MoegoeSpeakerId { get; private set; }
            #endregion
        }

    }

}
