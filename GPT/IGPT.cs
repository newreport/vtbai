using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPT
{
    public interface IGPT : IDisposable
    {

        protected void InQueue(string quesition);

        public (string, string)? GetLatestQueue();
        Task Initialization();
    }
}
