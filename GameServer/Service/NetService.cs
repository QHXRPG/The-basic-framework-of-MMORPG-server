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
using GameServer.Model;

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
        }

        // 不同的Conn对应其最后一次心跳包的时间，判断该连接是否断开
        private Dictionary<Connection, DateTime> heartBeatPairs = new Dictionary<Connection, DateTime>();   

        public void Start()
        {
            tcpServer.Start();
            MessageRouter.Instance.Start(10); //启动消息分发器

            MessageRouter.Instance.Subscribe<HeartBeatRequest>(_HeartBeatRequest);

            // 每5秒调用一次TimerCallback()
            Timer timer = new Timer(TimerCallback, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
        }

        void TimerCallback(object state)
        {
            var now = DateTime.Now; 
            foreach(var kv in heartBeatPairs)
            {
                var offsetTime = now - kv.Value;  // 计算时间间隔

                // 判断是否超时，若超时，关闭连接
                if(offsetTime.TotalSeconds > 10)
                {
                    Log.Information("连接{0}心跳包等待超时，断开", kv.Key);
                    Connection conn = kv.Key;
                    conn.Close();
                    heartBeatPairs.Remove(kv.Key);
                }
            }
        }


        // 收到心跳包
        private void _HeartBeatRequest(Connection conn, HeartBeatRequest msg)
        {
            heartBeatPairs[conn] = DateTime.Now; //记录心跳时间
            Log.Information("收到心跳包:" + conn);
            HeartBeatResponse resp = new HeartBeatResponse();
            conn.Send(resp);
        }


        //客户端接入
        private void OnClientConnected(Connection conn)
        {
            heartBeatPairs[conn] = DateTime.Now; //记录心跳时间
            Log.Information("客户端接入");

        }

        private void OnDisconnectedCallback(Connection conn)
        {
            heartBeatPairs.Remove(conn);
            Log.Information("连接断开" + conn);

            // 通知其它客户端，该客户端已离开该场景
            var space = conn.Get<Space>();
            if(space != null)
            {
                var co = conn.Get<Character>();
                space.CharacterLeave(conn, co);
            }
        }

    }
}
