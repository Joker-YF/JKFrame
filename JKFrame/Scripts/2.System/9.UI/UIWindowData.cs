using Sirenix.OdinInspector;
using UnityEngine;
namespace JKFrame
{
    /// <summary>
    /// UI元素数据
    /// </summary>
    public class UIWindowData
    {
        [LabelText("是否需要缓存")] public bool isCache;
        [LabelText("预制体Path或AssetKey")] public string assetPath;
        [LabelText("UI层级")] public int layerNum;
        /// <summary>
        /// 这个元素的窗口对象
        /// </summary>
        [LabelText("窗口实例")] public UI_WindowBase instance;

        public UIWindowData(bool isCache, string assetPath, int layerNum)
        {
            this.isCache = isCache;
            this.assetPath = assetPath;
            this.layerNum = layerNum;
            instance = null;
        }
    }
}