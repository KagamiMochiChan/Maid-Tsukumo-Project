using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Script
{
    class Echo : Spell
    {
        public override bool exe(List<string> words)
        {
            L.I("ECHO", string.Join(" ", words));
            return true;
        }
    }

    class Exit : Spell
    {
        public override bool exe(List<string> words)
        {
            reply = string.Join(" ", words);
            return true;
        }
    }

    class Systemdown : Spell
    {
        public override bool exe(List<string> words)
        {
            Core.Core.Exit();
            return true;
        }
    }
}
