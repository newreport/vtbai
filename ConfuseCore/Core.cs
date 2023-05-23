using GPT;
using Live;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfuseCore
{
    public class Core : IDisposable
    {
        ILive _live;
        IGPT _gpt;


        bool runing = false;

        public async Task Start()
        {
            runing = true;
            //goto Tt;
            #region live
            Log.WriteLine("---------- start init live platform ----------");
            switch (GModel.Conf.Live.Platform)
            {
                case "bili":
                    _live = new Bili(GModel.Conf.Live, GModel.IsDev);
                    break;
                case "douyin":
                    _live = new Douyin(GModel.Conf.Live, GModel.IsDev);
                    break;
                default:
                    _live = new Bili(GModel.Conf.Live, GModel.IsDev);
                    break;
            }
            await _live.Initialization();
            Log.WriteLine("---------- end init live platform ----------");
        #endregion
        Tt:;
            #region gpt
            Log.WriteLine("---------- start init gpt platform ----------");
            switch (GModel.Conf.Gpt.Platform)
            {
                case "chatgpt":
                    _gpt = new Chatgpt(GModel.Conf.Gpt, GModel.IsDev);
                    break;
                default:
                    break;
            }
            await _gpt.Initialization();
            Log.WriteLine("---------- end init gpt platform ----------");
            #endregion

        }

        public async Task Stop()
        {
            _live.Dispose();
            runing = false;
        }


        public void Dispose()
        {
            _live.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
