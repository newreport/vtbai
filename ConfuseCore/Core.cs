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
        string _livePlatform = GModel.Conf.Live.Platform;

        bool runing = false;

        public Core()
        {

            runing = true;
            #region live

            Log.WriteLine("---------- start init live platform ----------");
            switch (_livePlatform)
            {
                case "bili":
                    _live = new Bili(GModel.Conf.Live, GModel.IsDev);
                    break;
                case "douyin":
                    _live = new Bili(GModel.Conf.Live, GModel.IsDev);
                    break;
                default:
                    _live = new Bili(GModel.Conf.Live, GModel.IsDev);
                    break;
            }
            _live.Initialization().Wait();
            Log.WriteLine("---------- end init live platform ----------");
            #endregion
        }


        public void Dispose()
        {
            _live.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
