import multiprocessing
from flask import Flask, request
import time

def func(num):
    # 共享数值型变量
    # num.value = 2

    # 共享数组型变量
    num[2] = 9999

def rot(num):
    app = Flask(__name__)
    print("run le")
    app.run, ("0.0.0.0", 8081)

    @app.route('/', methods=['GET'])
    def put():
        return '1'

if __name__ == '__main__':
    # 共享数值型变量
    # num = multiprocessing.Value('d', 1)
    # print(num.value)

    # 共享数组型变量
    num = multiprocessing.Array('i', [1, 2, 3, 4, 5])
    print(num[:])


    p = multiprocessing.Process(target=func, args=(num,))
    p.start()
    p.join()
    p1 = multiprocessing.Process(target=rot, args=(num,))
    p1.start()
    p1.join

    # 共享数值型变量
    # print(num.value)
    time.sleep(100)
    # 共享数组型变量
    print(num[:])