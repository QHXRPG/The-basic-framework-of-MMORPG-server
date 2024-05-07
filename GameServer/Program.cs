using GameServer.Network;
using Network;
using Proto.Message;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using static System.Net.Mime.MediaTypeNames;



namespace GameServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            NetService netserver = new NetService();
            netserver.Init(32510);
            netserver.Start();
            Console.ReadKey();
        }
    }
}
