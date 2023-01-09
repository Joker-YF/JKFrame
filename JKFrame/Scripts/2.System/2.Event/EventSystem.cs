using System;
using System.Collections.Generic;
using System.Reflection;

namespace JKFrame
{
    /// <summary>
    /// 事件系统管理器
    /// </summary>
    public static class EventSystem
    {
        private static EventModule eventModule;
        static EventSystem()
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
        public static void AddEventListener<TAction>(string eventName, TAction action) where TAction : MulticastDelegate
        {
            eventModule.AddEventListener<TAction>(eventName, action);
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
            eventModule.EventTrigger<T0, T1, T2, T3,T4>(eventName, arg0, arg1, arg2, arg3,arg4);
        }
        /// <summary>
        /// 触发6个参数的事件
        /// </summary>
        public static void EventTrigger<T0, T1, T2, T3, T4, T5>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            eventModule.EventTrigger<T0, T1, T2, T3, T4,T5>(eventName, arg0, arg1, arg2, arg3, arg4,arg5);
        }
        /// <summary>
        /// 触发7个参数的事件
        /// </summary>
        public static void EventTrigger<T0, T1, T2, T3, T4, T5, T6>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            eventModule.EventTrigger<T0, T1, T2, T3, T4, T5,T6>(eventName, arg0, arg1, arg2, arg3, arg4, arg5,arg6);
        }
        /// <summary>
        /// 触发8个参数的事件
        /// </summary>
        public static void EventTrigger<T0, T1, T2, T3, T4, T5, T6, T7>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            eventModule.EventTrigger<T0, T1, T2, T3, T4, T5, T6,T7>(eventName, arg0, arg1, arg2, arg3, arg4, arg5, arg6,arg7);
        }
        /// <summary>
        /// 触发9个参数的事件
        /// </summary>
        public static void EventTrigger<T0, T1, T2, T3, T4, T5, T6, T7, T8>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            eventModule.EventTrigger<T0, T1, T2, T3, T4, T5, T6, T7,T8>(eventName, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7,arg8);
        }
        /// <summary>
        /// 触发10个参数的事件
        /// </summary>
        public static void EventTrigger<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(string eventName, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            eventModule.EventTrigger<T0, T1, T2, T3, T4, T5, T6, T7,T8,T9>(eventName, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7,arg8,arg9);
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
        public static void RemoveEventListener<TAction>(string eventName, TAction action)where TAction:MulticastDelegate
        {
            eventModule.RemoveEventListener<TAction>(eventName, action);
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
    }
}