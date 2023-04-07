# Function in coding ....
- VTS API 关键字触发表情
- 语句情感推断触发 VTS 表情
- Azure API TTS 
- Link CharacterAI 角色扮演最好的 AI
- Multi link 联动
- ChatGLM-6B 本地1
- ChatterBot 本地2
- Twitch & Youtube 平台支持

# 资源
- [vits](https://github.com/jaywalnut310/vits) vits source
- [MoeGoe](https://github.com/CjangCjengh/MoeGoe.git) vits chinese
- [vits_with_chatgpt-gpt3](https://github.com/Paraworks/vits_with_chatgpt-gpt3) tts 推理参考
- [blivedm](https://github.com/xfgryujk/blivedm/tree/master) 抓取 b 站直播间信息
- [演示模型](https://huggingface.co/newreport/live_tts_default_model/tree/main) vits model (商用请自炼自然人同意的合法声源或用 Azure)

- # 架构
> 从哔哩哔哩直播间抓取弹幕和礼物，接收后发送给 openai 官方的 chatgpt，等待 gpt 回调消息后使用 vits 进行 tts 推理，然后根据关键字/VTS API 触发表情和 playsound 播放语音， 播放时 vts 根据声音匹配口型  

> 除非公司或大佬，非常不建议手搓 live2d，一是耗时，二是 vts 和 prprlive 配合关键字触发表情快捷键、VTS API 直播效果还可以  

>  本直播流程仅在 win 下测试并通过，理论 linux 和 mac 在合适的 py 环境中也能使用，py 版本为 conda 3.10.10

>  注：有能者可以同理把老鼠和油管的扩展了，py 线程协程进程鲨我

## 工作流
blivedm（抓直播间信息）——>openai（猫娘对话）——>vits（tts 文本转语音）——>vts（语音转口型，快捷键触发表情）——>obs(推流)

## 功能
- 支持 api 请求，扩展用，比如联动，语音转文字后请求即可  httt://127.0.0.1:8080?text=你好
- 支持优先级：api>舰长>礼物>SC>弹幕，礼物和弹幕队列容量默认为5/10，满了自动剔除时间线最后的，SC、舰长没有容量限制

## 我的开发环境
- CPU：5700G
- GPU：核显
- 主板：华硕 B550m TUF
- 硬盘：凯侠 RC20
- 内存：16*4（3133hz 金百达）
- 电源：台达 300W
- 机箱：X40 5.5L
  
> 主要速度慢在请求 openai 和 cpu 推理，用 gpu 会快很多，由于需要检测同音字敏感词以防爆房，故不能使用 SSE

> obs、vts、雀魂 AI 全开的情况，推理时 cpu 负载约为 70%，理论讲 2k 内预算的丐中丐 5600G 也可以跑，功耗不超过 100W，ITX 都能跑，还要什么自行车，实际情况请用 n 卡 gpu 跑更好


# 搭建流程
请确保您已安装好 [conda](https://newreport.top/2023-02-27/ubuntu-amd-centos-install-conda/)、[obs](https://obsproject.com/)、[vts](https://denchisoft.com/)、[vscode](https://code.visualstudio.com/)
> 请用 conda 新建 python 3.10 环境，launch 了 vscode
```bash
git clone -b 1.0-py https://github.com/newreport/live_tts_chatgpt.git
cd live_tts_chatgpt
start.bat
# config\my_config.ini 填写房间号和 openai key
python main.py
```
win下 playsound 报错，找到 playsound 报错源文件
> command = ' '.join(command).encode('utf-16')
改为
> command = ' '.join(command)

![Star History Chart](https://api.star-history.com/svg?repos=newreport/live_tts_chatgpt&type=Date)]