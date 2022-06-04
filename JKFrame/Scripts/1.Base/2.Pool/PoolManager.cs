using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
namespace JKFrame
{
    public class PoolManager : ManagerBase<PoolManager>
    {
        // 根节点
        [SerializeField]
        private GameObject poolRootObj;
        /// <summary>
        /// GameObject对象容器
        /// </summary>
        public Dictionary<string, GameObjectPoolData> gameObjectPoolDic = new Dictionary<string, GameObjectPoolData>();
        /// <summary>
        /// 普通类 对象容器
        /// </summary>
        public Dictionary<string, ObjectPoolData> objectPoolDic = new Dictionary<string, ObjectPoolData>();

        public override void Init()
        {
            base.Init();
        }

        #region GameObject对象相关操作

        /// <summary>
        /// 获取GameObject,但是如果没有则返回Null
        /// </summary>
        public GameObject GetGameObject(string assetName, Transform parent = null)
        {
            GameObject obj = null;
            // 检查有没有这一层
            if (gameObjectPoolDic.TryGetValue(assetName, out GameObjectPoolData poolData) && poolData.poolQueue.Count > 0)
            {
                obj = poolData.GetObj(parent);
            }
            return obj;
        }

        /// <summary>
        /// GameObject放进对象池
        /// </summary>
        public void PushGameObject(GameObject obj)
        {
            string name = obj.name;
            // 现在有没有这一层
            if (gameObjectPoolDic.TryGetValue(name, out GameObjectPoolData poolData))
            {
                poolData.PushObj(obj);
            }
            else
            {
                gameObjectPoolDic.Add(name, new GameObjectPoolData(obj, poolRootObj));
            }
        }

        #endregion

        #region 普通对象相关操作
        /// <summary>
        /// 获取普通对象
        /// </summary>
        public T GetObject<T>() where T : class, new()
        {
            T obj;
            if (CheckObjectCache<T>())
            {
                string name = typeof(T).FullName;
                obj = (T)objectPoolDic[name].GetObj();
                return obj;
            }
            else
            {
                return new T();
            }
        }

        /// <summary>
        /// GameObject放进对象池
        /// </summary>
        /// <param name="obj"></param>
        public void PushObject(object obj)
        {
            string name = obj.GetType().FullName;
            // 现在有没有这一层
            if (objectPoolDic.ContainsKey(name))
            {
                objectPoolDic[name].PushObj(obj);
            }
            else
            {
                objectPoolDic.Add(name, new ObjectPoolData(obj));
            }
        }

        private bool CheckObjectCache<T>()
        {
            string name = typeof(T).FullName;
            return objectPoolDic.ContainsKey(name) && objectPoolDic[name].poolQueue.Count > 0;
        }
        #endregion


        #region 删除
        /// <summary>
        /// 删除全部
        /// </summary>
        /// <param name="clearGameObject">是否删除游戏物体</param>
        /// <param name="clearCObject">是否删除普通C#对象</param>
        public void Clear(bool clearGameObject = true, bool clearCObject = true)
        {
            if (clearGameObject)
            {
                for (int i = 0; i < poolRootObj.transform.childCount; i++)
                {
                    Destroy(poolRootObj.transform.GetChild(i).gameObject);
                }
                gameObjectPoolDic.Clear();
            }

            if (clearCObject)
            {
                objectPoolDic.Clear();
            }
        }

        public void ClearAllGameObject()
        {
            Clear(true, false);
        }
        public void ClearGameObject(string prefabName)
        {
            GameObject go = poolRootObj.transform.Find(prefabName).gameObject;
            if (go.IsNull() == false)
            {
                Destroy(go);
                gameObjectPoolDic.Remove(prefabName);

            }

        }
        public void ClearGameObject(GameObject prefab)
        {
            ClearGameObject(prefab.name);
        }

        public void ClearAllObject()
        {
            Clear(false, true);
        }
        public void ClearObject<T>()
        {
            objectPoolDic.Remove(typeof(T).FullName);
        }
        public void ClearObject(Type type)
        {
            objectPoolDic.Remove(type.FullName);
        }
        #endregion

    }
}