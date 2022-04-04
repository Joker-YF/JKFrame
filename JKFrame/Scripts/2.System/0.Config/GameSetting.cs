using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace JKFrame
{
    /// <summary>
    /// 框架层面的游戏设置
    /// 对象池缓存设置、UI元素的设置
    /// </summary>
    [CreateAssetMenu(fileName = "GameSetting", menuName = "JKFrame/Config/GameSetting")]
    public class GameSetting : ConfigBase
    {
        [LabelText("框架AB包名称")]
        public string JKFarmeABName;
        [LabelText("对象池设置")]
        [DictionaryDrawerSettings(KeyLabel = "类型", ValueLabel = "皆可缓存")]
        public Dictionary<string, bool> cacheDic = new Dictionary<string, bool>();

        [LabelText("UI窗口设置")]
        [DictionaryDrawerSettings(KeyLabel = "类型", ValueLabel = "UI窗口数据")]
        public Dictionary<Type, UIElement> UIElementDic = new Dictionary<Type, UIElement>();

#if UNITY_EDITOR
        [Button(Name = "初始化游戏配置", ButtonHeight = 50)]
        [GUIColor(0, 1, 0)]
        /// <summary>
        /// 编译前执行函数
        /// </summary>
        public void InitForEditor()
        {
            PoolAttributeOnEditor();
            UIElementAttributeOnEditor();
        }

        /// <summary>
        /// 将带有Pool特性的类型加入缓存池字典
        /// </summary>
        private void PoolAttributeOnEditor()
        {
            cacheDic.Clear();
            // 获取所有程序集
            System.Reflection.Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
            // 遍历程序集
            foreach (System.Reflection.Assembly assembly in asms)
            {
                // 遍历程序集下的每一个类型
                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    // 获取pool特性
                    PoolAttribute pool = type.GetCustomAttribute<PoolAttribute>();
                    if (pool != null)
                    {
                        if (pool.TypeOrAssetName == string.Empty)
                        {
                            cacheDic.Add(type.Name, true);
                        }
                        else
                        {
                            cacheDic.Add(pool.TypeOrAssetName, true);
                        }
                    }
                }
            }
        }

        private void UIElementAttributeOnEditor()
        {
            UIElementDic.Clear();
            // 获取所有程序集
            System.Reflection.Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
            Type baseType = typeof(UI_WindowBase);
            // 遍历程序集
            foreach (System.Reflection.Assembly assembly in asms)
            {
                // 遍历程序集下的每一个类型
                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    if (baseType.IsAssignableFrom(type)
                        && !type.IsAbstract)
                    {
                        UIElementAttribute attribute = type.GetCustomAttribute<UIElementAttribute>();
                        if (attribute != null)
                        {
                            UIElementDic.Add(type, new UIElement()
                            {
                                isCache = attribute.isCache,
                                prefabAssetName = attribute.prefabAssetName,
                                layerNum = attribute.layerNum
                            });
                        }
                    }
                }
            }
        }
#endif
    }


}