using Summer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Summer;

namespace GameServer.Model
{
    // 空间、地图、场景
    public class Space
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public string Music { get; set; }

        // 角色进入场景
        public Dictionary<int, Character> CharacterDict = new Dictionary<int, Character>();

        public void CharacterJoin(Connection conn, Character character)
        {
            CharacterDict[character.Id] = character;

        }
    }
}
