import test2
import multiprocessing
import time


print("主进程")
if __name__ == '__main__':
    multiprocessing.freeze_support()

    while True:
        p = multiprocessing.Process(target=test2.play)
        p.start()
        time.sleep(2)