using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using static JKFrame.GameObjectPoolModule;
using Unity.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace JKFrame
{
    /// <summary>
    /// 对象池系统
    /// </summary>
    public static class PoolSystem
    {
        #region 对象池系统数据及静态构造方法
        /// <summary>
        /// 对象池层物体的游戏物体名称，用于将层物体也放进对象池
        /// </summary>
        public const string PoolLayerGameObjectName = "PoolLayerGameObjectName";
        private static GameObjectPoolModule GameObjectPoolModule;
        private static ObjectPoolModule ObjectPoolModule;
        private static Transform poolRootTransform;
        public static void Init()
        {
            GameObjectPoolModule = new GameObjectPoolModule();
            ObjectPoolModule = new ObjectPoolModule();
            poolRootTransform = new GameObject("PoolRoot").transform;
            poolRootTransform.position = Vector3.zero;
            poolRootTransform.SetParent(JKFrameRoot.RootTransform);
            GameObjectPoolModule.Init(poolRootTransform);
        }

        #endregion

        #region GameObject对象池相关API(初始化、取出、放入、清空)
        /// <summary>
        /// 初始化一个GameObject类型的对象池类型
        /// </summary>
        /// <param name="keyName">资源名称</param>
        /// <param name="maxCapacity">容量限制，超出时会销毁而不是进入对象池，-1代表无限</param>
        /// <param name="defaultQuantity">默认容量，填写会向池子中放入对应数量的对象，0代表不预先放入</param>
        /// <param name="prefab">填写默认容量时预先放入的对象</param>
        public static void InitGameObjectPool(string keyName, int maxCapacity = -1, GameObject prefab = null, int defaultQuantity = 0)
        {
            GameObjectPoolModule.InitObjectPool(keyName, maxCapacity, prefab, defaultQuantity);
#if UNITY_EDITOR
            if (JKFrameRoot.EditorEventModule != null)
                JKFrameRoot.EditorEventModule.EventTrigger<string, int>("OnInitGameObjectPool", keyName, defaultQuantity);
#endif
        }
        /// <summary>
        /// 初始化对象池
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="maxCapacity">最大容量，-1代表无限</param>
        /// <param name="gameObjects">默认要放进来的对象数组</param>
        public static void InitGameObjectPool(string keyName, int maxCapacity, GameObject[] gameObjects = null)
        {
            GameObjectPoolModule.InitObjectPool(keyName, maxCapacity, gameObjects);
#if UNITY_EDITOR
            if (JKFrameRoot.EditorEventModule != null)
                JKFrameRoot.EditorEventModule.EventTrigger<string, int>("OnInitGameObjectPool", keyName, gameObjects.Length);
#endif
        }


        /// <summary>
        /// 初始化对象池并设置容量
        /// </summary>
        /// <param name="maxCapacity">容量限制，超出时会销毁而不是进入对象池，-1代表无限</param>
        /// <param name="defaultQuantity">默认容量，填写会向池子中放入对应数量的对象，0代表不预先放入</param>
        /// <param name="prefab">填写默认容量时预先放入的对象</param>
        public static void InitGameObjectPool(GameObject prefab, int maxCapacity = -1, int defaultQuantity = 0)
        {
            InitGameObjectPool(prefab.name, maxCapacity, prefab, defaultQuantity);
        }


        /// <summary>
        /// 获取GameObject，没有则返回Null
        /// </summary>
        public static GameObject GetGameObject(string keyName, Transform parent = null)
        {
            GameObject go = GameObjectPoolModule.GetObject(keyName, parent);
#if UNITY_EDITOR
            if (go != null && JKFrameRoot.EditorEventModule != null) JKFrameRoot.EditorEventModule.EventTrigger<string, int>("OnGetGameObject", keyName, 1);
#endif
            return go;
        }

        /// <summary>
        /// 获取GameObject，没有则返回Null
        /// T:组件
        /// </summary>
        public static T GetGameObject<T>(string keyName, Transform parent = null) where T : Component
        {
            GameObject go = GetGameObject(keyName, parent);
            if (go != null) return go.GetComponent<T>();
            else return null;
        }

        /// <summary>
        /// 游戏物体放置对象池中
        /// </summary>
        /// <param name="keyName">对象池中的key</param>
        /// <param name="obj">放入的物体</param>
        public static bool PushGameObject(string keyName, GameObject obj)
        {
            if (!obj.IsNull())
            {
                bool res = GameObjectPoolModule.PushObject(keyName, obj);
#if UNITY_EDITOR
                if (JKFrameRoot.EditorEventModule != null && res)
                    JKFrameRoot.EditorEventModule.EventTrigger<string, int>("OnPushGameObject", keyName, 1);
#endif
                return res;
            }
            else
            {
                JKLog.Error("您正在将Null放置对象池");
                return false;

            }

        }

        /// <summary>
        /// 游戏物体放置对象池中
        /// </summary>
        /// <param name="obj">放入的物体,并且基于它的name来确定它是什么物体</param>
        public static bool PushGameObject(GameObject obj)
        {
            return PushGameObject(obj.name, obj);
        }

        /// <summary>
        /// 清除某个游戏物体在对象池中的所有数据
        /// </summary>
        public static void ClearGameObject(string keyName)
        {
            GameObjectPoolModule.Clear(keyName);
#if UNITY_EDITOR
            if (JKFrameRoot.EditorEventModule != null)
                JKFrameRoot.EditorEventModule.EventTrigger<string>("OnClearGameObject", keyName);
#endif
        }
        #endregion

        #region Object对象池相关API(初始化、取出、放入、清空)

        /// <summary>
        /// 初始化对象池并设置容量
        /// </summary>
        /// <param name="keyName">资源名称</param>
        /// <param name="maxCapacity">容量限制，超出时会销毁而不是进入对象池，-1代表无限</param>
        /// <param name="defaultQuantity">默认容量，填写会向池子中放入对应数量的对象，0代表不预先放入</param>
        public static void InitObjectPool<T>(string keyName, int maxCapacity = -1, int defaultQuantity = 0) where T : new()
        {
            ObjectPoolModule.InitObjectPool<T>(keyName, maxCapacity, defaultQuantity);
#if UNITY_EDITOR
            if (JKFrameRoot.EditorEventModule != null)
                JKFrameRoot.EditorEventModule.EventTrigger<string, int>("OnInitObjectPool", keyName, defaultQuantity);
#endif
        }
        /// <summary>
        /// 初始化对象池并设置容量
        /// </summary>
        /// <param name="maxCapacity">容量限制，超出时会销毁而不是进入对象池，-1代表无限</param>
        /// <param name="defaultQuantity">默认容量，填写会向池子中放入对应数量的对象，0代表不预先放入</param>
        public static void InitObjectPool<T>(int maxCapacity = -1, int defaultQuantity = 0) where T : new()
        {
            InitObjectPool<T>(typeof(T).FullName, maxCapacity, defaultQuantity);
        }
        /// <summary>
        /// 初始化一个普通C#对象池类型
        /// </summary>
        /// <param name="keyName">keyName</param>
        /// <param name="maxCapacity">容量，超出时会丢弃而不是进入对象池，-1代表无限</param>
        public static void InitObjectPool(string keyName, int maxCapacity = -1)
        {
            ObjectPoolModule.InitObjectPool(keyName, maxCapacity);
#if UNITY_EDITOR
            if (JKFrameRoot.EditorEventModule != null)
                JKFrameRoot.EditorEventModule.EventTrigger<string, int>("OnInitObjectPool", keyName, 0);
#endif
        }

        /// <summary>
        /// 初始化对象池
        /// </summary>
        /// <param name="type">资源类型</param>
        /// <param name="maxCapacity">容量限制，超出时会销毁而不是进入对象池，-1代表无限</param>
        public static void InitObjectPool(System.Type type, int maxCapacity = -1)
        {
            ObjectPoolModule.InitObjectPool(type, maxCapacity);
#if UNITY_EDITOR
            if (JKFrameRoot.EditorEventModule != null)
                JKFrameRoot.EditorEventModule.EventTrigger<string, int>("OnInitObjectPool", type.FullName, 0);
#endif
        }

        /// <summary>
        /// 获取普通对象（非GameObject）
        /// </summary>
        public static T GetObject<T>() where T : class
        {
            return GetObject<T>(typeof(T).FullName);
        }

        /// <summary>
        /// 获取普通对象（非GameObject）
        /// </summary>
        public static T GetObject<T>(string keyName) where T : class
        {
            object obj = GetObject(keyName);
            if (obj == null) return null;
            else return (T)obj;
        }

        /// <summary>
        /// 获取普通对象（非GameObject）
        /// </summary>
        public static object GetObject(System.Type type)
        {
            return GetObject(type.FullName);
        }
        /// <summary>
        /// 获取普通对象（非GameObject）
        /// </summary>
        public static object GetObject(string keyName)
        {
            object obj = ObjectPoolModule.GetObject(keyName);
#if UNITY_EDITOR
            if (obj != null)
            {
                if (JKFrameRoot.EditorEventModule != null)
                    JKFrameRoot.EditorEventModule.EventTrigger<string, int>("OnGetObject", keyName, 1);
            }
#endif
            return obj;
        }
        /// <summary>
        /// 普通对象（非GameObject）放置对象池中
        /// 基于类型存放
        /// </summary>
        public static bool PushObject(object obj)
        {
            return PushObject(obj, obj.GetType().FullName);
        }

        /// <summary>
        /// 普通对象（非GameObject）放置对象池中
        /// 基于KeyName存放
        /// </summary>
        public static bool PushObject(object obj, string keyName)
        {
            if (obj == null)
            {
                JKLog.Error("您正在将Null放置对象池");
                return false;
            }
            else
            {
                bool res = ObjectPoolModule.PushObject(obj, keyName);
#if UNITY_EDITOR
                if (JKFrameRoot.EditorEventModule != null && res)
                {
                    JKFrameRoot.EditorEventModule.EventTrigger<string, int>("OnPushObject", keyName, 1);
                }
#endif
                return res;
            }
        }

        /// <summary>
        /// 清理某个C#类型数据
        /// </summary>
        public static void ClearObject<T>()
        {
            ClearObject(typeof(T).FullName);
        }

        /// <summary>
        /// 清理某个C#类型数据
        /// </summary>
        public static void ClearObject(System.Type type)
        {
            ClearObject(type.FullName);
        }

        /// <summary>
        /// 清理某个C#类型数据
        /// </summary>
        public static void ClearObject(string keyName)
        {
#if UNITY_EDITOR
            if (JKFrameRoot.EditorEventModule != null)
            {
                JKFrameRoot.EditorEventModule.EventTrigger<string>("OnClearnObject", keyName);
            }
#endif
            ObjectPoolModule.ClearObject(keyName);
        }

        #endregion

        #region 对GameObject和Object对象池同时启用的API（清空全部）
        /// <summary>
        /// 清除全部
        /// </summary>
        public static void ClearAll(bool clearGameObject = true, bool clearCSharpObject = true)
        {
            if (clearGameObject)
            {
                GameObjectPoolModule.ClearAll();
#if UNITY_EDITOR
                JKFrameRoot.EditorEventModule.EventTrigger("OnClearAllGameObject");
#endif
            }
            if (clearCSharpObject)
            {
                ObjectPoolModule.ClearAll();
#if UNITY_EDITOR
                if (JKFrameRoot.EditorEventModule != null)
                {
                    JKFrameRoot.EditorEventModule.EventTrigger("OnClearAllObject");
                }
#endif
            }
        }
        #endregion


        #region Editor
#if UNITY_EDITOR
        public static Dictionary<string, GameObjectPoolData> GetGameObjectLayerDatas()
        {
            return GameObjectPoolModule.poolDic;
        }
        public static Dictionary<string, ObjectPoolData> GetObjectLayerDatas()
        {
            return ObjectPoolModule.poolDic;
        }
#endif
        #endregion


    }
}