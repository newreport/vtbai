# -*- coding: utf-8 -*-
import multiprocessing
import onnxruntime as ort
import torchvision.models as models
from scipy.io import wavfile
import time
import wave
import pyaudio
import os
import sys
import numpy as np
import asyncio
import torch
import uuid
import configparser
sys.path.append('MoeGoe')
import commons as commons
import utils as ttsUtils
from text import text_to_sequence


def is_japanese(string):
    for ch in string:
        if ord(ch) > 0x3040 and ord(ch) < 0x30FF:
            return True
    return False


def is_english(string):
    import re
    pattern = re.compile('^[A-Za-z0-9.,:;!?()_*"\' ]+$')
    if pattern.fullmatch(string):
        return True
    else:
        return False


# 生成/推理语音
def generated_speech(hps, ort_sess, tts_config, text, fileName):
    text = f"[ZH]{text}[ZH]"
    if is_japanese(text):
        text = f"[JA]{text}[JA]"

    # 将文本字符串转换为id
    seq = text_to_sequence(text, symbols=hps.symbols,
                           cleaner_names=hps.data.text_cleaners)
    if hps.data.add_blank:
        seq = commons.intersperse(seq, 0)
    with torch.no_grad():
        x = np.array([seq], dtype=np.int64)
        x_len = np.array([x.shape[1]], dtype=np.int64)
        sid = tts_config['speaker_id']
        sid = np.array([sid], dtype=np.int64)
        scales = np.array([tts_config['noise_scale'], tts_config['noise_scale_w'],
                          tts_config['length_scale']], dtype=np.float32)
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
        print(spending_time+" 推理内容:"+text)
        wavfile.write('output/wav/'+fileName+'.wav', hps.data.sampling_rate,
                      audio.astype(np.int16))

# 播放语音和更改文字


def play(is_run, tts_config, wav_que, curr_txt):
    print("运行play子进程")
    is_auto_del = bool(int(tts_config['auto_del_wav']))
    while is_run:
        # 阻塞
        text = wav_que.get()
        print("开始播放内容::"+text)
        name = text.split("::")[0]
        txt = text.split("::")[1]
        p = multiprocessing.Process(
            target=change_txt, args=(tts_config, txt, curr_txt))
        p.start()
        play_audio('output/wav/'+name+'.wav')
        if is_auto_del:
            os.remove('output/wav/'+name+'.wav')
        time.sleep(0.5)
        p.join()

def change_txt(tts_config, text, curr_txt):
    # print("改变文字"+text)
    curr_txt.value = ''
    for txt in text:
        curr_txt.value = curr_txt.value + txt
        # print(curr_txt.value)
        time.sleep(float(tts_config['interval_ms'])/1000)

def play_audio(file_path):
    CHUNK = 1024
    wf = wave.open(file_path, 'rb')
    p = pyaudio.PyAudio()
    stream = p.open(format=p.get_format_from_width(wf.getsampwidth()),
                    channels=wf.getnchannels(),
                    rate=wf.getframerate(),
                    output=True)

    data = wf.readframes(CHUNK)

    while data:
        stream.write(data)
        data = wf.readframes(CHUNK)

    stream.stop_stream()
    stream.close()

    p.terminate()



def inference(is_run, tts_config,  ttsQue, wav_que):
    print("运行tts子进程")
    config_ini = 'config/config.ini'
    if os.path.exists('config/my_config.ini'):
        config_ini = 'config/my_config.ini'
    con = configparser.ConfigParser()
    con.read(config_ini, encoding='utf-8')
    tts_config = dict(con.items('tts'))

    hps = ttsUtils.get_hparams_from_file(tts_config['model_config'])
    ort_sess = ort.InferenceSession(tts_config['model_onnx'])

    while is_run:
        # 阻塞
        text = ttsQue.get()
        name = str(uuid.uuid1())
        print("生成语音：："+name+"::"+text)
        generated_speech(hps, ort_sess, tts_config, text,name)
        wav_que.put(name+"::"+text)

if __name__ == '__main__':
    config_ini = 'config/config.ini'
    if os.path.exists('config/my_config.ini'):
        config_ini = 'config/my_config.ini'
    con = configparser.ConfigParser()
    con.read(config_ini, encoding='utf-8')
    tts_config = dict(con.items('tts'))

    hps = ttsUtils.get_hparams_from_file(tts_config['model_config'])
    ort_sess = ort.InferenceSession(tts_config['model_onnx'])

    str1 = "最喜欢的季节是春天。我觉得春天是一个充满生机和活力的季节。随着百花的盛开，春天带来了新的生命和希望。在春天，气温逐渐升高，天气也变得温暖和宜人。大自然开始苏醒，鸟儿唧唧喳喳地叫着，树木渐渐发芽，花儿绽放出五彩斑斓的色彩。春天也是一个让人感到愉快的季节。人们开始换上轻便的衣服，走出户外，感受大自然的美妙。我喜欢在春天里去公园散步，享受春风拂面的感觉。在公园里，可以看到满眼的绿意和各种各样的鲜花，让人心情舒畅。此外，春天也是一个让人充满期待和希望的季节。在新的一年里，一切都是全新的开始。人们开始规划新的计划和目标，并努力实现它们。总之，对我来说，春天是一个非常特别的季节。它带给我无尽的欢乐和动力，让我感到充满希望和激情。我相信，在这个美好的季节里，我们可以收获更多的爱与快乐。"
    generated_speech(hps, ort_sess, tts_config, str1, 'temp1')
