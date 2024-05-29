/*// See https://aka.ms/new-console-template for more information
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

Connection conn = new Connection(socket);

var msg = new UserLoginRequest();
msg.Username = "qhsdfsdfsdfx";
msg.Password = "123";
conn.Send(msg);


Console.ReadKey();*/