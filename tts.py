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
    str1 = "你好呀，我的名字是七奈，现在五点了，下午好，喵喵喵~喵喵~喵~~喵喵喵~"
    generated_speech(str1, 'temp1.wav')
