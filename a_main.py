import openai
from bilibili_api import live, sync
import configparser
from queue import Queue, PriorityQueue
import json
import time
import requests
import os
import _thread
from multiprocessing import Process
import a_vits as vits
import datetime
from playsound import playsound
 

# 配置文件、弹幕房间、chatgpt配置
file = 'config.ini'
tempFile = 'output/temp.txt'
currTxt='output/currText.txt'

con = configparser.ConfigParser()
con.read(file, encoding='utf-8')
sections = con.sections()
mainConfig = dict(con.items('main'))
room = live.LiveDanmaku(json.loads(str(requests.get('https://api.live.bilibili.com/room/v1/Room/get_info?room_id=' +
                        mainConfig['roomid']).content, encoding="utf-8"))['data']['room_id'])
openai.api_key = mainConfig['key']

# 最优先队列、sc、礼物、弹幕队列
topQue = Queue(maxsize=0)
scQue = PriorityQueue(maxsize=0)
giftQue = PriorityQueue(maxsize=10)
danmuQue = PriorityQueue(maxsize=3)

baseContext = [{"role": "system", "content": mainConfig['nya1']}]
response = openai.ChatCompletion.create(
    model="gpt-3.5-turbo", messages=baseContext
)
print(response['choices'][0]['message']['content'])
message = []


def write_keyboard_text(text):
    print('vits当前进程id::'+str(os.getpid()))
    with open(currTxt, 'w') as w:
        w.write('')
        w.flush()
    with open(currTxt, 'a') as w:
        for txt in text:
            w.write(txt)
            w.flush()

def send_msg(msg):
    print('gpt当前进程id::'+str(os.getpid()))
    print("向gpt发送::"+msg)
    with open('output/'+str(datetime.date.today())+'.txt', 'a') as a:
            a.write(str(datetime.datetime.now())+"::发送::"+str(msg)+'\n')
            a.flush()
    message = baseContext
    message.append({"role": "user", "content": msg})
    response = openai.ChatCompletion.create(
        model="gpt-3.5-turbo", messages=message)
    responseText=response['choices'][0]['message']['content']
    print("从gpt接收::"+responseText)
    vits.generated_speech(str(responseText))
    pw = Process(target=write_keyboard_text, args=(str(responseText),))
    pw.start()
    playsound('output/temp1.wav')
    with open('output/'+str(datetime.date.today())+'.txt', 'a') as a:
            a.write(str(datetime.datetime.now())+"::接收::"+str(responseText)+'\n')
            a.flush()

def chatgpt35():
    print("运行gpt")
    while True:
        if topQue.qsize() > 0:
            send_msg(topQue.get())
        elif giftQue.qsize() > 0:
            send_msg(giftQue.get())
        elif scQue.qsize() > 0:
            send_msg(scQue.get())
        elif danmuQue.qsize() > 0:
            send_msg(danmuQue.get())
        time.sleep(1)

def filter_text(text):
    print("过滤::"+text)
    return True


@room.on('DANMU_MSG')
async def on_danmaku(event):
    print('弹幕当前进程id::'+str(os.getpid()))
    # 收到弹幕
    name = event['data']['info'][2][1]
    msg = event['data']['info'][1]
    msgs = name+"说："+msg
    msgType = event['data']['info'][0][12]
    # print(str(event))
    if mainConfig['env'] == 'dev':
        with open(tempFile, 'a') as a:
            a.write(str(event)+'\n')
            a.flush()
    if msgType == 0:
        if filter_text(msg):
            danmuQue.put(msgs)
    elif msgType == 1:
        print('表情::'+msgs)
    else:
        print('其他::'+msgs)


@room.on('SEND_GIFT')
async def on_gift(event):
    # 收到礼物
    if event['data']['data']['batch_combo_send'] is not None:
        name = event['data']['data']['batch_combo_send']['uname']
        action = event['data']['data']['batch_combo_send']['action']
        num = event['data']['data']['batch_combo_send']['gift_num']
        gift = event['data']['data']['batch_combo_send']['gift_name']
        msgs = name+action+str(num)+'个'+gift
        print("礼物::"+msgs)
        scQue.put(9, '感谢'+msgs)
        if mainConfig['env'] == 'dev':
            with open('girf.txt', 'a') as a:
                a.write(str(event)+'\n')
                a.flush()

def p0():
    while True:
        print("0")
        time.sleep(2)



if __name__ =='__main__':
    print('当前进程id::'+str(os.getpid()))
    _thread.start_new_thread(chatgpt35,())
    _thread.start_new_thread(sync, (room.connect(),))
    input('input q to ecs')
    print('All subprocesses done.')


# _thread.start_new_thread(sync, (room.connect(),()))
