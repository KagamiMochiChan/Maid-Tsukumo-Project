# -*- coding: utf-8 -*-
from turtledemo.paint import switchupdown

import log as L
import discord
from discord.ext import commands


class System(commands.Cog,name="SYSTEM"):
    def __init__(self, bot: commands.Bot):
        self.bot = bot
        self.activity = None
        self.status = discord.Status.online

    @commands.Cog.listener()
    async def on_ready(self):
        L.W("Login Success", Level="INFO")

    @commands.command(name="reload")
    @commands.is_owner()
    async def reload(self,ctx:commands.Context):
        await ctx.send("This Is Not Yet Implemented")

    @commands.command(name="state")
    @commands.is_owner()
    async def change_state(self,ctx:commands.Context,*args:str):
        L.W(f"Change State Message <{' '.join(args)}>",Level="INFO")
        self.activity = discord.Game(name=" ".join(args))
        await self.bot.change_presence(status=self.status,activity=self.activity)

    @commands.command(name="status")
    @commands.is_owner()
    async  def change_status(self,ctx:commands.Context,s:str):
        s = s.upper()
        if s in {"0","ONLINE"}:
            self.status = discord.Status.online
        elif s in {"1","OFFLINE"}:
            self.status = discord.Status.offline
        elif s in {"2","IDLE"}:
            self.status = discord.Status.idle
        elif s in {"3","DND"}:
            self.status = discord.Status.dnd
        elif s in {"4","INVISIBLE"}:
            self.status = discord.Status.invisible
        else:
            await ctx.send(f"{s} is wrong status")
            return
        L.W(f"Change Status <{self.status}>",Level="INFO")
        await self.bot.change_presence(status=self.status,activity=self.activity)

def setup(bot:commands.Bot):
    bot.add_cog(System(bot))
