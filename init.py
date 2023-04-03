import os
import urllib.request as urllib2
from shutil import copyfile

dirs = ['models','output']

for dir in dirs:
    if not os.path.exists(dir):
        os.makedirs(dir)

filenames = ['model.onnx','model.pth','config.json']

# https://huggingface.co/newreport/live_tts_default_model/resolve/main/model.onnx
# https://huggingface.co/newreport/live_tts_default_model/resolve/main/model.pth
# https://huggingface.co/newreport/live_tts_default_model/resolve/main/config.json
url='https://huggingface.co/newreport/live_tts_default_model/resolve/main/'

for filename in filenames:
    if not os.path.exists('models/'+filename):
          print("downloading..."+url+filename)
          f = urllib2.urlopen(url+filename) 
          with open("models/"+filename, "wb") as code:
            code.write(f.read())

configs =['config/my_config.ini','config/my_sensitive_words.txt']
for config in configs:
    if not os.path.exists(config):
        filename=config.replace("my_","")
        copyfile(filename,config)