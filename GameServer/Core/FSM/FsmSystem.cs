using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.FSM
{
    // 状态管理器 （添加/移除状态）
    public class FsmSystem<T>
    {
        public FsmSystem(T param) 
        {
            this.P = param;
        }

        // 状态字典
        private Dictionary<string, State<T>> _dict = new Dictionary<string, State<T>>();

        // 当前状态 （外部只能获取，不能设置）
        public string CurrentStateId { get; private set; }
        public State<T> CurrentState { get; private set; }

        public T P; // 共享参数


        // 添加状态
        public void AddState(string stateId, State<T> state)
        {
            if(CurrentStateId is null)
            {
                CurrentStateId = stateId;
                CurrentState = state;
            }
            _dict[stateId] = state;
            state.fsm = this;
        }


        //移除状态
        public void RemoveState(string stateId)
        {
            if(_dict.ContainsKey(stateId))
            {
                _dict[stateId].fsm = null;
                _dict.Remove(stateId);
            }
        }

        // 切换状态
        public void ChangeState(string stateId)
        {
            if (CurrentStateId == stateId) return;
            if (!_dict.ContainsKey(stateId)) return;
            if(CurrentState != null)
            {
                CurrentState.OnLeave();
            }
            CurrentStateId = stateId;
            CurrentState = _dict[stateId];
            CurrentState.OnEnter();
        }

        public void Update()
        {
            CurrentState?.OnUpdate();
        }
    }
}
