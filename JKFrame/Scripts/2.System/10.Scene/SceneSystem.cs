using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace JKFrame
{
    public static class SceneSystem
    {
        public static void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            SceneManager.LoadScene(sceneName, mode);
        }

        public static void LoadScene(int sceneBuildIndex, LoadSceneMode mode = LoadSceneMode.Single)
        {
            SceneManager.LoadScene(sceneBuildIndex, mode);
        }

        public static Scene LoadScene(string sceneName, LoadSceneParameters loadSceneParameters)
        {
            return SceneManager.LoadScene(sceneName, loadSceneParameters);
        }

        public static Scene LoadScene(int sceneBuildIndex, LoadSceneParameters loadSceneParameters)
        {
            return SceneManager.LoadScene(sceneBuildIndex, loadSceneParameters);
        }

        /// <summary>
        /// 异步加载场景
        /// 您可以选择EventSystem监听"LoadingSceneProgress"、"LoadSceneSucceed"等事件监听场景进度
        /// 也可以通过callBack参数
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="callBack">回调函数,注意：每次进度更新都会调用一次,参数为0-1的进度</param>
        public static void LoadSceneAsync(string sceneName, Action<float> callBack = null, LoadSceneMode mode = LoadSceneMode.Single)
        {
            MonoSystem.Start_Coroutine(DoLoadSceneAsync(sceneName, callBack, mode));
        }

        private static IEnumerator DoLoadSceneAsync(string sceneName, Action<float> callBack = null, LoadSceneMode mode = LoadSceneMode.Single)
        {
            AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName, mode);
            float progress = 0;
            while (progress < 1)
            {
                // 回调加载进度
                if (progress != ao.progress)
                {
                    progress = ao.progress;
                    callBack?.Invoke(progress);
                    // 把加载进度分发到事件中心
                    EventSystem.EventTrigger("LoadingSceneProgress", ao.progress);
                    if (progress == 1)
                    {
                        EventSystem.EventTrigger("LoadSceneSucceed");
                        break;
                    }
                }
                yield return CoroutineTool.WaitForFrames();
            }
        }

        /// <summary>
        /// 异步加载场景
        /// 您可以选择EventSystem监听"LoadingSceneProgress"、"LoadSceneSucceed"等事件监听场景进度
        /// 也可以通过callBack参数
        /// </summary>
        /// <param name="sceneBuildIndex">场景Index</param>
        /// <param name="callBack">回调函数</param>
        public static void LoadSceneAsync(int sceneBuildIndex, Action<float> callBack = null, LoadSceneMode mode = LoadSceneMode.Single)
        {
            MonoSystem.Start_Coroutine(DoLoadSceneAsync(sceneBuildIndex, callBack, mode));
        }

        private static IEnumerator DoLoadSceneAsync(int sceneBuildIndex, Action<float> callBack = null, LoadSceneMode mode = LoadSceneMode.Single)
        {
            AsyncOperation ao = SceneManager.LoadSceneAsync(sceneBuildIndex, mode);
            float progress = 0;
            while (progress < 1)
            {
                progress = ao.progress;
                callBack?.Invoke(progress);
                // 把加载进度分发到事件中心
                EventSystem.EventTrigger("LoadingSceneProgress", ao.progress);
                if (progress == 1)
                {
                    EventSystem.EventTrigger("LoadSceneSucceed");
                    break;
                }
                yield return CoroutineTool.WaitForFrames();
            }
        }
        
        /// <summary>
        /// 异步加载场景，加载完成后不立刻切换
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="successCallBack">加载完成回调，必须传入否则场景无法切换</param>
        /// <param name="loadingCallBack">加载时每帧回调</param>
        /// <param name="mode">加载模式</param>
        public static void LoadSceneAsyncWithoutActive(string sceneName, Action<AsyncOperation> successCallBack,
            Action<float> loadingCallBack = null, LoadSceneMode mode = LoadSceneMode.Single)
        {
            var ao = SceneManager.LoadSceneAsync(sceneName, mode);
            MonoSystem.Start_Coroutine(DoLoadSceneAsyncWithoutActive(ao, successCallBack, loadingCallBack,
                mode));
        }

        /// <summary>
        /// 异步加载场景，加载完成后不立刻切换
        /// </summary>
        /// <param name="sceneIndex">场景构建序号</param>
        /// <param name="successCallBack">加载完成回调，必须传入否则场景无法切换</param>
        /// <param name="loadingCallBack">加载时每帧回调</param>
        /// <param name="mode">加载模式</param>
        public static void LoadSceneAsyncWithoutActive(int sceneIndex, Action<AsyncOperation> successCallBack,
            Action<float> loadingCallBack = null, LoadSceneMode mode = LoadSceneMode.Single)
        {
            var ao = SceneManager.LoadSceneAsync(sceneIndex, mode);
            MonoSystem.Start_Coroutine(DoLoadSceneAsyncWithoutActive(ao, successCallBack, loadingCallBack,
                mode));
        }

        private static IEnumerator DoLoadSceneAsyncWithoutActive(AsyncOperation ao,
            Action<AsyncOperation> successCallBack, Action<float> loadingCallBack = null,
            LoadSceneMode mode = LoadSceneMode.Single)
        {
            ao.allowSceneActivation = false;
            ao.completed += successCallBack;
            while (!ao.isDone)
            {
                loadingCallBack?.Invoke(ao.progress);
                Debug.Log(ao.progress);
                EventSystem.EventTrigger("LoadingSceneProgress", ao.progress);
                yield return CoroutineTool.WaitForFrames();
            }

            EventSystem.EventTrigger("LoadSceneSucceed");
        }
    }
}

