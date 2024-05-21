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
using GameServer.Mgr;
using GameServer.Core;

namespace GameServer.Model
{
    // 空间、地图、场景
    public class Space
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public SpaceDefine Def { get; set; }

        // 当前场景中全部的角色 <角色ID， 角色对象>
        public Dictionary<int, Character> CharacterDict = new Dictionary<int, Character>();

        // 当前场景中的怪物 <MonsterId, Monster>
        public Dictionary<int, Monster> MonsterDict = new Dictionary<int, Monster>();

        // 记录连接对应的角色
        private Dictionary<Connection, Character> ConnCharater = new Dictionary<Connection, Character>();

        public MonsterManager monsterManager = new MonsterManager();

        public SpawnManager spawnManager = new SpawnManager();  


        public Space(SpaceDefine def)
        {
            this.Def = def;
            this.Id = def.SID;
            this.Name = def.Name;   
            monsterManager.Init(this);
            spawnManager.Init(this);    
        }

        public void CharacterJoin(Connection conn, Character character)
        {
            Log.Information($"角色 {character.Id} 进入场景：{character.SpaceId}" );
            conn.Get<Session>().Character = character;   // 把角色character存入session
            character.OnEnterSpace(this);

           CharacterDict[character.Id] = character;
            character.conn = conn; // 设置这个角色所对应的客户端连接
            if(ConnCharater.ContainsKey(conn))
            {
                ConnCharater[conn] = character; 
            }
            // 广播新客户端连接给场景其它玩家
            NEntity e = new NEntity();
            var resp = new SpaceCharaterEnterResponse();
            resp.SpaceId = this.Id;  // 当前场景的id
            character.Info.Entity = character.EntityData;

             //把 NEntity 对象加入到 Entity 列表中
            resp.CharacterList.Add(character.Info);

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

            resp.CharacterList.Clear();
            // 同步其它玩家
            foreach (var kv in CharacterDict)
            {
                if (kv.Value.conn == conn) // 当前客户端不需要接收自己的角色信息
                    continue;
                resp.CharacterList.Add(kv.Value.Info);
            }

            // 同步怪物
            foreach (var kv in MonsterDict)
            {
                resp.CharacterList.Add(kv.Value.Info);
            }
            conn.Send(resp);
        }

        // 角色离开地图 (客户端断开连接、去其它场景)
        public void CharacterLeave(Connection conn, Character character) 
        {
            Log.Information("角色离开场景：" + character.Id);
            CharacterDict.Remove(character.Id);  // 将 该连接 从 当前场景中的CharacterDict 中移除

            // 通知其它客户端，该客户端已离开该场景
            SpaceCharaterLeaveResponse resp = new SpaceCharaterLeaveResponse();
            resp.EntityId = character.entityId;
            foreach(var kv in CharacterDict) 
            {
                kv.Value.conn.Send(resp);
            }
        }

        // 广播更新Entity的信息
        public void UpdateEntity(NEntitySync entitySync)
        {
            // 广播自己的位置给其他人，不需要广播给自己，因为自己的客户端能够看到
            foreach (var kv in CharacterDict)
            {
                if (kv.Value.entityId == entitySync.Entity.Id) // 遍历到的客户端恰好是发送同步请求的客户端
                {
                    // 把传进来的 Entity 的状态 赋值给 服务器对象Entity当中
                    kv.Value.EntityData = entitySync.Entity;
                }
                else  //其他人
                {
                    SpaceEntitySyncResponse resp = new SpaceEntitySyncResponse();
                    resp.EntitySync = entitySync;
                    kv.Value.conn.Send(resp); 
                }
            }
        }

        // 怪物进入场景
        public void MonsterEnter(Monster monster)
        {
            MonsterDict.Add(monster.Id, monster);
            monster.OnEnterSpace(this);
            var resp = new SpaceCharaterEnterResponse();
            resp.SpaceId = this.Id;
            resp.CharacterList.Add(monster.Info);
            foreach (var kv in CharacterDict) 
            {
                kv.Value.conn.Send(resp);
            }
        }

        public void Update()
        {
            this.spawnManager.Update();
        }
    }
}
