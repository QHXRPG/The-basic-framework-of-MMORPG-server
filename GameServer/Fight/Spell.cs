using GameServer.Battle;
using GameServer.Core;
using GameServer.Mgr;
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
            var skill = Owner.skillMgr.GetSkill(skill_id);

            // 检查技能
            if(skill == null) 
            {
                Log.Warning("角色 {0} 技能 {1} 不存在", Owner.Name, skill_id);
                return;
            }

            //检查目标
            var target = Game.GetUnit(target_id);
            if(target == null)
            {
                Log.Warning("角色 {0} 不存在", Owner.Name);
            }

            // 执行技能
            SCObject sco = new SCEntity(target);
            var res = skill.CanUse(sco);
            if(res != CastResult.Success)
            {
                // 通知施法者技能失败
                OnSpellFailure(skill_id, res);
                return;
            }
            skill.Use(sco);
            CastInfo info = new CastInfo()
            {
                CasterId = Owner.entityId,
                TargetId = target_id,
                SkillId = skill_id,
            };
            Owner.Space.fightMgr.SpellQueue.Enqueue(info);
        }

        // 释放点目标技能
        public void SpellPosition(int skill_id, Vector3 position)
        {
            Log.Information("SpellPosition");

            // 执行技能
            SCObject sco = new SCPosition(position);
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
