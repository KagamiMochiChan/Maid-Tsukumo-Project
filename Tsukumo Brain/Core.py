# -*- coding: utf-8 -*-
from tkinter import messagebox
import sys
if __name__ == "__main__":
    sys.stdin = open(sys.stdin.fileno(), encoding="utf-8", closefd=False)
    sys.stdout = open(sys.stdout.fileno(), 'w', encoding="utf-8", closefd=False)

if __name__ == "__main__":
    input()
    print("LOG:INFO:Wake Up Brain")
    print("LOG:DEBUG:Loading Library")
import csv
import logging
import os
import typing

import MeCab
import numpy as np
from gensim.models.doc2vec import Doc2Vec, TaggedDocument

if __name__ == "__main__":
    print("Loading MeCab")
    mt = MeCab.Tagger('-Owakati')
    mt.parse('')

    pattern = dict()
    model = Doc2Vec.load("model")
    min_sim = 0.5

def load_Pattern(dir:str = "書庫\\辞書\\Tsukumo\\") -> None:
    for path in os.listdir(dir):
        print(f"Loading Pattern <{path[0:-4]}>")
        with open(dir + path,mode="r",encoding="utf-8") as f:
            reader = csv.reader(f)
            l = list()
            for row in reader:
                l.append([mt.parse(row[0]).split(" "),row[1],row[0]])
        pattern[path[0:-4]] = l

def learning_Model() -> None:
    print("Loading Files")
    global model
    docs = []
    livedoor = "書庫\\書物\\livedoor\\"
    for dir in os.listdir(livedoor):
        if os.path.isfile(livedoor + dir):
            continue
        print(f"Loading <{dir}>")
        for path in os.listdir(f"{livedoor}\\{dir}\\"):
            if os.path.isdir(f"{livedoor}\\{dir}\\{path}"):
                continue
            with open(f"{livedoor}\\{dir}\\{path}",mode="r",encoding="utf-8") as f:
                Lines = f.readlines()
                tag = Lines[0]
                for i in range(2):
                    Lines.remove(Lines[0])
                docs.append( TaggedDocument(words = mt.parse( str.join("\n",Lines)).split(" "),tags = tag))

    logging.basicConfig(format="%(message)s",level=logging.INFO)
    print("Learning Start!")
    model = Doc2Vec(documents=docs,dm=1,vector_size=300,window=16,min_count=10,workers=8,alpha=.00015)
    model.save("model")

def GetMost(word:str,type:str="talk") -> (float,int):
    words = mt.parse(word).split(" ")
    index = sim = 0
    for s in pattern[type]:
        i = model.docvecs.similarity_unseen_docs(model,words,s[0],alpha=.00015,steps=5)
        if sim < i:
            sim = i
            index = pattern[type].index(s)
    return (sim,index)

def AddDictionaly(key:str,value:str,type:str="talk"):
    with open(f"書庫\\辞書\\Tsukumo\\{type}.csv", mode="a", encoding="utf-8") as f:
        writer = csv.writer(f)
        writer.writerow([key,value])
    pattern[type].append([mt.parse(key).split(" "), value])

if __name__ == "__main__":
    load_Pattern()
    print("END")
    try:
        while True:
            s = input()
            print(f"Request <{s}>")
            r = "RETURN:"
            if s.startswith("TALK:"):
                w = s.split(":",3)
                if w[1] in pattern:
                    (sim,index) = GetMost(w[2],type=w[1])
                    print(f"Most Similar is {w[2]} <{sim}> {pattern[w[1]][index][2]} => {pattern[w[1]][index][1]}")
                    r += f"Most Similar is {w[2]} <{sim}> {pattern[w[1]][index][2]} => {pattern[w[1]][index][1]}" if sim < min_sim else pattern["talk"][index][1]
            elif s.startswith("EXIT"):
                break
            elif s.startswith("RELOAD"):
                load_Pattern()
            elif s.startswith("RELEARN"):
                learning_Model()
            elif s.startswith("SET"):
                w = s.split(":",3)
                AddDictionaly(key=w[2],value=w[3],type=w[1])
            else:
                (sim,index) = GetMost(s)
                print(f"Most Similar is {s} <{sim}> {pattern['talk'][index][2]} => {pattern['talk'][index][1]}")
                r += f"Most Similar is {s} <{sim}> {pattern['talk'][index][2]} => {pattern['talk'][index][1]}" if sim < min_sim else pattern["talk"][index][1]
            print("Request Waiting")
            print(r)
    except Exception:
        messagebox.showerror("Tsukumo Brain",f"ERROR:<{sys.exc_info()}>")
    messagebox.showerror("Tsukumo Brain","終了")