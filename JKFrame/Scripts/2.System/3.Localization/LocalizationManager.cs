using Sirenix.OdinInspector;
using UnityEngine;
namespace JKFrame
{
    /// <summary>
    /// 本地化管理器
    /// 持有本地化配置
    /// 提供本地化内容获取函数
    /// </summary>
    public class LocalizationManager : ManagerBase<LocalizationManager>
    {
        // 本地化配置资源
        public LocalizationSetting localizationSetting;

        [SerializeField]
        [OnValueChanged("UpdateLanguage")]
        private LanguageType currentLanguageType;

        public LanguageType CurrentLanguageType
        {
            get => currentLanguageType;
            set
            {
                currentLanguageType = value;
                UpdateLanguage();
            }
        }

        /// <summary>
        /// 获取当前语言设置下的内容
        /// </summary>
        /// <typeparam name="T">具体类型</typeparam>
        /// <param name="bagName">包名称</param>
        /// <param name="contentKey">内容名称</param>
        /// <returns></returns>
        public T GetContent<T>(string bagName, string contentKey) where T : class, ILanguage_Content
        {
            return localizationSetting.GetContent<T>(bagName, contentKey, currentLanguageType);
        }

        /// <summary>
        /// 更新语言
        /// </summary>
        private void UpdateLanguage()
        {
#if UNITY_EDITOR
            GameRoot.InitForEditor();
#endif
            // 触发更新语言 事件
            EventManager.EventTrigger("UpdateLanguage");
        }

    }
}
