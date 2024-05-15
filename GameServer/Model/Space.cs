using Summer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Summer;
using Serilog;
using Proto;
using Proto.Message;

namespace GameServer.Model
{
    // 空间、地图、场景
    public class Space
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public SpaceDefine Def { get; set; }

        // 当前场景中全部的角色
        public Dictionary<int, Character> CharacterDict = new Dictionary<int, Character>();

        // 记录连接对应的角色
        private Dictionary<Connection, Character> ConnCharater = new Dictionary<Connection, Character>();

        public Space() { }

        public Space(SpaceDefine def)
        {
            this.Def = def;
            this.Id = def.SID;
            this.Name = def.Name;   
        }

        public void CharacterJoin(Connection conn, Character character)
        {
            Log.Information("角色进入场景：" + character.entityId);
            conn.Set<Character>(character);   // 把角色character存入对应的conn连接当中
            conn.Set<Space>(this);            // 把场景 space 存入对应的 conn 连接当中
            character.SpaceId = this.Id;    

            CharacterDict[character.entityId] = character;
            character.conn = conn; // 设置这个角色所对应的客户端连接
            if(ConnCharater.ContainsKey(conn))
            {
                ConnCharater[conn] = character; 
            }
            // 广播新客户端连接给场景其它玩家
            NEntity e = new NEntity();
            var resp = new SpaceCharaterEnterResponse();
            resp.SpaceId = this.Id;  // 当前场景的id
            resp.EntityList.Add(character.GetData()); //把 NEntity 对象加入到 NEntity 列表中
            foreach(var kv in CharacterDict) 
            {
                // 发送角色进入场景的消息给其他人
                if(kv.Value.conn != conn)
                {
                    // 发送上线消息，把所有角色除了当前新连接的客户端
                    // 其它客户端接收后，更新当前新连接的客户端的角色的位置
                    kv.Value.conn.Send(resp);   
                }
            }

            // 新上线的角色需要获取全部角色的信息
            foreach (var kv in CharacterDict)
            {
                if (kv.Value.conn == conn) // 当前客户端不需要接收自己的角色信息
                    continue;

                // 清空再添加，再清空再添加
                resp.EntityList.Clear();                  
                resp.EntityList.Add(kv.Value.GetData());

                // 把所有角色挨个发出去, 当前的客户端接收后更新场景中其他人的位置
                conn.Send(resp);    
            }
        }

        // 角色离开地图 (客户端断开连接、去其它场景)
        public void CharacterLeave(Connection conn, Character character) 
        {
            Log.Information("角色离开场景：" + character.entityId);
            conn.Set<Character>(null);  // 取消该连接的当前场景记录
            CharacterDict.Remove(character.entityId);  // 将 该连接 从 当前场景中的CharacterDict 中移除

            // 通知其它客户端，该客户端已离开该场景
            SpaceCharaterLeaveResponse resp = new SpaceCharaterLeaveResponse();
            resp.EntityId = character.entityId;
            foreach(var kv in CharacterDict) 
            {
                kv.Value.conn.Send(resp);
            }
        }

        // 广播更新Entity的信息
        public void UpdataEntity(NEntitySync entitySync)
        {
            // 广播自己的位置给其他人，不需要广播给自己，因为自己的客户端能够看到
            foreach (var kv in CharacterDict)
            {
                if (kv.Value.entityId == entitySync.Entity.Id) // 自己
                {
                    // 把传进来的 Entity 的状态 赋值给 服务器对象Entity当中
                    kv.Value.SetEntityData(entitySync.Entity);
                }
                else  //其他人
                {
                    SpaceEntitySyncResponse resp = new SpaceEntitySyncResponse();   
                    resp.EntitySync = entitySync;
                    kv.Value.conn.Send(resp);
                }
            }
        }
    }
}
