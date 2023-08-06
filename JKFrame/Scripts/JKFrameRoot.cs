using UnityEngine;

namespace JKFrame
{
    using JK.Log;
#if UNITY_EDITOR
    using UnityEditor;
    [InitializeOnLoad]
#endif
    [DefaultExecutionOrder(-20)]
    /// <summary>
    /// 框架根节点
    /// </summary>
    public class JKFrameRoot : MonoBehaviour
    {
        private JKFrameRoot() { }
        private static JKFrameRoot Instance;
        public static Transform RootTransform { get; private set; }
        public static JKFrameSetting Setting { get => Instance.FrameSetting; }
        // 框架层面的配置文件
        [SerializeField] JKFrameSetting FrameSetting;

        private void Awake()
        {
            if (Instance != null && Instance != this) // 防止Editor下的Instance已经存在，并且是自身
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            RootTransform = transform;
            DontDestroyOnLoad(gameObject);
            Init();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            InitSystems();
        }

        #region System
        private void InitSystems()
        {
            PoolSystem.Init();
            EventSystem.Init();
            MonoSystem.Init();
            AudioSystem.Init();
            UISystem.Init();
            SaveSystem.Init();
            LocalizationSystem.Init();
#if ENABLE_LOG
            JKLog.Init(FrameSetting.LogConfig);
#endif
        }
        #endregion
        private void OnDisable()
        {
#if ENABLE_LOG
            JKLog.Close();
#endif
        }
        #region Editor
#if UNITY_EDITOR
        // 编辑器专属事件系统
        public static EventModule EditorEventModule;
        static JKFrameRoot()
        {
            EditorEventModule = new EventModule();
            EditorApplication.update += () =>
            {
                InitForEditor();
            };
        }
        [InitializeOnLoadMethod]
        public static void InitForEditor()
        {
            // 当前是否要进行播放或准备播放中
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            if (Instance == null)
            {
                Instance = GameObject.FindObjectOfType<JKFrameRoot>();
                if (Instance == null) return;
                Instance.FrameSetting.InitOnEditor();
                //// 场景的所有窗口都进行一次Show
                //UI_WindowBase[] window = Instance.transform.GetComponentsInChildren<UI_WindowBase>();
                //foreach (UI_WindowBase win in window)
                //{
                //    win.ShowGeneralLogic();
                //}
            }
        }
#endif
        #endregion
    }

}


