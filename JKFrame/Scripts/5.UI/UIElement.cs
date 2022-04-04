using Sirenix.OdinInspector;
using UnityEngine;
namespace JKFrame
{
    /// <summary>
    /// UI元素
    /// </summary>
    public class UIElement
    {
        [LabelText("是否需要缓存")]
        public bool isCache;
        [LabelText("预制体路径")]
        public string prefabAssetName;
        [LabelText("UI层级")]
        public int layerNum;
        /// <summary>
        /// 这个元素的窗口对象
        /// </summary>
        [HideInInspector]
        public UI_WindowBase objInstance;
    }
}