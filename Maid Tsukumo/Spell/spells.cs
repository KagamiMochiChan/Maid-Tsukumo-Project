using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using nse;

namespace Spell
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
            nse.Core.Exit();
            return true;
        }
    }
}
