
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTS
{
    public class TTS
    {
        protected bool running = true;
        protected readonly bool _isDev;
        protected readonly TtsConf _ttsConf;
        public TTS(TtsConf ttsConf, bool isDev)
        {
            running = true;
            _isDev = isDev;
            _ttsConf = ttsConf;
        }



        public string GetLatestQueue()
        {
            if (que.Count > 0)
            {
                return que.Dequeue();
            }
            return "";
        }


        /// <summary>
        ///根据调用该方法线程的时间进行排序
        /// </summary>
        public PriorityQueue<string, DateTime> que = new();

        #region 配置


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
        #endregion
    }
}
