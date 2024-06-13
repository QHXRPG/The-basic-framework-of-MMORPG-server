using GameServer.Mgr;
using GameServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Core
{
    public class Game
    {
        public static Actor GetUnit(int entityId)
        {
            return EntityManager.Instance.GetEntity(entityId) as Actor;  
        }
    }
}
