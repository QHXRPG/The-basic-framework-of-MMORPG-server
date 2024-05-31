using FreeSql;
using GameServer.Database;
using GameServer.Model;
using Proto.Message;
using Summer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Mgr
{
    /*
        统一管理全部的角色：创建、移除、获取
     */
    public class CharacterManager : Singleton<CharacterManager>
    {
        // 把新创建的角色记录起来 (在Space.cs中有定义类似的变量，但是那个只适用于当前场景，其它场景没办法存储)
        // 游戏当中任何场景所有的角色都在这个字典中记录
        private ConcurrentDictionary<int, Character> Characters = new ConcurrentDictionary<int, Character>();

        // 从数据库中获取 DbCharacter 实例
        IBaseRepository<DbCharacter> repo = Db.fsql.GetRepository<DbCharacter>();

        public CharacterManager() 
        {



            // 用中心计时器 单线程计时 执行 保存角色位置 任务（每两秒一次）
            Scheduler.Instance.AddTask(Save, 2); 
        }

        // 通过数据库对象创建一个角色对象
        public Character CreateCharacter(DbCharacter dbCharacter)
        {
            Character character = new Character(dbCharacter);
            Characters[dbCharacter.Id] = character;

            // 把这个 Character 添加到 AllEntities
            EntityManager.Instance.AddEntity(dbCharacter.SpaceId, character);

            return character;   
        }

        public void RemoveCharacter(int CharacterId)
        {
            Character character;
            if(Characters.TryRemove(CharacterId, out character))
            {
                EntityManager.Instance.RemoveEntity(character.Data.SpaceId, character);
            }
        }

        public Character GetCharacter(int CharacterId) 
        {
            // 找不到则返回空
            return Characters.GetValueOrDefault(CharacterId, null);
        }

        public void Clear()
        {
            Characters.Clear();
        }
        private void Save()
        {
            foreach (var character in Characters.Values) 
            {
                // 记录当前客户端自己的角色的位置信息, 以便于让  Character 对象中的 Save 更新给数据库
                character.Data.X = character.Position.x; 
                character.Data.Y = character.Position.y;
                character.Data.Z = character.Position.z;
                repo.UpdateAsync(character.Data);
            }
        }
    }
}
