using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Summer;
using GameServer.Mgr;
using Proto.Message;
using System.Threading;

namespace GameServer.Model
{
    /*

        Actor（角色）：
            Actor通常是一个更广泛的概念，可以表示游戏中的任何实体，不仅限于玩家控制的角色。
            Actor可以包括玩家角色、NPC（非玩家角色）、敌人、道具、特效等各种实体。
            Actor通常具有共性的行为和属性，比如位置、生命周期、碰撞等。
            Actor可以是游戏中的任何实体，不一定具有角色的特征。
    */
    public class Actor : Entity
    {
        public int Id{ get { return Info.Id; } set { Info.Id = value; } }

        public string Name { get { return Info.Name; } set { Info.Name = value; } }

        // 当前Actor所在的场景
        public Space Space { get; set; }

        public NCharacter Info { get; set; } = new NCharacter();

        public EntityType Type { get { return Info.EntityType; } set { Info.EntityType = value; } }

        public UnitDefine Define { get; set; }
        public EntityState State;

        public Actor(EntityType entityType, int TID, int level, Vector3Int position, Vector3Int direction)
            : base(position, direction)
        {   
            this.Define = DataManager.Instance.Units[TID];
            this.Info.Name = Define.Name;
            this.Info.Tid = TID;
            this.Info.Level = level;
            this.Info.EntityType = entityType;    // 实体类型
            this.Info.Entity = this.EntityData; 
            this.Speed = Define.Speed;
        }

        public void OnEnterSpace(Space space)
        {
            this.Space = space;
            this.Info.SpaceId = space.Id;
        }
    }
}
