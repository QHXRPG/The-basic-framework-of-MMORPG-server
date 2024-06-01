using GameServer.Battle;
using GameServer.Model;
using Proto.Message;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Fight
{
    // 技能释放器,每个人都有一个
    public class Spell
    {
        // 技能释放器的拥有者 
        public Actor Owner { get; private set; }

        public Spell(Actor owner)
        {
            this.Owner = owner;
        }

        // 吟唱技能
        public void Intonate(Skill skill)
        {

        }

        public void RunCast(CastInfo info)
        {
            var skill = Owner.skillMgr.GetSkill(info.SkillId);

            if (skill.IsUnitTarget)
            {
                SpellTarget(info.SkillId, info.TargetId);
            }
            if(skill.IsPointTarget) 
            {
                SpellPosition(info.SkillId, info.TargetLoc); 
            }
            if(skill.IsNullTarget)
            {
                SpellNoTarget(info.SkillId);
            }
        }

        // 释放无目标技能
        private void SpellNoTarget(int skill_id)
        {
            Log.Information("SpellNoTarget");
        }

        // 释放单位目标技能
        public void SpellTarget(int skill_id, int target_id)
        {
            Log.Information("SpellTarget");
        }

        // 释放点目标技能
        public void SpellPosition(int skill_id, Vector3 position)
        {
            Log.Information("SpellPosition");
        }

        // 通知玩家技能失败
        public void OnSpellFailure(int skill_id, CastResult reason)
        {
            if(Owner is Character chr)
            {
                SpellFailResponse response = new SpellFailResponse()
                {
                    CasterId = Owner.entityId,
                    SkillId = skill_id,
                    Reason = reason
                };
                chr.conn.Send(response);    
            }
        }
    }
}
