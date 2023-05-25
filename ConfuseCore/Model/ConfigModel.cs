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
            GModel.SensitiveWords = FileHelper.ReadAllLines(SensitivePath).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
            GModel.SensitiveWordsPY = new List<string>();
            foreach (var s in GModel.SensitiveWords)
            {
                GModel.SensitiveWordsPY.Add(PinyinHelper.Hanzi2Pinyin(s));
            }
            #endregion


            return configToml;
        }

        public Live.Live.LiveConf Live { get; set; }

        public GPT.GPT.GptConf Gpt { get; set; }

        public TTS.TTS.TtsConf Tts { get; set; }

        public L2dConf L2d { get; set; }

        public AigcConf Aigc { get; set; }




        public class L2dConf
        {
            public string Platform { get; set; }
        }


        public class AigcConf
        {

        }
    }
}
