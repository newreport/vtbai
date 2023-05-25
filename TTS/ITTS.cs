using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTS
{
    public interface ITTS : IDisposable
    {

        public void InQueue(string quesition);

        public string GetLatestQueue();

        Task Initialization();
    }
}
