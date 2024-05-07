using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// 消息分发
namespace Common.Network
{
    class MsgUnit
    {
        public NetConnection sender;
        public Google.Protobuf.IMessage message;
    }
    public class MessageRouter : Singleton<MessageRouter> // 设计成单例模式
    {
        // 每个单元包含消息发送者和消息体
        // 消息队列，所有客户端发来的消息都暂存在这里
        private Queue<MsgUnit> messsageQueue = new Queue<MsgUnit> ();

        // 消息处理器 : 给订阅者们提供的结构，订阅者们通过消息处理器获得对应业务的消息
        public delegate void MessageHandler<T>(NetConnection sender, T msg);

        // 频道字典：每一个名字（技能消息，坐标消息，地图消息）对应一个委托
        private Dictionary<string, Delegate> delegateMap = new Dictionary<string, Delegate>();

        // 订阅消息
        public void On<T>(MessageHandler<T> handler)  where T : Google.Protobuf.IMessage 
        {
            // 支持所有类型的消息，但必须是IMessage 

            // 取出类型名称
            string type = typeof(T).Name;

            // 监测这个消息类型(频道)在 delegateMap 中是否存在，没有则创建
            if (!delegateMap.ContainsKey(type))
            {
                delegateMap[type] = null;
            }

            delegateMap[type] = (MessageHandler<T>)delegateMap[type] + handler;
            Console.WriteLine(type + " : " + delegateMap[type].GetInvocationList().Length);
        }

        // 退订
        public void Off<T>(MessageHandler<T> handler) where T : Google.Protobuf.IMessage
        {
            // 支持所有类型的消息，但必须是IMessage 
            string key = typeof(T).Name;

            // 监测这个消息类型(频道)在 delegateMap 中是否存在，没有则创建
            if (!delegateMap.ContainsKey(key))
            {
                delegateMap[key] = null;
            }

            delegateMap[key] = (MessageHandler<T>)delegateMap[key] - handler;
        }

        public void AddMessage(NetConnection sender, Google.Protobuf.IMessage message)
        {
            // 添加新的消息到队列中
            messsageQueue.Enqueue(new MsgUnit() { sender = sender, message = message });
        }
    }
}
