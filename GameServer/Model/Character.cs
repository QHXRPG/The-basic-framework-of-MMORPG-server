﻿using Proto.Message;
using Summer;
using Summer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Model
{
    public class Character : Actor
    {
        // 当前角色客户端的连接
        public Connection conn;

        public Character(int id, Vector3Int position, Vector3Int direction) : base(id, position, direction)
        {

        }
    }
}
