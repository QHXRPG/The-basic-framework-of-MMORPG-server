using Summer.Network;
using GameServer.Network;
using Network;
using Proto.Message;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using Serilog;



namespace GameServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 设置 Serilog 配置, 保存在logs\\client-log.txt中，每隔三天删除一次日志
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs\\client-log.txt", rollingInterval:RollingInterval.Day, retainedFileCountLimit:3)
                .CreateLogger();

            NetService netserver = new NetService();
            netserver.Start();

            MessageRouter.Instance.Start(4);  // 启动消息分发器，开四个线程

            // 消息订阅：用户登录消息
            MessageRouter.Instance.On<UserLoginRequest>(OnUserLoginRequest);

            while(true)
            {
                Thread.Sleep(100);
            }
        }

        private static void OnUserLoginRequest(Connection sender, UserLoginRequest msg)
        {
            //当消息分发器发现了UserLoginRequest类型的数据，就会调用OnUserLoginRequest
            Log.Information("发现用户登录消息:{0}, {1}", msg.Username, msg.Password); 
        }
    }
}
