using System;

namespace JKFrame
{
    /// <summary>
    /// 事件系统管理器
    /// </summary>
    public static class EventSystem
    {
        private static EventModule eventModule;
        public static void Init()
        {
            eventModule = new EventModule();
        }

        #region 添加事件的监听，你想要关心某个事件，当这个事件触时，会执行你传递过来的Action
        /// <summary>
        /// 添加无参事件
        /// </summary>
        public static void AddEventListener(string eventName, Action action)
        {
            eventModule.AddEventListener(eventName, action);
        }

        /// <summary>
        /// 添加1个参数事件
        /// </summary>
        public static void AddEventListener<T>(string eventName, Action<T> action)
        {
            eventModule.AddEventListener<Action<T>>(eventName, action);
        }
        /// <summary>
        /// 添加2个参数事件
        /// </summary>
        public static void AddEventListener<T0, T1>(string eventName, Action<T0, T1> action)
        {
            eventModule.AddEventListener(eventName, action);
        }
        /// <summary>
        /// 添加3个参数事件
        /// </summary>
        public static void AddEventListener<T0, T1, T2>(string eventName, Action<T0, T1, T2> action)
        {
            eventModule.AddEventListener(eventName, action);
        }
        /// <summary>
        /// 添加4个参数事件
        /// </summary>
        public static void AddEventListener<T0, T1, T2, T3>(string eventName, Action<T0, T1, T2, T3> action)
        {
            eventModule.AddEventListener(eventName, action);
        }
        /// <summary>
        /// 添加5个参数事件
        /// </summary>
        public static void AddEventListener<T0, T1, T2, T3, T4>(string eventName, Action<T0, T1, T2, T3, T4> action)
        {
            eventModule.AddEventListener(eventName, action);
        }
        /// <summary>
        /// 添加6个参数事件
        /// </summary>
        public static void AddEventListener<T0, T1, T2, T3, T4, T5>(string eventName, Action<T0, T1, T2, T3, T4, T5> action)
        {
            eventModule.AddEventListener(eventName, action);
        }
        /// <summary>
        /// 添加7个参数事件
        /// </summary>
        public static void AddEventListener<T0, T1, T2, T3, T4, T5, T6>(string eventName, Action<T0, T1, T2, T3, T4, T5, T6> action)
        {
            eventModule.AddEventListener(eventName, action);
        }
        /// <summary>
        /// 添加8个参数事件
        /// </summary>
        public static void AddEventListener<T0, T1, T2, T3, T4, T5, T6, T7>(string eventName, Action<T0, T1, T2, T3, T4, T5, T6, T7> action)
        {
            eventModule.AddEventListener(eventName, action);
        }
        /// <summary>
        /// 添加9个参数事件
        /// </summary>
        public static void AddEventListener<T0, T1, T2, T3, T4, T5, T6, T7, T8>(string eventName, Action<T0, T1, T2, T3, T4, T5, T6, T7, T8> action)
        {
            eventModule.AddEventListener(eventName, action);
        }
        /// <summary>
        /// 添加10个参数事件
        /// </summary>
        public static void AddEventListener<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(string eventName, Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> action)
        {
            eventModule.AddEventListener(eventName, action);
        }
        /// <summary>
        /// 添加11个参数事件
        /// </summary>
        public static void AddEventListener<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string eventName, Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action)
        {
            eventModule.AddEventListener(eventName, action);
        }
        /// <summary>
        /// 添加12个参数事件
        /// </summary>
        public static void AddEventListener<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(string eventName, Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> action)
        {
            eventModule.AddEventListener(eventName, action);
        }
        /// <summary>
        /// 添加13个参数事件
        /// </summary>
        public static void AddEventListener<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(string eventName, Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T2> action)
        {
            eventModule.AddEventListener(eventName, action);
        }
        /// <summary>
        /// 添加14个参数事件
        /// </summary>
        public static void AddEventListener<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(string eventName, Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> action)
        {
            eventModule.AddEventListener(eventName, action);
        }
        /// <summary>
        /// 添加15个参数事件
        /// </summary>
        public static void AddEventListener<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(string eventName, Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> action)
        {
            eventModule.AddEventListener(eventName, action);
        }
        /// <summary>
        /// 添加16个参数事件
        /// </summary>
        public static void AddEventListener<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(string eventName, Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> action)
        {
            eventModule.AddEventListener(eventName, action);
        }
        #endregion

        #region 触发事件
        /// <summary>
        /// 触发无参的事件
        /// </summary>
        public static void EventTrigger(string eventName)
        {
            eventModule.EventTrigger(eventName);
        }
        /// <summary>
        /// 触发1个参数的事件
        /// </summary>
        public static void EventTrigger<T>(string eventName, T arg)
        {
            eventModule.EventTrigger<T>(eventName, arg);
        }
        /// <summary>
        /// 触发2个参数的事件
        /// </summary>
        public static void EventTrigger<T0, T1>(string eventName, T0 arg0, T1 arg1)
        {
            eventModule.EventTrigger(eventName, arg0, arg1);
        }
        /// <summary>
        /// 触发3个参数的事件
        /// </summary>
        public static void EventTrigger<T0, T1, T2>(string eventName, T0 arg0, T1 arg1, T2 arg2)
        {
            eventModule.EventTrigger<T0, T1, T2>(eventName, arg0, arg1, arg2);
        }
        /// <summary>
        /// 触发4个参数的事件
        /// </summary>
        public static void EventTrigger<T0, T1, T2, T3>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            eventModule.EventTrigger<T0, T1, T2, T3>(eventName, arg0, arg1, arg2, arg3);
        }
        /// <summary>
        /// 触发5个参数的事件
        /// </summary>
        public static void EventTrigger<T0, T1, T2, T3, T4>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            eventModule.EventTrigger<T0, T1, T2, T3, T4>(eventName, arg0, arg1, arg2, arg3, arg4);
        }
        /// <summary>
        /// 触发6个参数的事件
        /// </summary>
        public static void EventTrigger<T0, T1, T2, T3, T4, T5>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            eventModule.EventTrigger<T0, T1, T2, T3, T4, T5>(eventName, arg0, arg1, arg2, arg3, arg4, arg5);
        }
        /// <summary>
        /// 触发7个参数的事件
        /// </summary>
        public static void EventTrigger<T0, T1, T2, T3, T4, T5, T6>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            eventModule.EventTrigger<T0, T1, T2, T3, T4, T5, T6>(eventName, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }
        /// <summary>
        /// 触发8个参数的事件
        /// </summary>
        public static void EventTrigger<T0, T1, T2, T3, T4, T5, T6, T7>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            eventModule.EventTrigger<T0, T1, T2, T3, T4, T5, T6, T7>(eventName, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }
        /// <summary>
        /// 触发9个参数的事件
        /// </summary>
        public static void EventTrigger<T0, T1, T2, T3, T4, T5, T6, T7, T8>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            eventModule.EventTrigger<T0, T1, T2, T3, T4, T5, T6, T7, T8>(eventName, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }
        /// <summary>
        /// 触发10个参数的事件
        /// </summary>
        public static void EventTrigger<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            eventModule.EventTrigger<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(eventName, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        /// <summary>
        /// 触发11个参数的事件
        /// </summary>
        public static void EventTrigger<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            eventModule.EventTrigger<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(eventName, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }


        /// <summary>
        /// 触发12个参数的事件
        /// </summary>
        public static void EventTrigger<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            eventModule.EventTrigger<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(eventName, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        /// <summary>
        /// 触发13个参数的事件
        /// </summary>
        public static void EventTrigger<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            eventModule.EventTrigger<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(eventName, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        /// <summary>
        /// 触发14个参数的事件
        /// </summary>
        public static void EventTrigger<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            eventModule.EventTrigger<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(eventName, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        /// <summary>
        /// 触发15个参数的事件
        /// </summary>
        public static void EventTrigger<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            eventModule.EventTrigger<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(eventName, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        /// <summary>
        /// 触发16个参数的事件
        /// </summary>
        public static void EventTrigger<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            eventModule.EventTrigger<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(eventName, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        #endregion

        #region 取消事件的监听
        /// <summary>
        /// 移除无参的事件监听
        /// </summary>
        public static void RemoveEventListener(string eventName, Action action)
        {
            eventModule.RemoveEventListener(eventName, action);
        }
        /// <summary>
        /// 移除1个参数的事件监听
        /// </summary>
        public static void RemoveEventListener<T>(string eventName, Action<T> action)
        {
            eventModule.RemoveEventListener(eventName, action);
        }
        /// <summary>
        /// 移除2个参数的事件监听
        /// </summary>
        public static void RemoveEventListener<T0, T1>(string eventName, Action<T0, T1> action)
        {
            eventModule.RemoveEventListener(eventName, action);
        }
        /// <summary>
        /// 移除3个参数的事件监听
        /// </summary>
        public static void RemoveEventListener<T0, T1, T2>(string eventName, Action<T0, T1, T2> action)
        {
            eventModule.RemoveEventListener(eventName, action);
        }
        /// <summary>
        /// 移除4个参数的事件监听
        /// </summary>
        public static void RemoveEventListener<T0, T1, T2, T3>(string eventName, Action<T0, T1, T2, T3> action)
        {
            eventModule.RemoveEventListener(eventName, action);
        }
        /// <summary>
        /// 移除5个参数的事件监听
        /// </summary>
        public static void RemoveEventListener<T0, T1, T2, T3, T4>(string eventName, Action<T0, T1, T2, T3, T4> action)
        {
            eventModule.RemoveEventListener(eventName, action);
        }
        /// <summary>
        /// 移除6个参数的事件监听
        /// </summary>
        public static void RemoveEventListener<T0, T1, T2, T3, T4, T5>(string eventName, Action<T0, T1, T2, T3, T4, T5> action)
        {
            eventModule.RemoveEventListener(eventName, action);
        }
        /// <summary>
        /// 移除7个参数的事件监听
        /// </summary>
        public static void RemoveEventListener<T0, T1, T2, T3, T4, T5, T6>(string eventName, Action<T0, T1, T2, T3, T4, T5, T6> action)
        {
            eventModule.RemoveEventListener(eventName, action);
        }
        /// <summary>
        /// 移除8个参数的事件监听
        /// </summary>
        public static void RemoveEventListener<T0, T1, T2, T3, T4, T5, T6, T7>(string eventName, Action<T0, T1, T2, T3, T4, T5, T6, T7> action)
        {
            eventModule.RemoveEventListener(eventName, action);
        }
        /// <summary>
        /// 移除9个参数的事件监听
        /// </summary>
        public static void RemoveEventListener<T0, T1, T2, T3, T4, T5, T6, T7, T8>(string eventName, Action<T0, T1, T2, T3, T4, T5, T6, T7, T8> action)
        {
            eventModule.RemoveEventListener(eventName, action);
        }
        /// <summary>
        /// 移除10个参数的事件监听
        /// </summary>
        public static void RemoveEventListener<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(string eventName, Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> action)
        {
            eventModule.RemoveEventListener(eventName, action);
        }
        /// <summary>
        /// 移除11个参数的事件监听
        /// </summary>
        public static void RemoveEventListener<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string eventName, Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> action)
        {
            eventModule.RemoveEventListener(eventName, action);
        }
        /// <summary>
        /// 移除12个参数的事件监听
        /// </summary>
        public static void RemoveEventListener<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(string eventName, Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> action)
        {
            eventModule.RemoveEventListener(eventName, action);
        }
        /// <summary>
        /// 移除13个参数的事件监听
        /// </summary>
        public static void RemoveEventListener<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(string eventName, Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> action)
        {
            eventModule.RemoveEventListener(eventName, action);
        }
        /// <summary>
        /// 移除14个参数的事件监听
        /// </summary>
        public static void RemoveEventListener<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(string eventName, Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> action)
        {
            eventModule.RemoveEventListener(eventName, action);
        }
        /// <summary>
        /// 移除15个参数的事件监听
        /// </summary>
        public static void RemoveEventListener<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(string eventName, Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> action)
        {
            eventModule.RemoveEventListener(eventName, action);
        }
        /// <summary>
        /// 移除16个参数的事件监听
        /// </summary>
        public static void RemoveEventListener<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(string eventName, Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> action)
        {
            eventModule.RemoveEventListener(eventName, action);
        }
        #endregion

        #region 移除事件
        /// <summary>
        /// 移除/删除一个事件
        /// </summary>
        public static void RemoveEvent(string eventName)
        {
            eventModule.RemoveEvent(eventName);
        }

        /// <summary>
        /// 清空事件中心
        /// </summary>
        public static void Clear()
        {
            eventModule.Clear();
        }

        #endregion

        #region 类型事件
        /// <summary>
        /// 添加类型事件的监听
        /// 本质上是以T的名称作为事件名称
        /// </summary>
        /// <typeparam name="T">参数类型,建议为struct类型</typeparam>
        /// <param name="action">回调函数</param>
        public static void AddTypeEventListener<T>(Action<T> action)
        {
            AddEventListener<T>(nameof(T), action);
        }

        /// <summary>
        /// 移除/删除一个类型事件
        /// </summary>
        /// <typeparam name="T">事件的参数类型</typeparam>
        public static void RemoveTypeEvent<T>()
        {
            RemoveEvent(nameof(T));
        }

        /// <summary>
        /// 移除类型事件的监听
        /// </summary>
        public static void RemoveTypeEventListener<T>(Action<T> action)
        {
            eventModule.RemoveEventListener(nameof(T), action);
        }

        /// <summary>
        /// 触发类型事件
        /// </summary>
        public static void TypeEventTrigger<T>(T arg)
        {
            EventTrigger(nameof(T), arg);
        }
        #endregion
    }
}
