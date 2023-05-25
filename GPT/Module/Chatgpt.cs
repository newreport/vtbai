using OpenAI;
using OpenAI.Chat;
using OpenAI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using static GPT.GPT;

namespace GPT.Module
{
    public class Chatgpt : GPT, IGPT
    {
        public Chatgpt(GptConf gptConf, bool isDev) : base(gptConf, isDev)
        {


        }
        OpenAIClient _api;

        List<Message> baseMessages = new List<Message>();

        List<Message> contentMessages = new List<Message>();

        List<Message> sendMessgaes => baseMessages.Union(contentMessages).ToList();

        public async Task Initialization()
        {
            baseMessages.Clear();
            //https://github.com/RageAgainstThePixel/OpenAI-DotNet#openai-api-proxy
            _api = new OpenAIClient(_gptConf.Chatgpt.key);
            if (!string.IsNullOrWhiteSpace(_gptConf.Chatgpt.ProxyDomain))
            {
                var settings = new OpenAIClientSettings(domain: _gptConf.Chatgpt.ProxyDomain);
                _api = new OpenAIClient(_gptConf.Chatgpt.key, settings);
            }
            var models = await _api.ModelsEndpoint.GetModelsAsync();
            Log.WriteLine("chatgpt可用model列表", string.Join(",", models));
            baseMessages.Add(new Message(Role.System, _gptConf.Chatgpt.System));
            baseMessages.Add(new Message(Role.User, _gptConf.Chatgpt.User));
            baseMessages.Add(new Message(Role.Assistant, _gptConf.Chatgpt.Assistant));

            var fistMessages = new List<Message>();
            fistMessages.AddRange(baseMessages);
            fistMessages.Add(new Message(Role.User, _gptConf.Chatgpt.User));

            var chatRequest = new ChatRequest(fistMessages);
            var result = await _api.ChatEndpoint.GetCompletionAsync(chatRequest);
            Log.WriteLine("初始回复", $"{result.FirstChoice.Message.Role}: {result.FirstChoice.Message.Content}");
        }

        public  void InQueue(string qes)
        {
            DateTime dateTime = DateTime.Now;
            Task.Run(() =>
            {
                if (contentMessages.Count > _gptConf.Chatgpt.MaxContext) contentMessages.RemoveAt(0);
                contentMessages.Add(new Message(Role.User, _gptConf.Chatgpt.User));

                var chatRequest = new ChatRequest(sendMessgaes);
                var result = _api.ChatEndpoint.GetCompletionAsync(chatRequest).Result;
                qaQueue.Enqueue((qes, result.FirstChoice.Message.Content), dateTime);
            });
        }

        public void Dispose()
        {
            Log.WriteLine("释放chatgpt");
            GC.SuppressFinalize(this);
        }

    }
}
