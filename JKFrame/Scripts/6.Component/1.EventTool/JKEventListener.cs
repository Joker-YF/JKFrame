using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace JKFrame
{
    /// <summary>
    /// 事件类型,枚举本质上是int，所以您可以自定义事件，只需要枚举的值和下方不重复即可
    /// </summary>
    public enum JKEventType
    {
        OnMouseEnter = -10001,
        OnMouseExit = -10002,
        OnClick = -10003,
        OnClickDown = -10004,
        OnClickUp = -10005,
        OnDrag = -10006,
        OnBeginDrag = -10007,
        OnEndDrag = -10008,
        OnCollisionEnter = -10009,
        OnCollisionStay = -10010,
        OnCollisionExit = -10011,
        OnCollisionEnter2D = -10012,
        OnCollisionStay2D = -10013,
        OnCollisionExit2D = -10014,
        OnTriggerEnter = -10015,
        OnTriggerStay = -10016,
        OnTriggerExit = -10017,
        OnTriggerEnter2D = -10018,
        OnTriggerStay2D = -10019,
        OnTriggerExit2D = -10020,
        OnReleaseAddressableAsset = -10021,
        OnDestroy = -10022,
    }

    public interface IMouseEvent : IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
    { }

    /// <summary>
    /// 事件工具
    /// 可以添加 鼠标、碰撞、触发等事件
    /// </summary>
    public class JKEventListener : MonoBehaviour, IMouseEvent
    {
        private static ObjectPoolModule poolModul = new ObjectPoolModule();
        #region 内部类、接口等
        /// <summary>
        /// 持有关键字典数据，主要用于将这个引用放入对象池中
        /// </summary>
        private class JKEventListenerData
        {
            public Dictionary<int, IJKEventListenerEventInfos> eventInfoDic = new Dictionary<int, JKEventListener.IJKEventListenerEventInfos>();
        }


        private interface IJKEventListenerEventInfo<T>
        {
            void TriggerEvent(T eventData);
            void Destory();
        }

        /// <summary>
        /// 某个事件中一个事件的数据包装类
        /// </summary>
        private class JKEventListenerEventInfo<T, TEventArg> : IJKEventListenerEventInfo<T>
        {
            // T：事件本身的参数（PointerEventData、Collision）
            // object[]:事件的参数
            public Action<T, TEventArg> action;
            public TEventArg arg;
            public void Init(Action<T, TEventArg> action, TEventArg args = default(TEventArg))
            {
                this.action = action;
                this.arg = args;
            }
            public void Destory()
            {
                this.action = null;
                this.arg = default(TEventArg);
                poolModul.PushObject(this);
            }
            public void TriggerEvent(T eventData)
            {
                action?.Invoke(eventData, arg);
            }
        }


        /// <summary>
        /// 某个事件中一个事件的数据包装类（无参）
        /// </summary>
        private class JKEventListenerEventInfo<T> : IJKEventListenerEventInfo<T>
        {
            // T：事件本身的参数（PointerEventData、Collision）
            // object[]:事件的参数
            public Action<T> action;
            public void Init(Action<T> action)
            {
                this.action = action;
            }
            public void Destory()
            {
                this.action = null;
                poolModul.PushObject(this);
            }
            public void TriggerEvent(T eventData)
            {
                action?.Invoke(eventData);
            }
        }


        interface IJKEventListenerEventInfos
        {
            void RemoveAll();

        }

        /// <summary>
        /// 一类事件的数据包装类型：包含多个JKEventListenerEventInfo
        /// </summary>
        private class JKEventListenerEventInfos<T> : IJKEventListenerEventInfos
        {
            // 所有的事件
            private List<IJKEventListenerEventInfo<T>> eventList = new List<IJKEventListenerEventInfo<T>>();


            /// <summary>
            /// 添加事件 无参
            /// </summary>
            public void AddListener(Action<T> action)
            {
                JKEventListenerEventInfo<T> info = poolModul.GetObject<JKEventListenerEventInfo<T>>();
                if (info == null) info = new JKEventListenerEventInfo<T>();
                info.Init(action);
                eventList.Add(info);
            }

            /// <summary>
            /// 添加事件 有参
            /// </summary>
            public void AddListener<TEventArg>(Action<T, TEventArg> action, TEventArg args = default(TEventArg))
            {
                JKEventListenerEventInfo<T, TEventArg> info = poolModul.GetObject<JKEventListenerEventInfo<T, TEventArg>>();
                if (info == null) info = new JKEventListenerEventInfo<T, TEventArg>();
                info.Init(action, args);
                eventList.Add(info);
            }

            public void TriggerEvent(T evetData)
            {
                for (int i = 0; i < eventList.Count; i++)
                {
                    eventList[i].TriggerEvent(evetData);
                }
            }


            /// <summary>
            /// 移除事件（无参）
            /// 同一个函数+参数注册过多次，无论如何该方法只会移除一个事件
            /// </summary>
            public void RemoveListener(Action<T> action)
            {
                for (int i = 0; i < eventList.Count; i++)
                {
                    JKEventListenerEventInfo<T> eventInfo = eventList[i] as JKEventListenerEventInfo<T>;
                    if (eventInfo == null) continue; // 类型不符

                    // 找到这个事件，查看是否相等
                    if (eventInfo.action.Equals(action))
                    {
                        // 移除
                        eventInfo.Destory();
                        eventList.RemoveAt(i);
                        return;
                    }
                }
            }

            /// <summary>
            /// 移除事件（有参）
            /// 同一个函数+参数注册过多次，无论如何该方法只会移除一个事件
            /// </summary>
            public void RemoveListener<TEventArg>(Action<T, TEventArg> action, TEventArg args = default(TEventArg))
            {
                for (int i = 0; i < eventList.Count; i++)
                {
                    JKEventListenerEventInfo<T, TEventArg> eventInfo = eventList[i] as JKEventListenerEventInfo<T, TEventArg>;
                    if (eventInfo == null) continue; // 类型不符

                    // 找到这个事件，查看是否相等
                    if (eventInfo.action.Equals(action))
                    {
                        // 移除
                        eventInfo.Destory();
                        eventList.RemoveAt(i);
                        return;
                    }
                }
            }

            /// <summary>
            /// 移除全部，全部放进对象池
            /// </summary>
            public void RemoveAll()
            {
                for (int i = 0; i < eventList.Count; i++)
                {
                    eventList[i].Destory();
                }
                eventList.Clear();
                poolModul.PushObject(this);
            }
        }

        #endregion

        private JKEventListenerData data;
        private JKEventListenerData Data
        {
            get
            {
                if (data == null)
                {
                    data = poolModul.GetObject<JKEventListenerData>();
                    if (data == null) data = new JKEventListenerData();
                }
                return data;
            }
        }

        #region 外部的访问
        /// <summary>
        /// 添加无参事件 
        /// </summary>
        public void AddListener<T>(int eventTypeInt, Action<T> action)
        {
            if (Data.eventInfoDic.TryGetValue(eventTypeInt, out IJKEventListenerEventInfos info))
            {
                ((JKEventListenerEventInfos<T>)info).AddListener(action);
            }
            else
            {
                JKEventListenerEventInfos<T> infos = poolModul.GetObject<JKEventListenerEventInfos<T>>();
                if (infos == null) infos = new JKEventListenerEventInfos<T>();
                infos.AddListener(action);
                Data.eventInfoDic.Add(eventTypeInt, infos);
            }
        }

        /// <summary>
        /// 添加事件（有参）
        /// </summary>
        public void AddListener<T, TEventArg>(int eventTypeInt, Action<T, TEventArg> action, TEventArg args)
        {
            if (Data.eventInfoDic.TryGetValue(eventTypeInt, out IJKEventListenerEventInfos info))
            {
                ((JKEventListenerEventInfos<T>)info).AddListener(action, args);
            }
            else
            {
                JKEventListenerEventInfos<T> infos = poolModul.GetObject<JKEventListenerEventInfos<T>>();
                if (infos == null) infos = new JKEventListenerEventInfos<T>();
                infos.AddListener(action, args);
                Data.eventInfoDic.Add(eventTypeInt, infos);
            }
        }


        /// <summary>
        /// 添加事件（无参）
        /// </summary>
        public void AddListener<T>(JKEventType eventType, Action<T> action)
        {
            AddListener((int)eventType, action);
        }
        /// <summary>
        /// 添加事件（有参）
        /// </summary>
        public void AddListener<T, TEventArg>(JKEventType eventType, Action<T, TEventArg> action, TEventArg args)
        {
            AddListener((int)eventType, action, args);
        }


        /// <summary>
        /// 移除事件（无参）
        /// </summary>
        public void RemoveListener<T>(int eventTypeInt, Action<T> action)
        {
            if (Data.eventInfoDic.TryGetValue(eventTypeInt, out IJKEventListenerEventInfos info))
            {
                ((JKEventListenerEventInfos<T>)info).RemoveListener(action);
            }
        }
        /// <summary>
        /// 移除事件（无参）
        /// </summary>
        public void RemoveListener<T>(JKEventType eventType, Action<T> action)
        {
            RemoveListener((int)eventType, action);
        }


        /// <summary>
        /// 移除事件（有参）
        /// </summary>
        public void RemoveListener<T, TEventArg>(int eventTypeInt, Action<T, TEventArg> action)
        {
            if (Data.eventInfoDic.TryGetValue(eventTypeInt, out IJKEventListenerEventInfos info))
            {
                ((JKEventListenerEventInfos<T>)info).RemoveListener(action);
            }
        }
        /// <summary>
        /// 移除事件（有参）
        /// </summary>
        public void RemoveListener<T, TEventArg>(JKEventType eventType, Action<T, TEventArg> action)
        {
            RemoveListener((int)eventType, action);
        }

        /// <summary>
        /// 移除某一个事件类型下的全部事件
        /// </summary>
        public void RemoveAllListener(JKEventType eventType)
        {
            RemoveAllListener((int)eventType);
        }

        /// <summary>
        /// 移除某一个事件类型下的全部事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventType"></param>
        public void RemoveAllListener(int eventType)
        {
            if (Data.eventInfoDic.TryGetValue(eventType, out IJKEventListenerEventInfos infos))
            {
                infos.RemoveAll();
                Data.eventInfoDic.Remove(eventType);
            }
        }

        /// <summary>
        /// 移除全部事件
        /// </summary>
        public void RemoveAllListener()
        {
            foreach (IJKEventListenerEventInfos infos in Data.eventInfoDic.Values)
            {
                infos.RemoveAll();
            }

            data.eventInfoDic.Clear();
            // 将整个数据容器放入对象池
            poolModul.PushObject(data);
            data = null;
        }

        #endregion
        /// <summary>
        /// 触发事件
        /// </summary>
        public void TriggerAction<T>(int eventTypeInt, T eventData)
        {
            if (Data.eventInfoDic.TryGetValue(eventTypeInt, out IJKEventListenerEventInfos infos))
            {
                (infos as JKEventListenerEventInfos<T>).TriggerEvent(eventData);
            }
        }
        /// <summary>
        /// 触发事件
        /// </summary>
        public void TriggerAction<T>(JKEventType eventType, T eventData)
        {
            TriggerAction<T>((int)eventType, eventData);
        }

        #region 鼠标事件
        public void OnPointerEnter(PointerEventData eventData)
        {
            TriggerAction(JKEventType.OnMouseEnter, eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TriggerAction(JKEventType.OnMouseExit, eventData);
        }


        public void OnBeginDrag(PointerEventData eventData)
        {
            TriggerAction(JKEventType.OnBeginDrag, eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            TriggerAction(JKEventType.OnDrag, eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            TriggerAction(JKEventType.OnEndDrag, eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            TriggerAction(JKEventType.OnClick, eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            TriggerAction(JKEventType.OnClickDown, eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            TriggerAction(JKEventType.OnClickUp, eventData);
        }
        #endregion

        #region 碰撞事件
        private void OnCollisionEnter(Collision collision)
        {
            TriggerAction(JKEventType.OnCollisionEnter, collision);
        }
        private void OnCollisionStay(Collision collision)
        {
            TriggerAction(JKEventType.OnCollisionStay, collision);
        }
        private void OnCollisionExit(Collision collision)
        {
            TriggerAction(JKEventType.OnCollisionExit, collision);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            TriggerAction(JKEventType.OnCollisionEnter2D, collision);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            TriggerAction(JKEventType.OnCollisionStay2D, collision);
        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            TriggerAction(JKEventType.OnCollisionExit2D, collision);
        }
        #endregion

        #region 触发事件
        private void OnTriggerEnter(Collider other)
        {
            TriggerAction(JKEventType.OnTriggerEnter, other);
        }
        private void OnTriggerStay(Collider other)
        {
            TriggerAction(JKEventType.OnTriggerStay, other);
        }
        private void OnTriggerExit(Collider other)
        {
            TriggerAction(JKEventType.OnTriggerExit, other);
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            TriggerAction(JKEventType.OnTriggerEnter2D, collision);
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            TriggerAction(JKEventType.OnTriggerStay2D, collision);
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            TriggerAction(JKEventType.OnTriggerExit2D, collision);
        }
        #endregion

        #region 销毁事件
        private void OnDestroy()
        {
            TriggerAction(JKEventType.OnReleaseAddressableAsset, gameObject);
            TriggerAction(JKEventType.OnDestroy, gameObject);

            // 销毁所有数据，并将一些数据放回对象池中
            RemoveAllListener();
        }
        #endregion
    }
}