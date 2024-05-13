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
using Common.Network.Server;
using GameServer.Service;
using GameServer.Database;


namespace GameServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 设置 Serilog 配置, 保存在logs\\client-log.txt中，每隔三天删除一次日志
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Async(a=>a.Console())
                .WriteTo.Async(a=>a.File("logs\\client-log.txt", 
                                rollingInterval:RollingInterval.Day, retainedFileCountLimit:3))
                .CreateLogger();


            // 网络服务模块
            NetService netserver = new NetService();
            netserver.Start();
            Log.Debug("服务器启动");

            // 玩家服务模块
            UserService userService = UserService.Instance;
            userService.Start();
            Log.Debug("玩家服务启动");

            // 地图同步服务
            SpaceService spaceService = SpaceService.Instance;
            spaceService.Start();
            Log.Debug("地图同步服务启动");

            // 用户登录消息
            MessageRouter.Instance.Subscribe<UserLoginRequest>(OnUserLoginRequest);


            while (true)
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
