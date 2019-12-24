using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spell
{
    class Display : Spell
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern IntPtr SendMessage(int hWnd, uint Msg, int wParam, int lParam);
        public override bool exe(List<string> words)
        {
            int i = words[0].ToUpper() switch
            {
                "SAVE" => 1,
                "OFF" => 2,
                "ON" => -1,
                _ => 0,
            };
            if (i == 0) return false;
            SendMessage(-1, 0x112, 0xf170, i);
            return true;
        }
    }
}
