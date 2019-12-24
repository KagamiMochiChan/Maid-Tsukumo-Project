using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using nse;

namespace Spell
{
    class Run : Spell
    {
        public override bool exe(List<string> words)
        {
            L.D("RUN", string.Join(" ", words));

            Process process = new Process();
            process.StartInfo.UseShellExecute = true;

            bool path = true;
            bool skip = false;

            for (int i = 0; i < words.Count; i++)
            {
                if (skip)
                    break;

                switch (words[i].ToUpper())
                {
                    case "-W":
                        switch (words[++i])
                        {
                            case "hide":
                                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                                break;
                            case "max":
                                process.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                                break;
                            case "min":
                                process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                                break;
                            default:
                                process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                                break;
                        }
                        break;
                    case "-A":
                        skip = true;
                        break;
                    default:
                        if (path)
                        {
                            process.StartInfo.FileName = words[i];
                            words.RemoveAt(i);
                            path = false;
                        }
                        break;
                }
            }

            process.StartInfo.Arguments = string.Join(" ", words);

            try
            {
                process.Start();
                return true;
            }
            catch (Exception e)
            {
                error = e.Message;
                return false;
            }
        }
        
    }
}
