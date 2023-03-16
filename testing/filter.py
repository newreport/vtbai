from pypinyin import lazy_pinyin

# 敏感词检测 同音字过滤
f = open(r'sensitive_words.txt', 'r', encoding='utf-8')

hz = f.readlines()
py = []
for i in range(len(hz)):
    hz[i] = hz[i].replace('\n', '')
    py.append(str.join('', lazy_pinyin(hz[i])))
# print(hz)
# print(py)


readLine = True


def filter_text(text):
    textPY=str.join('',lazy_pinyin(text))
    print(textPY)
    for i in range(len(hz)):
        if hz[i] in text or py[i] in textPY:
            return False
    return True

while readLine:
    inputStr = input('请输入：')
    if inputStr == 'q':
        readLine = False
    state = filter_text(inputStr)
    print(inputStr+':'+str(state))
