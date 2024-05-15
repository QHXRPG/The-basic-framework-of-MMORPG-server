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
    public class Character : Actor
    {
        // 当前角色客户端的连接
        public Connection conn;

        // 当前角色对应的数据库对象
        public DbCharacter Data = new DbCharacter();

        public Character(int entityId, Vector3Int position, Vector3Int direction) : base(entityId, position, direction)
        {
            // 从数据库中获取 DbCharacter 实例
            var repo = Db.fsql.GetRepository<DbCharacter>();


            // 用中心计时器 单线程计时 执行 保存角色位置 任务（每两秒一次）
            Schedule.Instance.AddTask(()=>
            {
                repo.UpdateAsync(Data);
            }, 2);  // 每两秒调用一次Save
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
