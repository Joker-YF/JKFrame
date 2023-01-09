using System.Collections.Generic;

namespace JKFrame
{
    /// <summary>
    /// 普通类 对象 对象池数据
    /// </summary>
    public class ObjectPoolData
    {
        #region ObjectPoolData持有的数据及初始化方法
        // 对象容器
        public Queue<object> PoolQueue;
        // 容量限制 -1代表无限
        public int maxCapacity = -1;
        public ObjectPoolData(int capacity = -1)
        {
            maxCapacity = capacity;
            if (maxCapacity == -1) PoolQueue = new Queue<object>();
            else PoolQueue = new Queue<object>(capacity);
        }
        #endregion

        #region ObjectPool数据相关操作
        /// <summary>
        /// 将对象放进对象池
        /// </summary>
        public bool PushObj(object obj)
        {
            // 检测是不是超过容量
            if (maxCapacity != -1 && PoolQueue.Count >= maxCapacity)
            {
                return false;
            }
            PoolQueue.Enqueue(obj);
            return true;
        }

        /// <summary>
        /// 从对象池中获取对象
        /// </summary>
        /// <returns></returns>
        public object GetObj()
        {
            return PoolQueue.Dequeue();
        }

        public void Desotry(bool pushThisToPool = false)
        {
            PoolQueue.Clear();
            maxCapacity = -1;
            if (pushThisToPool)
            {
                PoolSystem.PushObject(this);
            }
        }
        #endregion

    }
}