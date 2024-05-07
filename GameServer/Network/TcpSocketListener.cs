using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Network
{
    /// <summary>
    /// 负责监听TCP网络端口，异步接收Socket连接
    /// </summary>
    public class TcpSocketListener
    {
        private IPEndPoint endPoint;
        private Socket serverSocket;    //服务端监听对象

        public event EventHandler<Socket> SocketConnected; //客户端接入事件

        public TcpSocketListener(string host, int port)  
        {
            // 接受主机地址和端口作为参数，并创建一个IPEndPoint对象来表示要监听的终结点
            endPoint = new IPEndPoint(IPAddress.Parse(host), port);
        }

        public void Start()
        {
            // 加锁，同一时间只允许一个线程去操作
            lock (this)
            {
                if (!IsRunning)
                {
                    // AddressFamily.InterNetwork : IPv4
                    // SocketType.Stream : 流式传输
                    // ProtocolType.Tcp : TCP协议
                    serverSocket = new Socket(AddressFamily.InterNetwork, 
                                                SocketType.Stream, ProtocolType.Tcp);
                    serverSocket.Bind(endPoint);  //绑定端口号
                    serverSocket.Listen();  //监听
                    Console.WriteLine("开始监听端口：" + endPoint.Port); //等待用户连接

                    SocketAsyncEventArgs args = new SocketAsyncEventArgs();  //接受客户端
                    args.Completed += OnAccept;      // 当有人连入的时候, 直接执行OnAccept， 把args传给OnAccept
                    serverSocket.AcceptAsync(args);  // 异步接收客户端连接请求，不会阻塞在这里
                }
            }
        }

        private void OnAccept(object? sender, SocketAsyncEventArgs e) // object? 表示可空的object类型
        {
            // 判断是否成功
            if (e.SocketError == SocketError.Success)
            {
                // 定义一个客户端Socket对象去接收
                Socket client = e.AcceptSocket; //连入的人, client:客户端套接字
                if (client!=null)
                {
                    SocketConnected?.Invoke(this, client);
                }
                
            }

            //继续接收下一位
            e.AcceptSocket = null;  //清空接收器
            serverSocket.AcceptAsync(e);  //继续接收下一个
        }

        public bool IsRunning
        {
            get { return serverSocket != null; }
        }

        public void Stop()
        {
            lock (this)
            {
                if (serverSocket == null)  // 假如Socket启动失败，或者没启动
                    return;
                serverSocket.Close();
                serverSocket = null;  //给对象一个空指针
            }
        }
    }
}
