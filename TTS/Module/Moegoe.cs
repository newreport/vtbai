using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TTS.Module
{
    public class Moegoe : TTS, ITTS
    {
        public Moegoe(TtsConf ttsConf, bool isDev) : base(ttsConf, isDev)
        {
        }

        public void Dispose()
        {
            Log.WriteLine("释放TTS Moegoe");
            GC.SuppressFinalize(this);
        }

        public async Task Initialization()
        {

            await Task.Delay(0);
        }

        public void InQueue(string quesition)
        {
            DateTime now = DateTime.Now;
            Task.Run(() =>
            {
                var fileName = Guid.NewGuid().ToString() + ".wav";

                que.Enqueue(fileName, now);
            });
        }


    }
}
