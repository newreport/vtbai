# -*- coding: utf-8 -*- 
import argparse
import onnxruntime as ort
import io
import torchvision.models as models
from scipy.io import wavfile
import time
import os
import sys
import numpy as np
import torch
sys.path.append('MoeGoe')
from text import text_to_sequence
import utils as utils
import commons as commons

def is_japanese(string):
    for ch in string:
        if ord(ch) > 0x3040 and ord(ch) < 0x30FF:
            return True
    return False


def get_args():
    parser = argparse.ArgumentParser(description='inference')
    # 模型文件夹
    parser.add_argument('--onnx_model', default='./models/model.onnx')
    # 模型配置
    parser.add_argument('--cfg', default="./models/config.json")
    args = parser.parse_args()
    return args


args = get_args()
hps = utils.get_hparams_from_file(args.cfg)
ort_sess = ort.InferenceSession(args.onnx_model)

# 生成语音
def generated_speech(text, fileName):
    text = f"[JA]{text}[JA]" if is_japanese(text) else f"[ZH]{text}[ZH]"
    # 将文本字符串转换为id
    seq = text_to_sequence(text, symbols=hps.symbols,
                           cleaner_names=hps.data.text_cleaners)
    if hps.data.add_blank:
        seq = commons.intersperse(seq, 0)
    with torch.no_grad():
        x = np.array([seq], dtype=np.int64)
        x_len = np.array([x.shape[1]], dtype=np.int64)
        sid = 0
        sid = np.array([sid], dtype=np.int64)
        scales = np.array([0.667, 0.8, 1], dtype=np.float32)
        scales.resize(1, 3)
        ort_inputs = {
            'input': x,
            'input_lengths': x_len,
            'scales': scales,
            'sid': sid
        }
        t1 = time.time()
        audio = np.squeeze(ort_sess.run(None, ort_inputs))
        audio *= 32767.0 / max(0.01, np.max(np.abs(audio))) * 0.6
        audio = np.clip(audio, -32767.0, 32767.0)
        t2 = time.time()
        spending_time = "推理时间："+str(t2-t1)+"s"
        print(spending_time)
        wavfile.write('output/'+fileName, hps.data.sampling_rate,
                      audio.astype(np.int16))


if __name__ == '__main__':
    str1 = "最喜欢的季节是春天。我觉得春天是一个充满生机和活力的季节。随着百花的盛开，春天带来了新的生命和希望。在春天，气温逐渐升高，天气也变得温暖和宜人。大自然开始苏醒，鸟儿唧唧喳喳地叫着，树木渐渐发芽，花儿绽放出五彩斑斓的色彩。春天也是一个让人感到愉快的季节。人们开始换上轻便的衣服，走出户外，感受大自然的美妙。我喜欢在春天里去公园散步，享受春风拂面的感觉。在公园里，可以看到满眼的绿意和各种各样的鲜花，让人心情舒畅。此外，春天也是一个让人充满期待和希望的季节。在新的一年里，一切都是全新的开始。人们开始规划新的计划和目标，并努力实现它们。总之，对我来说，春天是一个非常特别的季节。它带给我无尽的欢乐和动力，让我感到充满希望和激情。我相信，在这个美好的季节里，我们可以收获更多的爱与快乐。"
    generated_speech(str1, 'temp1.wav')
