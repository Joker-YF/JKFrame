#if ENABLE_ADDRESSABLES
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static JKFrame.GameObjectPoolModule;

namespace JKFrame
{
    public static class ResSystem
    {

        #region 普通class对象
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

        #region 游戏物体
        /// <summary>
        /// 初始化一个GameObject类型的对象池类型
        /// </summary>  
        /// <param name="keyName">资源名称</param>
        /// <param name="maxCapacity">容量限制，超出时会销毁而不是进入对象池，-1代表无限</param>
        /// <param name="defaultQuantity">默认容量，填写会向池子中放入对应数量的对象，0代表不预先放入</param>
        /// <param name="assetName">AB资源名称</param>
        public static void InitGameObjectPoolForKeyName(string keyName, int maxCapacity = -1, string assetName = null, int defaultQuantity = 0)
        {
            if (defaultQuantity <= 0 || assetName == null)
            {
                PoolSystem.InitGameObjectPool(keyName, maxCapacity, null, 0);
            }
            else
            {
                GameObject[] gameObjects = new GameObject[defaultQuantity];
                for (int i = 0; i < defaultQuantity; i++)
                {
                    gameObjects[i] = Addressables.InstantiateAsync(assetName).WaitForCompletion();
                }
                PoolSystem.InitGameObjectPool(keyName, maxCapacity, gameObjects);
            }
        }

        /// <summary>
        /// 初始化对象池并设置容量
        /// </summary>
        /// <param name="maxCapacity">容量限制，超出时会销毁而不是进入对象池，-1代表无限</param>
        /// <param name="defaultQuantity">默认容量，填写会向池子中放入对应数量的对象，0代表不预先放入</param>
        /// <param name="assetName">AB资源名称</param>
        public static void InitGameObjectPoolForAssetName(string assetName, int maxCapacity = -1, int defaultQuantity = 0)
        {
            InitGameObjectPoolForKeyName(assetName, maxCapacity, assetName, defaultQuantity);
        }

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
        /// 加载游戏物体
        /// 会自动检查对象池中是否包含，如果包含则返回对象池中的
        /// </summary>
        /// <param name="keyName">对象池中的分组名称</param>
        /// <param name="parent">父物体</param>
        /// <param name="autoRelease">物体销毁时，会自动去调用一次Addressables.Release</param>
        public static GameObject InstantiateGameObject(Transform parent, string keyName, bool autoRelease = true)
        {
            GameObject go;
            go = PoolSystem.GetGameObject(keyName, parent);
            if (go.IsNull() == false) return go;
            else
            {
                go = Addressables.InstantiateAsync(keyName, parent).WaitForCompletion();
                if (autoRelease)
                {
                    go.transform.OnReleaseAddressableAsset<int>(AutomaticReleaseAssetAction);
                }
                go.name = keyName;
            }
            return go;
        }

        /// <summary>
        /// 加载游戏物体
        /// 会自动检查对象池中是否包含，如果包含则返回对象池中的
        /// </summary>
        /// <param name="assetName">AB资源名称</param>
        /// <param name="keyName">对象池中的分组名称，可为Null</param>
        /// <param name="parent">父物体</param>
        /// <param name="autoRelease">物体销毁时，会自动去调用一次Addressables.Release</param>
        public static GameObject InstantiateGameObject(string assetName, Transform parent = null, string keyName = null, bool autoRelease = true)
        {
            GameObject go;
            if (keyName == null) go = PoolSystem.GetGameObject(assetName, parent);
            else go = PoolSystem.GetGameObject(keyName, parent);
            if (go.IsNull() == false) return go;
            else
            {
                go = Addressables.InstantiateAsync(assetName, parent).WaitForCompletion();
                if (autoRelease)
                {
                    go.transform.OnReleaseAddressableAsset<int>(AutomaticReleaseAssetAction);
                }
                go.name = keyName != null ? keyName : assetName;
            }
            return go;
        }

        /// <summary>
        /// 加载游戏物体并获取组件
        /// 会自动检查对象池中是否包含，如果包含则返回对象池中的
        /// </summary>
        /// <param name="keyName">对象池中的分组名称</param>
        /// <param name="parent">父物体</param>
        /// <param name="autoRelease">物体销毁时，会自动去调用一次Addressables.Release</param>
        public static T InstantiateGameObject<T>(Transform parent, string keyName, bool autoRelease = true) where T : Component
        {
            GameObject go = InstantiateGameObject(parent, keyName, autoRelease);
            if (go.IsNull() == false) return go.GetComponent<T>();
            else return null;
        }

        /// <summary>
        /// 加载游戏物体并获取组件
        /// 会自动检查对象池中是否包含，如果包含则返回对象池中的
        /// </summary>
        /// <param name="assetName">AB资源名称</param>
        /// <param name="keyName">对象池中的分组名称，可为Null</param>
        /// <param name="parent">父物体</param>
        /// <param name="autoRelease">物体销毁时，会自动去调用一次Addressables.Release</param>
        public static T InstantiateGameObject<T>(string assetName, Transform parent = null, string keyName = null, bool autoRelease = true) where T : Component
        {
            GameObject go = InstantiateGameObject(assetName, parent, keyName, autoRelease);
            if (go.IsNull() == false) return go.GetComponent<T>();
            else return null;
        }

        /// <summary>
        /// 自动释放资源事件，基于事件工具
        /// </summary>
        private static void AutomaticReleaseAssetAction(GameObject obj, int arg)
        {
            Addressables.ReleaseInstance(obj);
        }

        /// <summary>
        /// 异步加载游戏物体
        /// </summary>
        /// <param name="assetName">AB资源名称</param>
        /// <param name="callBack">实例化后的回调函数</param>
        /// <param name="parent">父物体</param>
        public static void InstantiateGameObjectAsync(string assetName, Action<GameObject> callBack = null, Transform parent = null, string keyName = null, bool autoRelease = true)
        {
            GameObject go;
            if (keyName == null) go = PoolSystem.GetGameObject(assetName, parent);
            else go = PoolSystem.GetGameObject(keyName, parent);
            // 对象池中有
            if (!go.IsNull())
            {
                if (autoRelease) go.transform.OnReleaseAddressableAsset<int>(AutomaticReleaseAssetAction);
                callBack?.Invoke(go);
                return;
            }
            // 不通过缓存池
            MonoSystem.Start_Coroutine(DoLoadGameObjectAsync(assetName, callBack, parent));

        }

        /// <summary>
        /// 异步加载游戏物体并获取组件
        /// </summary>
        /// <typeparam name="T">物体身上的组件</typeparam>
        /// <param name="assetName">AB资源名称</param>
        /// <param name="callBack">实例化后的回调函数</param>
        /// <param name="parent">父物体</param>
        public static void InstantiateGameObjectAsync<T>(string assetName, Action<T> callBack = null, Transform parent = null, string keyName = null, bool autoRelease = true) where T : UnityEngine.Object
        {
            GameObject go;
            if (keyName == null) go = PoolSystem.GetGameObject(assetName, parent);
            else go = PoolSystem.GetGameObject(keyName, parent);
            // 对象池中有
            if (!go.IsNull())
            {
                if (autoRelease) go.transform.OnReleaseAddressableAsset<int>(AutomaticReleaseAssetAction);
                callBack?.Invoke(go.GetComponent<T>());
                return;
            }
            // 不通过缓存池
            MonoSystem.Start_Coroutine(DoLoadGameObjectAsync<T>(assetName, callBack, parent));

        }

        static IEnumerator DoLoadGameObjectAsync(string assetName, Action<GameObject> callBack = null, Transform parent = null, bool autoRelease = true)
        {
            AsyncOperationHandle<GameObject> request = Addressables.InstantiateAsync(assetName, parent);
            yield return request;
            if (autoRelease) request.Result.transform.OnReleaseAddressableAsset<int>(AutomaticReleaseAssetAction);
            callBack?.Invoke(request.Result);
        }
        static IEnumerator DoLoadGameObjectAsync<T>(string assetName, Action<T> callBack = null, Transform parent = null, bool autoRelease = true) where T : UnityEngine.Object
        {
            AsyncOperationHandle<GameObject> request = Addressables.InstantiateAsync(assetName, parent);
            yield return request;
            if (autoRelease) request.Result.transform.OnReleaseAddressableAsset<int>(AutomaticReleaseAssetAction);
            callBack?.Invoke(request.Result.GetComponent<T>());
        }



        #endregion

        #region 游戏Asset
        /// <summary>
        /// 加载Unity资源  如AudioClip Sprite 预制体
        /// 要注意，资源不在使用时候，需要调用一次Release
        /// </summary>
        /// <param name="assetName">AB资源名称</param>
        public static T LoadAsset<T>(string assetName) where T : UnityEngine.Object
        {
            return Addressables.LoadAssetAsync<T>(assetName).WaitForCompletion();
        }

        /// <summary>
        /// 异步加载Unity资源 AudioClip Sprite GameObject(预制体)
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="assetName">AB资源名称</param>
        /// <param name="callBack">回调函数</param>
        public static void LoadAssetAsync<T>(string assetName, Action<T> callBack = null) where T : UnityEngine.Object
        {
            MonoSystem.Start_Coroutine(DoLoadAssetAsync<T>(assetName, callBack));
        }

        static IEnumerator DoLoadAssetAsync<T>(string assetName, Action<T> callBack = null) where T : UnityEngine.Object
        {
            AsyncOperationHandle<T> request = Addressables.LoadAssetAsync<T>(assetName);
            yield return request;
            callBack?.Invoke(request.Result);
        }

        /// <summary>
        /// 加载指定Key的所有资源
        /// </summary>
        /// <typeparam name="T">加载类型</typeparam>
        /// <param name="keyName">一般是lable</param>
        /// <param name="callBackOnEveryOne">注意这里是针对每一个资源的回调</param>
        /// <returns>所有资源</returns>
        public static IList<T> LoadAssets<T>(string keyName, Action<T> callBackOnEveryOne = null)
        {
            return Addressables.LoadAssetsAsync<T>(keyName, callBackOnEveryOne).WaitForCompletion();
        }

        /// <summary>
        /// 异步加载指定Key的所有资源
        /// </summary>
        /// <typeparam name="T">加载类型</typeparam>
        /// <param name="keyName">一般是lable</param>
        /// <param name="callBack">注意这里是针对每一个资源的回调</param>
        public static void LoadAssetsAsync<T>(string keyName, Action<IList<T>> callBack = null, Action<T> callBackOnEveryOne = null)
        {
            MonoSystem.Start_Coroutine(DoLoadAssetsAsync<T>(keyName, callBack, callBackOnEveryOne));
        }

        static IEnumerator DoLoadAssetsAsync<T>(string keyName, Action<IList<T>> callBack = null, Action<T> callBackOnEveryOne = null)
        {
            AsyncOperationHandle<IList<T>> request = Addressables.LoadAssetsAsync<T>(keyName, callBackOnEveryOne);
            yield return request;
            callBack?.Invoke(request.Result);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">具体对象</param>
        public static void UnloadAsset<T>(T obj)
        {
            Addressables.Release(obj);
        }

        /// <summary>
        /// 销毁游戏物体并释放资源
        /// </summary>
        public static bool UnloadInstance(GameObject obj)
        {
            return Addressables.ReleaseInstance(obj);
        }
        #endregion
    }
}
#endif

