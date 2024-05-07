// See https://aka.ms/new-console-template for more information
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;
using Proto.Message;
using Google.Protobuf;

Console.WriteLine("Hello, World!");
IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 32510); // 连接服务器
Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
socket.Connect(iPEndPoint);

Console.WriteLine("成功连接到服务器");

//用户登录消息
package package = new package();
package.Request = new Request();
package.Request.UserLogin = new UserLoginRequest();
package.Request.UserLogin.Username = "QHXRPG";
package.Request.UserLogin.Password = "123456";

//存储序列化后的消息内容
MemoryStream rawOutput = new MemoryStream();
CodedOutputStream output = new CodedOutputStream(rawOutput);
package.WriteTo(output);
output.Flush();
SendMessage(socket, rawOutput.ToArray());



static void SendMessage(Socket socket, byte[] body)  // 角色信息、消息信息、战斗记录 
{
    //  网络参数的基本单位是字节，所有模态的数据都要转化为字节进行传输
    int len = body.Length; // 消息的真正的长度
    byte[] lenBytes = BitConverter.GetBytes(len); // 转字节

    socket.Send(lenBytes);  // 发四个字节的长度
    socket.Send(body);   // 发送消息体
}