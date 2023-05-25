using GPT;
using Live;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTS;

namespace ConfuseCore
{
    public class Core : IDisposable
    {
        ILive _live;
        IGPT _gpt;
        ITTS _tts;


        bool runing = false;

        public async Task Start()
        {
            runing = true;
            await Init();
            Running();
        }

        private async void Running()
        {
            while (runing)
            {

                var qModel = _live.GetLatestQueue();
                if (qModel != null)
                {
                    string qusetion = qModel.NickName;
                    switch (qModel.Type)
                    {
                        case Live.Live.LiveMessageType.None:
                            break;
                        case Live.Live.LiveMessageType.SC:
                            qusetion += $"使用了SupperChat说{qModel.Message}";
                            break;
                        case Live.Live.LiveMessageType.Gift:
                            qusetion += qModel.Action + qModel.GiftName;
                            break;
                        case Live.Live.LiveMessageType.Guard:
                            qusetion += "上了" + qModel.GuardType;
                            break;
                        case Live.Live.LiveMessageType.Danmu:
                            qusetion += $"说{qModel.Message}";
                            break;
                        default:
                            break;
                    }
                    if (qusetion.Length > qModel.NickName.Length && VerifyWords(qusetion))
                    {

                        _gpt.InQueue(qusetion);
                    }
                }

                var qa = _gpt.GetLatestQueue();

                if (qa != null && VerifyWords(qa.Value.Item2))
                {
                    _tts.InQueue(qa.Value.Item1);
                    await Task.Delay(1);
                    _tts.InQueue(qa.Value.Item2);
                }
            }
        }

        //public (bool,Stream

        bool VerifyWords(string text)
        {
            var pinyin = PinyinHelper.Hanzi2Pinyin(text);
            foreach (var item in GModel.SensitiveWords)
            {
                if (text.Contains(item)) return false;
            }
            foreach (var item in GModel.SensitiveWordsPY)
            {
                if (item.Contains(pinyin)) return false;
            }
            return true;
        }

        async Task Init()
        {
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

            #region gpt
            Log.WriteLine("---------- start init gpt ----------");
            switch (GModel.Conf.Gpt.Platform)
            {
                case "chatgpt":
                    _gpt = new Chatgpt(GModel.Conf.Gpt, GModel.IsDev);
                    break;
                case "glm6b":
                    break;
                default:
                    break;
            }
            await _gpt.Initialization();
            Log.WriteLine("---------- end init gpt ----------");
            #endregion

            #region tts
            Log.WriteLine("---------- start init tts ----------");
            switch (GModel.Conf.Tts.Platform)
            {
                case "moegoe":

                    break;
                default:
                    break;
            }

            Log.WriteLine("---------- end init tts ----------");
            #endregion
        }


        public async Task Stop()
        {
            _live.Dispose();
            _gpt.Dispose();
            await Task.Delay(0);
            runing = false;
        }


        public void Dispose()
        {
            _live.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
