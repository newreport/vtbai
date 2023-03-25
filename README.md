# Function in coding ....
- Shortcut key with expression
- Azure API TTS 
- Link CharacterAI 角色扮演最好的 AI
- Multi link 联动
- ChatGLM-6B 本地1
- ChatterBot 本地2
- Twitch & Youtube 平台支持
- main暂时不更了，python 联动性太差，下版本换 C#
# 引用

- [vits](https://github.com/jaywalnut310/vits) vits source
- [MoeGoe](https://github.com/CjangCjengh/MoeGoe.git) vits chinese
- [vits_with_chatgpt-gpt3](https://github.com/Paraworks/vits_with_chatgpt-gpt3) tts 推理参考
- [blivedm](https://github.com/xfgryujk/blivedm/tree/master) 抓取 b 站直播间信息
- [演示模型](https://huggingface.co/Mahiruoshi/vits_onnx_model/tree/main) vits model (商用请自炼自然人同意的合法声源或用 Azure)

# 架构
> 从哔哩哔哩直播间抓取弹幕和礼物，接收后发送给 openai 官方的 chatgpt，等待 gpt 回调消息后使用 vits 进行 tts 推理，然后根据关键字触发 vts 的表情快捷键和 playsound 播放语音， 播放时 vts 根据声音匹配口型  

> 除非公司或大佬，非常不建议手搓 live2d，一是耗时，二是目前的 vts 和 prprlive 配合关键字触发表情快捷键直播效果还可以  

>  本直播流程仅在 win 下测试并通过，理论 linux 和 mac 在合适的 py 环境中也能使用，py 版本 3.6 ~ 3.9，不要用 3.10+  

>  注：有能者可以同理把老鼠和油管的扩展了，py 线程协程进程鲨我

## 工作流
blivedm（抓直播间信息）——>openai（猫娘对话）——>vits（tts 文本转语音）——>vts（语音转口型，快捷键触发表情）——>obs(推流)  

## 功能
- 支持 api 请求，扩展用，比如联动  httt://127.0.0.1:8080?text=你好
- 支持优先级：api>舰长>礼物>SC>弹幕，礼物和弹幕队列容量默认为5/10，满了自动剔除时间线最后的，SC、舰长没有容量限制

## 性能
- CPU：5700G
- GPU：核显
- 主板：华硕 B550m TUF
- 硬盘：凯侠 RC20
- 内存：16*4（3133hz 金百达）
- 电源：小1U 300W
- 机箱：傻瓜超人 4.5L
> 主要速度慢在请求 openai 和推理，由于需要检测同音字敏感词以防爆房，故不能使用 SSE

> obs、vts、雀魂 AI 全开的情况，推理时 cpu 负载约为 70%，理论讲 2k 内预算的丐中丐 5600G 也可以跑，功耗不超过 100W，ITX 都能跑，还要什么自行车


# 搭建流程
请确保您已安装好 conda、obs、vts、vscode
## windows
> windows 请 conda 图形化新建环境，并用 conda 创建了 python 3.9 环境，launch 了 vscode
1. clone 项目
```bash
git clone https://github.com/newreport/live_tts_chatgpt.git
cd live_tts_chatgpt
mkdir models
mkdir output
```
2. 下载 vits 模型和配置

放入 models/

https://huggingface.co/Mahiruoshi/vits_onnx_model/resolve/main/model.onnx 
https://huggingface.co/Mahiruoshi/vits_onnx_model/resolve/main/config.json

3. 初始化项目依赖
```bash
pip install -r requirements.txt
git submodule update --init --recursive
pip install -r blivedm/requirements.txt
pip install -r MoeGoe/requirements.txt

cp config/config.ini config/my_config.ini
cp config/sensitive_words.txt config/my_sensitive_words.txt
```

4.win下 playsound 报错
> command = ' '.join(command).encode('utf-16')
改为
> command = ' '.join(command)
## linux
1. [安装conda](https://newreport.top/2023-02-28/ubuntu-amd-centos-install-conda/)

2. 创建环境 clone 项目

```bash
conda create -n live python=3.9
conda activate live

git clone https://github.com/newreport/live_vits_chatgpt.git
cd live_vits_chatgpt

# 去 my_config.ini 里配置好 openai key 和代理
```
