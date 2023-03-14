from bilibili_api import live, sync
import json
import requests


env = 'pro'  # dev|pro

roomID = ''
# 获取房间真实 id
room = live.LiveDanmaku(json.loads(str(requests.get('https://api.live.bilibili.com/room/v1/Room/get_info?room_id=' +
                        roomID).content, encoding="utf-8"))['data']['room_id'])

@room.on('DANMU_MSG')
async def on_danmaku(event):
    print(event)
    # 收到弹幕
    name = event['data']['info'][2][1]
    msg = event['data']['info'][1]
    msgs = name+"说："+msg
    msgType = event['data']['info'][0][12]
    if env == 'dev':
        with open('danmu.txt', 'a') as a:
            a.write(str(event)+'\n')
            a.flush()
    if msgType == 0:
        m = 1
        print("弹幕::"+msgs)
    elif msgType == 1:
        print('表情::'+msgs)
    else:
        print('其他::'+msgs)

@room.on('SEND_GIFT')
async def on_gift(event):
    print(event)
    # 收到礼物
    if event['data']['data']['batch_combo_send'] is not None:
        name = event['data']['data']['batch_combo_send']['uname']
        action = event['data']['data']['batch_combo_send']['action']
        num = event['data']['data']['batch_combo_send']['gift_num']
        gift = event['data']['data']['batch_combo_send']['gift_name']
        msgs = name+action+str(num)+'个'+gift
        print("礼物::"+msgs)
        if env == 'dev':
            with open('gift.txt', 'a') as a:
                a.write(str(event)+'\n')
                a.flush()
if __name__ == '__main__':
    sync(room.connect())
