using GameServer.Database;
using GameServer.Mgr;
using Proto.Message;
using Serilog;
using Summer;
using Summer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Model
{
    /*
        Character（角色）：
            Character通常特指玩家控制的角色，在游戏中扮演特定的角色扮演者。
            Character通常具有独特的外观、技能、属性等特征，与其他角色有所区别。
            Character通常是游戏中的主要角色之一，玩家可以操控其行为和决策。
            Character通常是游戏中的重要角色之一，具有故事性和情感性。
     */
    public class Character : Actor
    {
        // 当前角色客户端的连接
        public Connection conn;

        // 当前角色对应的数据库对象
        public DbCharacter Data = new DbCharacter();

        //public Character(Vector3Int position, Vector3Int direction) : base(position, direction)
        //{

        //}

        public Character(DbCharacter dbCharacter) 
            : base(EntityType.Character, dbCharacter.JobId, dbCharacter.Level,
                  new Vector3Int(dbCharacter.X, dbCharacter.Y, dbCharacter.Z), Vector3Int.zero)
        {
            UnitDefine unitDefine = DataManager.Instance.Units[dbCharacter.JobId];
            this.Id = dbCharacter.Id;
            this.Name = dbCharacter.Name;
            this.Info.Name = dbCharacter.Name;
            this.Info.Id = dbCharacter.Id;        // 主键
            this.Info.Tid = dbCharacter.JobId;   // 角色类型
            this.Info.Exp = dbCharacter.Exp;
            this.Info.SpaceId = dbCharacter.SpaceId;
            this.Info.Gold = dbCharacter.Gold;
            this.Info.Hp = dbCharacter.Hp;
            this.Info.Mp = dbCharacter.Mp;
            this.Data = dbCharacter;
            this.Speed = unitDefine.Speed;
        }

        // Character 类型隐式转换 dbCharacter
        public static implicit operator Character(DbCharacter dbCharacter)
        {
            return new Character(dbCharacter);
        }

    }
}
