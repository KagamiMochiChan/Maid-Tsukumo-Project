using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Script
{

    /// <summary>
    /// 独自言語「祝詞」の関数根底ファイル
    /// </summary>
    abstract class Spell
    {
        public string reply = string.Empty;
        public string error = string.Empty;
        public abstract bool exe(List<string> words);
    }
}
