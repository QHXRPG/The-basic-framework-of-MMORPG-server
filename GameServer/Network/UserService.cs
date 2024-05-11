﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proto.Message;
using Summer.Network;
using Serilog;
using Summer;
using GameServer.Model;
using GameServer.Mgr;

namespace Common.Network.Server
{
    // 玩家服务： 注册登录，创建角色，进入游戏
    internal class UserService
    {
        //创建空间对象:新手村场景
        Space space = new Space();

        public void Start()
        {
            // 订阅进入游戏的消息
            MessageRouter.Instance.Subscribe<GameEnterRequest>(_GameEnterRequest);
            space.Name = "新手村";
            space.Id = 6; // 新手村id
        }

        private void _GameEnterRequest(Connection conn, GameEnterRequest msg)
        {
            Log.Information("有玩家进入游戏");
            int entityId = EntityManager.Instance.NewEntityId;  // 获得一个独有的实体Id
            Random random = new Random();
            Vector3Int pos = new Vector3Int(500+random.Next(-5, 5), 0, 500 + random.Next(-5, 5));     // 玩家坐标
            Vector3Int r_pos = new Vector3Int(0, 0, 0);
            Character character = new Character(entityId, pos, r_pos); // 分配id、坐标、方向
            space.CharacterJoin(conn, character);
        }
    }
}
