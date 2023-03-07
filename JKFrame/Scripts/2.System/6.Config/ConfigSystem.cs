using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace JKFrame
{
    /// <summary>
    /// 配置系统
    /// </summary>
    public static class ConfigSystem
    {
#if ENABLE_ADDRESSABLES
        /// <summary>
        /// 获取某个类型下的 配置文件
        /// Addressables中的name，configTypeName_id
        /// </summary>
        public static T GetConfig<T>(string configName) where T : ConfigBase
        {
            return ResSystem.LoadAsset<T>(configName);
        }

        /// <summary>
        /// 获取某一个配置类型下的全部配置
        /// Addressable中某一个GroupName下的全部配置文件
        /// </summary>
        public static List<T> GetConfigList<T>(string configTypeName) where T : ConfigBase
        {
            return (List<T>)ResSystem.LoadAssets<T>(configTypeName);
        }
#else

        /// <summary>
        /// 获取某一个路径下的全部配置
        /// </summary>
        public static T[] GetConfigs<T>(string path) where T : ConfigBase
        {
            return ResSystem.LoadAssets<T>(path);
        }
        /// <summary>
        /// 获取某一个路径下的全部配置
        /// 并且转为List（有额外性能消耗，尽可能使用 GetConfigs<T>()）
        /// </summary>
        public static List<T> GetConfigList<T>(string path) where T : ConfigBase
        {
            return ResSystem.LoadAssets<T>(path).ToList();
        }
#endif
    }
}

