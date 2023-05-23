using Newtonsoft.Json.Linq;
using static Live.Live;

namespace Live
{
    public interface ILive : IDisposable
    {
        protected void InQueue(JObject obj);

        public LiveModel? GetLatestQueue();

        Task Initialization();

    }
}
