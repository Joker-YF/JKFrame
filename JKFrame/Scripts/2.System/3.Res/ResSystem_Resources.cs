#if ENABLE_ADDRESSABLES == false
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JKFrame
{
    public static class ResSystem
    {
#region 普通class对象
        /// <summary>
        /// 获取实例-普通Class
        /// 如果类型需要缓存，会从对象池中获取
        /// 如果对象池没有或返回null
        /// </summary>
        public static T Get<T>() where T : class
        {
            return PoolSystem.GetObject<T>();
        }

        /// <summary>
        /// 获取实例-普通Class
        /// 如果类型需要缓存，会从对象池中获取
        /// 如果对象池没有或返回null
        /// </summary>
        /// <param name="keyName">对象池中的名称</param>
        public static T Get<T>(string keyName) where T : class
        {
            return PoolSystem.GetObject<T>(keyName);
        }
        /// <summary>
        /// 获取实例-普通Class
        /// 如果类型需要缓存，会从对象池中获取
        /// 如果对象池没有或new一个返回
        /// <summary>
        public static T GetOrNew<T>() where T : class, new()
        {
            T obj = PoolSystem.GetObject<T>();
            if (obj == null) obj = new T();
            return obj;
        }


        /// <summary>
        /// 获取实例-普通Class
        /// 如果类型需要缓存，会从对象池中获取
        /// 如果对象池没有或new一个返回
        /// <summary>
        /// <param name="keyName">对象池中的名称</param>
        public static T GetOrNew<T>(string keyName) where T : class, new()
        {
            T obj = PoolSystem.GetObject<T>(keyName);
            if (obj == null) obj = new T();
            return obj;
        }

        /// <summary>
        /// 卸载普通对象，这里是使用对象池的方式
        /// </summary>
        public static void PushObjectInPool(System.Object obj)
        {
            PoolSystem.PushObject(obj);
        }

        /// <summary>
        /// 普通对象（非GameObject）放置对象池中
        /// 基于KeyName存放
        /// </summary>
        public static void PushObjectInPool(object obj, string keyName)
        {
            PoolSystem.PushObject(obj, keyName);
        }

        /// <summary>
        /// 初始化对象池并设置容量
        /// </summary>
        /// <param name="keyName">资源名称</param>
        /// <param name="maxCapacity">容量限制，超出时会销毁而不是进入对象池，-1代表无限</param>
        /// <param name="defaultQuantity">默认容量，填写会向池子中放入对应数量的对象，0代表不预先放入</param>
        public static void InitObjectPool<T>(string keyName, int maxCapacity = -1, int defaultQuantity = 0) where T : new()
        {
            PoolSystem.InitObjectPool<T>(keyName, maxCapacity, defaultQuantity);
        }
        /// <summary>
        /// 初始化对象池并设置容量
        /// </summary>
        /// <param name="maxCapacity">容量限制，超出时会销毁而不是进入对象池，-1代表无限</param>
        /// <param name="defaultQuantity">默认容量，填写会向池子中放入对应数量的对象，0代表不预先放入</param>
        public static void InitObjectPool<T>(int maxCapacity = -1, int defaultQuantity = 0) where T : new()
        {
            PoolSystem.InitObjectPool<T>(maxCapacity, defaultQuantity);
        }
        /// <summary>
        /// 初始化一个普通C#对象池类型
        /// </summary>
        /// <param name="keyName">keyName</param>
        /// <param name="maxCapacity">容量，超出时会丢弃而不是进入对象池，-1代表无限</param>
        public static void InitObjectPool(string keyName, int maxCapacity = -1)
        {
            PoolSystem.InitObjectPool(keyName, maxCapacity);
        }

        /// <summary>
        /// 初始化对象池
        /// </summary>
        /// <param name="type">资源类型</param>
        /// <param name="maxCapacity">容量限制，超出时会销毁而不是进入对象池，-1代表无限</param>
        public static void InitObjectPool(System.Type type, int maxCapacity = -1)
        {
            PoolSystem.InitObjectPool(type, maxCapacity);
        }

#endregion

#region 游戏对象

        /// <summary>
        /// 卸载游戏对象，这里是使用对象池的方式
        /// </summary>
        public static void PushGameObjectInPool(GameObject gameObject)
        {
            PoolSystem.PushGameObject(gameObject);
        }

        /// <summary>
        /// 游戏物体放置对象池中
        /// </summary>
        /// <param name="keyName">对象池中的key</param>
        /// <param name="obj">放入的物体</param>
        public static void PushGameObjectInPool(string keyName, GameObject gameObject)
        {
            PoolSystem.PushGameObject(keyName, gameObject);
        }


        /// <summary>
        /// 初始化一个GameObject类型的对象池类型
        /// </summary>
        /// <param name="keyName">对象池中的标识</param>
        /// <param name="maxCapacity">容量限制，超出时会销毁而不是进入对象池，-1代表无限</param>
        /// <param name="defaultQuantity">默认容量，填写会向池子中放入对应数量的对象，0代表不预先放入</param>
        /// <param name="prefab">填写默认容量时预先放入的对象</param>
        public static void InitGameObjectPool(string keyName, int maxCapacity = -1, string assetPath = null, int defaultQuantity = 0)
        {
            GameObject prefab = LoadAsset<GameObject>(assetPath);
            PoolSystem.InitGameObjectPool(keyName, maxCapacity, prefab, defaultQuantity);
            //UnloadAsset(prefab);
        }

        /// <summary>
        /// 初始化对象池并设置容量
        /// </summary>
        /// <param name="maxCapacity">容量限制，超出时会销毁而不是进入对象池，-1代表无限</param>
        /// <param name="defaultQuantity">默认容量，填写会向池子中放入对应数量的对象，0代表不预先放入</param>
        /// <param name="assetPath">资源路径</param>
        public static void InitGameObjectPool(string assetPath, int maxCapacity = -1, int defaultQuantity = 0)
        {
            GameObject prefab = LoadAsset<GameObject>(assetPath);
            PoolSystem.InitGameObjectPool(prefab, maxCapacity, defaultQuantity);
            //UnloadAsset(prefab);

        }


        /// <summary>
        /// <summary>
        /// 加载游戏物体
        /// 会自动考虑是否在对象池中存在
        /// <param name="assetPath"></param>
        /// <param name="parent"></param>
        /// <param name="keyName"></param>
        public static GameObject InstantiateGameObject(Transform parent, string keyName)
        {
            GameObject go;
            go = PoolSystem.GetGameObject(keyName, parent);
            if (!go.IsNull()) return go;

            GameObject prefab = LoadAsset<GameObject>(keyName);
            if (!prefab.IsNull())
            {
                go = GameObject.Instantiate(prefab, parent);
                go.name = keyName;
                //UnloadAsset(prefab);
            }
            return go;
        }


        /// <summary>
        /// 加载游戏物体
        /// 会自动考虑是否在对象池中存在
        /// </summary>
        /// <param name="assetPath">资源路径</param>
        /// <param name="parent">父物体</param>
        public static GameObject InstantiateGameObject(string assetPath, Transform parent = null, string keyName = null)
        {
            string assetName = GetAssetNameByPath(assetPath);
            GameObject go;
            if (keyName == null) go = PoolSystem.GetGameObject(assetName, parent);
            else go = PoolSystem.GetGameObject(keyName, parent);
            if (!go.IsNull()) return go;

            GameObject prefab = LoadAsset<GameObject>(assetPath);
            if (!prefab.IsNull())
            {
                go = GameObject.Instantiate(prefab, parent);
                go.name = keyName!=null?keyName:assetName;
                //UnloadAsset(prefab);
            }
            return go;
        }

        /// <summary>
        /// 加载游戏物体并获取组件
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="parent">父物体</param>
        public static T InstantiateGameObject<T>(Transform parent, string keyName) where T : UnityEngine.Component
        {
            GameObject go = InstantiateGameObject(parent, keyName);
            if (!go.IsNull())
            {
                return go.GetComponent<T>();
            }
            return null;
        }

        /// <summary>
        /// 加载游戏物体并获取组件
        /// </summary>
        /// <param name="path">资源路径</param>
        /// <param name="parent">父物体</param>
        public static T InstantiateGameObject<T>(string assetPath, Transform parent = null, string keyName = null) where T : UnityEngine.Component
        {
            GameObject go = InstantiateGameObject(assetPath, parent, keyName);
            if (!go.IsNull())
            {
                return go.GetComponent<T>();
            }
            return null;
        }

        /// <summary>
        /// 异步实例化游戏物体
        /// </summary>
        public static void InstantiateGameObjectAsync(string assetPath, Action<GameObject> callBack = null, Transform parent = null, string keyName = null)
        {
            // 切割路径获取实际的资源名称
            string assetName = GetAssetNameByPath(assetPath);
            GameObject go;
            if (keyName == null) go = PoolSystem.GetGameObject(assetName, parent);
            else go = PoolSystem.GetGameObject(keyName, parent);
            // 对象池有
            if (!go.IsNull())
            {
                callBack?.Invoke(go);
                return;
            }
            // 不通过缓存池
            MonoSystem.Start_Coroutine(DoInstantiateGameObjectAsync(assetPath, callBack, parent));
        }


        /// <summary>
        /// 异步实例化游戏物体并获取组件
        /// </summary>
        /// <typeparam name="T">游戏物体上的组件</typeparam>
        public static void InstantiateGameObjectAsync<T>(string assetPath, Action<T> callBack = null, Transform parent = null, string keyName = null) where T : UnityEngine.Component
        {
            string assetName = GetAssetNameByPath(assetPath);
            // 缓存字典里面有
            GameObject go;
            if (keyName == null) go = PoolSystem.GetGameObject(assetName, parent);
            else go = PoolSystem.GetGameObject(keyName, parent);
            // 对象有
            if (!go.IsNull())
            {
                callBack?.Invoke(go.GetComponent<T>());
                return;
            }
            // 不通过缓存池
            MonoSystem.Start_Coroutine(DoInstantiateGameObjectAsync<T>(assetPath, callBack, parent));
        }

        static IEnumerator DoInstantiateGameObjectAsync(string assetPath, Action<GameObject> callBack = null, Transform parent = null)
        {
            ResourceRequest resourceRequest = Resources.LoadAsync<GameObject>(assetPath);
            yield return resourceRequest;
            GameObject prefab = resourceRequest.asset as GameObject;
            GameObject go = GameObject.Instantiate<GameObject>(prefab);
            go.name = prefab.name;
            //UnloadAsset(prefab);
            callBack?.Invoke(go);
        }
        static IEnumerator DoInstantiateGameObjectAsync<T>(string assetPath, Action<T> callBack = null, Transform parent = null) where T : UnityEngine.Object
        {
            ResourceRequest resourceRequest = Resources.LoadAsync<GameObject>(assetPath);
            yield return resourceRequest;
            GameObject prefab = resourceRequest.asset as GameObject;
            GameObject go = GameObject.Instantiate<GameObject>(prefab);
            go.name = prefab.name;
            //UnloadAsset(prefab);
            callBack?.Invoke(go.GetComponent<T>());
        }
#endregion
#region 游戏Asset
        /// <summary>
        /// 加载Unity资源  如AudioClip Sprite 预制体
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        public static T LoadAsset<T>(string assetPath) where T : UnityEngine.Object
        {
            return Resources.Load<T>(assetPath);
        }

        /// <summary>
        /// 通过path获取资源名称
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string GetAssetNameByPath(string assetPath)
        {
            return assetPath.Substring(assetPath.LastIndexOf("/") + 1);
        }
        /// <summary>
        /// 异步加载Unity资源 AudioClip Sprite GameObject(预制体)
        /// </summary>
        /// <typeparam name="T">具体类型</typeparam>
        /// <param name="path">资源路径</param>
        /// <param name="callBack">加载完成后的回调</param>
        public static void LoadAssetAsync<T>(string assetPath, Action<T> callBack) where T : UnityEngine.Object
        {
            MonoSystem.Start_Coroutine(DoLoadAssetAsync<T>(assetPath, callBack));
        }

        static IEnumerator DoLoadAssetAsync<T>(string assetPath, Action<T> callBack) where T : UnityEngine.Object
        {
            ResourceRequest resourceRequest = Resources.LoadAsync<T>(assetPath);
            yield return resourceRequest;
            callBack?.Invoke(resourceRequest.asset as T);
        }

        /// <summary>
        /// 加载指定路径的所有资源
        /// </summary>
        public static UnityEngine.Object[] LoadAssets(string assetPath)
        {
            return Resources.LoadAll(assetPath);
        }

        /// <summary>
        /// 加载指定路径的所有资源
        /// </summary>
        public static T[] LoadAssets<T>(string assetPath) where T : UnityEngine.Object
        {
            return Resources.LoadAll<T>(assetPath);
        }

        /// <summary>
        /// 卸载资源
        /// </summary>
        public static void UnloadAsset(UnityEngine.Object assetToUnload)
        {
            Resources.UnloadAsset(assetToUnload);
        }


        /// <summary>
        /// 卸载全部未使用的资源
        /// </summary>
        public static void UnloadUnusedAssets()
        {
            Resources.UnloadUnusedAssets();
        }
#endregion


    }
}
#endif