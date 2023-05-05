# python 版本 
若 C# 不稳定或者不能使用，请使用下面版本
https://github.com/newreport/vtbai/releases/tag/1.1-py

# Function in coding ....
- 抖音支持
- VTS API 关键字触发表情
- 语句情感推断触发 VTS 表情
- Azure API TTS 
- Link CharacterAI 角色扮演最好的 AI
- Multi link 联动
- ChatGLM-6B 本地1
- ChatterBot 本地2
- Twitch & Youtube 平台支持


## 功能
- AI 直播
- 支持 api 请求，扩展用，比如联动，语音转文字后请求即可  httt://127.0.0.1:8080?text=你好
- 支持优先级：api>舰长>礼物>SC>弹幕，礼物和弹幕队列容量默认为5/10，满了自动剔除时间线最后的，SC、舰长没有容量限制

# 资源
- [vits](https://github.com/jaywalnut310/vits) vits source
- [MoeGoe](https://github.com/CjangCjengh/MoeGoe.git) vits chinese
- [vits_with_chatgpt-gpt3](https://github.com/Paraworks/vits_with_chatgpt-gpt3) tts 推理参考
- [blivedm](https://github.com/xfgryujk/blivedm/tree/master) 抓取 b 站直播间信息
- [演示模型](https://huggingface.co/newreport/live_tts_default_model/tree/main) vits model (商用请自炼自然人同意的合法声源或用 Azure)

- # 架构
> 从直播间抓取弹幕和礼物/SC，接收后发送给 openai 官方的 chatgpt，等待 gpt 回调消息后使用 vits 进行 tts 推理，然后根据关键字或者情感推理使用快捷键或者 VTS API 触发表情和 playsound 播放语音， 播放时 vts（需在 VTS 中进行配置） 根据声音匹配口型  

> 除非公司或大佬，非常不建议手搓 live2d，一是耗时，二是 vts 和 prprlive 配合关键字触发表情快捷键直播效果还可以  

>  本直播流程仅在 docker 下测试并通过，理论 OS 系统一样适用，但安装麻烦这里不做部署教程


## 工作流
blivedm（抓直播间信息）——>openai（猫娘对话）——>vits（tts 文本转语音）——>vts（语音转口型，快捷键触发表情）——>obs(推流)


## 开发环境
> 仅 openai 模式 cpu tts 推理下可用
- CPU：5700G
- GPU：核显
- 主板：华硕 B550m TUF PLUS WIFI
- 硬盘：凯侠 RC20
- 内存：32*4（3133hz 金百达）
- 电源：海韵Core 650W

## 生产环境
- CPU：5900X
- GPU：3060 12G
- 主板：华硕 B550m TUF PLUS WIFI Ⅱ
- 硬盘：三星 980pro
- 内存：16*4（3133hz 金百达）
- 电源：先马黑钻 1000W

# Docker

```bash
docker run -d --restart=always --name vtbai -p 3939:3939 -v vtbai-data:/data newreport/vtbai
```

![Star History Chart](https://api.star-history.com/svg?repos=newreport/live_tts_chatgpt&type=Date)]
