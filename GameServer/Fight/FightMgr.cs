using GameServer.Mgr;
using GameServer.Model;
using Proto.Message;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Fight
{
    // 战斗管理器
    public class FightMgr
    {
        private Space space { get; }

        public FightMgr(Space space)
        {
            this.space = space;
        }

        // 技能施法队列
        public ConcurrentQueue<CastInfo> CastQueue = new ConcurrentQueue<CastInfo>();

        // 等待广播的队列
        public ConcurrentQueue<CastInfo> SpellQueue = new();

        // 施法响应对象，每帧发送一次s
        private SpellResponse SpellResponse = new();

        public void OnUpdate(float delta)
        {
            while(CastQueue.TryDequeue(out var cast))
            {
                Log.Information("执行施法：{0}", cast);
                RunCast(cast);
            }
            BroadcastSpell();
        }

        // 广播施法信息
        private void BroadcastSpell()
        {
            // 把技能都放入SpellResponse中的施法列表中
            while (SpellQueue.TryDequeue(out var item))
            {
                SpellResponse.CastList.Add(item);   
            }
            space.Broadcast(SpellResponse);
            SpellResponse.CastList.Clear();
        }

        private void RunCast(CastInfo cast)
        {
            // 检查施法者
            var caster = EntityManager.Instance.GetEntity(cast.CasterId) as Actor;
            if(caster == null)
            {
                Log.Error("RunCast（）：施法者不存在");
                return;
            }
            caster.Spell.RunCast(cast);
        }
    }
}
