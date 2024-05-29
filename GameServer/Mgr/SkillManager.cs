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
            var list = DataManager.Instance.Skills.Values
                        .Where(s => s.TID == owner.Define.TID).ToList();  // 什么单位就拿什么技能
            Log.Information("角色  " + owner.Name+ "角色TID  " + owner.Define.TID + 
                            " 加载技能:" + list+"   "+list.Count);
            foreach (var define in list)
            {
                owner.Info.Skills.Add(new SkillInfo() { Id = define.ID });
                var skill = new Skill(owner, define.ID);
                Skills.Add(skill);
                Log.Information("角色{0}加载技能{1}-{2}",
                                owner.Name, skill.Define.ID, skill.Define.Name);
            }
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
