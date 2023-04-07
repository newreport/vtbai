# -*- coding: utf-8 -*-
import asyncio
import random
from queue import Queue, PriorityQueue
import sys
sys.path.append('../')
import blivedm.blivedm as blivedm


# 直播间ID的取值看直播间URL
TEST_ROOM_IDS = [
    47867,
]


async def main():
    await run_single_client()


async def run_single_client():
    room_id = random.choice(TEST_ROOM_IDS)
    # 如果SSL验证失败就把ssl设为False，B站真的有过忘续证书的情况
    client = blivedm.BLiveClient(room_id, ssl=True)
    handler = MyHandler()
    client.add_handler(handler)

    client.start()
    try:
        await client.join()
    finally:
        await client.stop_and_close()

# sc 队列
scQue = PriorityQueue(maxsize=0)
# 舰长队列
guardQue = PriorityQueue(maxsize=0)
# 礼物
giftQue = PriorityQueue(maxsize=5)
# 普通弹幕队列
danmuQue = PriorityQueue(maxsize=5)


class MyHandler(blivedm.BaseHandler):
    async def _on_danmaku(self, client: blivedm.BLiveClient, message: blivedm.DanmakuMessage):
        if message.dm_type == 0:
            print(f'弹幕：[{client.room_id}] {message.uname}：{message.msg}')
            # 权重计算
            privilege_type = message.privilege_type
            if privilege_type == 0:
                privilege_type = 9
            rank = (99999-message.user_level*10+(10-privilege_type)
                    * 10+message.mobile_verify*10)
            if danmuQue.full():
                danmuQue.get()
            danmuQue.put((rank, {"name": message.uname, "type": 'danmu',
                                'num': 1, 'action': '说', 'msg': message.msg, 'price': 0}))

    async def _on_gift(self, client: blivedm.BLiveClient, message: blivedm.GiftMessage):
        if message.coin_type == 'gold':
            print(f'礼物：[{client.room_id}] {message.uname} 赠送{message.gift_name}x{message.num}'
                  f' （{message.coin_type}瓜子x{message.total_coin}）')
            price = message.total_coin/1000
            if giftQue.full():
                giftQue.get()
            if price > 1:
                giftQue.put((999999-price, {"name": message.uname, "type": 'gift',
                                           'num': message.num, 'action': message.action, 'msg': '-1', 'price': price}))

    async def _on_buy_guard(self, client: blivedm.BLiveClient, message: blivedm.GuardBuyMessage):
        print(f'上舰：[{client.room_id}] {message.username} 购买{message.gift_name}')
        guardQue([message.guard_level, {
                 "name": message.username, "type": 'guard',
                 'num': 1, 'action': '上', 'msg': '-1', 'price': message.price/1000}])

    async def _on_super_chat(self, client: blivedm.BLiveClient, message: blivedm.SuperChatMessage):
        print(
            f'SC:[{client.room_id}] 醒目留言 ¥{message.price} {message.uname}：{message.message}')
        # 名称、类型、数量、动作、消息、价格
        scQue.put((999999-message.price, {"name": message.uname, "type": 'sc',
                  'num': 1, 'action': '发送', 'msg': message.message, 'price': message.price}))




      
if __name__ == '__main__':
     asyncio.run(main())
