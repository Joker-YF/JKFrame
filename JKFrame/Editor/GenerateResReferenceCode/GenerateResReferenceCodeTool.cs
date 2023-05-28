
#if ENABLE_ADDRESSABLES
using JKFrame;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
public static class GenerateResReferenceCodeTool
{

    static string scriptPath = Application.dataPath + "/JKFrame/R.cs";
    private static string fileStr =
@"using JKFrame;
~~命名空间~~
namespace R
{
~~成员~~
}";

    private static string classTemplate =
@" 
    public static class ##类名##
    {
~~成员~~
    }";
    private static string PropertyTemplate =
@" 
        public static ##类型## ##资源名称## { get => ResSystem.LoadAsset<##类型##>(""##资源路径##""); }";
    private static string SubAssetPropertyTemplate =
@"  
        public static ##类型## ##资源名称## { get => ResSystem.LoadAsset<##类型##>(""##资源路径##""); }";
    private static string GameObjectPropertyTemplate =
@"  
        public static GameObject ##资源名称##_GameObject(Transform parent = null,string keyName=null,bool autoRelease = true)
        {
            return ResSystem.InstantiateGameObject(""##资源路径##"", parent, keyName,autoRelease);
        }";

    public static void CleanResReferenceCode()
    {
        if (File.Exists(scriptPath))
        {
            File.Delete(scriptPath);
            AssetDatabase.Refresh();
        }
        JKLog.Succeed("清除资源代码脚本成功");
    }
    public static void GenerateResReferenceCode()
    {
        // 开始生成
        JKLog.Log("开始生成资源代码");
        if (File.Exists(scriptPath)) File.Delete(scriptPath);

        FileStream file = new FileStream(scriptPath, FileMode.CreateNew);
        StreamWriter fileW = new StreamWriter(file, System.Text.Encoding.UTF8);

        HashSet<string> allTypeAssemblyNames = new HashSet<string>();
        allTypeAssemblyNames.Add("UnityEngine");

        // 获取全部Addressable的Group
        string groupsStr = "";
        AddressableAssetSettings assets = AddressableAssetSettingsDefaultObject.Settings;
        foreach (AddressableAssetGroup group in assets.groups)
        {
            if (group.name == "Built In Data") continue;
            string name = group.name.Replace(" ", "");   // 去除空格
            // 建立子类名称
            string groupStr = classTemplate.Replace("##类名##", name);

            // 找到子类全部资源以及类型
            List<AddressableAssetEntry> allAssetEntry = new List<AddressableAssetEntry>();

            group.GatherAllAssets(allAssetEntry, true, true, true);
            string propertyStrs = "";   // 属性的字符串
            for (int i = 0; i < allAssetEntry.Count; i++)
            {
                AddressableAssetEntry assetItem = allAssetEntry[i];
                // 添加到类型列表中等待添加命名空间
                string assemblyNam = assetItem.MainAssetType.Assembly.GetName().Name;
                if (assemblyNam != "UnityEngine.CoreModule" && !allTypeAssemblyNames.Contains(assemblyNam))
                {
                    allTypeAssemblyNames.Add(assemblyNam);
                }

                if (assetItem.IsSubAsset)   // sub资源主要存在[]无法生成class
                {
                    string subAssetPropertyStr = SubAssetPropertyTemplate.Replace("##类型##", assetItem.MainAssetType.Name);
                    string assetName = assetItem.address.Replace("[", "_").Replace("]", ""); // 去除子资源中的括号
                    subAssetPropertyStr = subAssetPropertyStr.Replace("##资源名称##", assetName.Replace(" ", ""));
                    subAssetPropertyStr = subAssetPropertyStr.Replace("##资源路径##", assetItem.address);
                    propertyStrs += subAssetPropertyStr;
                }
                else
                {
                    string propertyStr = PropertyTemplate.Replace("##类型##", assetItem.MainAssetType.Name);
                    propertyStr = propertyStr.Replace("##资源名称##", assetItem.address.Replace(" ", ""));
                    propertyStr = propertyStr.Replace("##资源路径##", assetItem.address);
                    propertyStrs += propertyStr;
                    if (assetItem.MainAssetType == typeof(GameObject))  // 游戏物体增加一个用于直接实例化的
                    {
                        string gameObjectPropertyStr = GameObjectPropertyTemplate.Replace("##资源名称##", assetItem.address.Replace(" ", ""));
                        gameObjectPropertyStr = gameObjectPropertyStr.Replace("##资源路径##", assetItem.address);
                        propertyStrs += gameObjectPropertyStr;
                    }
                }
            }
            groupStr = groupStr.Replace("~~成员~~", propertyStrs);
            groupsStr += groupStr;
        }

        string namespaceString = "";
        // 生成命名空间
        foreach (string item in allTypeAssemblyNames)
        {
            namespaceString += @" using {item};
".Replace("{item}", item);
        }

        fileStr = fileStr.Replace("~~命名空间~~", namespaceString);
        fileStr = fileStr.Replace("~~成员~~", groupsStr);


        fileW.Write(fileStr);
        fileW.Flush();
        fileW.Close();
        file.Close();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        // 结束生成
        JKLog.Succeed("生成资源代码成功");
    }
}
#endif