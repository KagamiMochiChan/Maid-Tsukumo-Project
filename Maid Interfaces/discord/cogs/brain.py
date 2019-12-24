# -*- coding: utf-8 -*-
import log as L
import socket
import discord
from discord.ext import commands
from cogs import core


def eden():
    async def predicate(ctx: commands.Context):
        return ctx.channel.id == 649302464910065719 and ctx.guild.id == 533310245565628416

    return commands.check(predicate)


class Brain(commands.Cog, name="BRAIN"):
    def __init__(self, bot: commands.Bot):
        core.load_Pattern()
        self.bot = bot

    def sending(self, msg: str) -> str:
        L.W(f"Sending Request <{msg}>")
        with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
            s.connect(("localhost", 23145))
            L.W(f"Connecting <{s.getsockname()}>")
            s.send((msg + "\x1a").encode("utf-8"))
            L.W("Reply Waiting")
            re = s.recv(1024).decode("utf-8").replace("\x1a", "")
            L.W(f"Receive {re}")
        return re

    @commands.Cog.listener(name="on_message")
    @eden()
    async def receive(self, msg: discord.message):
        if msg.author == self.bot.user or msg.content.startswith("/"):
            return
        else:
            L.W(f"Receive <{msg.content}>")
            (tf, ti) = core.GetMost(word=msg.content)
            (df, di) = core.GetMost(word=msg.content, type="discord")
            (sim, index, dic) = (tf, ti, "talk") if tf > df else (df, di, "discord")
            if sim < core.min_sim:
                await msg.channel.send(
                    f"最小値{core.min_sim}を満たしませんでした\n> **最類似**\n> **辞書** {dic} **Index** {index}\n> **Cos類似度** {sim}\n> **Key** {core.pattern[dic][index][2]} **Value** {core.pattern[dic][index][1]}")
            else:
                await msg.channel.send(core.pattern[dic][index][1])

    @commands.command(name="add")
    @commands.is_owner()
    async def send(self, ctx: commands.Context, key: str, value: str, model: str = "talk"):
        core.AddDictionaly(key=key, value=value, type=model)
        ctx.send(f"辞書に新規登録しました\n> 辞書 {model}\n> **Key** {key} **Value** {value}")


def setup(bot: commands.Bot):
    bot.add_cog(Brain(bot))
