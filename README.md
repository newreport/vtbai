# 引用
- [jaywalnut310](https://github.com/jaywalnut310/vits)
- [CjangCjengh](https://github.com/CjangCjengh/MoeGoe.git)
- [blivedm](https://github.com/xfgryujk/blivedm/tree/master)
- [演示模型](https://huggingface.co/Mahiruoshi/vits_onnx_model/tree/main)

# 架构
> 从哔哩哔哩直播间抓取弹幕和礼物，接收后发送给 openai 官方的 chatgpt，等待 gpt 回调消息后使用 vits 进行 tts（文本转语音）推理，然后根据关键字触发 vts 的表情快捷键和 playsound 播放语音， 播放时 vts 根据声音匹配口型  
注：本直播流程仅在 win 下测试已通过，理论 linux 和 mac 在合适的 py 环境中也能使用，py 版本 3.6 ~ 3.9，不要用 3.10+，咱连写带学只写了一周的py（23-03-16记）， py线程协程进程鲨我  
再注：有能者可以同理把老鼠和油管的扩展了，咱的代码很烂

流程：blivedm————>openai————>vits————>vts


# 搭建流程
此时请确保您已安装好 conda、obs、vts
## windows


windows 请 conda 图形化新建环境，

## linux
```bash
conda create -n live python=3.9
conda activate live

git clone https://github.com/newreport/live_vits_chatgpt.git
cd live_vits_chatgpt
bash start.sh
# 去 my_config.ini 里配置好 openai key 和代理
python main.py
```