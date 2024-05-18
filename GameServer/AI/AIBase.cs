using GameServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.AI
{
    public abstract class AIBase
    {
        public Monster Owner;

        public AIBase(Monster owner)
        {
            Owner = owner;
        }

        public abstract void Update();
    }
}
