using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JKFrame
{
    /// <summary>
    /// 整个游戏只有一个Update、LateUpdate等
    /// </summary>
    public class MonoSystem : MonoBehaviour
    {
        private MonoSystem() { }
        private static MonoSystem instance;
        private Action updateEvent;
        private Action lateUpdateEvent;
        private Action fixedUpdateEvent;

        public static void Init()
        {
            instance = JKFrameRoot.RootTransform.GetComponent<MonoSystem>();
            instance.updateEvent = null;
            instance.lateUpdateEvent = null;
            instance.fixedUpdateEvent = null;
        }

        #region 生命周期函数
        /// <summary>
        /// 添加Update监听
        /// </summary>
        /// <param name="action"></param>
        public static void AddUpdateListener(Action action)
        {
            instance.updateEvent += action;
        }

        /// <summary>
        /// 移除Update监听
        /// </summary>
        /// <param name="action"></param>
        public static void RemoveUpdateListener(Action action)
        {
            instance.updateEvent -= action;
        }

        /// <summary>
        /// 添加LateUpdate监听
        /// </summary>
        /// <param name="action"></param>
        public static void AddLateUpdateListener(Action action)
        {
            instance.lateUpdateEvent += action;
        }

        /// <summary>
        /// 移除LateUpdate监听
        /// </summary>
        /// <param name="action"></param>
        public static void RemoveLateUpdateListener(Action action)
        {
            instance.lateUpdateEvent -= action;
        }

        /// <summary>
        /// 添加FixedUpdate监听
        /// </summary>
        /// <param name="action"></param>
        public static void AddFixedUpdateListener(Action action)
        {
            instance.fixedUpdateEvent += action;
        }

        /// <summary>
        /// 移除FixedUpdate监听
        /// </summary>
        /// <param name="action"></param>
        public static void RemoveFixedUpdateListener(Action action)
        {
            instance.fixedUpdateEvent -= action;
        }

        private void Update()
        {
            updateEvent?.Invoke();
        }
        private void LateUpdate()
        {
            lateUpdateEvent?.Invoke();
        }
        private void FixedUpdate()
        {
            fixedUpdateEvent?.Invoke();
        }

        #endregion
        #region 协程
        private Dictionary<object, List<Coroutine>> coroutineDic = new Dictionary<object, List<Coroutine>>();
        private static ObjectPoolModule poolModule = new ObjectPoolModule();

        /// <summary>
        /// 启动一个协程序
        /// </summary>
        public static Coroutine Start_Coroutine(IEnumerator coroutine)
        {
            return instance.StartCoroutine(coroutine);
        }

        /// <summary>
        /// 启动一个协程序并且绑定某个对象
        /// </summary>
        public static Coroutine Start_Coroutine(object obj, IEnumerator coroutine)
        {
            Coroutine _coroutine = instance.StartCoroutine(coroutine);
            if (!instance.coroutineDic.TryGetValue(obj, out List<Coroutine> coroutineList))
            {
                coroutineList = poolModule.GetObject<List<Coroutine>>();
                if (coroutineList == null) coroutineList = new List<Coroutine>();
                instance.coroutineDic.Add(obj, coroutineList);
            }
            coroutineList.Add(_coroutine);
            return _coroutine;
        }

        /// <summary>
        /// 停止一个协程序并基于某个对象
        /// </summary>
        public static void Stop_Coroutine(object obj, Coroutine routine)
        {
            if (instance.coroutineDic.TryGetValue(obj, out List<Coroutine> coroutineList))
            {
                instance.StopCoroutine(routine);
                coroutineList.Remove(routine);
            }
        }

        /// <summary>
        /// 停止一个协程序
        /// </summary>
        public static void Stop_Coroutine(Coroutine routine)
        {
            instance.StopCoroutine(routine);
        }

        /// <summary>
        /// 停止某个对象的全部协程
        /// </summary>
        public static void StopAllCoroutine(object obj)
        {
            if (instance.coroutineDic.Remove(obj, out List<Coroutine> coroutineList))
            {
                for (int i = 0; i < coroutineList.Count; i++)
                {
                    instance.StopCoroutine(coroutineList[i]);
                }
                coroutineList.Clear();
                poolModule.PushObject(coroutineList);
            }
        }

        /// <summary>
        /// 整个系统全部协程都会停止
        /// </summary>
        public static void StopAllCoroutine()
        {
            // 全部数据都会无效
            foreach (List<Coroutine> item in instance.coroutineDic.Values)
            {
                item.Clear();
                poolModule.PushObject(item);
            }
            instance.coroutineDic.Clear();
            instance.StopAllCoroutines();
        }
        #endregion
    }
}