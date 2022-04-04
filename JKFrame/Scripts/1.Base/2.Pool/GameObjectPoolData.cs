using System.Collections.Generic;
using UnityEngine;

namespace JKFrame
{
    /// <summary>
    /// GameObject对象池数据
    /// </summary>
    public class GameObjectPoolData
    {
        // 对象池中 父节点
        public GameObject fatherObj;
        // 对象容器
        public Queue<GameObject> poolQueue;

        public GameObjectPoolData(GameObject obj, GameObject poolRootObj)
        {
            // 创建父节点 并设置到对象池根节点下方
            fatherObj = new GameObject(obj.name);
            fatherObj.transform.SetParent(poolRootObj.transform);
            poolQueue = new Queue<GameObject>();
            // 把首次创建时候 需要放入的对象 放进容器
            PushObj(obj);
        }


        /// <summary>
        /// 将对象放进对象池
        /// </summary>
        public void PushObj(GameObject obj)
        {
            // 对象进容器
            poolQueue.Enqueue(obj);
            // 设置父物体
            obj.transform.SetParent(fatherObj.transform);
            // 设置隐藏
            obj.SetActive(false);
        }

        /// <summary>
        /// 从对象池中获取对象
        /// </summary>
        /// <returns></returns>
        public GameObject GetObj(Transform parent = null)
        {
            GameObject obj = poolQueue.Dequeue();

            // 显示对象
            obj.SetActive(true);
            // 设置父物体
            obj.transform.SetParent(parent);
            if (parent == null)
            {
                // 回归默认场景
                UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(obj, UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            }

            return obj;
        }
    }

}