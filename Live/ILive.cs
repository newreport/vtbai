using Newtonsoft.Json.Linq;

namespace Live
{
    public interface ILive : IDisposable
    {
        protected void InQueue(JObject obj);

        Task Initialization();

    }
}
