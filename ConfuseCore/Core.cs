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
        public Core()
        {
            var livePlatform = ConfigModel.ConfigToml.Live.Platform;

            Log.WriteLine("---------- start init live platform ----------");
            ILive live = null;
            switch (livePlatform)
            {
                case "bili":
                    live = new Bili();
                    break;
                case "douyin":
                    live = new Bili();
                    break;
                default:
                    live = new Bili();
                    break;
            }
            Log.WriteLine("---------- end init live platform ----------");

            Log.WriteLine("");

        }

    }
}
