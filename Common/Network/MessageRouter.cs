using Common;
using Proto.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// 消息分发
namespace Common.Network
{
    class Msg  // 消息单元
    {
        public NetConnection sender;
        public package message;
    }
    public class MessageRouter : Singleton<MessageRouter> // 消息分发器，设计成单例模式
    {
        // 工作线程数
        int ThreadCount = 1;

        // 正在工作的线程数
        int WorkerCount = 0;

        // 当前消息分发器的工作状态
        bool Running = false;

        // 协调多个线程的通信, 通过set，每次可唤醒一个线程
        AutoResetEvent threadEvent = new AutoResetEvent(true);

        // 每个单元包含消息发送者和消息体
        // 消息队列，所有客户端发来的消息都暂存在这里
        private Queue<Msg> messsageQueue = new Queue<Msg> ();

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

        public void AddMessage(NetConnection sender, package message)
        {
            // 添加新的消息到队列中
            messsageQueue.Enqueue(new Msg() { sender = sender, message = message });

            // 唤醒一个线程
            this.threadEvent.Set();
        }

        public void Start(int ThreadCount)
        {
            this.Running = true;
            this.ThreadCount = Math.Max(1, ThreadCount);  // 线程数大于1
            this.ThreadCount = Math.Min(ThreadCount, 200); // 线程数小于200

            for(int i=0; i<this.ThreadCount; i++)  // 给每个线程分配任务
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(MessageWork));
            }
            while(WorkerCount < this.ThreadCount)  // 当工人没有全部就绪
            {
                Thread.Sleep(100);  // 等待100毫秒
            }
        }

        private void MessageWork(object? state)
        {
            Console.WriteLine("MessageWorker thread start");
            try
            {
                WorkerCount = Interlocked.Increment(ref WorkerCount);  // 原子性 线程安全 地 +1

                // 工作
                while (this.Running)
                {
                    if(messsageQueue.Count == 0) // 如果消息队列没有消息
                    {
                        this.threadEvent.WaitOne(); // 把所有线程阻塞,当有消息入队时会唤醒一个线程
                        continue;
                    }
                    Msg msg = messsageQueue.Dequeue();  // 出队一个消息，给pack
                    package package = msg.message;
                    if(package != null)
                    {
                        if(package.Request != null)
                        {
                            if(package.Request.UserRegister != null)
                            {
                                Console.WriteLine("用户注册：" + package.Request.UserRegister.Username);
                            }
                            if(package.Request.UserLogin != null)
                            {
                                Console.WriteLine("用户注册：" + package.Request.UserLogin.Username);
                            }
                        }
                        if(package.Response != null)
                        {

                        }
                    }
                }
            }
            catch { }
            finally
            {
                WorkerCount = Interlocked.Decrement(ref WorkerCount);  // 原子性 线程安全 地 -1
            }
            Console.WriteLine("MessageWorker thread end");
        }

        public void Stop()
        {
            this.Running = false;
            messsageQueue.Clear();  // 清空消息队列
            while(this.WorkerCount > 0)  // 还有线程没退出
            {
                this.threadEvent.Set();
            }
            Thread.Sleep(100);
        }
    }
}
