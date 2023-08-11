using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace JKFrame
{
    /// <summary>
    /// 窗口基类
    /// </summary>
    public class UI_WindowBase : MonoBehaviour
    {
        protected bool uiEnable;
        public bool UIEnable { get => uiEnable; }
        protected int currentLayer;
        public int CurrentLayer { get => currentLayer; }

        // 窗口类型
        public Type Type { get { return this.GetType(); } }

        public bool EnableLocalization => localizationConfig == null;

        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Init() { }

        public void ShowGeneralLogic(int layerNum)
        {
            this.currentLayer = layerNum;
            if (!uiEnable)
            {
                RegisterEventListener();
                // 绑定本地化事件
                LocalizationSystem.RegisterLanguageEvent(UpdateLanguageGeneralLogic);
            }

            OnShow();
            OnUpdateLanguage(LocalizationSystem.LanguageType);
            uiEnable = true;
        }

        /// <summary>
        /// 显示
        /// </summary>
        public virtual void OnShow() { }

        /// <summary>
        /// 关闭的基本逻辑
        /// </summary>
        public void CloseGeneralLogic()
        {
            uiEnable = false;
            UnRegisterEventListener();
            LocalizationSystem.UnregisterLanguageEvent(UpdateLanguageGeneralLogic);
            OnClose();
        }

        /// <summary>
        /// 关闭时额外执行的内容
        /// </summary>
        public virtual void OnClose() { }

        /// <summary>
        /// 注册事件
        /// </summary>
        protected virtual void RegisterEventListener() { }

        /// <summary>
        /// 取消事件
        /// </summary>
        protected virtual void UnRegisterEventListener() { }

        #region 本地化
        /// <summary>
        /// 当本地化配置中不包含指定key时，会自动在全局配置中尝试
        /// </summary>
        [SerializeField, LabelText("本地化配置")]
        public LocalizationConfig localizationConfig;

        protected void UpdateLanguageGeneralLogic(LanguageType languageType)
        {
            OnUpdateLanguage(languageType);
        }

        /// <summary>
        /// 当语言更新时
        /// </summary>
        protected virtual void OnUpdateLanguage(LanguageType languageType)
        {

        }
        #endregion
    }
}