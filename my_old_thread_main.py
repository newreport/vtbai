import openai
import asyncio
import random
import blivedm.blivedm as blivedm
import configparser
from queue import Queue, PriorityQueue
import json
import time
import requests
import os
import _thread
from multiprocessing import Process
import my_vits as vits
import datetime
from playsound import playsound
import xlrd
import xlwt
from xlutils.copy import copy
from pypinyin import lazy_pinyin


def write_excel_xls_append(value):
    workbook = xlrd.open_workbook(xlslPATH)  # 打开工作簿

    sheets = workbook.sheet_names()  # 获取工作簿中的所有表格
    rows_old = 0
    sheetName = str(datetime.date.today())
    if sheetName in sheets:
        worksheet = workbook.sheet_by_name(sheetName)
        rows_old = worksheet.nrows  # 获取表格中已存在的数据的行数
    new_workbook = copy(workbook)  # 将xlrd对象拷贝转化为xlwt对象
    if sheetName not in sheets:
        new_workbook.add_sheet(sheetName)
    new_worksheet = new_workbook.get_sheet(sheetName)  # 获取转化后工作簿中的第一个表格
    new_worksheet.write(rows_old, 0, value['datetime'])
    new_worksheet.write(rows_old, 1, value['user'])
    new_worksheet.write(rows_old, 2, value['type'])
    new_worksheet.write(rows_old, 3, value['num'])
    new_worksheet.write(rows_old, 4, value['action'])
    new_worksheet.write(rows_old, 5, value['msg'])
    new_worksheet.write(rows_old, 6, value['price'])
    new_workbook.save(xlslPATH)  # 保存工作簿
    if mainConfig['env'] == 'dev':
        print("xls格式表格【追加】写入数据成功！")


def write_keyboard_text(text):
    if mainConfig['env'] == 'dev':
        print('vits当前进程id::'+str(os.getpid()))
    with open(currTXT, 'w') as w:
        w.write('')
        w.flush()
    with open(currTXT, 'a') as w:
        for txt in text:
            w.write(txt)
            w.flush()


def send2gpt(msg):
    if mainConfig['env'] == 'dev':
        print('gpt当前进程id::'+str(os.getpid()))

    # 向gpt发送消息
    sendGptMsg = ''
    sendVitsMsg = ''
    if msg['type'] == 'danmu':
        sendGptMsg = msg['name']+msg['action']+msg['msg']
        sendVitsMsg = msg['msg']
    elif msg['type'] == 'sc':
        sendGptMsg = msg['name']+msg['action']+msg['price']+'块钱sc说'+msg['msg']
        sendVitsMsg = sendGptMsg
    elif msg['type'] == 'guard':
        guardType = '舰长'
        if msg['price'] > 200:
            guardType = '提督'
        elif msg['price'] > 2000:
            guardType = '总督'
        sendGptMsg = msg['name']+msg['action'] + \
            guardType+'了,花了'+msg['price']+'元'
        sendVitsMsg = msg['name']+msg['action'] + guardType+'了'
    elif msg['type'] == 'gift':
        sendGptMsg = msg['name']+msg['action'] + \
            msg['msg']+',花了'+msg['price']+'元'
        sendVitsMsg = msg['name']+msg['action'] + msg['msg']

    print("向gpt发送::"+sendGptMsg)
    # 对话日志 excel
    with open('output/'+str(datetime.date.today())+'.txt', 'a') as a:
        a.write(str(datetime.datetime.now())+"::发送::"+sendGptMsg+'\n')
        a.flush()
        write_excel_xls_append({
            'datetime': str(datetime.datetime.now()),
            'user': msg['name'],
            'type': msg['type'],
            'num': msg['num'],
            'action': msg['action'],
            'msg': msg['msg'],
            'price': msg['price']
        })

    # 发送给gpt
    tempMessage.append({"role": "user", "content": sendGptMsg})
    if len(tempMessage) > 3:
        del (tempMessage[0])
    message = baseContext+tempMessage

    # 接收
    response = openai.ChatCompletion.create(
        model="gpt-3.5-turbo", messages=message)
    responseText = str(response['choices'][0]['message']['content'])
    if filter_text(responseText) == False:
        return
    print("从gpt接收::"+responseText)

    # 生成发送语音
    vits.generated_speech(sendVitsMsg, 'sendVits.wav')
    # 生成接收语音
    vits.generated_speech(responseText, 'recVits.wav')

    playsound('output/sendVits.wav')
    time.sleep(0.5)
    # 模拟键盘输入
    write_keyboard_text(responseText)
    playsound('output/recVits.wav')

    # 对话日志
    with open('output/'+str(datetime.date.today())+'.txt', 'a') as a:
        a.write(str(datetime.datetime.now())+"::接收::"+responseText+'\n')
        a.flush()
        write_excel_xls_append({
            'datetime': str(datetime.datetime.now()),
            'user': 'gpt35',
            'type': '',
            'num': '',
            'action': '说',
            'msg': responseText,
            'price': 0
        })


def chatgpt35():
    print("运行gpt循环任务")
    while True:
        chatObj = [0, {"name": '', "type": '', 'num': 0,
                       'action': '', 'msg': '', 'price': 0}]
        # 从队列获取信息
        try:
            if topQue.qsize() > 0:
                chatObj = topQue.get(True, 1)
            elif guardQue.qsize() > 0:
                chatObj = guardQue.get(True, 1)
            elif giftQue.qsize() > 0:
                chatObj = giftQue.get(True, 1)
            elif scQue.qsize() > 0:
                chatObj = scQue.get(True, 1)
            elif danmuQue.qsize() > 0:
                chatObj = danmuQue.get(True, 1)
            chatObj = chatObj[1]
        except:
            time.sleep(0)
            continue
        # 过滤队列
        if len(chatObj['name']) > 0:
            if filter_text(chatObj['name']) and filter_text(chatObj['msg']):
                send2gpt(chatObj)
        else:
            time.sleep(1)


def filter_text(text):
    if text == '-1':
        return True
    textPY = str.join('', lazy_pinyin(text))
    for i in range(len(hzSensitiveWord)):
        if hzSensitiveWord[i] in text or pySensitiveWord[i] in textPY:
            return False
    return True


async def run_single_client():
    # 如果SSL验证失败就把ssl设为False，B站真的有过忘续证书的情况
    client = blivedm.BLiveClient(roomID, ssl=True)
    handler = MyHandler()
    client.add_handler(handler)

    client.start()
    try:
        await client.join()
    finally:
        await client.stop_and_close()


class MyHandler(blivedm.BaseHandler):
    async def _on_danmaku(self, client: blivedm.BLiveClient, message: blivedm.DanmakuMessage):
        if message.dm_type == 0:
            # print(f'弹幕：[{client.room_id}] {message.uname}：{message.msg}')
            # 权重计算
            privilege_type = message.privilege_type
            if privilege_type == 0:
                privilege_type = 9
            rank = (99999-message.user_level*10+(10-privilege_type)
                    * 10+message.mobile_verify*10)
            print("rank:"+str(rank)+";name:"+message.uname+";msg:"+message.msg.replace('[', '').replace(']', ''))
            danmuQue.put((rank, {'name': message.uname, 'type': 'danmu','num': 1, 'action': '说', 'msg': message.msg.replace('[', '').replace(']', ''), 'price': 0}))

    async def _on_gift(self, client: blivedm.BLiveClient, message: blivedm.GiftMessage):
        if message.coin_type == 'gold':
            print(f'礼物：[{client.room_id}] {message.uname} 赠送{message.gift_name}x{message.num}'
                  f' （{message.coin_type}瓜子x{message.total_coin}）')
            price = message.total_coin/1000
            if price > 1:
                giftQue.put((999999-price, {"name": message.uname, "type": 'gift',
                                            'num': message.num, 'action': message.action, 'msg': '-1', 'price': price}))

    async def _on_buy_guard(self, client: blivedm.BLiveClient, message: blivedm.GuardBuyMessage):
        print(f'上舰：[{client.room_id}] {message.username} 购买{message.gift_name}')
        guardQue((message.guard_level, {
            "name": message.username, "type": 'guard',
            'num': 1, 'action': '上', 'msg': '-1', 'price': message.price/1000}))

    async def _on_super_chat(self, client: blivedm.BLiveClient, message: blivedm.SuperChatMessage):
        print(
            f'SC:[{client.room_id}] 醒目留言 ¥{message.price} {message.uname}：{message.message}')
        # 名称、类型、数量、动作、消息、价格
        scQue.put((999999-message.price, {"name": message.uname, "type": 'sc',
                                          'num': 1, 'action': '发送', 'msg': message.message, 'price': message.price}))

def cleanQue():
    while True:
        try:
            if giftQue.full():
                giftQue.get(True, 1)
            elif danmuQue.full():
                danmuQue.get(True, 1)
        except:
            print("满了")

# 配置文件、日志、当前文本、记录excel、敏感词文本
configINI = 'config.ini'
tempTXT = 'output/temp.txt'
currTXT = 'output/currText.txt'
xlslPATH = 'output/record.xlsx'
sensitiveTXT = 'sensitive_words.txt'

# openai
if os.path.exists('my_config.ini'):
    configINI = 'my_config.ini'
con = configparser.ConfigParser()
con.read(configINI, encoding='utf-8')
sections = con.sections()
mainConfig = dict(con.items('main'))
roomID = json.loads(str(requests.get('https://api.live.bilibili.com/room/v1/Room/get_info?room_id=' +
                                     mainConfig['roomid']).content, encoding="utf-8"))['data']['room_id']
openai.api_key = mainConfig['key']
baseContext = [{"role": "system", "content": mainConfig['nya1']}]
response = openai.ChatCompletion.create(
    model="gpt-3.5-turbo", messages=baseContext)
time.sleep(1)
print("主线程发送::"+mainConfig['nya1']+"\n主线程接收::" +
      str(response['choices'][0]['message']['content']).replace('\r', '').replace('\n', ''))
# 带基础设定最大3条上下文
contextMessage = []
tempMessage = []

# 敏感词
if os.path.exists('my_sensitive_words.txt'):
    sensitiveTXT = 'my_sensitive_words.txt'
sensitiveF = open(sensitiveTXT, 'r', encoding='utf-8')
hzSensitiveWord = sensitiveF.readlines()
pySensitiveWord = []
for i in range(len(hzSensitiveWord)):
    hzSensitiveWord[i] = hzSensitiveWord[i].replace('\n', '')
    pySensitiveWord.append(str.join('', lazy_pinyin(hzSensitiveWord[i])))

# 最优先队列、sc、礼物、弹幕队列
topQue = Queue(maxsize=0)
# sc 队列
scQue = PriorityQueue(maxsize=0)
# 舰长队列
guardQue = PriorityQueue(maxsize=0)
# 礼物
giftQue = PriorityQueue(maxsize=5)
# 普通弹幕队列
danmuQue = PriorityQueue(maxsize=10)
topIDs = mainConfig['topid'].split(',')

# excel数据库
if os.path.exists(xlslPATH) == False:
    workbook = xlwt.Workbook()
    sheet = workbook.add_sheet("test")  # 在工作簿中新建一个表格
    workbook.save(xlslPATH)
    print("xls格式表格初始化成功！")
    print('当前进程id::'+str(os.getpid()))


if __name__ == '__main__':
    isRun = True
    _thread.start_new_thread(cleanQue, ())
    _thread.start_new_thread(chatgpt35, ())
    _thread.start_new_thread(asyncio.get_event_loop(
    ).run_until_complete, (run_single_client(),))
    print('All subprocesses start.')
    time.sleep(2)
    input('input to exit::')
    isRun = False
    print('All subprocesses done.')
