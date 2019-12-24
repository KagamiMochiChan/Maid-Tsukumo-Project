using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;

namespace Script
{
    /// <summary>
    /// 祝詞の実行クラス
    /// </summary>
    class Executer
    {
        public Dictionary<string, Spell> spells { get; set; }
        private Dictionary<string, string> var { get; set; }
        private void AddVar(string key, string value)
        {
            var.Remove(key);
            var.Add(key, value);
        }

        public Executer()
        {
            spells = new Dictionary<string, Spell>();
            spells.Add("ECHO", new Echo());
            spells.Add("RUN", new Run());
            spells.Add("EXIT", new Exit());
            spells.Add("SYSTEMDOWN", new Systemdown());
            spells.Add("DISPLAY", new Display());

            var = new Dictionary<string, string>();
            using (var parser = new TextFieldParser($@"{Environment.CurrentDirectory}\.var", Encoding.UTF8))
            {
                parser.Delimiters = new string[] { "," };
                while (!parser.EndOfData)
                {
                    var fields = parser.ReadFields();
                    if (fields.Length >= 2)
                    {
                        AddVar(fields[0], fields[1]);
                    }
                }
            }

        }


        /// <summary>
        /// 実行オンリー
        /// </summary>
        /// <param name="words"></param>
        public string Exe(List<string> words)
        {
            // タイプの抜き出し
            string Type = words[0].ToUpper();
            words.RemoveAt(0);

            // 内部祝詞の検索
            if (spells.TryGetValue(Type, out Spell spell))
            {
                L.D("Executer", $"内部祝詞 {Type} を紡ぎます");
                return spells[Type].exe(words) ? spells[Type].reply : spells[Type].error;
            }

            // 外部祝詞の検索
            foreach (string path in Directory.GetFiles($@"{Environment.CurrentDirectory}\書庫\祝詞"))
            {
                if (Path.GetFileNameWithoutExtension(path).Equals(Type) || Path.GetFileName(path).Equals(Type))
                {
                    L.D("Executer", $"外部祝詞 {Type} を紡ぎます");
                    return SpellBook(Type, words);
                }
            }

            // 神具の検索
            // 直下だけ
            foreach (string path in Directory.GetFiles($@"{Environment.CurrentDirectory}\神具"))
            {
                if (Path.GetFileNameWithoutExtension(path).Equals(Type) || Path.GetFileName(path).Equals(Type + ".sb"))
                {
                    L.D("Executer", $"神具 {Type} を使います");
                    words.InsertRange(0, new string[2] { Type, "-A" });
                    return spells["RUN"].exe(words) ? spells["RUN"].reply : spells["RUN"].error;
                }
            }

            // サブディレクトリも検索
            foreach (string dpath in Directory.GetDirectories($@"{Environment.CurrentDirectory}\神具"))
            {
                foreach (string path in Directory.GetFiles(dpath))
                {
                    if (Path.GetFileNameWithoutExtension(path).Equals(Type) || Path.GetFileName(path).Equals(Type))
                    {
                        L.D("Executer", $"神具 {Type} を使います");
                        words.InsertRange(0, new string[2] { Type, "-A" });
                        return spells["RUN"].exe(words) ? spells["RUN"].reply : spells["RUN"].error;
                    }
                }
            }

            return "Not Found";
        }

        public string Analyze(string word)
        {
            L.D("EXECUTER", $"Analyze <{word}>");
            spells["EXIT"].reply = string.Empty;
            string[] s = Split(new char[] { ';' }, TranceVar(word)).ToArray();
            for (int i = 0; i < s.Length; i++)
            {
                string[] str = Split(new char[] { '|' }, s[i]).ToArray();
                for (int f = str.Length - 1; f >= 0; f--)
                {
                    List<string> words = Split(new char[] { ' ', '　', '\t', '\n', '\x1a' }, str[f]);
                    Exe(words);
                }

            }
            return spells["EXIT"].reply;
        }

        private string TranceVar(string word)
        {
            string reply = string.Empty;
            string buf = null;
            foreach (char c in word)
            {
                if (c == '%')
                {
                    if (buf == null)
                    {
                        buf = string.Empty;
                    }
                    else
                    {
                        reply += var.TryGetValue(buf, out string s) ? s : $"%{buf}%";
                        buf = null;
                    }
                }
                else if (buf != null)
                {
                    buf += c;
                }
                else
                {
                    reply += c;
                }
            }

            return reply;
        }

        private List<string> Split(char[] sep, string s)
        {
            List<string> pars = new List<string>(s.Split(sep, StringSplitOptions.RemoveEmptyEntries));

            for (int i = 0; i < pars.Count; i++)
            {
                if (pars[i].StartsWith("\"") && !pars[i].EndsWith("\""))
                {
                    pars[i] += sep[0] + pars[i + 1];
                    i--;
                }
            }

            return pars;
        }

        private string SpellBook(string path, List<string> words)
        {
            int i;
            string reply = string.Empty;
            for (i = 0; i < words.Count; i++)
            {
                AddVar(i.ToString(), words[i]);
            }
            if (File.Exists(path))
            {
                reply = Analyze(new StreamReader(path, Encoding.UTF8).ReadToEnd());
            }
            for (; i >= 0; i--)
            {
                var.Remove(i.ToString());
            }
            return reply;
        }
    }
}
