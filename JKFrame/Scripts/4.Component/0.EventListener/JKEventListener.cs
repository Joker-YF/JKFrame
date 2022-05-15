using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace JKFrame
{
    /// <summary>
    /// 事件类型
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
    }

    public interface IMouseEvent : IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
    { }

    /// <summary>
    /// 事件工具
    /// 可以添加 鼠标、碰撞、触发等事件
    /// </summary>
    public class JKEventListener : MonoBehaviour, IMouseEvent
    {
        #region 内部类、接口等
        /// <summary>
        /// 某个事件中一个时间的数据包装类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class JKEventListenerEventInfo<T>
        {
            // T：事件本身的参数（PointerEventData、Collision）
            // object[]:事件的参数
            public Action<T, object[]> action;
            public object[] args;
            public void Init(Action<T, object[]> action, object[] args)
            {
                this.action = action;
                this.args = args;
            }
            public void Destory()
            {
                this.action = null;
                this.args = null;
                this.JKObjectPushPool();
            }
            public void TriggerEvent(T eventData)
            {
                action?.Invoke(eventData, args);
            }
        }

        interface IJKEventListenerEventInfos
        {
            void RemoveAll();

        }

        /// <summary>
        /// 一类事件的数据包装类型：包含多个JKEventListenerEventInfo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class JKEventListenerEventInfos<T> : IJKEventListenerEventInfos
        {

            // 所有的事件
            private List<JKEventListenerEventInfo<T>> eventList = new List<JKEventListenerEventInfo<T>>();

            /// <summary>
            /// 添加事件
            /// </summary>
            public void AddListener(Action<T, object[]> action, params object[] args)
            {
                JKEventListenerEventInfo<T> info = PoolManager.Instance.GetObject<JKEventListenerEventInfo<T>>();
                info.Init(action, args);
                eventList.Add(info);
            }

            /// <summary>
            /// 移除事件
            /// </summary>
            public void RemoveListener(Action<T, object[]> action, bool checkArgs = false, params object[] args)
            {
                for (int i = 0; i < eventList.Count; i++)
                {
                    // 找到这个事件
                    if (eventList[i].action.Equals(action))
                    {
                        // 是否需要检查参数
                        if (checkArgs && args.Length > 0)
                        {
                            // 参数如果相等
                            if (args.ArraryEquals(eventList[i].args))
                            {
                                // 移除
                                eventList[i].Destory();
                                eventList.RemoveAt(i);
                                return;
                            }
                        }
                        else
                        {
                            // 移除
                            eventList[i].Destory();
                            eventList.RemoveAt(i);
                            return;
                        }
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
                this.JKObjectPushPool();
            }

            public void TriggerEvent(T evetData)
            {
                for (int i = 0; i < eventList.Count; i++)
                {
                    eventList[i].TriggerEvent(evetData);
                }
            }

        }

        #endregion

        private Dictionary<int, IJKEventListenerEventInfos> eventInfoDic = new Dictionary<int, JKEventListener.IJKEventListenerEventInfos>();

        #region 外部的访问

        /// <summary>
        /// 添加事件
        /// </summary>
        public void AddListener<T>(int eventTypeInt, Action<T, object[]> action, params object[] args)
        {
            if (eventInfoDic.TryGetValue(eventTypeInt, out IJKEventListenerEventInfos info))
            {
                (info as JKEventListenerEventInfos<T>).AddListener(action, args);
            }
            else
            {
                JKEventListenerEventInfos<T> infos = PoolManager.Instance.GetObject<JKEventListenerEventInfos<T>>();
                infos.AddListener(action, args);
                eventInfoDic.Add(eventTypeInt, infos);
            }
        }
        /// <summary>
        /// 添加事件
        /// </summary>
        public void AddListener<T>(JKEventType eventType, Action<T, object[]> action, params object[] args)
        {
            AddListener<T>((int)eventType, action, args);
        }

        /// <summary>
        /// 移除事件
        /// </summary>
        public void RemoveListener<T>(int eventTypeInt, Action<T, object[]> action, bool checkArgs = false, params object[] args)
        {
            if (eventInfoDic.TryGetValue(eventTypeInt,out IJKEventListenerEventInfos info))
            {
                (info as JKEventListenerEventInfos<T>).RemoveListener(action, checkArgs, args);
            }
        }
        /// <summary>
        /// 移除事件
        /// </summary>
        public void RemoveListener<T>(JKEventType eventType, Action<T, object[]> action, bool checkArgs = false, params object[] args)
        {
            RemoveListener((int)eventType, action, checkArgs, args);
        }

        /// <summary>
        /// 移除某一个事件类型下的全部事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventType"></param>
        public void RemoveAllListener(JKEventType eventType)
        {
            if (eventInfoDic.TryGetValue((int)eventType, out IJKEventListenerEventInfos infos))
            {
                infos.RemoveAll();
                eventInfoDic.Remove((int)eventType);
            }
        }
        /// <summary>
        /// 移除全部事件
        /// </summary>
        public void RemoveAllListener()
        {
            foreach (IJKEventListenerEventInfos infos in eventInfoDic.Values)
            {
                infos.RemoveAll();
            }

            eventInfoDic.Clear();
        }

        #endregion

        /// <summary>
        /// 触发事件
        /// </summary>
        public void TriggerAction<T>(int eventTypeInt, T eventData)
        {
            if (eventInfoDic.TryGetValue(eventTypeInt, out IJKEventListenerEventInfos infos))
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
    }
}