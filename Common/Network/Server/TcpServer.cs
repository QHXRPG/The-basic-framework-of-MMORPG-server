using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Summer.Network;

/// <summary>
/// 负责监听TCP网络端口，异步接收Socket连接
/// 对外提供的接口：
/// ---Connected     有连接
/// ---DataReceived  有新的消息
/// ---Disconnected  有连接断开
/// ---isRunning     是否运行ing
/// ---Stop()        关闭服务器
/// ---Start()       启动服务器
/// </summary>
public class TcpServer
{
    private IPEndPoint endPoint;
    private Socket serverSocket;    //服务端监听对象
    private int backlog = 100;

    public delegate void ConnectedCallback(Connection conn);
    public delegate void DataReceivedCallback(Connection conn, Google.Protobuf.IMessage data);
    public delegate void DisconnectedCallback(Connection conn);

    // 客户端接入事件
    public event EventHandler<Socket> SocketConnected; //客户端接入事件
    // 事件委托：有连接
    public event ConnectedCallback Connected;
    // 事件委托：有新的消息
    public event DataReceivedCallback DataReceived;
    // 事件委托：有连接断开
    public event DisconnectedCallback Disconnected;


    public TcpServer(string host, int port)  
    {
        // 接受主机地址和端口作为参数，并创建一个IPEndPoint对象来表示要监听的终结点
        endPoint = new IPEndPoint(IPAddress.Parse(host), port);
    }

    public TcpServer(string host, int port, int backlog)
        :this(host, port)
    {
        this.backlog = backlog;
    }

    public void Start()
    {
        // 加锁，同一时间只允许一个线程去操作
        lock (this)
        {
            if (!IsRunning)  // 如果没有运行，就去监听端口
            {
                // AddressFamily.InterNetwork : IPv4
                // SocketType.Stream : 流式传输
                // ProtocolType.Tcp : TCP协议
                serverSocket = new Socket(AddressFamily.InterNetwork, 
                                            SocketType.Stream, ProtocolType.Tcp);
                serverSocket.Bind(endPoint);  //绑定端口号
                serverSocket.Listen(backlog);  //监听, 队列长度为backlog
                Log.Information("开始监听端口：" + endPoint.Port); //等待用户连接

                SocketAsyncEventArgs args = new SocketAsyncEventArgs();  //接受客户端
                args.Completed += OnAccept;      // 当有人连入的时候,Completed事件被触发, 直接执行OnAccept， 把args传给OnAccept
                serverSocket.AcceptAsync(args);  // 异步接收客户端连接请求，不会阻塞在这里
            }
            else
            {
                Log.Information("This TcpServe is already running");
            }
        }
    }

    private void OnAccept(object? sender, SocketAsyncEventArgs e) // object? 表示可空的object类型
    {
        // 定义一个客户端Socket对象去接收
        Socket client = e.AcceptSocket; //连入的人, client:客户端套接字

        //继续接收下一位
        e.AcceptSocket = null;  //清空接收器
        serverSocket.AcceptAsync(e);  //继续接收下一个

        // 判断是否成功
        if (e.SocketError == SocketError.Success)
        {
            if (client!=null)
            {
                SocketConnected?.Invoke(this, client);
                OnSocketConnected(client);
            }
        }
    }

    // 新的Socket接入
    private void OnSocketConnected(Socket socket)
    {
        SocketConnected?.Invoke(this, socket);
        Connection conn = new Connection(socket);
        conn.OnDataReceived += (conn, data) => DataReceived?.Invoke(conn, data);
        conn.OnDisconnected += (conn) => Disconnected?.Invoke(conn);
        Connected?.Invoke(conn);
    }

    // 判断serverSocket是否存在
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
