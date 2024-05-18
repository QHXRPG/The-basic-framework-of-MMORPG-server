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
        public Vector3 moveTaraget;  // 移动的目标
        public Vector3 movePosition; // 当前移动的位置
        public Vector3 initPosition;

        public Monster(int tid, int level, Vector3Int pos, Vector3Int dir)
            : base(EntityType.Monster, tid, level, pos, dir)
        {
            this.initPosition = pos;
            Speed = 4000;
            State = EntityState.Idle;
            Random rand = new Random();
            Schedule.Instance.AddTask(() =>
            {
                float x = pos.x + (rand.NextSingle() * 10f - 5f) * 1000;
                float z = pos.z + (rand.NextSingle() * 10f - 5f) * 1000;
                MoveTo(new Vector3(x, 0, z));
            },15);

            Schedule.Instance.AddTask(() =>
            {
                if(State != EntityState.Move) return;
                // 广播消息
                NEntitySync nEntitySync = new NEntitySync();
                nEntitySync.Entity = EntityData;
                nEntitySync.State = State;
                this.Space.UpdateEntity(nEntitySync); // 让当前的地图进行广播
            }, 0.15f);
            
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
                Direction = (moveTaraget - movePosition).normalized;

                // 广播消息
                NEntitySync nEntitySync = new NEntitySync();
                nEntitySync.Entity = EntityData;
                nEntitySync.State = State;
                this.Space.UpdateEntity(nEntitySync); // 让当前的地图进行广播
            }
        }

        public void StopMove()
        {
            State = EntityState.Idle;
            movePosition = moveTaraget;
            // 广播消息
            NEntitySync nEntitySync = new NEntitySync();
            nEntitySync.Entity = EntityData;
            nEntitySync.State = State;
            this.Space.UpdateEntity(nEntitySync); // 让当前的地图进行广播
        }

        public override void Update()
        {
            if(State == EntityState.Move)
            {
                // 移动的方向
                var dir = (moveTaraget - movePosition).normalized;
                this.Direction = LookRotation(dir) * 1000;
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

    }
}
