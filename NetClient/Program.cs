// See https://aka.ms/new-console-template for more information
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;
using Proto.Message;
using Google.Protobuf;
using Summer.Network;
using Summer;
using Proto;
using Serilog;

// 设置 Serilog 配置
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Async(a => a.Console())
    .CreateLogger();

Log.Information("Hello, World!");
IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 32510); // 连接服务器
Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
socket.Connect(iPEndPoint);

Log.Information("成功连接到服务器");

//用户登录消息 压力测试
/*for (int i = 0; i < 100000; i++)
{
    MyConnection conn = new MyConnection(socket);
    conn.Request.UserLogin = new UserLoginRequest();
    conn.Request.UserLogin.Username = "QHXRPG-" + i;
    conn.Request.UserLogin.Password = "pwd-" + i;
    conn.Send();
}*/

Connection conn = new Connection(socket);

var msg = new UserLoginRequest();
msg.Username = "qhsdfsdfsdfx";
msg.Password = "123";
conn.Send(msg);


Console.ReadKey();