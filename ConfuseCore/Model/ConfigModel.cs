using Microsoft.VisualBasic;
using System.Runtime.Serialization;
using System.Xml.Linq;
using Tomlyn;

namespace ConfuseCore.Model
{
    public class ConfigModel
    {

        private const string ConfigPath = "data/config/config.toml";

        private const string SensitivePath = "data/config/sensitive_words.txt";

        public static List<string> SensitiveWords { get; private set; }
        public static List<string> SensitiveWordsPY { get; private set; }


        public string Env { get; set; }

        public string ApiListen { get; set; }

        internal static ConfigModel RefreshConfig()
        {
            var configToml = Toml.ToModel<ConfigModel>(FileHelper.ReadAllText(ConfigPath));
            #region 阈值设定
            if (configToml.Live.FreeQueueLength < 5) configToml.Live.FreeQueueLength = 5;
            if (configToml.Live.ApiQueueLength < 5 && configToml.Live.ApiQueueLength > 0) configToml.Live.ApiQueueLength = 5;
            if (configToml.Live.PayQueueLength < 5 && configToml.Live.PayQueueLength > 0) configToml.Live.PayQueueLength = 5;

            #endregion

            #region SensitiveWords敏感词
            SensitiveWords = FileHelper.ReadAllLines(SensitivePath).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
            SensitiveWordsPY = new List<string>();
            foreach (var s in SensitiveWords)
            {
                SensitiveWordsPY.Add(PinyinHelper.Hanzi2Pinyin(s));
            }
            #endregion


            return configToml;
        }

        public Live.Live.LiveConf Live { get; set; }

        public GptConf Gpt { get; set; }

        public TtsConf Tts { get; set; }

        public L2ddConf L2d { get; set; }

        public AigcConf Aigc { get; set; }


        public class GptConf
        {
            public string platform { get; set; }

            public OpenaiConf Openai { get; set; }

            public Glm6bConf Glm6b { get; set; }

            public OtherConf Other { get; set; }
            public class OpenaiConf
            {
                public string key { get; set; }

                public string Nya1 { get; set; }

                public string ProxyDomain { get; set; }

                public int MaxContext { get; set; }

                public string Model { get; set; }
            }

            public class Glm6bConf
            {

            }

            public class OtherConf
            {
                public string TransmitUrl { get; set; }

                [DataMember(Name = "q_name")]
                public string QName { get; set; }
            }
        }

        public class TtsConf
        {

            public string Platform { get; set; }

            public string TextIntervalMs { get; set; }

            public int MaxTextLength { get; set; }

            public int MaxTtsLength { get; set; }

            public int MaxWavQueue { get; set; }
            public bool AutoDelWav { get; set; }

            public MoegoeConf Moegoe { get; set; }

            public class MoegoeConf
            {
                public string ModelOnnx { get; set; }

                public string ModelConfig { get; set; }

                public string ModelPth { get; set; }

                public int SpeakerId { get; set; }

                public decimal LengthScale { get; set; }

                public decimal NoiseScale { get; set; }

                public decimal NoiseScaleW { get; set; }
            }
        }

        public class L2ddConf
        {
            public string Platform { get; set; }
        }

        public class AigcConf
        {

        }
    }
}
