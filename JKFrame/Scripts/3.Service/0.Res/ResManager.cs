using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace JKFrame
{
    public static class ResManager
    {
        // 需要缓存的类型 
        // Key：TypeNameOrAssetName
        // Value：是否缓存
        private static Dictionary<string, bool> wantCacheDic;

        static ResManager()
        {
            wantCacheDic = GameRoot.Instance.GameSetting.cacheDic;
        }

        /// <summary>
        /// 检查一个类型是否需要缓存
        /// </summary>
        private static bool CheckCacheDic(Type type)
        {
            return CheckCacheDic(type.Name);
        }

        /// <summary>
        /// 检查一个类型是否需要缓存
        /// </summary>
        private static bool CheckCacheDic(string typeOrAssetName)
        {
            return wantCacheDic.ContainsKey(typeOrAssetName);
        }

        /// <summary>
        /// 加载Unity资源  如AudioClip Sprite 预制体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T LoadAsset<T>(string assetName) where T : UnityEngine.Object
        {
           return Addressables.LoadAssetAsync<T>(assetName).WaitForCompletion();
        }

        /// <summary>
        /// 获取实例-普通Class
        /// 如果类型需要缓存，会从对象池中获取
        /// </summary>
        public static T Load<T>() where T : class, new()
        {
            // 需要缓存
            if (CheckCacheDic(typeof(T)))
            {
                return PoolManager.Instance.GetObject<T>();
            }
            else
            {
                return new T();
            }
        }

        /// <summary>
        /// 加载游戏物体
        /// </summary>
        /// <param name="assetName">资源名称</param>
        /// <param name="parent">父物体</param>
        /// <param name="automaticRelease">物体销毁时，会自动去调用一次Addressables.Release</param>
        /// <returns></returns>
        public static GameObject LoadGameObject(string assetName, Transform parent = null,bool automaticRelease = true)
        {
            GameObject go = null;
            if (CheckCacheDic(assetName))
            {
                go = PoolManager.Instance.GetGameObject(assetName, parent);
                if (go.IsNull() == false) return go;
            }
            go = Addressables.InstantiateAsync(assetName, parent).WaitForCompletion();
            if (automaticRelease)
            {
                go.transform.OnReleaseAddressableAsset(AutomaticReleaseAssetAction);
            }
            go.name = assetName;
            return go;
        }

        /// <summary>
        /// 自动释放资源事件，基于事件工具
        /// </summary>
        private static void AutomaticReleaseAssetAction(GameObject obj, object[] arg2)
        {
            Addressables.ReleaseInstance(obj);
        }


        /// <summary>
        /// 获取实例--组件模式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static T Load<T>(string assetName, Transform parent = null) where T : Component
        {
            return LoadGameObject(assetName, parent).GetComponent<T>(); ;
        }

        /// <summary>
        /// 异步加载游戏物体
        /// </summary>
        /// <typeparam name="T">具体的组件</typeparam>
        public static void LoadGameObjectAsync<T>(string assetName, Action<T> callBack = null, Transform parent = null) where T : UnityEngine.Object
        {
            // 缓存字典里面有
            if (CheckCacheDic(typeof(T)))
            {
                GameObject go = PoolManager.Instance.GetGameObject(assetName, parent);
                // 对象有
                if (!go.IsNull())
                {
                    callBack?.Invoke(go.GetComponent<T>());
                    return;
                }
            }
            // 不通过缓存池
            MonoManager.Instance.StartCoroutine(DoLoadGameObjectAsync<T>(assetName, callBack, parent));

        }
        static IEnumerator DoLoadGameObjectAsync<T>(string assetName, Action<T> callBack = null, Transform parent = null) where T : UnityEngine.Object
        {
            AsyncOperationHandle<GameObject> request = Addressables.InstantiateAsync(assetName, parent);
            yield return request;
            callBack?.Invoke(request.Result.GetComponent<T>());
        }

        /// <summary>
        /// 异步加载Unity资源 AudioClip Sprite GameObject(预制体)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="callBack"></param>
        public static void LoadAssetAsync<T>(string assetName, Action<T> callBack) where T : UnityEngine.Object
        {
            MonoManager.Instance.StartCoroutine(DoLoadAssetAsync<T>(assetName, callBack));
        }

        static IEnumerator DoLoadAssetAsync<T>(string assetName, Action<T> callBack) where T : UnityEngine.Object
        {
            AsyncOperationHandle<T> request = Addressables.LoadAssetAsync<T>(assetName);
            yield return request;
            callBack?.Invoke(request.Result);
        }

        /// <summary>
        /// 加载指定Key的所有资源
        /// </summary>
        public static IList<T> LoadAssets<T>(string keyName,Action<T> callBack =null)
        {
             return Addressables.LoadAssetsAsync<T>(keyName,callBack).WaitForCompletion();
        }

        /// <summary>
        /// 异步加载指定Key的所有资源
        /// </summary>
        public static void LoadAssetsAsync<T>(string keyName, Action<IList<T>> callBack = null, Action<T> callBackOnEveryOne = null)
        {
            MonoManager.Instance.StartCoroutine(DoLoadAssetsAsync<T>(keyName,callBack, callBackOnEveryOne));
        }

        static IEnumerator DoLoadAssetsAsync<T>(string keyName, Action<IList<T>> callBack = null,Action<T> callBackOnEveryOne = null)
        {
            AsyncOperationHandle<IList<T>> request = Addressables.LoadAssetsAsync<T>(keyName, callBackOnEveryOne);
            yield return request;
            callBack?.Invoke(request.Result);
        }

        public static void Release<T>(T obj)
        {
            Addressables.Release<T>(obj);
        }
        /// <summary>
        /// 释放实例
        /// </summary>
        public static bool ReleaseInstance(GameObject obj)
        {
           return  Addressables.ReleaseInstance(obj);
        }
    }
}