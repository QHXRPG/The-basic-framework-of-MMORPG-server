﻿using GameServer.Model;
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

        public void OnUpdate(float delta)
        {
            while(CastQueue.TryDequeue(out var cast))
            {
                Log.Information("执行施法：{0}", cast);
            }
        }
    }
}