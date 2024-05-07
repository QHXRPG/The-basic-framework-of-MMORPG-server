using Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Proto.Message;

namespace GameServer.Network
{
    public class NetService
    {
        TcpSocketListener listener = null;
        public NetService() { }

        public void Init(int port)
        {
            //TCP协议， 绑定端口号
            // 0-65536   选一万以上的
            //创建服务器主要依靠 Socket对象,服务器客户端两个Socket
            // Socket ------------- Socket
            // 绑定一个监听端口
            listener = new TcpSocketListener("0.0.0.0", port);
            listener.SocketConnected += OnClientConnected; // 将连接的套接字传出，传参给OnClientConnected
        }

        public void Start()
        {
            listener.Start();
        }

        private void OnClientConnected(object? sender, Socket socket)
        {
            new NetConnection(socket, new NetConnection.DataReceivedCallback(OnDataReceiveCallback),
                                      new NetConnection.DisConnectedCallback(OnDisconnectedCallback));
        }

        private void OnDisconnectedCallback(NetConnection sender)
        {
            Console.WriteLine("连接断开");
        }

        private void OnDataReceiveCallback(NetConnection sender, byte[] data)
        {
            // 反序列化（字节 转 对象）
            Vector3 v = Vector3.Parser.ParseFrom(data);
            Console.WriteLine(v.ToString());
        }

        private static void OnDataReceived(object? sender, byte[] buffer)
        {

        }
    }
}
