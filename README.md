# 引用
- [jaywalnut310](https://github.com/jaywalnut310/vits)
- [CjangCjengh](https://github.com/CjangCjengh/MoeGoe.git)
- [blivedm](https://github.com/xfgryujk/blivedm/tree/master)
- [演示模型](https://huggingface.co/Mahiruoshi/vits_onnx_model/tree/main)

# 架构
> 从哔哩哔哩直播间抓取弹幕和礼物，接收后发送给 openai 官方的 chatgpt，等待 gpt 回调消息后使用 vits 进行 tts（文本转语音）推理，然后根据关键字触发 vts 的表情快捷键和 playsound 播放语音， 播放时 vts 根据声音匹配口型  
流程：blivedm————>openai————>vits————>vts