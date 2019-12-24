using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

using Spell;

namespace nse
{
    class Core
    {
        public static L l;
        public static Executer e;
        private static Process brain;
        private static Process Interface;

        private static TcpListener listener;
        private static TcpClient client;
        private static NetworkStream network;

        private static Semaphore semaphore;

        [STAThread]
        public static int Main(string[] args)
        {
            if (args == null || args.Length == 0)
                args = new string[] { "" };

            if(Semaphore.TryOpenExisting("Tsukumo", out Semaphore semaphore))
            {


                TcpClient client = new TcpClient("localhost", 23145);
                NetworkStream network = client.GetStream();
                byte[] b = Encoding.UTF8.GetBytes(":START:" + string.Join(" ", args) + '\x1a');
                network.Write(b, 0, b.Length);
                network.Close();
                client.Close();
                return 0;
            }
            Core.semaphore = new Semaphore(1, 1, "Tsukumo");

            L.I("CORE","System Booting");

            L.D("CORE","Loading Logger");
            l = new L();

            L.D("CORE", "Loading Executer");
            e = new Executer();

            L.D("CORE", "Starting Up");
            e.Analyze("startup");
            Interface = Process.Start(new ProcessStartInfo().FileName = "Console.exe");

            L.I("CORE", "Brain Booting");
            brain = new Process();
            brain.StartInfo.FileName = @"C:\ProgramData\Anaconda3\python.exe";
            brain.StartInfo.Arguments = @"""Tsukumo Brain\Core.py""";
            brain.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
            brain.StartInfo.UseShellExecute = false;
            brain.StartInfo.RedirectStandardInput = true;
            brain.StartInfo.RedirectStandardOutput = true;
            brain.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            brain.Exited += Dead;
            L.D("CORE", "Process Start");
            brain.Start();
            Send("");
            Response();

            L.I("CORE", "Opening Transmission Control Protocol Server");
            Server();

            listener.Start();
            L.D("CORE", $"Listen Start <{((IPEndPoint)listener.LocalEndpoint).Address.ToString()}:{((IPEndPoint)listener.LocalEndpoint).Port.ToString()}>");
            while (true)
            {
                L.D("CORE", "Access Waiting");
                client = listener.AcceptTcpClient();
                L.D("CORE", "Connected");

                network = client.GetStream();

                MemoryStream memory = new MemoryStream();
                byte[] b = new byte[1024];
                int size = 0;
                do
                {
                    size = network.Read(b, 0, b.Length);
                    if (size == 0)
                        break;
                    memory.Write(b, 0, size);
                } while (network.DataAvailable || b[size - 1] != '\x1a');
                string re = Encoding.UTF8.GetString(memory.GetBuffer(), 0, (int)memory.Length).TrimEnd('\x1a');

                L.D("CORE", $"Receive <{re}>");
                string reply = Receive(re);
                if (reply != "")
                {
                    L.D("CORE", $"Sending <{reply}>");
                    b = Encoding.UTF8.GetBytes(reply + '\x1a');
                    network.Write(b, 0, b.Length);
                }
                network.Close();
                client.Close();
            }
        }

        static void Send(string s)
        {
            brain.StandardInput.WriteLine(s);
            brain.StandardInput.Flush();
        }
        static string Response()
        {
            while (true)
            {
                string re = brain.StandardOutput.ReadLine();
                if (re == null) continue;
                List<string> s = new List<string>(re.Split(':'));
                switch (s[0])
                {
                    case "LOG":
                        string rank = s[1];
                        s.RemoveRange(0, 2);
                        switch (rank)
                        {
                            case "DEBUG":
                                L.D("BRAIN", string.Join(":", s));
                                break;
                            case "INFO":
                                L.I("BRAIN", string.Join(":", s));
                                break;
                        }
                        break;
                    case "RETURN":
                        if (s.Count > 1)
                        {
                            s.RemoveAt(0);
                            return string.Join(":", s);
                        }
                        return string.Empty;
                    case "END":
                        return string.Empty;
                    default:
                        L.D("BRAIN", string.Join(":", s));
                        break;
                }
            }
            
        }
        private static string Receive(string re)
        {
            if (re.StartsWith(":START:"))
            {
                re.Remove(0, 7);
            }
            else if (re.StartsWith(":SPELL:"))
            {
                return e.Analyze(re.Remove(0, 7));
            }
            else if (re.StartsWith(":TALK:"))
            {
                brain.StandardInput.WriteLineAsync(re.Remove(0, 6));
                return Response();
            }
            else if (re.StartsWith(":LOG:"))
            {
                return Log.ToString();
            }
            return string.Empty;
        }
        private static void Server()
        {
            IPAddress address = Dns.GetHostEntry("localhost").AddressList[0];
            L.D("CORE", $"Getting IPAdress <{address.ToString()}>");

            L.D("CORE", "Opening Listener");
            listener = new TcpListener(IPAddress.Any, 23145);
        }
        public static void Exit()
        {
            semaphore.Close();
            network.Close();
            client.Close();
            listener.Stop();
            L.I("CORE", "System Down");
            Environment.Exit(0);
        }

        public static void Dead(object o,EventArgs e)
        {
            Exit();
        }
    }
}
