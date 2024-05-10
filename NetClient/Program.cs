// See https://aka.ms/new-console-template for more information
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;
using Proto.Message;
using Google.Protobuf;
using Summer.Network;
using Summer;
using Common;

Log.Print += (text, args) =>
{
    Console.WriteLine(text, args);
};

Console.WriteLine("Hello, World!");
IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 32510); // 连接服务器
Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
socket.Connect(iPEndPoint);

Log.Info("成功连接到服务器");

//用户登录消息 压力测试
/*for(int i=0; i<100000; i++)
{
    MyConnection conn = new MyConnection(socket);
    conn.Request.UserLogin = new UserLoginRequest();
    conn.Request.UserLogin.Username = "QHXRPG-" + i;
    conn.Request.UserLogin.Password = "pwd-" + i;
    conn.Send();
}*/
MyConnection conn = new MyConnection(socket);
void SendRequest(Google.Protobuf.IMessage message)
{
    //网络传输的数据包
    var package = new package() { Request = new Request() };
    // 拆开Request，遍历属性
    foreach (var p in package.Request.GetType().GetProperties())
    {
        if(p.PropertyType == message.GetType())  // 当p是Request时
        {
            p.SetValue(package.Request, message);  // 把message赋值给package的Request
            
        }
    }
    conn.Send(package);  // 发送这个package
}

var msg = new UserLoginRequest();
msg.Username = "qhx";
msg.Password = "123";
SendRequest(msg);
Console.ReadKey();


static void SendMessage(Socket socket, byte[] body)  // 角色信息、消息信息、战斗记录 
{
    //  网络参数的基本单位是字节，所有模态的数据都要转化为字节进行传输
    int len = body.Length; // 消息的真正的长度
    byte[] lenBytes = BitConverter.GetBytes(len); // 转字节

    socket.Send(lenBytes);  // 发四个字节的长度
    socket.Send(body);   // 发送消息体
}