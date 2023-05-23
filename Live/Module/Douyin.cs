using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Live.Module
{
    public class Douyin : Live, ILive
    {
        public Douyin(LiveConf liveConf, bool isDev) : base(liveConf, isDev)
        {
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task Initialization()
        {
            throw new NotImplementedException();
        }

        void IDisposable.Dispose()
        {
            throw new NotImplementedException();
        }

        Task ILive.Initialization()
        {
            throw new NotImplementedException();
        }

        void ILive.InQueue(JObject obj)
        {
            throw new NotImplementedException();
        }
    }
}
