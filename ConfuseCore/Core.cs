using Live;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfuseCore
{
    public class Core
    {
        ILive _live;
        public Core()
        {
            var livePlatform = GModel.Conf.Live.Platform;

            Log.WriteLine("---------- start init live platform ----------");
            switch (livePlatform)
            {
                case "bili":
                    _live = new Bili(GModel.Conf.Live.Bili.Roomid);
                    break;
                case "douyin":
                    _live = new Bili(GModel.Conf.Live.Bili.Roomid);
                    break;
                default:
                    _live = new Bili(GModel.Conf.Live.Bili.Roomid);
                    break;
            }

            Log.WriteLine("---------- end init live platform ----------");

        }

    }
}
