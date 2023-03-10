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
import xlrd
import xlwt
from xlutils.copy import copy


# 配置文件、弹幕房间、chatgpt配置
file = 'config.ini'
tempFile = 'output/temp.txt'
currTxt = 'output/currText.txt'
xlslPath = 'output/record.xlsx'

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
response = openai.ChatCompletion.create(model="gpt-3.5-turbo", messages=baseContext)
time.sleep(1)
print("主线程"+response['choices'][0]['message']['content'])

message = []
if os.path.exists(xlslPath) == False:
    workbook = xlwt.Workbook()
    sheet = workbook.add_sheet("test")  # 在工作簿中新建一个表格
    workbook.save(xlslPath)
    print("xls格式表格初始化成功！")


def write_excel_xls_append(value):
    index = len(value)  # 获取需要写入数据的行数
    workbook = xlrd.open_workbook(xlslPath)  # 打开工作簿

    sheets = workbook.sheet_names()  # 获取工作簿中的所有表格
    rows_old = 0
    sheetName = str(datetime.date.today())
    if sheetName in sheets:
        worksheet = workbook.sheet_by_name(sheetName)
        rows_old = worksheet.nrows  # 获取表格中已存在的数据的行数
    new_workbook = copy(workbook)  # 将xlrd对象拷贝转化为xlwt对象
    if sheetName not in sheets:
        new_workbook.add_sheet(sheetName)
    new_worksheet = new_workbook.get_sheet(0)  # 获取转化后工作簿中的第一个表格
    for i in range(0, index):
        for j in range(0, len(value[i])):
            # 追加写入数据，注意是从i+rows_old行开始写入
            new_worksheet.write(i+rows_old, j, value[i][j])
    new_workbook.save(xlslPath)  # 保存工作簿
    print("xls格式表格【追加】写入数据成功！")


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
    responseText = response['choices'][0]['message']['content']
    print("从gpt接收::"+responseText)
    ### 报错
    vits.generated_speech(str(responseText))
    write_keyboard_text(str(responseText))
    # pw = Process(target=write_keyboard_text, args=(str(responseText),))
    # pw.start()
    playsound('output/temp1.wav')
    with open('output/'+str(datetime.date.today())+'.txt', 'a') as a:
        a.write(str(datetime.datetime.now())+"::接收::"+str(responseText)+'\n')
        a.flush()
        write_excel_xls_append(
            [str(datetime.datetime.now()), "gpt", str(responseText)])


def chatgpt35():
    print("运行gpt")
    while True:
        if topQue.qsize() > 0:
            txt = topQue.get()
            write_excel_xls_append([str(datetime.datetime.now()), "top", txt])
            send_msg(txt)
        elif giftQue.qsize() > 0:
            txt = giftQue.get()
            write_excel_xls_append([str(datetime.datetime.now()), "gift", txt])
            send_msg(txt)
        elif scQue.qsize() > 0:
            txt = scQue.get()
            write_excel_xls_append([str(datetime.datetime.now()), "sc", txt])
            send_msg(txt)
        elif danmuQue.qsize() > 0:
            txt = danmuQue.get()
            write_excel_xls_append(
                [str(datetime.datetime.now()), "danmu", txt])
            send_msg(txt)
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



if __name__ == '__main__':
    print('当前进程id::'+str(os.getpid()))
    _thread.start_new_thread(chatgpt35, ())
    _thread.start_new_thread(sync, (room.connect(),))
    input('input q to ecs')
    print('All subprocesses done.')


# _thread.start_new_thread(sync, (room.connect(),()))
