using GameServer.Model;
using Summer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Mgr
{

    // 刷怪管理器
    public class SpawnManager 
    {
        public List<Spawner> Rules = new List<Spawner>();   

        public Space Space{get; set;}


        // 初始化
        public void Init(Space space)
        { 
            Space = space;

            // 根据当前场景加载刷怪规则
            var rules = DataManager.Instance.Spawns.Values
                        .Where(r => r.SpaceId == space.Id);
            foreach (var rule in rules) 
            {
                Rules.Add(new Spawner(rule, space));    
            }
        }

        public void Update()
        {
            foreach (var rule in Rules) 
            {
                rule.Update();
            }
        }
    }
}
