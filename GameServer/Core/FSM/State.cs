using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.FSM
{
    // 状态基础类
    public class State<T>
    {
        public FsmSystem<T> fsm;

        public T P => fsm.P;

        // 进入状态
        public virtual void OnEnter()
        {

        }

        // 循环更新
        public virtual void OnUpdate()
        {

        }

        // 离开状态
        public virtual void OnLeave()
        {

        }
    }
}
