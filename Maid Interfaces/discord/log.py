import datetime
dir = "C:\\Users\\nizir\\Desktop\\Maid Tsukumo\\書庫\\履歴\\discord\\"
def W(content:str,Level:str="DEBUG",Type:str="DISCORD"):
    if "\n" in content:
        content = content.replace("\n","\\n")
    now = datetime.datetime.now()
    with open(dir + now.strftime("%Y-%m-%d.txt"),mode="a",encoding="utf-8") as f:
        s = f"[{now.strftime('%Y/%m/%d %H:%M:%S.%f')[:-2]}][{Level}][{Type}]{content}"
        print(s)
        f.writelines(s+"\n")
