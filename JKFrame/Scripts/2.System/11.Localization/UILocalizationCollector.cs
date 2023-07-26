using JKFrame;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 本地化收集者
/// </summary>
public class UILocalizationCollector : SerializedMonoBehaviour
{
    public LocalizationConfig localizationConfig;
    [TableList]
    public List<UILocalizationData> localizationDataList = new List<UILocalizationData>();
    private Action<object, string> analyzer;
    private void Reset()
    {
        UI_WindowBase window = GetComponent<UI_WindowBase>();
        if (window != null && window.localizationConfig != null)
        {
            localizationConfig = window.localizationConfig;
        }
    }

    private void OnEnable()
    {
        LocalizationSystem.RegisterLanguageEvent(OnUpdateLanguage);
        OnUpdateLanguage(LocalizationSystem.LanguageType);
    }
    private void OnDisable()
    {
        LocalizationSystem.UnregisterLanguageEvent(OnUpdateLanguage);
    }

    private void OnUpdateLanguage(LanguageType type)
    {
        foreach (UILocalizationData item in localizationDataList)
        {
            Analysis(item.component, item.key, type);
        }
    }

    /// <summary>
    /// 初始化分析器
    /// 分析器：根据组件的类型不同返回不同的数据
    /// </summary>
    /// <param name="analyzer"></param>
    public void InitAnalyzer(Action<object, string> analyzer)
    {
        this.analyzer = analyzer;
    }

    /// <summary>
    /// 优先采用外部传进来的解析器
    /// 如果没有则采用内部简单解析器，优先在本地配置中寻找，如果没有则在全局配置中寻找
    /// </summary>
    public void Analysis(MaskableGraphic component, string key, LanguageType languageType)
    {
        if (component == null) return;
        if (analyzer != null)
        {
            analyzer.Invoke(component, key);
            return;
        }

        // 内置简单解析
        if (component is Text)
        {
            LocalizationStringData data = localizationConfig.GetContent<LocalizationStringData>(key, languageType);
            if (data == null) data = LocalizationSystem.GetContent<LocalizationStringData>(key, languageType);
            if (data != null) ((Text)component).text = data.content;
        }
        else if (component is Image)
        {
            LocalizationImageData data = localizationConfig.GetContent<LocalizationImageData>(key, languageType);
            if (data == null) data = LocalizationSystem.GetContent<LocalizationImageData>(key, languageType);
            if (data != null) ((Image)component).sprite = data.content;
        }
    }
}

public class UILocalizationData
{
    public MaskableGraphic component;
    public string key;
}
