using GameServer.Battle;
using GameServer.Model;
using Proto.Message;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Mgr
{
    // 技能管理器，每个Actor都有独立的技能管理器
    public class SkillManager
    {
        // 归属对象
        private Actor owner;


        // 技能列表
        public List<Skill> Skills = new List<Skill>();


        public SkillManager(Actor owner) 
        {
            this.owner = owner;
            this.InitSkills();  
        }


        public void InitSkills()
        {
            // 初始化技能信息，正常是通过读取数据库来加载技能信息
            if(this.owner.Define.TID == 1)
            {
                loadSkill(4, 7, 8);
            }

            if(this.owner.Define.TID == 2)
            {
                loadSkill(9, 10);
            }

            if (this.owner.Define.TID == 3)
            {
                loadSkill(9, 10);
            }
        }

        private void loadSkill(params int[] ids)
        {
            foreach(int skid in ids) 
            {
                owner.Info.Skills.Add(new Proto.Message.SkillInfo() { Id = skid });
                var skill = new Skill(owner, skid);
                Skills.Add(skill);
                Log.Information("角色{0}加载技能{1}-{2}",
                                owner.Name, skill.Define.ID, skill.Define.Name);
            }
        }

        public Skill GetSkill(int skillId)
        {
            return new Skill(owner, skillId);
        }


        public void Update() 
        {
            foreach (var skill in Skills) 
            {
                skill.Update();
            }
        }
    }
}
