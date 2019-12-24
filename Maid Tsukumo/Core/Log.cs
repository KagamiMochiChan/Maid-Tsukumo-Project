using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace nse
{
    /// <summary>
    /// システムログ関係の処理
    /// </summary>
    public class Log
    {

        /// <summary>
        /// システムログのランクを表すENUM
        /// </summary>
        public enum Level{
            DEBUG,
            INFO,
            WARNING,
            ERROR,
            FATAL
        };

        /// <summary>
        /// システム系列の記録用
        /// </summary>
        public static List<(DateTime, Level, string, string)> system = new List<(DateTime, Level, string, string)>();
        public static void Print(Level level, string type, string text)
        {
            DateTime now = DateTime.Now;
            StreamWriter writer = new StreamWriter($@"{Environment.CurrentDirectory}\書庫\履歴\system\{now.ToString("yyyy-MM-dd")}.txt", true, Encoding.UTF8);
            writer.WriteLine($"[{now.ToString("yyyy/MM/dd HH:mm:ss.ffff")}][{level.ToString()}][{type}]{text}");
            writer.Close();
            system.Add((now,level, type, text));
        }

        public static new string ToString()
        {
            string reply = string.Empty;
            foreach ((DateTime now, Level level, string type, string text) in system)
                reply += $"[{now.ToString("yyyy/MM/dd HH:mm:ss.ffff")}][{level.ToString()}][{type}]{text}\n";
            return reply;
        }
    }

    /// <summary>
    /// ショートカット
    /// </summary>
    public class L
    {
        public Log l;
        public L()
        {
            l = new Log();
        }

        public static void D(string type, string text) => Log.Print(Log.Level.DEBUG, type, text); // デバッグ
        public static void I(string type, string text) => Log.Print(Log.Level.INFO, type, text); // システム
        public static void W(string type, string text) => Log.Print(Log.Level.WARNING, type, text); // 少々注意
        public static void E(string type, string text) => Log.Print(Log.Level.ERROR, type, text); // ミスった
        public static void F(string type, string text) => Log.Print(Log.Level.FATAL, type, text); // 致命的失敗
    }
}
