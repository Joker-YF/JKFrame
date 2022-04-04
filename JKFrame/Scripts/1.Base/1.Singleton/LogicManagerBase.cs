using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JKFrame
{
    /// <summary>
    /// 逻辑管理器基类
    /// </summary>
    public abstract class LogicManagerBase<T> : SingletonMono<T> where T : LogicManagerBase<T>
    {
        /// <summary>
        /// 注册事件的监听
        /// </summary>
        protected virtual void RegisterEventListener() { }
        /// <summary>
        /// 取消事件的监听
        /// </summary>
        protected virtual void CancelEventListener() { }

        protected virtual void OnEnable()
        {
            RegisterEventListener();
        }

        protected virtual void OnDisable()
        {
            CancelEventListener();
        }
    }
}
