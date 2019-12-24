# -*- coding: utf-8 -*-
import log as L
L.W("Starting",Level="INFO")
L.W("Loading Library")
from discord.ext import commands
from  cogs import system,brain

L.W("Setup Bot")
BOT = commands.Bot(command_prefix="/")
L.W("Remove Help")
BOT.remove_command("help")

L.W("Loading <System>")
BOT.add_cog(system.System(bot=BOT))
L.W("Loading <Brain>")
BOT.add_cog(brain.Brain(bot=BOT))

L.W("Bot Runnning",Level="INFO")
BOT.run("NjQ1MTI3OTA2NDIyNTU0NjM0.Xd4vrQ.npS_XJWz2THA72Y-kNuDHANcYj8")