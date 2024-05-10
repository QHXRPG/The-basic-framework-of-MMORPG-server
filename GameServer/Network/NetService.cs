using Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Proto.Message;
using Summer.Network;
using Summer;
using Serilog;

namespace GameServer.Network
{
    public class NetService
    {
        TcpServer tcpServer;
        public NetService()
        {
            tcpServer = new TcpServer("0.0.0.0", 32510);
            tcpServer.Connected += OnClientConnected;
            tcpServer.Disconnected += OnDisconnectedCallback;
            tcpServer.DataReceived += OnDataReceiveCallback;
        }

        public void Start()
        {
            tcpServer.Start();
            MessageRouter.Instance.Start(10); //启动消息分发器
        }

        private void OnClientConnected(Connection conn)
        {
            Log.Information("客户端接入");
        }

        private void OnDisconnectedCallback(Connection conn)
        {
            Log.Information("连接断开" + conn);
        }

        private void OnDataReceiveCallback(Connection conn, byte[] data)
        {
            // 反序列化（字节 转 对象）
            package package = package.Parser.ParseFrom(data);  // 使用一个全局的package作为解析的数据包
            MessageRouter.Instance.AddMessage(conn, package);
        }
    }
}
