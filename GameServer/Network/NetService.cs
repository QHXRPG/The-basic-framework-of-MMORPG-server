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

        private void OnDataReceiveCallback(Connection conn, Google.Protobuf.IMessage msg)
        {
            MessageRouter.Instance.AddMessage(conn, msg);
        }
    }
}
