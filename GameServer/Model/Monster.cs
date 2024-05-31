using GameServer.AI;
using GameServer.Database;
using Proto.Message;
using Summer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Model
{
    public class Monster : Actor
    {
        public AIBase AI;
        public Actor target;  // 目标
        public Vector3 moveTaraget;  // 移动的目标
        public Vector3 movePosition; // 当前移动的位置
        public Vector3 initPosition; // 出生点
        public static Vector3Int Y1000 = new Vector3Int(0, 1, 0);
        public static Vector3Int XZ1000 = new Vector3Int(1, 0, 1);

        Random rand = new Random(); 

        public Monster(int tid, int level, Vector3Int pos, Vector3Int dir)
            : base(EntityType.Monster, tid, level, pos, dir)
        {
            this.initPosition = pos;
            Speed = 4000;
            State = EntityState.Idle;
            Random rand = new Random();

            // 位置同步
            Scheduler.Instance.AddTask(() =>
            {
                if(State != EntityState.Move) return;
                // 广播消息
                NetEntitySync NetEntitySync = new NetEntitySync();
                NetEntitySync.Entity = EntityData;
                NetEntitySync.State = State;
                this.Space.UpdateEntity(NetEntitySync); // 让当前的地图进行广播
            }, 0.15f);

            // 设置AI对象
            switch(Define.AI)
            {
                case "Monster":
                    this.AI = new MonsterAI(this); break;
            }
            
        }

        public void MoveTo(Vector3 target)
        {
            if(this.State == EntityState.Idle)
            {
                State = EntityState.Move;
            }
            if(moveTaraget !=  target) 
            {
                moveTaraget = target;
                movePosition = Position;
                var dir = (moveTaraget - movePosition).normalized;  // 计算方向向量
                Direction = LookRotation(dir) * 1000 * Y1000; // 计算欧拉角,只更新y轴

                // 广播消息
                NetEntitySync NetEntitySync = new NetEntitySync();
                NetEntitySync.Entity = EntityData;
                NetEntitySync.State = State;
                this.Space.UpdateEntity(NetEntitySync); // 让当前的地图进行广播
            }
        }

        public void StopMove()
        {
            State = EntityState.Idle;
            movePosition = moveTaraget;
            // 广播消息
            NetEntitySync NetEntitySync = new NetEntitySync();
            NetEntitySync.Entity = EntityData;
            NetEntitySync.State = State;
            this.Space.UpdateEntity(NetEntitySync); // 让当前的地图进行广播
        }

        public override void Update()
        {
            base.Update();
            AI?.Update();
            if(State == EntityState.Move)
            {
                // 移动的方向
                var dir = (moveTaraget - movePosition).normalized;
                this.Direction = LookRotation(dir) * 1000 * Y1000;
                float dist = Speed * Time.deltaTime;
                if(Vector3.Distance(moveTaraget, movePosition) < dist)
                {
                    StopMove();
                }
                else
                {
                    movePosition += dist * dir;
                }
                this.Position = movePosition;
            }
        }

        //方向向量转欧拉角
        public Vector3 LookRotation(Vector3 fromDir)
        {
            float Rad2Deg = 57.29578f;
            Vector3 eulerAngles = new Vector3();
            //AngleX = arc cos(sqrt((x^2 + z^2)/(x^2+y^2+z^2)))
            eulerAngles.x = MathF.Acos(MathF.Sqrt((fromDir.x * fromDir.x + fromDir.z * fromDir.z) / (fromDir.x * fromDir.x + fromDir.y * fromDir.y + fromDir.z * fromDir.z))) * Rad2Deg;
            if (fromDir.y > 0) eulerAngles.x = 360 - eulerAngles.x;
            //AngleY = arc tan(x/z)
            eulerAngles.y = MathF.Atan2(fromDir.x, fromDir.z) * Rad2Deg;
            if (eulerAngles.y < 0) eulerAngles.y += 180;
            if (fromDir.x < 0) eulerAngles.y += 180;
            //AngleZ = 0
            eulerAngles.z = 0;
            return eulerAngles;
        }

        // 计算出生点附近的随机坐标
        public Vector3 RandomPointWithBirth(float range)
        {
            float x = rand.NextSingle() * 2f - 1f;
            float z = rand.NextSingle() * 2f - 1f;
            Vector3 dir = new Vector3(x, 0, z).normalized;
            return initPosition + dir * range * rand.NextSingle();
        }

    }
}
