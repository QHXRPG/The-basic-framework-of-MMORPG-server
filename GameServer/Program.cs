using Common.Network;
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

            MessageRouter.Instance.Start(4);  // 启动消息分发器，开四个线程

            // 消息订阅：用户登录消息
            MessageRouter.Instance.On<UserLoginRequest>(OnUserLoginRequest);

            Console.ReadKey();
        }

        private static void OnUserLoginRequest(NetConnection sender, UserLoginRequest msg)
        {
            //当消息分发器发现了UserLoginRequest类型的数据，就会调用OnUserLoginRequest
            Console.WriteLine("发现用户登录消息:{0}, {1}", msg.Username, msg.Password); 
        }
    }
}
