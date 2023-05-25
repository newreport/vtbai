> 该分支正在编写中，实际不可用

# python 版本 
若 C# 版本（Docker）不稳定或者不能使用，请使用下面版本，并阅读 Source 中的 Readme
https://github.com/newreport/vtbai/releases/tag/1.1-py

# C#
```bash
git clone https://github.com/newreport/vtbai.git
git checkout -b master origin/master
```
clone 下来切换到master分支，安装 [dotnet 7](https://dotnet.microsoft.com/zh-cn/download) 和 [vs](https://visualstudio.microsoft.com/zh-hans/vs/) 进行调试启动

# Docker(未启用)
> [vits演示模型](https://huggingface.co/newreport/live_tts_default_model/tree/main) 商用请自炼自然人同意的合法声源或用 Azure
 
> [敏感词](https://snginx.newreport.top/my_sensitive_words.7z)  解压密码：kkDXCD6mwaU7wU
```bash
docker run -d --restart=always --name vtbai -p 3939:3939 -v vtbai-data:/data newreport/vtbai
```

| 功能                                  | 地址                                                 |
| ------------------------------------- | ---------------------------------------------------- |
| swagger 后端 api 列表                 | http://127.0.0.1:3939/swagger                        |
| obs弹幕(api 方式，固定 0.1s 刷新一次) | http://127.0.0.1:3939/static/subtitle_api.html       |
| obs弹幕(websocket 方式，实时)         | http://127.0.0.1:3939/static/subtitle_websocket.html |
| 后台管理，配置toml和模型（暂未实现）  | http://127.0.0.1:3939/admin                          |

# 架构
blivedm（抓直播间信息）——>openai（猫娘对话）——>vits（tts 文本转语音）——>vts（语音转口型，快捷键触发表情）——>obs(推流)

> 从直播间抓取弹幕和礼物/SC，接收后发送 gpt，等待 gpt 回调消息后进行 tts 推理，然后根据关键字或者情感推理使用 VTS API 触发表情和 playsound 播放语音， 播放时 vts（需在 VTS 中进行配置） 根据声音匹配口型

> 除非公司或大佬，非常不建议手搓 live2d，一是耗时，二是 vts 配合 api 触发表情快捷键直播效果还可以  

>  本流程仅在 docker 下测试并通过，理论 OS 系统一样适用，但安装麻烦这里不做部署教程


## 功能

- 支持 api 扩展 gpt 和 tts，扩展见swagger
- 支持自定义优先级队列

##  编写中...
- 抖音支持
- VTS API 关键字触发表情
- 语句情感推断触发 VTS 表情
- Azure API TTS 
- Link CharacterAI 角色扮演最好的 AI
- Multi link 联动
- ChatGLM-6B 本地1
- ChatterBot 本地2
- Twitch & Youtube 平台支持


## 注意事项
> 关于鉴权，所有请求均为了方便使用和调试均为 get，此项目一般在本地跑不会做任何鉴权。需要鉴权请用 nginx/[nginx proxy manager](https://nginxproxymanager.com/) 添加信任用户。或者用 [frp](https://github.com/fatedier/frp)/ [nps](https://github.com/ehang-io/nps) 内穿到本地使用




## 开发环境
> 编译器：VSCode、VS

> 主机：itx，仅 openai 模式 cpu tts 推理下可用
- CPU：5700G
- GPU：核显
- 主板：华硕 B550m TUF PLUS WIFI
- 硬盘：铠侠 RC20
- 内存：32*4（3133hz 金百达）
- 电源：台达 300W
- 机箱：X40 5.5L

## 生产环境
- CPU：5900X
- GPU：3060 12G
- 主板：华硕 B550m TUF PLUS WIFI Ⅱ
- 硬盘：三星 980pro
- 内存：16*4（3133hz 金百达）
- 电源：先马黑钻 1000W
- 机箱：长城阿基米德7

# 参考资源

## Docker版（C#）
- [bilibili-API-collect](https://github.com/SocialSisterYi/bilibili-API-collect) b 站 api 文档
- [BiliBiliLive](https://github.com/a820715049/BiliBiliLive) b 站 websocket 实现
- [MoeGoe](https://github.com/CjangCjengh/MoeGoe.git) vits chinese
- [OpenAI-DotNet](https://github.com/RageAgainstThePixel/OpenAI-DotNet) openai 封装库
- [douyin-live](https://github.com/YunzhiYike/douyin-live/blob/main/dy.py) 抖音协议参考

## python版
- [vits](https://github.com/jaywalnut310/vits) vits source
- [MoeGoe](https://github.com/CjangCjengh/MoeGoe.git) vits chinese
- [vits_with_chatgpt-gpt3](https://github.com/Paraworks/vits_with_chatgpt-gpt3) tts 推理
- [blivedm](https://github.com/xfgryujk/blivedm/tree/master) python 抓取 b 站直播间信息



![Star History Chart](https://api.star-history.com/svg?repos=newreport/live_tts_chatgpt&type=Date)]


# 二次开发
## 项目架构
```mermaid
    graph TD
    A2(TestConsole)
    A1(vtbai)

    B(ConfuseCore)

    D1(AIGC)
    D2(GPT)
    D3(L2d)
    D4(Live)
    D5(TTS)

    Z(CommonHelper)
    A1-->B
    A1-->Z

    B-->D1
    B-->D2
    B-->D3
    B-->D4
    B-->D5
    B-->Z

    D1-->Z
    
    D2-->Z

    D3-->Z

    D4-->Z

    D5-->Z

    Z-->Tomlyn
    Z-->TinyPinyin.Net
    Z-->Newtonsoft.Json


```
* **TestConsole** 测试用终端，和程序无关
* **vtbai** 主程序，提供 api 接口和启动 Core
  * **ConfuseCore** 核心程序，引用所有模块进行处理
    * **AIGC**
    * **GPT** 给 chatgpt/glm6b 发送请求并接收
    * **L2d** 给 vts 发送 api 请求
    * **Live**  抓取直播间
    * **TTS** 文本转语音
      * **CommonHelper**  工具类
## Live
### Bilibili
#### 接收
| 命令                          | 注释             |
| ----------------------------- | ---------------- |
| **DANMU_MSG**                 | 弹幕内容         |
| **GUARD_BUY**                 | 购买舰长         |
| **USER_TOAST_MSG**            | 续费舰长         |
| **SUPER_CHAT_MESSAGE**        | 超级留言         |
| **SUPER_CHAT_MESSAGE_JPN**    | 超级留言-JP      |
| **SEND_GIFT**                 | 赠送礼物         |
| COMBO_SEND                    | 赠送礼物         |
| ROOM_REAL_TIME_MESSAGE_UPDATE | 主播粉丝信息更新 |
| ENTRY_EFFECT                  | 进入特效         |
| HOT_RANK_CHANGED              | 主播实时活动排名 |
| INTERACT_WORD                 | 进入房间         |
| LIVE                          | 直播开始         |
| LIVE_INTERACTIVE_GAME         |
| NOTICE_MSG                    | 直播间广播       |
| ONLINE_RANK_COUNT             | 高能榜数量更新   |
| ONLINE_RANK_V2                | 高能榜数据       |
| ONLINE_RANK_TOP3              | 用户进高能榜前三 |
| PREPARING                     | 主播准备中       |
| STOP_LIVE_ROOM_LIST           | 停止的直播间信息 |
| SYS_MSG                       | 系统消息         |
| TRADING_SCORE                 |
| WELCOME                       | 迎加入房间       |
| WELCOME_GUARD                 | 欢迎房管加入房间 |