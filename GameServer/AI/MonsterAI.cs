using GameServer.FSM;
using GameServer.Model;
using Summer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            fsmSystem.AddState("walk", new WalkState()); // 把巡逻状态添加到状态机
        }

        public override void Update()
        {
            fsmSystem?.Update();
        }

        public class Param
        {
            public Monster Owner;
        }


        // 巡逻
        class WalkState : State<Param>
        { 
            // 游戏运行时间
            float lastTime = Time.time;
            public override void OnEnter()
            {
                P.Owner.StopMove();
            }

            public override void OnUpdate()
            {
                Monster monster = P.Owner;
                if (monster.State == Proto.Message.EntityState.Idle)
                {
                    // 到了刷新的时间
                    if(lastTime+10f<Time.time) 
                    {
                        lastTime = Time.time;

                        // 移动到随机位置
                        var target = monster.RandomPointWithBirth(6000);
                        monster.MoveTo(target);
                    }
                }
            }
        }
    }
}
