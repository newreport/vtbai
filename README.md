# 引用
- [blivedm](https://github.com/xfgryujk/blivedm/tree/master) 抓取 b 站弹幕、sc、舰长、礼物
- [jaywalnut310](https://github.com/jaywalnut310/vits) vits source
- [CjangCjengh](https://github.com/CjangCjengh/MoeGoe.git) vits chinese
- [Paraworks](https://github.com/Paraworks/vits_with_chatgpt-gpt3) vits 推理参考
- [演示模型](https://huggingface.co/Mahiruoshi/vits_onnx_model/tree/main) vits model

# 架构
> 从哔哩哔哩直播间抓取弹幕和礼物，接收后发送给 openai 官方的 chatgpt，等待 gpt 回调消息后使用 vits 进行 tts（文本转语音）推理，然后根据关键字触发 vts 的表情快捷键和 playsound 播放语音， 播放时 vts 根据声音匹配口型  

> 除非公司或大佬，非常不建议手搓 live2d，一是耗时，二是目前的 vts 和 prprlive 配合关键字触发表情快捷键直播效果还可以  

>  本直播流程仅在 win 下测试并通过，理论 linux 和 mac 在合适的 py 环境中也能使用，py 版本 3.6 ~ 3.9，不要用 3.10+  

>  注：有能者可以同理把老鼠和油管的扩展了，咱本职 C# 和 go，py 线程协程进程鲨我

## 工作流
blivedm（抓直播间信息）——>openai（猫娘对话）——>vits（tts 文本转语音）——>vts（语音转口型，快捷键触发表情）
main.py 为启动文件  


## 性能
- CPU：5700G
- GPU：核显
- 主板：华硕 B550m TUF
- 硬盘：凯侠 RC20
- 内存：16*4（3133hz 金百达）
- 电源：小1U 300W
- 机箱：傻瓜超人 4.5L
> 实测 100 汉字，在 cpu 推理下能在 2s 内完成，主要速度慢在请求 openai，由于需要检测同音字敏感词以防爆房，故不能使用 SSE

> obs、vts、雀魂 AI 全开的情况，推理时 cpu 负载约为 70%，理论讲 2k 内预算的丐中丐 5600G 也可以跑，功耗不超过 100W，ITX 都能跑，还要什么自行车


# 搭建流程
请确保您已安装好 conda、obs、vts

## windows
windows 请 conda 图形化新建环境，

## linux
[安装conda](https://newreport.top/2023-02-28/ubuntu-amd-centos-install-conda/)
```bash
conda create -n live python=3.9
conda activate live

git clone https://github.com/newreport/live_vits_chatgpt.git
cd live_vits_chatgpt
bash start.sh
# 去 my_config.ini 里配置好 openai key 和代理
python main.py
```


# 常见问题
## playsoud 报错
windows下需要删除 utf-16