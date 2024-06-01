using GameServer.Model;
using Serilog;
using Summer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Battle
{
    public enum SkillState
    {
        None,
        Casting,
        Active
    }
    public class Skill
    {
        public SkillDefine Define;  // 技能设定
        public Actor Owner;         // 技能归属者
        public float Cooldown;      // 冷却时间
        public float _time;       // 技能运行时间
        public SkillState State;   // 当前技能状态

        public bool IsUnitTarget { get => Define.TargetType == "单位"; }

        public bool IsPointTarget { get => Define.TargetType == "点"; }

        public bool IsNullTarget { get => Define.TargetType == "None"; }

        public Skill(Actor owner, int skid)
        {
            owner = owner;
            Define = DataManager.Instance.Skills[skid];
        }

        public void Update()
        {
            if(Cooldown > 0) 
            {
                Cooldown -= Time.deltaTime;
            }
            else if(Cooldown < 0)
            {
                Cooldown = 0;
            }
            _time += Time.deltaTime;  //在每一帧更新中增加时间的累计，用于记录技能的持续时间

            // 开始-前摇-激活-结束
            if (State == SkillState.Casting && _time >= Define.CastTime)
            {
                State = SkillState.Active;
            }
            if(State == SkillState.Active)
            {
                Log.Information("Skill Active {0}", Define.Name);
            }
        }
    }
}
