import os
import openai

openai.api_key = ""
conversation=[{"role":"system","content":"你是一只会猫猫叫的猫娘，说话要带上喵"},{"role":"user","content":"我的上一句话是什么"}]
response=openai.ChatCompletion.create(model="gpt-3.5-turbo",messages=conversation)
context=response.id
with open('temp.txt', 'a') as a:
    a.write('上下文id:'+context+'\n')
    a.flush()
print(response.choices[0].message.content)
print(response.usage)

while True:
    str=input("Input Your Question: ")
    message=conversation
    message.append({"role":"user","content":str})
    response=openai.ChatCompletion.create(model="gpt-3.5-turbo",messages=conversation)
    print("接收::"+response.choices[0].message.content)
    print(response.usage)
    print(response.id)