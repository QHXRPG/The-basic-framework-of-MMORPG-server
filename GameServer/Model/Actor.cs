using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Summer;
using GameServer.Mgr;
using Proto.Message;

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
        public int Id{get; set; }

        public string Name { get; set; }

        public int Speed {  get; set; }

        // 当前Actor所在的场景
        public Space Space { get; set; }

        public NCharacter Info { get; set; } = new NCharacter();

        public Actor(int id, Vector3Int position, Vector3Int direction)
            : base(id, position, direction)
        {
            
        }
    }
}
