using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JKFrame
{
    public class ObjectPoolModule
    {
        #region ObjectPoolModule持有的数据及初始化方法
        /// <summary>
        /// 普通类 对象容器
        /// </summary>
        public Dictionary<string, ObjectPoolData> poolDic { get; private set; } = new Dictionary<string, ObjectPoolData>();

        /// <summary>
        /// 初始化对象池并设置容量
        /// </summary>
        /// <param name="maxCapacity">容量限制，超出时会销毁而不是进入对象池，-1代表无限</param>
        /// <param name="defaultQuantity">默认容量，填写会向池子中放入对应数量的对象，0代表不预先放入</param>
        public void InitObjectPool<T>(string keyName, int maxCapacity = -1, int defaultQuantity = 0) where T : new()
        {
            //设置的对象池已经存在
            if (poolDic.TryGetValue(keyName, out ObjectPoolData poolData))
            {
                //更新容量限制
                poolData.maxCapacity = maxCapacity;
                //底层Queue自动扩容这里不管

                //在指定默认容量时才有意义
                if (defaultQuantity != 0)
                {
                    int nowCapacity = poolData.PoolQueue.Count;
                    // 生成差值容量个数的物体放入对象池
                    for (int i = 0; i < defaultQuantity - nowCapacity; i++)
                    {
                        T obj = new T();
                        PushObject(obj, keyName);
                    }
                }

            }
            //设置的对象池不存在
            else
            {
                //创建对象池
                poolData = CreateObjectPoolData(keyName, maxCapacity);

                //在指定默认容量和默认对象时才有意义
                if (defaultQuantity != 0)
                {
                    // 生成容量个数的物体放入对象池
                    for (int i = 0; i < defaultQuantity; i++)
                    {
                        T obj = new T();
                        PushObject(obj, keyName);
                    }
                }
            }
        }

        /// <summary>
        /// 初始化对象池并设置容量
        /// </summary>
        /// <param name="maxCapacity">容量限制，超出时会销毁而不是进入对象池，-1代表无限</param>
        /// <param name="defaultQuantity">默认容量，填写会向池子中放入对应数量的对象，0代表不预先放入</param>
        public void InitObjectPool<T>(int maxCapacity = -1, int defaultQuantity = 0) where T : new()
        {
            InitObjectPool<T>(typeof(T).FullName, maxCapacity, defaultQuantity);
        }

        /// <summary>
        /// 初始化对象池
        /// </summary>
        /// <param name="keyName">资源名称</param>
        /// <param name="maxCapacity">容量限制，超出时会销毁而不是进入对象池，-1代表无限</param>
        public void InitObjectPool(string keyName, int maxCapacity = -1)
        {
            //设置的对象池已经存在
            if (poolDic.TryGetValue(keyName, out ObjectPoolData poolData))
            {
                //更新容量限制
                poolData.maxCapacity = maxCapacity;
                //底层Queue自动扩容这里不管
            }
            //设置的对象池不存在
            else
            {
                //创建对象池
                CreateObjectPoolData(keyName, maxCapacity);
            }
        }
        /// <summary>
        /// 初始化对象池
        /// </summary>
        /// <param name="type">资源类型</param>
        /// <param name="maxCapacity">容量限制，超出时会销毁而不是进入对象池，-1代表无限</param>
        public void InitObjectPool(System.Type type, int maxCapacity = -1)
        {
            InitObjectPool(type.FullName, maxCapacity);
        }

        /// <summary>
        /// 创建一条新的对象池数据
        /// </summary>
        private ObjectPoolData CreateObjectPoolData(string layerName, int capacity = -1)
        {
            // 交由Object对象池拿到poolData的类
            ObjectPoolData poolData = this.GetObject<ObjectPoolData>();

            //Object对象池中没有再new
            if (poolData == null)
            {
                poolData = new ObjectPoolData(capacity);
            }

            //对拿到的poolData副本进行初始化（覆盖之前的数据）
            poolData.maxCapacity = capacity;
            poolDic.Add(layerName, poolData);
            return poolData;
        }
        #endregion
        #region ObjectPool相关功能
        public object GetObject(string keyName)
        {
            object obj = null;
            if (poolDic.TryGetValue(keyName, out ObjectPoolData objectPoolData) && objectPoolData.PoolQueue.Count > 0)
            {
                obj = poolDic[keyName].GetObj();
            }
            return obj;
        }

        public object GetObject(System.Type type)
        {
            return GetObject(type.FullName);
        }

        public T GetObject<T>() where T : class
        {
            return (T)GetObject(typeof(T));
        }

        public T GetObject<T>(string keyName) where T : class
        {
            return (T)GetObject(keyName);
        }

        public bool PushObject(object obj)
        {
            return PushObject(obj, obj.GetType().FullName);
        }
        public bool PushObject(object obj, string keyName)
        {
            if (poolDic.TryGetValue(keyName, out ObjectPoolData poolData) == false)
            {
                poolData = CreateObjectPoolData(keyName);
            }
            return poolData.PushObj(obj);
        }

        public void ClearAll()
        {
            var enumerator = poolDic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.Value.Desotry(false);
            }
            poolDic.Clear();
        }
        public void ClearObject<T>()
        {
            ClearObject(typeof(T).FullName);
        }
        public void ClearObject(System.Type type)
        {
            ClearObject(type.FullName);
        }

        public void ClearObject(string keyName)
        {
            if (poolDic.TryGetValue(keyName, out ObjectPoolData objectPoolData))
            {
                objectPoolData.Desotry(true);
                poolDic.Remove(keyName);
            }
        }
    }
    #endregion

}