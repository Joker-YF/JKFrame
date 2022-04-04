using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using UnityEngine.Video;
namespace JKFrame
{
    /// <summary>
    /// 语言类型
    /// </summary>
    public enum LanguageType
    {
        Chinese = 0,
        English = 1
    }

    /// <summary>
    /// 具体内容 接口
    /// </summary>
    public interface ILanguage_Content { }


    [Serializable]
    public class L_Text : ILanguage_Content
    {
        public string content;
    }
    [Serializable]
    public class L_Image : ILanguage_Content
    {
        public Sprite content;
    }
    [Serializable]
    public class L_Audio : ILanguage_Content
    {
        public AudioClip content;
    }
    [Serializable]
    public class L_Video : ILanguage_Content
    {
        public VideoClip content;
    }

    /// <summary>
    /// 本地化数据
    /// 一个图片，不同语言下，不同的Sprite
    /// </summary>
    public class LocalizationModel
    {
        [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.OneLine, KeyLabel = "语言类型", ValueLabel = "值")]
        public Dictionary<LanguageType, ILanguage_Content> contentDic = new Dictionary<LanguageType, ILanguage_Content>()
        {
          { LanguageType.Chinese,new L_Text()},
         { LanguageType.English,new L_Text()}
        };

    }

    [CreateAssetMenu(fileName = "本地化", menuName = "JKFrame/本地化配置")]
    public class LocalizationSetting : ConfigBase
    {
        // 包：UI还是玩家啊....敌人的配置之类
        // <包名称，<内容的名称，具体的值（包含了不同语言的不同设置）>>
        [SerializeField]
        [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.OneLine, KeyLabel = "数据包", ValueLabel = "语言数据")]
        private Dictionary<string, Dictionary<string, LocalizationModel>> dateBag;

        /// <summary>
        /// 获取本地化内容
        /// </summary>
        /// <typeparam name="T">具体返回的类型</typeparam>
        /// <param name="bagName">数据包名称</param>
        /// <param name="contentKey">内容名称</param>
        /// <param name="languageType">语言类型</param>
        public T GetContent<T>(string bagName, string contentKey, LanguageType languageType) where T : class, ILanguage_Content
        {
            return dateBag[bagName][contentKey].contentDic[languageType] as T;
        }
    }
}

