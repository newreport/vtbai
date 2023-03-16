import asyncio
import _thread
import multiprocessing
import time

def fun1():
    while True:
        print("fun1")
        time.sleep(1)
        

def fun2():
    print("fun2")
    time.sleep(10)

def fun3():
    while True:
        print("fun3")

p=multiprocessing.Process(target=fun2, args=())
p.start()
_thread.start_new_thread(fun1,())
_thread.start_new_thread(fun3,())
print("end1")
input("input to here .....")
print('end2')