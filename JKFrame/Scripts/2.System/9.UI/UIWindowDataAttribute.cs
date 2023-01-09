using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JKFrame
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class UIWindowDataAttribute: Attribute
    {
        public string windowKey;
        public bool isCache;
        public string assetPath;
        public int layerNum;

        public UIWindowDataAttribute(string windowKey, bool isCache, string assetPath, int layerNum)
        {
            this.windowKey = windowKey;
            this.isCache = isCache;
            this.assetPath = assetPath;
            this.layerNum = layerNum;
        }
        public UIWindowDataAttribute(Type type,bool isCache, string assetPath, int layerNum)
        {
            this.windowKey = type.FullName;
            this.isCache = isCache;
            this.assetPath = assetPath;
            this.layerNum = layerNum;
        }
    }
}
