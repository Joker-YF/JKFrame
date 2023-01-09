﻿using System;
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
        public static void AddEventListener<T>(this Component com, JKEventType eventType, Action<T, object[]> action, params object[] args)
        {
            AddEventListener(com, (int)eventType, action, args);
        }
        public static void AddEventListener<T>(this Component com, int customEventTypeInt, Action<T, object[]> action, params object[] args)
        {
            JKEventListener lis = GetOrAddJKEventListener(com);
            lis.AddListener(customEventTypeInt, action, args);
        }
        public static void RemoveEventListener<T>(this Component com, int customEventTypeInt, Action<T, object[]> action, bool checkArgs = false, params object[] args)
        {
            JKEventListener lis = GetOrAddJKEventListener(com);
            lis.RemoveListener(customEventTypeInt, action, checkArgs, args);
        }
        public static void RemoveEventListener<T>(this Component com, JKEventType eventType, Action<T, object[]> action, bool checkArgs = false, params object[] args)
        {
            RemoveEventListener(com,(int)eventType,action,checkArgs,args);
        }
        public static void RemoveAllListener(this Component com, int customEventTypeInt)
        {
            JKEventListener lis = GetOrAddJKEventListener(com);
            lis.RemoveAllListener(customEventTypeInt);
        }
        public static void RemoveAllListener(this Component com, JKEventType eventType)
        {
            RemoveAllListener(com, (int)eventType);
        }
        public static void RemoveAllListener(this Component com)
        {
            JKEventListener lis = GetOrAddJKEventListener(com);
            lis.RemoveAllListener();
        }
        public static void TriggerCustomEvent<T>(this Component com,int customEventTypeInt,T eventData)
        {
            JKEventListener lis = GetOrAddJKEventListener(com);
            lis.TriggerAction<T>(customEventTypeInt,eventData);
        }
        #endregion

        #region 鼠标相关事件
        public static void OnMouseEnter(this Component com, Action<PointerEventData, object[]> action, params object[] args)
        {
            AddEventListener(com, JKEventType.OnMouseEnter, action, args);
        }
        public static void OnMouseExit(this Component com, Action<PointerEventData, object[]> action, params object[] args)
        {
            AddEventListener(com, JKEventType.OnMouseExit, action, args);
        }
        public static void OnClick(this Component com, Action<PointerEventData, object[]> action, params object[] args)
        {
            AddEventListener(com, JKEventType.OnClick, action, args);
        }
        public static void OnClickDown(this Component com, Action<PointerEventData, object[]> action, params object[] args)
        {
            AddEventListener(com, JKEventType.OnClickDown, action, args);
        }
        public static void OnClickUp(this Component com, Action<PointerEventData, object[]> action, params object[] args)
        {
            AddEventListener(com, JKEventType.OnClickUp, action, args);
        }
        public static void OnDrag(this Component com, Action<PointerEventData, object[]> action, params object[] args)
        {
            AddEventListener(com, JKEventType.OnDrag, action, args);
        }
        public static void OnBeginDrag(this Component com, Action<PointerEventData, object[]> action, params object[] args)
        {
            AddEventListener(com, JKEventType.OnBeginDrag, action, args);
        }
        public static void OnEndDrag(this Component com, Action<PointerEventData, object[]> action, params object[] args)
        {
            AddEventListener(com, JKEventType.OnEndDrag, action, args);
        }

        public static void RemoveOnClick(this Component com, Action<PointerEventData, object[]> action, bool checkArgs = false, params object[] args)
        {
            RemoveEventListener(com, JKEventType.OnClick, action, checkArgs, args);
        }
        public static void RemoveOnClickDown(this Component com, Action<PointerEventData, object[]> action, bool checkArgs = false, params object[] args)
        {
            RemoveEventListener(com, JKEventType.OnClickDown, action, checkArgs, args);
        }

        public static void RemoveOnMouseEnter(this Component com, Action<PointerEventData, object[]> action, bool checkArgs = false, params object[] args)
        {
            RemoveEventListener(com, JKEventType.OnMouseEnter, action, checkArgs, args);
        }
        public static void RemoveOnMouseExit(this Component com, Action<PointerEventData, object[]> action, bool checkArgs = false, params object[] args)
        {
            RemoveEventListener(com, JKEventType.OnMouseExit, action, checkArgs, args);
        }
        public static void RemoveOnClickUp(this Component com, Action<PointerEventData, object[]> action, bool checkArgs = false, params object[] args)
        {
            RemoveEventListener(com, JKEventType.OnClickUp, action, checkArgs, args);
        }
        public static void RemoveOnDrag(this Component com, Action<PointerEventData, object[]> action, bool checkArgs = false, params object[] args)
        {
            RemoveEventListener(com, JKEventType.OnDrag, action, checkArgs, args);
        }
        public static void RemoveOnBeginDrag(this Component com, Action<PointerEventData, object[]> action, bool checkArgs = false, params object[] args)
        {
            RemoveEventListener(com, JKEventType.OnBeginDrag, action, checkArgs, args);
        }
        public static void RemoveOnEndDrag(this Component com, Action<PointerEventData, object[]> action, bool checkArgs = false, params object[] args)
        {
            RemoveEventListener(com, JKEventType.OnEndDrag, action, checkArgs, args);
        }


        #endregion

        #region 碰撞相关事件

        public static void OnCollisionEnter(this Component com, Action<Collision, object[]> action, params object[] args)
        {
            com.AddEventListener(JKEventType.OnCollisionEnter, action, args);
        }


        public static void OnCollisionStay(this Component com, Action<Collision, object[]> action, params object[] args)
        {
            AddEventListener(com, JKEventType.OnCollisionStay, action, args);
        }
        public static void OnCollisionExit(this Component com, Action<Collision, object[]> action, params object[] args)
        {
            AddEventListener(com, JKEventType.OnCollisionExit, action, args);
        }
        public static void OnCollisionEnter2D(this Component com, Action<Collision, object[]> action, params object[] args)
        {
            AddEventListener(com, JKEventType.OnCollisionEnter2D, action, args);
        }
        public static void OnCollisionStay2D(this Component com, Action<Collision, object[]> action, params object[] args)
        {
            AddEventListener(com, JKEventType.OnCollisionStay2D, action, args);
        }
        public static void OnCollisionExit2D(this Component com, Action<Collision, object[]> action, params object[] args)
        {
            AddEventListener(com, JKEventType.OnCollisionExit2D, action, args);
        }
        public static void RemoveOnCollisionEnter(this Component com, Action<Collision, object[]> action, bool checkArgs = false, params object[] args)
        {
            RemoveEventListener(com, JKEventType.OnCollisionEnter, action, checkArgs, args);
        }
        public static void RemoveOnCollisionStay(this Component com, Action<Collision, object[]> action, bool checkArgs = false, params object[] args)
        {
            RemoveEventListener(com, JKEventType.OnCollisionStay, action, checkArgs, args);
        }
        public static void RemoveOnCollisionExit(this Component com, Action<Collision, object[]> action, bool checkArgs = false, params object[] args)
        {
            RemoveEventListener(com, JKEventType.OnCollisionExit, action, checkArgs, args);
        }
        public static void RemoveOnCollisionEnter2D(this Component com, Action<Collision2D, object[]> action, bool checkArgs = false, params object[] args)
        {
            RemoveEventListener(com, JKEventType.OnCollisionEnter2D, action, checkArgs, args);
        }
        public static void RemoveOnCollisionStay2D(this Component com, Action<Collision2D, object[]> action, bool checkArgs = false, params object[] args)
        {
            RemoveEventListener(com, JKEventType.OnCollisionStay2D, action, checkArgs, args);
        }
        public static void RemoveOnCollisionExit2D(this Component com, Action<Collision2D, object[]> action, bool checkArgs = false, params object[] args)
        {
            RemoveEventListener(com, JKEventType.OnCollisionExit2D, action, checkArgs, args);
        }
        #endregion

        #region 触发相关事件
        public static void OnTriggerEnter(this Component com, Action<Collider, object[]> action, bool checkArgs = false, params object[] args)
        {
            AddEventListener(com, JKEventType.OnTriggerEnter, action, checkArgs, args);
        }
        public static void OnTriggerStay(this Component com, Action<Collider, object[]> action, bool checkArgs = false, params object[] args)
        {
            AddEventListener(com, JKEventType.OnTriggerStay, action, checkArgs, args);
        }
        public static void OnTriggerExit(this Component com, Action<Collider, object[]> action, bool checkArgs = false, params object[] args)
        {
            AddEventListener(com, JKEventType.OnTriggerExit, action, checkArgs, args);
        }
        public static void OnTriggerEnter2D(this Component com, Action<Collider, object[]> action, bool checkArgs = false, params object[] args)
        {
            AddEventListener(com, JKEventType.OnTriggerEnter2D, action, checkArgs, args);
        }
        public static void OnTriggerStay2D(this Component com, Action<Collider, object[]> action, bool checkArgs = false, params object[] args)
        {
            AddEventListener(com, JKEventType.OnTriggerStay2D, action, checkArgs, args);
        }
        public static void OnTriggerExit2D(this Component com, Action<Collider, object[]> action, bool checkArgs = false, params object[] args)
        {
            AddEventListener(com, JKEventType.OnTriggerExit2D, action, checkArgs, args);
        }
        public static void RemoveOnTriggerEnter(this Component com, Action<Collider, object[]> action, bool checkArgs = false, params object[] args)
        {
            RemoveEventListener(com, JKEventType.OnTriggerEnter, action, checkArgs, args);
        }
        public static void RemoveOnTriggerStay(this Component com, Action<Collider, object[]> action, bool checkArgs = false, params object[] args)
        {
            RemoveEventListener(com, JKEventType.OnTriggerStay, action, checkArgs, args);
        }
        public static void RemoveOnTriggerExit(this Component com, Action<Collider, object[]> action, bool checkArgs = false, params object[] args)
        {
            RemoveEventListener(com, JKEventType.OnTriggerExit, action, checkArgs, args);
        }
        public static void RemoveOnTriggerEnter2D(this Component com, Action<Collider2D, object[]> action, bool checkArgs = false, params object[] args)
        {
            RemoveEventListener(com, JKEventType.OnTriggerEnter2D, action, checkArgs, args);
        }
        public static void RemoveOnTriggerStay2D(this Component com, Action<Collider2D, object[]> action, bool checkArgs = false, params object[] args)
        {
            RemoveEventListener(com, JKEventType.OnTriggerStay2D, action, checkArgs, args);
        }
        public static void RemoveOnTriggerExit2D(this Component com, Action<Collider2D, object[]> action, bool checkArgs = false, params object[] args)
        {
            RemoveEventListener(com, JKEventType.OnTriggerExit2D, action, checkArgs, args);
        }
        #endregion

        #region 资源相关事件
        public static void OnReleaseAddressableAsset(this Component com, Action<GameObject, object[]> action, params object[] args)
        {
            AddEventListener(com, JKEventType.OnReleaseAddressableAsset, action, args);
        }
        public static void RemoveOnReleaseAddressableAsset(this Component com, Action<GameObject, object[]> action, bool checkArgs = false, params object[] args)
        {
            RemoveEventListener(com, JKEventType.OnReleaseAddressableAsset, action, checkArgs, args);
        }

        public static void OnDesotry(this Component com, Action<GameObject, object[]> action, params object[] args)
        {
            AddEventListener(com, JKEventType.OnDestory, action, args);
        }
        public static void RemoveOnDesotry(this Component com, Action<GameObject, object[]> action, bool checkArgs = false, params object[] args)
        {
            RemoveEventListener(com, JKEventType.OnDestory, action, checkArgs, args);
        }
        #endregion
    }
}