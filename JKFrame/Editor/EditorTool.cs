using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public static class EditorTool
{
    /// <summary>
    /// 增加预处理指令
    /// </summary>
    public static void AddScriptCompilationSymbol(string name)
    {
        BuildTargetGroup buildTargetGroup = UnityEditor.EditorUserBuildSettings.selectedBuildTargetGroup;
        string group = UnityEditor.PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
        if (!group.Contains(name))
        {
            UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, group + ";" + name);
        }
    }

    /// <summary>
    /// 移除预处理指令
    /// </summary>
    public static void RemoveScriptCompilationSymbol(string name)
    {
        BuildTargetGroup buildTargetGroup = UnityEditor.EditorUserBuildSettings.selectedBuildTargetGroup;
        string group = UnityEditor.PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
        if (group.Contains(name))
        {
            UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, group.Replace(";" + name, string.Empty));
        }
    }
}
