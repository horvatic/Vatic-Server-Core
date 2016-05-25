﻿using System;
using System.IO;
using System.Net;
using System.Text;

namespace Server.Core
{
    public class Program
    {

        public static int Main(string[] args)
        {
            RunServer(MakeServer(args));
            return 0;
        }

        public static void RunServer(IMainServer runningServer)
        {

            if (runningServer == null) return;
            Console.WriteLine("Server Running...");
            do
            {
                runningServer.Run();
            } while (runningServer.AccectingNewConn);

            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
            {
                e.Cancel = true;
                runningServer.StopNewConn();
                Console.WriteLine("Server Shuting Down...");
                runningServer.CleanUp();
            };
        }

        public static IMainServer MakeServer(string[] args)
        {
            try
            {
                switch (args.Length)
                {
                    case 2:
                        return HelloWorldServer(args);
                    case 4:
                        return DirectoryServer(args);
                    default:
                        Console.WriteLine(WrongNumberOfArgs());
                        return null;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Another Server is running on that port");
                return null;
            }
        }

        public static IMainServer MakedirectoryServer(string chosenPort, string homeDirectory)
        {
            var cleanHomeDir = homeDirectory.Replace('\\', '/');
            var port = PortWithinRange(chosenPort);
            if (port == -1) return null;
            if (!VaildDrive(cleanHomeDir)) return null;
            var endPoint = new IPEndPoint((IPAddress.Loopback), port);
            var manager = new DataManager(new SocketProxy(), endPoint);
            return new MainServer(manager, new WebPageMaker(port), cleanHomeDir, new DirectoryProxy(),
                new FileProxy());
        }


        public static IMainServer DirectoryServer(string[] args)
        {
            if (args[0] == "-p" && args[2] == "-d")
            {
                return MakedirectoryServer(args[1], args[3]);
            }
            if (args[2] == "-p" && args[0] == "-d")
            {
                return MakedirectoryServer(args[3], args[1]);
            }
            Console.WriteLine(InvaildOption());
            return null;
        }

        public static IMainServer HelloWorldServer(string[] args)
        {
            if (args[0] != "-p") return null;
            var port = PortWithinRange(args[1]);
            if (port == -1) return null;
            var endPoint = new IPEndPoint((IPAddress.Loopback), port);
            var manager = new DataManager(new SocketProxy(), endPoint);
            return new MainServer(manager, new WebPageMaker(), null, new DirectoryProxy(), new FileProxy());
        }
        private static bool VaildDrive(string dir)
        {
            if (Directory.Exists(dir))
            {
                return true;
            }
            else
            {
                Console.WriteLine("Not a vaild directory");
                return false;
            }
        }
        private static int PortWithinRange(string port)
        {
            int portconvert;
            if (int.TryParse(port, out portconvert))
            {
                if (portconvert <= 1999 || portconvert >= 65001)
                {
                    Console.Write(GetInvaildPortError());
                    return -1;
                }
                else
                {
                    return portconvert;
                }
            }
            else
            {
                Console.Write(GetInvaildPortError());
                return -1;
            }

        }

        private static string WrongNumberOfArgs()
        {
            var error = new StringBuilder();
            error.Append("Invaild Number of Arguments.\n");
            error.Append("Can only be -p PORT\n");
            error.Append("or -p PORT -d DIRECTORY\n");
            error.Append("Examples:\n");
            error.Append("Server.exe -p 8080 -d C:/\n");
            error.Append("Server.exe -d C:/HelloWorld -p 5555\n");
            error.Append("Server.exe -p 9999");

            return error.ToString();
        }

        private static string GetInvaildPortError()
        {
            var error = new StringBuilder();
            error.Append("Invaild Port Detected.");
            error.Append("Vaild Ports 2000 - 65000");

            return error.ToString();

        }
        private static string InvaildOption()
        {
            var error = new StringBuilder();
            error.Append("Invaild Input Detected.\n");
            error.Append("Can only be -p\n");
            error.Append("or -p -d\n");
            error.Append("Examples:\n");
            error.Append("Server.exe -p 8080 -d C:/\n");
            error.Append("Server.exe -d C:/HelloWorld -p 5555\n");
            error.Append("Server.exe -p 9999");

            return error.ToString();
        }
    }
}