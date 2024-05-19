using GameServer.FSM;
using GameServer.Mgr;
using GameServer.Model;
using Org.BouncyCastle.Asn1.X509;
using Proto.Message;
using Serilog;
using Summer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Time = Summer.Time;

namespace GameServer.AI
{
    public class MonsterAI : AIBase
    {
        public FsmSystem<Param> fsmSystem;

        public MonsterAI(Monster owner) : base(owner)
        {
            Param param = new Param();
            param.Owner = owner;
            fsmSystem = new FsmSystem<Param>(param);
            fsmSystem.AddState("walk", new WalkState());   // 把巡逻状态添加到状态机
            fsmSystem.AddState("chase", new ChaseState());
            fsmSystem.AddState("goback", new GobackState());
        }

        public override void Update()
        {
            fsmSystem?.Update();
        }

        public class Param
        {
            public Monster Owner;
            public int viewRange = 8000;  // 视野范围
            public int walkRange = 8000;  // 相对于出生点的活动范围
            public int chaseRange = 12000; //相对于出生点的追击范围
            public Random rand = new Random();
        }


        // 巡逻
        class WalkState : State<Param>
        { 
            // 游戏运行时间
            float lastTime = Time.time;

            float waitTime = 10f;  // 等待时间

            public override void OnEnter()
            {
                P.Owner.StopMove();
            }

            public override void OnUpdate()
            {
                Monster monster = P.Owner;
                
                // 查询8000范围内的玩家
                var character = EntityManager.Instance.GetNearest<Character>(monster.Space.Id, monster.Position, P.viewRange);
                if(character != null)
                {
                    Log.Information("最近的目标：" + character + "  " + Vector3Int.Distance(monster.Position * Monster.XZ1000, character.Position * Monster.XZ1000));
                    monster.target = character;
                    fsm.ChangeState("chase");
                    return;
                }
                if (monster.State == EntityState.Idle)
                {
                    // 到了刷新的时间
                    if(lastTime + waitTime < Time.time) 
                    {
                        lastTime = Time.time;

                        waitTime = P.rand.NextSingle() * 20f + 10f;

                        // 移动到随机位置
                        var target = monster.RandomPointWithBirth(P.walkRange);
                        monster.MoveTo(target);
                    }
                }
            }
        }


        // 追击状态
        class ChaseState : State<Param>
        {
            public override void OnUpdate()
            {
                var monster = P.Owner;

                // 判断角色是否为 空、死亡、下线
                if(monster.target == null || monster.target.IsDeath || 
                   ! EntityManager.Instance.Exist(monster.target.entityId))
                {
                    monster.target = null;
                    fsm.ChangeState("walk");
                    return;
                }

                // 自身与出生点的距离
                float m = Vector3.Distance(monster.initPosition * Monster.XZ1000, 
                                            monster.Position * Monster.XZ1000);

                // 自身与目标的距离
                float n = Vector3.Distance(monster.Position * Monster.XZ1000, 
                                            monster.target.Position * Monster.XZ1000);

                //  目标大于追击范围，或者大于视野范围
                if(m > P.chaseRange || n>P.viewRange) 
                {
                    //返回出生点
                    fsm.ChangeState("goback");
                    return;
                }

                if(n < 1200)
                {
                    if(monster.State == EntityState.Move)
                    {
                        monster.StopMove();
                    }
                    Log.Information("发起攻击");
                }
                else
                {
                    monster.MoveTo(monster.target.Position);
                }
            }
        }

        // 返回状态
        class GobackState : State<Param> 
        {
            public override void OnEnter() 
            {
                P.Owner.MoveTo(P.Owner.initPosition);
            }

            public override void OnUpdate()
            {
                var monster = P.Owner;
                if(Vector3.Distance(monster.initPosition * Monster.XZ1000, monster.Position * Monster.XZ1000) < 100)
                {
                    fsm.ChangeState("walk");
                }
            }
        }
    }
}
