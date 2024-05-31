﻿using GameServer.Core;
using Proto.Message;
using Serilog;
using Summer;
using Summer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Service
{
    public class BattleService : Singleton<BattleService>
    {
        public void Start()
        {
            // 客户端传来的技能释放请求
            MessageRouter.Instance.Subscribe<SpellRequest>(_SpellRequest);
        }

        private void _SpellRequest(Connection conn, SpellRequest msg)
        {
            Log.Information("技能施法请求：{0}", msg);
            var session = conn.Get<Session>();
            var chr = session.Character;
            if(chr.entityId != msg.Info.CasterId)
            {
                Log.Error("施法者ID错误");
                return;
            }

            // 加入战斗管理器
            chr.Space.fightMgr.CastQueue.Enqueue(msg.Info);
        }
    }
}
