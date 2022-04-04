using System;

namespace JKFrame
{
    /// <summary>
    /// 确定一个类是否需要对象池
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PoolAttribute : Attribute
    {
        public string TypeOrAssetName = string.Empty;
    }
}