using System;
using System.Collections;
using UnityEngine;
namespace JKFrame
{
    /// <summary>
    /// 场景管理器
    /// </summary>
    public static class SceneManager
    {
        /// <summary>
        /// 同步加载场景
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="callBack">回调函数</param>
        public static void LoadScene(string sceneName, Action callBack = null)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
            callBack?.Invoke();
        }

        /// <summary>
        /// 异步加载场景
        /// 会自动分发进度到事件中心，事件名称“LoadingSceneProgress”
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="callBack">回调函数</param>
        public static void LoadSceneAsync(string sceneName, Action callBack = null)
        {
            MonoManager.Instance.StartCoroutine(DoLoadSceneAsync(sceneName, callBack));
        }

        private static IEnumerator DoLoadSceneAsync(string sceneName, Action callBack = null)
        {
            AsyncOperation ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            while (ao.isDone == false)
            {
                // 把加载进度分发到事件中心
                EventManager.EventTrigger("LoadingSceneProgress", ao.progress);
                yield return ao.progress;
            }
            EventManager.EventTrigger<float>("LoadingSceneProgress", 1F);
            EventManager.EventTrigger("LoadSceneSucceed");
            callBack?.Invoke();
        }

    }
}