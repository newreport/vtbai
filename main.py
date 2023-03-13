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
    new_worksheet.write(rows_old, 2, value['action'])
    new_worksheet.write(rows_old, 3, value['msg'])
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
    sendGptMsg = msg['name']+msg['action']+msg['msg']
    print("向gpt发送::"+sendGptMsg)
    # 对话日志
    with open('output/'+str(datetime.date.today())+'.txt', 'a') as a:
        a.write(str(datetime.datetime.now())+"::发送::"+sendGptMsg+'\n')
        a.flush()
        write_excel_xls_append({
            'datetime': str(datetime.datetime.now()),
            'user': msg['name'],
            'action': msg['action'],
            'msg': msg['msg']
        })
    # 发送给gpt
    tempMessage.append({"role": "user", "content": sendGptMsg})
    if len(tempMessage) > 3:
        del(tempMessage[0])
    message = baseContext+tempMessage
    print(message)

    # 接收
    response = openai.ChatCompletion.create(
        model="gpt-3.5-turbo", messages=message)
    responseText = str(response['choices'][0]['message']['content'])
    print("从gpt接收::"+responseText)

    # 生成发送语言
    sendVitsMsg = msg['msg']
    vits.generated_speech(sendVitsMsg, 'sendVits.wav')
    # 生成接收语言
    vits.generated_speech(responseText, 'recVits.wav')
    write_keyboard_text(responseText)

    playsound('output/sendVits.wav')
    time.sleep(0.5)
    playsound('output/recVits.wav')

    # 对话日志
    with open('output/'+str(datetime.date.today())+'.txt', 'a') as a:
        a.write(str(datetime.datetime.now())+"::接收::"+responseText+'\n')
        a.flush()
        write_excel_xls_append({
            'datetime': str(datetime.datetime.now()),
            'user': 'gpt35',
            'action': '说',
            'msg': responseText
        })


def chatgpt35():
    print("运行gpt循环任务")
    while isRun:
        msgs = {"name": ''}
        if topQue.qsize() > 0:
            # 从队列获取信息
            msgs = topQue.get()
        elif giftQue.qsize() > 0:
            msgs = giftQue.get()
        elif scQue.qsize() > 0:
            msgs = scQue.get()
        elif danmuQue.qsize() > 0:
            msgs = danmuQue.get()
        # 过滤队列
        if len(msgs['name']) > 0 and filter_text(msgs['name']) and filter_text(msgs['msg']):
            send2gpt(msgs)
            msgs = {"name": ''}
        else:
            time.sleep(1)


def filter_text(text):
    textPY = str.join('', lazy_pinyin(text))
    for i in range(len(hzSensitiveWord)):
        if hzSensitiveWord[i] in text or pySensitiveWord[i] in textPY:
            return False
    return True


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
room = live.LiveDanmaku(json.loads(str(requests.get('https://api.live.bilibili.com/room/v1/Room/get_info?room_id=' +
                                                    mainConfig['roomid']).content, encoding="utf-8"))['data']['room_id'])
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
scQue = PriorityQueue(maxsize=0)
giftQue = PriorityQueue(maxsize=10)
danmuQue = PriorityQueue(maxsize=3)
topIDs = mainConfig['topid'].split(',')
# excel数据库
if os.path.exists(xlslPATH) == False:
    workbook = xlwt.Workbook()
    sheet = workbook.add_sheet("test")  # 在工作簿中新建一个表格
    workbook.save(xlslPATH)
    print("xls格式表格初始化成功！")
    print('当前进程id::'+str(os.getpid()))


@ room.on('DANMU_MSG')
async def on_danmaku(event):
    if mainConfig['env'] == 'dev':
        print('弹幕当前进程id::'+str(os.getpid()))
    # 收到弹幕
    name = event['data']['info'][2][1]
    msg = event['data']['info'][1]
    msgType = event['data']['info'][0][12]
    if mainConfig['env'] == 'dev':
        with open(tempTXT, 'a') as a:
            a.write(str(event)+'\n')
            a.flush()
    # print('弹幕::'+str(msg))
    if msgType == 0:
        danmuQue.put({"name": name, "action": '说', "msg": msg})
    elif msgType == 1:
        print('表情::'+str(msg))
    else:
        print('其他::'+str(msg))


@ room.on('SEND_GIFT')
async def on_gift(event):
    if mainConfig['env'] == 'dev':
        print('礼物当前进程id::'+str(os.getpid()))
    # 收到礼物
    if event['data']['data']['batch_combo_send'] is not None:
        name = event['data']['data']['batch_combo_send']['uname']
        action = event['data']['data']['batch_combo_send']['action']
        # num = event['data']['data']['batch_combo_send']['gift_num']
        gift = event['data']['data']['batch_combo_send']['gift_name']
        scQue.put({"name": name, "action": action, "msg": gift}, 9)
        if mainConfig['env'] == 'dev':
            with open('girf.txt', 'a') as a:
                a.write(str(event)+'\n')
                a.flush()
if __name__ == '__main__':
    isRun = True
    _thread.start_new_thread(chatgpt35, ())
    sync(room.connect())
    _thread.start_new_thread(sync, (room.connect(),))
    print('All subprocesses start.')
    time.sleep(2)
    input('input to exit::')
    isRun = False
    print('All subprocesses done.')
