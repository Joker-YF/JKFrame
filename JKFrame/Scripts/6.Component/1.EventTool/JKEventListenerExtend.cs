using System;
using UnityEngine;
using UnityEngine.EventSystems;
namespace JKFrame
{
    public static class JKEventListenerExtend
    {
        #region 工具函数
        private static JKEventListener GetOrAddJKEventListener(Component com)
        {
            JKEventListener lis = com.GetComponent<JKEventListener>();
            if (lis == null) return com.gameObject.AddComponent<JKEventListener>();
            else return lis;
        }
        public static void AddEventListener<T>(this Component com, JKEventType eventType, Action<T> action)
        {
            AddEventListener(com, (int)eventType, action);
        }
        public static void AddEventListener<T>(this Component com, int customEventTypeInt, Action<T> action)
        {
            JKEventListener lis = GetOrAddJKEventListener(com);
            lis.AddListener(customEventTypeInt, action);
        }

        public static void AddEventListener<T, TEventArg>(this Component com, JKEventType eventType, Action<T, TEventArg> action, TEventArg args = default(TEventArg))
        {
            AddEventListener(com, (int)eventType, action, args);
        }
        public static void AddEventListener<T, TEventArg>(this Component com, int customEventTypeInt, Action<T, TEventArg> action, TEventArg args = default(TEventArg))
        {
            JKEventListener lis = GetOrAddJKEventListener(com);
            lis.AddListener(customEventTypeInt, action, args);
        }

        public static void RemoveEventListener<T, TEventArg>(this Component com, int customEventTypeInt, Action<T, TEventArg> action)
        {
            JKEventListener lis = com.GetComponent<JKEventListener>();
            if (lis != null) lis.RemoveListener(customEventTypeInt, action);
        }
        public static void RemoveEventListener<T, TEventArg>(this Component com, JKEventType eventType, Action<T, TEventArg> action)
        {
            RemoveEventListener(com, (int)eventType, action);
        }

        public static void RemoveEventListener<T>(this Component com, int customEventTypeInt, Action<T> action)
        {
            JKEventListener lis = com.GetComponent<JKEventListener>();
            if (lis != null) lis.RemoveListener(customEventTypeInt, action);
        }
        public static void RemoveEventListener<T>(this Component com, JKEventType eventType, Action<T> action)
        {
            RemoveEventListener(com, (int)eventType, action);
        }

        public static void RemoveAllListener(this Component com, int customEventTypeInt)
        {
            JKEventListener lis = com.GetComponent<JKEventListener>();
            if (lis != null) lis.RemoveAllListener(customEventTypeInt);
        }
        public static void RemoveAllListener(this Component com, JKEventType eventType)
        {
            RemoveAllListener(com, (int)eventType);
        }
        public static void RemoveAllListener(this Component com)
        {
            JKEventListener lis = com.GetComponent<JKEventListener>();
            if (lis != null) lis.RemoveAllListener();
        }
        public static void TriggerCustomEvent<T>(this Component com, int customEventTypeInt, T eventData)
        {
            JKEventListener lis = GetOrAddJKEventListener(com);
            lis.TriggerAction<T>(customEventTypeInt, eventData);
        }
        #endregion

        #region 鼠标相关事件
        public static void OnMouseEnter<TEventArg>(this Component com, Action<PointerEventData, TEventArg> action, TEventArg args = default(TEventArg))
        {
            AddEventListener(com, JKEventType.OnMouseEnter, action, args);
        }
        public static void OnMouseEnter(this Component com, Action<PointerEventData> action)
        {
            AddEventListener(com, JKEventType.OnMouseEnter, action);
        }

        public static void OnMouseExit<TEventArg>(this Component com, Action<PointerEventData, TEventArg> action, TEventArg args = default(TEventArg))
        {
            AddEventListener(com, JKEventType.OnMouseExit, action, args);
        }
        public static void OnMouseExit(this Component com, Action<PointerEventData> action)
        {
            AddEventListener(com, JKEventType.OnMouseExit, action);
        }

        public static void OnClick<TEventArg>(this Component com, Action<PointerEventData, TEventArg> action, TEventArg args = default(TEventArg))
        {
            AddEventListener(com, JKEventType.OnClick, action, args);
        }
        public static void OnClick(this Component com, Action<PointerEventData> action)
        {
            AddEventListener(com, JKEventType.OnClick, action);
        }


        public static void OnClickDown<TEventArg>(this Component com, Action<PointerEventData, TEventArg> action, TEventArg args = default(TEventArg))
        {
            AddEventListener(com, JKEventType.OnClickDown, action, args);
        }
        public static void OnClickDown(this Component com, Action<PointerEventData> action)
        {
            AddEventListener(com, JKEventType.OnClickDown, action);
        }

        public static void OnClickUp<TEventArg>(this Component com, Action<PointerEventData, TEventArg> action, TEventArg args = default(TEventArg))
        {
            AddEventListener(com, JKEventType.OnClickUp, action, args);
        }
        public static void OnClickUp(this Component com, Action<PointerEventData> action)
        {
            AddEventListener(com, JKEventType.OnClickUp, action);
        }


        public static void OnDrag<TEventArg>(this Component com, Action<PointerEventData, TEventArg> action, TEventArg args = default(TEventArg))
        {
            AddEventListener(com, JKEventType.OnDrag, action, args);
        }
        public static void OnDrag(this Component com, Action<PointerEventData> action)
        {
            AddEventListener(com, JKEventType.OnDrag, action);
        }

        public static void OnBeginDrag<TEventArg>(this Component com, Action<PointerEventData, TEventArg> action, TEventArg args = default(TEventArg))
        {
            AddEventListener(com, JKEventType.OnBeginDrag, action, args);
        }
        public static void OnBeginDrag(this Component com, Action<PointerEventData> action)
        {
            AddEventListener(com, JKEventType.OnBeginDrag, action);
        }

        public static void OnEndDrag<TEventArg>(this Component com, Action<PointerEventData, TEventArg> action, TEventArg args = default(TEventArg))
        {
            AddEventListener(com, JKEventType.OnEndDrag, action, args);
        }
        public static void OnEndDrag(this Component com, Action<PointerEventData> action)
        {
            AddEventListener(com, JKEventType.OnEndDrag, action);
        }

        public static void RemoveOnClick<TEventArg>(this Component com, Action<PointerEventData, TEventArg> action)
        {
            RemoveEventListener(com, JKEventType.OnClick, action);
        }
        public static void RemoveOnClick(this Component com, Action<PointerEventData> action)
        {
            RemoveEventListener(com, JKEventType.OnClick, action);
        }

        public static void RemoveOnClickDown<TEventArg>(this Component com, Action<PointerEventData, TEventArg> action)
        {
            RemoveEventListener(com, JKEventType.OnClickDown, action);
        }
        public static void RemoveOnClickDown(this Component com, Action<PointerEventData> action)
        {
            RemoveEventListener(com, JKEventType.OnClickDown, action);
        }

        public static void RemoveOnMouseEnter<TEventArg>(this Component com, Action<PointerEventData, TEventArg> action)
        {
            RemoveEventListener(com, JKEventType.OnMouseEnter, action);
        }
        public static void RemoveOnMouseEnter(this Component com, Action<PointerEventData> action)
        {
            RemoveEventListener(com, JKEventType.OnMouseEnter, action);
        }

        public static void RemoveOnMouseExit<TEventArg>(this Component com, Action<PointerEventData, TEventArg> action)
        {
            RemoveEventListener(com, JKEventType.OnMouseExit, action);
        }
        public static void RemoveOnMouseExit(this Component com, Action<PointerEventData> action)
        {
            RemoveEventListener(com, JKEventType.OnMouseExit, action);
        }

        public static void RemoveOnClickUp<TEventArg>(this Component com, Action<PointerEventData, TEventArg> action)
        {
            RemoveEventListener(com, JKEventType.OnClickUp, action);
        }
        public static void RemoveOnClickUp(this Component com, Action<PointerEventData> action)
        {
            RemoveEventListener(com, JKEventType.OnClickUp, action);
        }

        public static void RemoveOnDrag<TEventArg>(this Component com, Action<PointerEventData, TEventArg> action)
        {
            RemoveEventListener(com, JKEventType.OnDrag, action);
        }
        public static void RemoveOnDrag(this Component com, Action<PointerEventData> action)
        {
            RemoveEventListener(com, JKEventType.OnDrag, action);
        }

        public static void RemoveOnBeginDrag<TEventArg>(this Component com, Action<PointerEventData, TEventArg> action)
        {
            RemoveEventListener(com, JKEventType.OnBeginDrag, action);
        }
        public static void RemoveOnBeginDrag(this Component com, Action<PointerEventData> action)
        {
            RemoveEventListener(com, JKEventType.OnBeginDrag, action);
        }

        public static void RemoveOnEndDrag<TEventArg>(this Component com, Action<PointerEventData, TEventArg> action)
        {
            RemoveEventListener(com, JKEventType.OnEndDrag, action);
        }
        public static void RemoveOnEndDrag(this Component com, Action<PointerEventData> action)
        {
            RemoveEventListener(com, JKEventType.OnEndDrag, action);
        }

        #endregion

        #region 碰撞相关事件

        public static void OnCollisionEnter<TEventArg>(this Component com, Action<Collision, TEventArg> action, TEventArg args = default(TEventArg))
        {
            AddEventListener(com, JKEventType.OnCollisionEnter, action, args);
        }
        public static void OnCollisionEnter(this Component com, Action<Collision> action)
        {
            AddEventListener(com, JKEventType.OnCollisionEnter, action);
        }


        public static void OnCollisionStay<TEventArg>(this Component com, Action<Collision, TEventArg> action, TEventArg args = default(TEventArg))
        {
            AddEventListener(com, JKEventType.OnCollisionStay, action, args);
        }
        public static void OnCollisionStay(this Component com, Action<Collision> action)
        {
            AddEventListener(com, JKEventType.OnCollisionStay, action);
        }

        public static void OnCollisionExit<TEventArg>(this Component com, Action<Collision, TEventArg> action, TEventArg args = default(TEventArg))
        {
            AddEventListener(com, JKEventType.OnCollisionExit, action, args);
        }
        public static void OnCollisionExit(this Component com, Action<Collision> action)
        {
            AddEventListener(com, JKEventType.OnCollisionExit, action);
        }

        public static void OnCollisionEnter2D<TEventArg>(this Component com, Action<Collision2D, TEventArg> action, TEventArg args = default(TEventArg))
        {
            AddEventListener(com, JKEventType.OnCollisionEnter2D, action, args);
        }
        public static void OnCollisionEnter2D(this Component com, Action<Collision2D> action)
        {
            AddEventListener(com, JKEventType.OnCollisionEnter2D, action);
        }

        public static void OnCollisionStay2D<TEventArg>(this Component com, Action<Collision2D, TEventArg> action, TEventArg args = default(TEventArg))
        {
            AddEventListener(com, JKEventType.OnCollisionStay2D, action, args);
        }
        public static void OnCollisionStay2D(this Component com, Action<Collision2D> action)
        {
            AddEventListener(com, JKEventType.OnCollisionStay2D, action);
        }

        public static void OnCollisionExit2D<TEventArg>(this Component com, Action<Collision2D, TEventArg> action, TEventArg args = default(TEventArg))
        {
            AddEventListener(com, JKEventType.OnCollisionExit2D, action, args);
        }
        public static void OnCollisionExit2D(this Component com, Action<Collision2D> action)
        {
            AddEventListener(com, JKEventType.OnCollisionExit2D, action);
        }

        public static void RemoveOnCollisionEnter<TEventArg>(this Component com, Action<Collision, TEventArg> action)
        {
            RemoveEventListener(com, JKEventType.OnCollisionEnter, action);
        }
        public static void RemoveOnCollisionEnter(this Component com, Action<Collision> action)
        {
            RemoveEventListener(com, JKEventType.OnCollisionEnter, action);
        }

        public static void RemoveOnCollisionStay<TEventArg>(this Component com, Action<Collision, TEventArg> action)
        {
            RemoveEventListener(com, JKEventType.OnCollisionStay, action);
        }
        public static void RemoveOnCollisionStay(this Component com, Action<Collision> action)
        {
            RemoveEventListener(com, JKEventType.OnCollisionStay, action);
        }

        public static void RemoveOnCollisionExit<TEventArg>(this Component com, Action<Collision, TEventArg> action)
        {
            RemoveEventListener(com, JKEventType.OnCollisionExit, action);
        }
        public static void RemoveOnCollisionExit(this Component com, Action<Collision> action)
        {
            RemoveEventListener(com, JKEventType.OnCollisionExit, action);
        }

        public static void RemoveOnCollisionEnter2D<TEventArg>(this Component com, Action<Collision2D, TEventArg> action)
        {
            RemoveEventListener(com, JKEventType.OnCollisionEnter2D, action);
        }
        public static void RemoveOnCollisionEnter2D(this Component com, Action<Collision2D> action)
        {
            RemoveEventListener(com, JKEventType.OnCollisionEnter2D, action);
        }

        public static void RemoveOnCollisionStay2D<TEventArg>(this Component com, Action<Collision2D, TEventArg> action)
        {
            RemoveEventListener(com, JKEventType.OnCollisionStay2D, action);
        }
        public static void RemoveOnCollisionStay2D(this Component com, Action<Collision2D> action)
        {
            RemoveEventListener(com, JKEventType.OnCollisionStay2D, action);
        }

        public static void RemoveOnCollisionExit2D<TEventArg>(this Component com, Action<Collision2D, TEventArg> action)
        {
            RemoveEventListener(com, JKEventType.OnCollisionExit2D, action);
        }

        public static void RemoveOnCollisionExit2D(this Component com, Action<Collision2D> action)
        {
            RemoveEventListener(com, JKEventType.OnCollisionExit2D, action);
        }
        #endregion

        #region 触发相关事件
        public static void OnTriggerEnter<TEventArg>(this Component com, Action<Collider, TEventArg> action, TEventArg args = default(TEventArg))
        {
            AddEventListener(com, JKEventType.OnTriggerEnter, action, args);
        }
        public static void OnTriggerEnter(this Component com, Action<Collider> action)
        {
            AddEventListener(com, JKEventType.OnTriggerEnter, action);
        }

        public static void OnTriggerStay<TEventArg>(this Component com, Action<Collider, TEventArg> action, TEventArg args = default(TEventArg))
        {
            AddEventListener(com, JKEventType.OnTriggerStay, action, args);
        }
        public static void OnTriggerStay(this Component com, Action<Collider> action)
        {
            AddEventListener(com, JKEventType.OnTriggerStay, action);
        }

        public static void OnTriggerExit<TEventArg>(this Component com, Action<Collider, TEventArg> action, TEventArg args = default(TEventArg))
        {
            AddEventListener(com, JKEventType.OnTriggerExit, action, args);
        }
        public static void OnTriggerExit(this Component com, Action<Collider> action)
        {
            AddEventListener(com, JKEventType.OnTriggerExit, action);
        }

        public static void OnTriggerEnter2D<TEventArg>(this Component com, Action<Collider2D, TEventArg> action, TEventArg args = default(TEventArg))
        {
            AddEventListener(com, JKEventType.OnTriggerEnter2D, action, args);
        }
        public static void OnTriggerEnter2D(this Component com, Action<Collider2D> action)
        {
            AddEventListener(com, JKEventType.OnTriggerEnter2D, action);
        }

        public static void OnTriggerStay2D<TEventArg>(this Component com, Action<Collider2D, TEventArg> action, TEventArg args = default(TEventArg))
        {
            AddEventListener(com, JKEventType.OnTriggerStay2D, action, args);
        }
        public static void OnTriggerStay2D(this Component com, Action<Collider2D> action)
        {
            AddEventListener(com, JKEventType.OnTriggerStay2D, action);
        }

        public static void OnTriggerExit2D<TEventArg>(this Component com, Action<Collider2D, TEventArg> action, TEventArg args = default(TEventArg))
        {
            AddEventListener(com, JKEventType.OnTriggerExit2D, action, args);
        }
        public static void OnTriggerExit2D(this Component com, Action<Collider2D> action)
        {
            AddEventListener(com, JKEventType.OnTriggerExit2D, action);
        }

        public static void RemoveOnTriggerEnter<TEventArg>(this Component com, Action<Collider, TEventArg> action)
        {
            RemoveEventListener(com, JKEventType.OnTriggerEnter, action);
        }
        public static void RemoveOnTriggerEnter(this Component com, Action<Collider> action)
        {
            RemoveEventListener(com, JKEventType.OnTriggerEnter, action);
        }

        public static void RemoveOnTriggerStay<TEventArg>(this Component com, Action<Collider, TEventArg> action)
        {
            RemoveEventListener(com, JKEventType.OnTriggerStay, action);
        }
        public static void RemoveOnTriggerStay(this Component com, Action<Collider> action)
        {
            RemoveEventListener(com, JKEventType.OnTriggerStay, action);
        }

        public static void RemoveOnTriggerExit<TEventArg>(this Component com, Action<Collider, TEventArg> action)
        {
            RemoveEventListener(com, JKEventType.OnTriggerExit, action);
        }
        public static void RemoveOnTriggerExit(this Component com, Action<Collider> action)
        {
            RemoveEventListener(com, JKEventType.OnTriggerExit, action);
        }

        public static void RemoveOnTriggerEnter2D<TEventArg>(this Component com, Action<Collider2D, TEventArg> action)
        {
            RemoveEventListener(com, JKEventType.OnTriggerEnter2D, action);
        }
        public static void RemoveOnTriggerEnter2D(this Component com, Action<Collider2D> action)
        {
            RemoveEventListener(com, JKEventType.OnTriggerEnter2D, action);
        }

        public static void RemoveOnTriggerStay2D<TEventArg>(this Component com, Action<Collider2D, TEventArg> action)
        {
            RemoveEventListener(com, JKEventType.OnTriggerStay2D, action);
        }
        public static void RemoveOnTriggerStay2D(this Component com, Action<Collider2D> action)
        {
            RemoveEventListener(com, JKEventType.OnTriggerStay2D, action);
        }

        public static void RemoveOnTriggerExit2D<TEventArg>(this Component com, Action<Collider2D, TEventArg> action)
        {
            RemoveEventListener(com, JKEventType.OnTriggerExit2D, action);
        }
        public static void RemoveOnTriggerExit2D(this Component com, Action<Collider2D> action)
        {
            RemoveEventListener(com, JKEventType.OnTriggerExit2D, action);
        }
        #endregion

        #region 资源相关事件
        public static void OnReleaseAddressableAsset<TEventArg>(this Component com, Action<GameObject, TEventArg> action, TEventArg args = default(TEventArg))
        {
            AddEventListener(com, JKEventType.OnReleaseAddressableAsset, action, args);
        }
        public static void OnReleaseAddressableAsset(this Component com, Action<GameObject> action)
        {
            AddEventListener(com, JKEventType.OnReleaseAddressableAsset, action);
        }

        public static void RemoveOnReleaseAddressableAsset<TEventArg>(this Component com, Action<GameObject, TEventArg> action)
        {
            RemoveEventListener(com, JKEventType.OnReleaseAddressableAsset, action);
        }
        public static void RemoveOnReleaseAddressableAsset(this Component com, Action<GameObject> action)
        {
            RemoveEventListener(com, JKEventType.OnReleaseAddressableAsset, action);
        }

        public static void OnDestroy<TEventArg>(this Component com, Action<GameObject, TEventArg> action, TEventArg args = default(TEventArg))
        {
            AddEventListener(com, JKEventType.OnDestroy, action, args);
        }
        public static void OnDestroy(this Component com, Action<GameObject> action)
        {
            AddEventListener(com, JKEventType.OnDestroy, action);
        }

        public static void RemoveOnDestroy<TEventArg>(this Component com, Action<GameObject, TEventArg> action)
        {
            RemoveEventListener(com, JKEventType.OnDestroy, action);
        }
        public static void RemoveOnDestroy(this Component com, Action<GameObject> action)
        {
            RemoveEventListener(com, JKEventType.OnDestroy, action);
        }
        #endregion
    }
}
