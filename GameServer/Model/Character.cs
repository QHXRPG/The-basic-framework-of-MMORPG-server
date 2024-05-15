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

        public Character(int entityId, Vector3Int position, Vector3Int direction) : base(entityId, position, direction)
        {

        }


        // Character 类型隐式转换 dbCharacter
        public static implicit operator Character(DbCharacter dbCharacter)
        {
            // 申请实体
            int entityId = EntityManager.Instance.NewEntityId;

            // 把数据库角色 转换为 游戏对象
            Character character = new Character(entityId, new Vector3Int(dbCharacter.X, dbCharacter.Y, dbCharacter.Z), Vector3Int.zero);
            character.Id = dbCharacter.Id;
            character.Name = dbCharacter.Name;
            character.Info.Name = dbCharacter.Name;
            character.Info.Id = dbCharacter.Id;        // 主键
            character.Info.TypeId = dbCharacter.JobId;   // 角色类型
            character.Info.Level = dbCharacter.Level;
            character.Info.Exp = dbCharacter.Exp;
            character.Info.SpaceId = dbCharacter.SpaceId;
            character.Info.Gold = dbCharacter.Gold;
            character.Info.Hp = dbCharacter.Hp;
            character.Info.Mp = dbCharacter.Mp;
            character.Data = dbCharacter;
            return character;
        }

    }
}
