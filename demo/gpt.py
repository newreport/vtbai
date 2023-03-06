import os
import openai

openai.api_key = "your key"
conversation=[{"role":"system","content":"你好，现在几点？天气如何"}]
response=openai.ChatCompletion.create(  model="gpt-3.5-turbo",messages=conversation)
print(response['choices'][0]['message']['content'])
