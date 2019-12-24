# -*- coding: utf-8 -*-
import log as L
import socket
import discord
from discord.ext import commands

def eden():
    async def predicate(ctx:commands.Context):
        return ctx.channel.id == 649302464910065719 and ctx.guild.id == 533310245565628416
    return  commands.check(predicate)

class Brain(commands.Cog,name="BRAIN"):
    def __init__(self, bot: commands.Bot):
        self.bot = bot

    def sending(self,msg:str) -> str:
        L.W(f"Sending Request <{msg}>")
        with socket.socket(socket.AF_INET,socket.SOCK_STREAM) as s:
            s.connect(("localhost",23145))
            L.W(f"Connecting <{s.getsockname()}>")
            s.send((msg + "\x1a").encode("utf-8"))
            L.W("Reply Waiting")
            re = s.recv(1024).decode("utf-8").replace("\x1a","")
            L.W(f"Receive {re}")
        return re

    @commands.Cog.listener(name="on_message")
    @eden()
    async def receive(self,msg:discord.message):
        if msg.author == self.bot.user or msg.content.startswith("/"):
            return
        else:
            L.W(f"Receive <{msg.content}>")
            await msg.channel.send(self.sending(":TALK:" + msg.content))

    @commands.command(name="send")
    @commands.is_owner()
    async def send(self,ctx:commands.Context,*args:str):
        await ctx.send(self.sending(" ".join(args)))

def setup(bot:commands.Bot):
    bot.add_cog(Brain(bot))
