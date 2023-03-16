import openai
import asyncio
import _thread
import random
import blivedm.blivedm as blivedm
import configparser
from queue import Queue, PriorityQueue
import json
import time
import requests
import os
import multiprocessing
import tts
import datetime
from playsound import playsound
import xlrd
import xlwt
import sys
from xlutils.copy import copy
from pypinyin import lazy_pinyin

# 记录弹幕上下文到 excel


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

# 模拟 SSE 键盘输入，供 obs 抓取字幕


def write_keyboard_text(text):
    with open(currTXT, 'w', encoding='utf-8') as w:
        w.write('')
        w.flush()
    for txt in text:
        with open(currTXT, 'a', encoding='utf-8') as w:
            w.write(txt)
            w.flush()
        time.sleep(0.2)


def send2gpt(msg):
    if mainConfig['env'] == 'dev':
        print('gpt当前进程id::'+str(os.getpid()))

    # 向 gpt 发送的消息
    sendGptMsg = ''
    # 向 tts 写入的数据
    sendVitsMsg = ''
    if msg['type'] == 'danmu':
        sendGptMsg = msg['name']+msg['action']+msg['msg']
        sendVitsMsg = msg['msg']
    elif msg['type'] == 'sc':
        sendGptMsg = msg['name']+msg['action'] + \
            str(msg['price'])+'块钱sc说'+msg['msg']
        sendVitsMsg = sendGptMsg
    elif msg['type'] == 'guard':
        guardType = '舰长'
        if msg['price'] > 200:
            guardType = '提督'
        elif msg['price'] > 2000:
            guardType = '总督'
        sendGptMsg = msg['name']+msg['action'] + \
            guardType+'了,花了'+str(msg['price'])+'元'
        sendVitsMsg = msg['name']+msg['action'] + guardType+'了'
    elif msg['type'] == 'gift':
        sendGptMsg = msg['name']+msg['action'] + msg['msg']
        sendVitsMsg = sendGptMsg

    # 生成上下文
    tempMessage.append({"role": "user", "content": sendGptMsg})
    # 上下文最大值
    if len(tempMessage) > 3:
        del (tempMessage[0])
    message = baseContext+tempMessage

    # 开启 openai 和 tts 进程
    p = multiprocessing.Process(target=rec2tts, args=(
        msg, sendGptMsg, message, sendVitsMsg))
    p.start()
    # join 会阻塞当前 gpt 循环线程，但不会阻塞弹幕线程
    p.join()


def rec2tts(msg, sendGptMsg, message, sendVitsMsg):
    print("进入gtp&&tts进程，向gpt发送::"+sendGptMsg)

    # 对话日志写入 excel
    with open('output/'+str(datetime.date.today())+'.txt', 'a', encoding='utf-8') as a:
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

    # 发送并收
    response = openai.ChatCompletion.create(
        model="gpt-3.5-turbo", messages=message)
    responseText = str(response['choices'][0]['message']['content'])

    # 敏感词词音过滤
    if filter_text(responseText) == False:
        print("检测到敏感词内容::"+responseText)
        return

    print("从gpt接收::"+responseText)

    # 生成发送语音
    tts.generated_speech(sendVitsMsg, 'sendVits.wav')
    # 生成接收语音
    tts.generated_speech(responseText, 'recVits.wav')

    playsound('output/sendVits.wav')
    # 模拟键盘输入
    p = multiprocessing.Process(
        target=write_keyboard_text, args=(responseText,))
    p.start()
    time.sleep(0.5)
    # 播放接受
    playsound('output/recVits.wav')

    # 对话日志
    with open('output/'+str(datetime.date.today())+'.txt', 'a', encoding='utf-8') as a:
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

# 监控队列的循环任务


def chatgpt35():
    print("运行gpt循环任务")
    while True:
        chatObj = {"name": '', "type": '', 'num': 0,
                   'action': '', 'msg': '', 'price': 0}
        # 从队列获取信息
        try:
            if topQue.empty() == False:
                chatObj = topQue.get(True, 1)
            elif guardQue.empty() == False:
                chatObj = guardQue.get(True, 1)
                chatObj = chatObj[1]
            elif giftQue.empty() == False:
                chatObj = giftQue.get(True, 1)
                chatObj = chatObj[1]
            elif scQue.empty() == False:
                chatObj = scQue.get(True, 1)
                chatObj = chatObj[1]
            elif danmuQue.empty() == False:
                chatObj = danmuQue.get(True, 1)
                chatObj = chatObj[1]
        except Exception as e:
            print("-----------ErrorStart--------------")
            print(e)
            print("gpt获取弹幕异常，当前线程：：")
            print(chatObj)
            print("-----------ErrorEnd--------------")
            time.sleep(2)
            continue
        # print(chatObj)
        # 过滤队列
        if len(chatObj['name']) > 0:
            if filter_text(chatObj['name']) and filter_text(chatObj['msg']):
                send2gpt(chatObj)
                print("子进程退出")
        else:
            time.sleep(1)


# 敏感词音检测
def filter_text(text):
    # 为上舰时直接过
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
            guardLevel = message.privilege_type
            if guardLevel == 0:
                guardLevel = 0
            elif guardLevel == 3:
                guardLevel = 200
            elif guardLevel == 2:
                guardLevel = 2000
            elif guardLevel == 1:
                guardLevel = 20000
            # 舰长权重，勋章id权重*100，lv权重*100
            medalevel = 0
            if message.medal_room_id == roomID:
                medalevel = message.medal_level*100
            rank = (999999-message.user_level*100 -
                    guardLevel - medalevel-message.user_level*10+random.random())
            if danmuQue.full():
                try:
                    danmuQue.get(True, 1)
                except:
                    print("on_danmuku时，get异常")

            queData = {'name': message.uname, 'type': 'danmu', 'num': 1, 'action': '说',
                       'msg': message.msg.replace('[', '').replace(']', ''), 'price': 0}
            if mainConfig['env'] == 'dev':
                print("前弹幕队列容量："+str(danmuQue.qsize()))
                print("rank:"+str(rank)+";name:"+message.uname+";msg:" +
                      message.msg.replace('[', '').replace(']', ''))
                print(queData)
            try:
                danmuQue.put((rank, queData), True, 2)
            except Exception as e:
                print("ErrorStart-------------------------")
                print(e)
                print("put弹幕队列异常")
                print(queData)
                print("错误"+str(danmuQue.full()))
                print("错误"+str(danmuQue.empty()))
                print("后弹幕队列容量："+str(danmuQue.qsize()))
                print("ErrorEnd-------------------------")

    async def _on_gift(self, client: blivedm.BLiveClient, message: blivedm.GiftMessage):
        if message.coin_type == 'gold':
            print(f'礼物：：[{client.room_id}] {message.uname} 赠送{message.gift_name}x{message.num}'
                  f' （{message.coin_type}瓜子x{message.total_coin}）')
            price = message.total_coin/1000
            if giftQue.full():
                giftQue.get(False, 1)
            if price > 1:
                queData = {"name": message.uname, "type": 'gift', 'num': message.num,
                           'action': message.action, 'msg': message.gift_name, 'price': price}
                giftQue.put((999999-price+random.random(), queData), True, 1)

    async def _on_buy_guard(self, client: blivedm.BLiveClient, message: blivedm.GuardBuyMessage):
        print(f'上舰：：[{client.room_id}] {message.username} 购买{message.gift_name}')
        queData = {"name": message.username, "type": 'guard', 'num': 1,
                   'action': '上', 'msg': '-1', 'price': message.price/1000}
        guardQue.put((message.guard_level+random.random(), queData))

    async def _on_super_chat(self, client: blivedm.BLiveClient, message: blivedm.SuperChatMessage):
        print(
            f'SC：：[{client.room_id}] 醒目留言 ¥{message.price} {message.uname}：{message.message}')
        # 名称、类型、数量、动作、消息、价格
        queData = {"name": message.uname, "type": 'sc', 'num': 1,
                   'action': '发送', 'msg': message.message, 'price': message.price}
        scQue.put((999999-message.price+random.random(), queData))


# 配置文件、当前文本、excel（对话列表数据库）、敏感词文本
configINI = 'cconfig/onfig.ini'
currTXT = 'output/currText.txt'
xlslPATH = 'output/record.xlsx'
sensitiveTXT = 'config/sensitive_words.txt'
if os.path.exists('config/my_config.ini'):
    configINI = 'config/my_config.ini'
if os.path.exists('config/my_sensitive_words.txt'):
    sensitiveTXT = 'config/my_sensitive_words.txt'

con = configparser.ConfigParser()
con.read(configINI, encoding='utf-8')
mainConfig = dict(con.items('main'))

# 配置openai
openai.api_key = mainConfig['key']
openai.api_base = mainConfig['proxy_domain']
baseContext = [{"role": "system", "content": mainConfig['nya1']}]
contextMessage = []
tempMessage = []  # 最大3条上下文

# 敏感词
sensitiveF = open(sensitiveTXT, 'r', encoding='utf-8')
hzSensitiveWord = sensitiveF.readlines()
pySensitiveWord = []
for i in range(len(hzSensitiveWord)):
    hzSensitiveWord[i] = hzSensitiveWord[i].replace('\n', '')
    pySensitiveWord.append(str.join('', lazy_pinyin(hzSensitiveWord[i])))

# bilibili
roomID = json.loads(str(requests.get('https://api.live.bilibili.com/room/v1/Room/get_info?room_id=' +
                                     mainConfig['roomid']).content, encoding="utf-8"))['data']['room_id']
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
    _thread.start_new_thread(chatgpt35, ())
    _thread.start_new_thread(asyncio.get_event_loop(
    ).run_until_complete, (run_single_client(),))
    print('All subprocesses start.')
    time.sleep(2)
    input('input to exit::\n')
    isRun = False
    print('All subprocesses done.')
