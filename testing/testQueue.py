from queue import Queue, PriorityQueue
import _thread
import datetime
import time

q=PriorityQueue(maxsize=20)


def Put(vau):
    while True:
        if q.qsize()>10:
            print("is full")
            try:
                q.get(True,1)
            except:
                q.put(vau,vau)
        q.put(vau,vau)
        # time.sleep(1)

def Get():
    while True:
        str1=q.get()
        print(str(datetime.datetime.now())+':'+str(str1))
        # time.sleep(1)

_thread.start_new_thread(Get, ())
_thread.start_new_thread(Put, (2,))
_thread.start_new_thread(Put, (1,))
input()