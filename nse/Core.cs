using System;

namespace nse
{

    /// <summary>
    /// 内部スクリプト"祝詞"を処理するためのプログラム
    /// </summary>
    class Core
    {
        /// <summary>
        /// Main
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            if(args == null)
            {
                while (true) ;
            }
            else
            {
                foreach(string arg in args)
                {
                    if (arg.StartsWith('-') || arg.StartsWith('/'))
                    {
                        switch(arg[1])
                        {
                            case 'w':
                                
                                break;
                        }
                    }

                }
            }
        }

        public void Execute(string s)
        {

        }
    }
}
