using JKFrame;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class LocalizationSystem : MonoBehaviour
{
    private static LocalizationSystem instance;
    private const string OnUpdaterLanguage = "OnUpdaterLanguage";

    /// <summary>
    /// 访问或设置语言类型，设置时会自动分发语言修改事件
    /// </summary>
    public static LanguageType LanguageType
    {
        get { return instance.languageType; }
        set
        {
            instance.languageType = value;
            OnLanguageValueChanged();

        }
    }
    public static void Init()
    {
        instance = JKFrameRoot.RootTransform.GetComponentInChildren<LocalizationSystem>();
    }

    /// <summary>
    /// 全局的配置
    /// 可以运行时修改此配置
    /// </summary>
    [SerializeField] private LocalizationConfig globalConfig;

    [SerializeField] private LanguageType languageType;

    private void OnValidate()
    {
        OnLanguageValueChanged();
    }
    public static void OnLanguageValueChanged()
    {
        if (instance == null) return; // 应该没有运行
        EventSystem.EventTrigger(OnUpdaterLanguage, instance.languageType);
    }

    /// <summary>
    /// 获取内容，如果不存在会返回Null
    /// </summary>
    /// <returns></returns>
    public static T GetContent<T>(string key, LanguageType languageType) where T : LocalizationDataBase => instance.GetContentByKey<T>(key, languageType);

    public T GetContentByKey<T>(string key, LanguageType languageType) where T : LocalizationDataBase
    {
        if (globalConfig == null)
        {
            JKLog.Warning("缺少globalConfig");
            return null;
        }
        return globalConfig.GetContent<T>(key, languageType);
    }

    public static void RegisterLanguageEvent(Action<LanguageType> action)
    {
        EventSystem.AddEventListener(OnUpdaterLanguage, action);
    }

    public static void UnregisterLanguageEvent(Action<LanguageType> action)
    {
        EventSystem.RemoveEventListener(OnUpdaterLanguage, action);
    }

}
