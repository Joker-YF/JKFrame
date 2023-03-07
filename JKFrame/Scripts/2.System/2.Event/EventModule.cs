using System;
using System.Collections.Generic;

namespace JKFrame
{
    public class EventModule
    {
        private static ObjectPoolModule objectPoolModule = new ObjectPoolModule();

        private Dictionary<string, IEventInfo> eventInfoDic = new Dictionary<string, IEventInfo>();
        #region 内部接口、内部类

        private interface IEventInfo { void Destory(); }

        /// <summary>
        /// 无参-事件信息
        /// </summary>
        private class EventInfo : IEventInfo
        {
            public Action action;
            public void Init(Action action) { this.action = action; }
            public void Destory()
            {
                action = null;
                objectPoolModule.PushObject(this);
            }
        }

        /// <summary>
        /// 多参Action事件信息
        /// </summary>
        private class MultipleParameterEventInfo<TAction> : IEventInfo where TAction : MulticastDelegate
        {
            public TAction action;
            public void Init(TAction action) { this.action = action; }
            public void Destory()
            {
                action = null;
                objectPoolModule.PushObject(this);
            }
        };
        #endregion
        #region 添加事件的监听，你想要关心某个事件，当这个事件触时，会执行你传递过来的Action
        /// <summary>
        /// 添加无参事件
        /// </summary>
        public void AddEventListener(string eventName, Action action)
        {
            // 有没有对应的事件可以监听
            if (eventInfoDic.ContainsKey(eventName))
            {
                (eventInfoDic[eventName] as EventInfo).action += action;
            }
            // 没有的话，需要新增 到字典中，并添加对应的Action
            else
            {
                EventInfo eventInfo = objectPoolModule.GetObject<EventInfo>();
                if (eventInfo == null) eventInfo = new EventInfo();
                eventInfo.Init(action);
                eventInfoDic.Add(eventName, eventInfo);
            }
        }


        // <summary>
        // 添加1参事件监听
        // </summary>
        public void AddEventListener<TAction>(string eventName, TAction action) where TAction : MulticastDelegate
        {
            // 有没有对应的事件可以监听
            if (eventInfoDic.TryGetValue(eventName, out IEventInfo eventInfo))
            {
                MultipleParameterEventInfo<TAction> info = (MultipleParameterEventInfo<TAction>)eventInfo;
                info.action = (TAction)Delegate.Combine(info.action, action);
            }
            else AddMultipleParameterEventInfo(eventName, action);
        }

        private void AddMultipleParameterEventInfo<TAction>(string eventName, TAction action) where TAction : MulticastDelegate
        {
            MultipleParameterEventInfo<TAction> newEventInfo = objectPoolModule.GetObject<MultipleParameterEventInfo<TAction>>();
            if (newEventInfo == null) newEventInfo = new MultipleParameterEventInfo<TAction>();
            newEventInfo.Init(action);
            eventInfoDic.Add(eventName, newEventInfo);
        }
        #endregion

        #region 触发无返回值事件，之所以这么多函数，是避免使用params产生数组GC、装箱问题
        /// <summary>
        /// 触发无参的事件
        /// </summary>
        public void EventTrigger(string eventName)
        {
            if (eventInfoDic.ContainsKey(eventName))
            {
                ((EventInfo)eventInfoDic[eventName]).action?.Invoke();
            }
        }
        /// <summary>
        /// 触发1个参数的事件
        /// </summary>
        public void EventTrigger<T>(string eventName, T arg)
        {
            if (eventInfoDic.TryGetValue(eventName, out IEventInfo eventInfo)) ((MultipleParameterEventInfo<Action<T>>)eventInfo).action?.Invoke(arg);
        }
        /// <summary>
        /// 触发2个参数的事件
        /// </summary>
        public void EventTrigger<T0, T1>(string eventName, T0 arg0, T1 arg1)
        {
            if (eventInfoDic.TryGetValue(eventName, out IEventInfo eventInfo)) ((MultipleParameterEventInfo<Action<T0, T1>>)eventInfo).action?.Invoke(arg0, arg1);
        }
        /// <summary>
        /// 触发3个参数的事件
        /// </summary>
        public void EventTrigger<T0, T1, T2>(string eventName, T0 arg0, T1 arg1, T2 arg2)
        {
            if (eventInfoDic.TryGetValue(eventName, out IEventInfo eventInfo)) ((MultipleParameterEventInfo<Action<T0, T1, T2>>)eventInfo).action?.Invoke(arg0, arg1, arg2);
        }
        /// <summary>
        /// 触发4个参数的事件
        /// </summary>
        public void EventTrigger<T0, T1, T2, T3>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            if (eventInfoDic.TryGetValue(eventName, out IEventInfo eventInfo)) ((MultipleParameterEventInfo<Action<T0, T1, T2, T3>>)eventInfo).action?.Invoke(arg0, arg1, arg2, arg3);
        }
        /// <summary>
        /// 触发5个参数的事件
        /// </summary>
        public void EventTrigger<T0, T1, T2, T3, T4>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (eventInfoDic.TryGetValue(eventName, out IEventInfo eventInfo)) ((MultipleParameterEventInfo<Action<T0, T1, T2, T3, T4>>)eventInfo).action?.Invoke(arg0, arg1, arg2, arg3, arg4);
        }
        /// <summary>
        /// 触发6个参数的事件
        /// </summary>
        public void EventTrigger<T0, T1, T2, T3, T4, T5>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            if (eventInfoDic.TryGetValue(eventName, out IEventInfo eventInfo)) ((MultipleParameterEventInfo<Action<T0, T1, T2, T3, T4, T5>>)eventInfo).action?.Invoke(arg0, arg1, arg2, arg3, arg4, arg5);
        }
        /// <summary>
        /// 触发7个参数的事件
        /// </summary>
        public void EventTrigger<T0, T1, T2, T3, T4, T5, T6>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            if (eventInfoDic.TryGetValue(eventName, out IEventInfo eventInfo)) ((MultipleParameterEventInfo<Action<T0, T1, T2, T3, T4, T5, T6>>)eventInfo).action?.Invoke(arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }
        /// <summary>
        /// 触发8个参数的事件
        /// </summary>
        public void EventTrigger<T0, T1, T2, T3, T4, T5, T6, T7>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            if (eventInfoDic.TryGetValue(eventName, out IEventInfo eventInfo)) ((MultipleParameterEventInfo<Action<T0, T1, T2, T3, T4, T5, T6, T7>>)eventInfo).action?.Invoke(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }
        /// <summary>
        /// 触发9个参数的事件
        /// </summary>
        public void EventTrigger<T0, T1, T2, T3, T4, T5, T6, T7, T8>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            if (eventInfoDic.TryGetValue(eventName, out IEventInfo eventInfo)) ((MultipleParameterEventInfo<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8>>)eventInfo).action.Invoke(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }
        /// <summary>
        /// 触发10个参数的事件
        /// </summary>
        public void EventTrigger<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            if (eventInfoDic.TryGetValue(eventName, out IEventInfo eventInfo)) ((MultipleParameterEventInfo<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>>)eventInfo).action?.Invoke(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        /// <summary>
        /// 触发11个参数的事件
        /// </summary>
        public void EventTrigger<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            if (eventInfoDic.TryGetValue(eventName, out IEventInfo eventInfo)) ((MultipleParameterEventInfo<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>>)eventInfo).action?.Invoke(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        /// <summary>
        /// 触发12个参数的事件
        /// </summary>
        public void EventTrigger<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            if (eventInfoDic.TryGetValue(eventName, out IEventInfo eventInfo)) ((MultipleParameterEventInfo<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>>)eventInfo).action?.Invoke(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        /// <summary>
        /// 触发13个参数的事件
        /// </summary>
        public void EventTrigger<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            if (eventInfoDic.TryGetValue(eventName, out IEventInfo eventInfo)) ((MultipleParameterEventInfo<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>>)eventInfo).action?.Invoke(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        /// <summary>
        /// 触发14个参数的事件
        /// </summary>
        public void EventTrigger<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            if (eventInfoDic.TryGetValue(eventName, out IEventInfo eventInfo)) ((MultipleParameterEventInfo<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>>)eventInfo).action?.Invoke(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        /// <summary>
        /// 触发15个参数的事件
        /// </summary>
        public void EventTrigger<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            if (eventInfoDic.TryGetValue(eventName, out IEventInfo eventInfo)) ((MultipleParameterEventInfo<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>>)eventInfo).action?.Invoke(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        /// <summary>
        /// 触发16个参数的事件
        /// </summary>
        public void EventTrigger<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            if (eventInfoDic.TryGetValue(eventName, out IEventInfo eventInfo)) ((MultipleParameterEventInfo<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>>)eventInfo).action?.Invoke(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        #endregion

        #region 取消事件的监听
        /// <summary>
        /// 移除无参的事件监听
        /// </summary>
        public void RemoveEventListener(string eventName, Action action)
        {
            if (eventInfoDic.TryGetValue(eventName, out IEventInfo eventInfo))
            {
                ((EventInfo)eventInfo).action -= action;
            }
        }
        /// <summary>
        /// 移除有参数的事件监听
        /// </summary>
        public void RemoveEventListener<TAction>(string eventName, TAction action) where TAction : MulticastDelegate
        {
            if (eventInfoDic.TryGetValue(eventName, out IEventInfo eventInfo))
            {
                MultipleParameterEventInfo<TAction> info = (MultipleParameterEventInfo<TAction>)eventInfo;
                info.action = (TAction)Delegate.Remove(info.action, action);
            }
        }
        #endregion

        #region 移除事件
        /// <summary>
        /// 移除/删除一个事件
        /// </summary>
        public void RemoveEvent(string eventName)
        {
            if (eventInfoDic.Remove(eventName, out IEventInfo eventInfo))
            {
                eventInfo.Destory();
            }
        }

        /// <summary>
        /// 清空事件中心
        /// </summary>
        public void Clear()
        {
            foreach (string eventName in eventInfoDic.Keys)
            {
                eventInfoDic[eventName].Destory();
            }
            eventInfoDic.Clear();
        }

        #endregion
    }
}
