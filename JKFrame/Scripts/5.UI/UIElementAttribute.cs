using System;
namespace JKFrame
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    /// <summary>
    /// UI元素的特性
    /// 每个UI窗口都应该添加
    /// </summary>
    public class UIElementAttribute : Attribute
    {
        public bool isCache;
        public string prefabAssetName;
        public int layerNum;

        public UIElementAttribute(bool isCache, string prefabAssetName, int layerNum)
        {
            this.isCache = isCache;
            this.prefabAssetName = prefabAssetName;
            this.layerNum = layerNum;
        }
    }
}
