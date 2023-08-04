using System;
using System.Collections.Generic;

namespace JKFrame
{
    public interface IStateMachineOwner { }

    /// <summary>
    /// 状态机控制器
    /// </summary>
    public class StateMachine
    {
        // 当前状态
        public Type CurrStateType { get; private set; } = null;

        // 当前生效中的状态
        private StateBase currStateObj;

        // 宿主
        private IStateMachineOwner owner;

        // 所有的状态 Key:状态枚举的值 Value:具体的状态
        private Dictionary<Type, StateBase> stateDic = new Dictionary<Type, StateBase>();

        private Dictionary<string, object> stateShareDataDic;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="owner">宿主</param>
        /// <param name="enableStateShareData">启用状态共享数据，但是注意存在装箱和拆箱情况！</param>
        /// <typeparam name="T">初始状态类型</typeparam>
        public void Init<T>(IStateMachineOwner owner, bool enableStateShareData = false) where T : StateBase, new()
        {
            this.owner = owner;
            if (enableStateShareData) stateShareDataDic = new Dictionary<string, object>();
            ChangeState<T>();
        }
        /// <summary>
        /// 初始化（无默认状态，状态机待机）
        /// </summary>
        /// <param name="owner">宿主</param>
        public void Init(IStateMachineOwner owner, bool enableStateShareData = false)
        {
            if (enableStateShareData) stateShareDataDic = new Dictionary<string, object>();
            this.owner = owner;
        }

        #region 状态
        /// <summary>
        /// 切换状态
        /// </summary>
        /// <typeparam name="T">具体要切换到的状态脚本类型</typeparam>
        /// <param name="newState">新状态</param>
        /// <param name="reCurrstate">新状态和当前状态一致的情况下，是否也要切换</param>
        /// <returns></returns>
        public bool ChangeState<T>(bool reCurrstate = false) where T : StateBase, new()
        {
            Type stateType = typeof(T);
            // 状态一致，并且不需要刷新状态，则切换失败
            if (stateType == CurrStateType && !reCurrstate) return false;

            // 退出当前状态
            if (currStateObj != null)
            {
                currStateObj.Exit();
                currStateObj.RemoveUpdate(currStateObj.Update);
                currStateObj.RemoveLateUpdate(currStateObj.LateUpdate);
                currStateObj.RemoveFixedUpdate(currStateObj.FixedUpdate);
            }
            // 进入新状态
            currStateObj = GetState<T>();
            CurrStateType = stateType;
            currStateObj.Enter();
            currStateObj.AddUpdate(currStateObj.Update);
            currStateObj.AddLateUpdate(currStateObj.LateUpdate);
            currStateObj.AddFixedUpdate(currStateObj.FixedUpdate);

            return true;
        }

        /// <summary>
        /// 从对象池获取一个状态
        /// </summary>
        private StateBase GetState<T>() where T : StateBase, new()
        {
            Type stateType = typeof(T);
            if (stateDic.TryGetValue(stateType, out var st)) return st;
            StateBase state = ResSystem.GetOrNew<T>();
            state.InitInternalData(this);
            state.Init(owner);
            stateDic.Add(stateType, state);
            return state;
        }

        /// <summary>
        /// 停止工作
        /// 把所有状态都释放，但是StateMachine未来还可以工作
        /// </summary>
        public void Stop()
        {
            // 处理当前状态的额外逻辑
            if (currStateObj != null)
            {
                currStateObj.Exit();
                currStateObj.RemoveUpdate(currStateObj.Update);
                currStateObj.RemoveLateUpdate(currStateObj.LateUpdate);
                currStateObj.RemoveFixedUpdate(currStateObj.FixedUpdate);
                currStateObj = null;
            }
            CurrStateType = null;
            // 处理缓存中所有状态的逻辑
            foreach (var state in stateDic.Values)
            {
                state.UnInit();
            }
            stateDic.Clear();
        }
        #endregion
        #region 状态共享数据
        public bool TryGetShareData<T>(string key, out T data)
        {
            bool res = stateShareDataDic.TryGetValue(key, out object stateData);
            if (res)
            {
                data = (T)stateData;
            }
            else
            {
                data = default(T);
            }
            return res;
        }
        public void AddShareData(string key, object data)
        {
            stateShareDataDic.Add(key, data);
        }
        public bool RemoveShareData(string key)
        {
            return stateShareDataDic.Remove(key);
        }
        public bool ContainsShareData(string key)
        {
            return stateShareDataDic.ContainsKey(key);
        }
        public bool UpdateShareData(string key, object data)
        {
            if (ContainsShareData(key))
            {
                stateShareDataDic[key] = data;
                return true;
            }
            else return false;
        }
        public void CleanShareData()
        {
            stateShareDataDic?.Clear();
        }
        #endregion

        /// <summary>
        /// 销毁，宿主应该释放掉StateMachine的引用
        /// </summary>
        public void Destroy()
        {
            // 处理所有状态
            Stop();
            // 清除共享数据
            CleanShareData();
            // 放弃所有资源的引用
            owner = null;
            // 放进对象池
            this.ObjectPushPool();
        }
    }
}