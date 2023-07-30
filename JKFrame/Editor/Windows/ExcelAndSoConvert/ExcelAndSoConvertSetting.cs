using OfficeOpenXml;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace JKFrame.Editor
{
    [Serializable]
    public class ExcelAndSoConvertSetting : ConfigBase
    {
        const int typeIndex = 1;  // 类型
        const int fieldNameIndex = 2; // 变量名
        const int remarkIndex = 3;    // 备注
        #region 生成Excel模板
        [TabGroup("生成Excel模板"), LabelText("生成路径"), FolderPath(AbsolutePath = true, RequireExistingPath = true)] public string generateExcelTemplatePath;
        [TabGroup("生成Excel模板"), LabelText("文件名(空则使用class名)")] public string fileName;
        [TabGroup("生成Excel模板"), LabelText("生成类型"), EnumToggleButtons] public Type generateExcelTemplateClassType;
        [TabGroup("生成Excel模板"), LabelText("使用OdinLabelText进行备注")] public bool useLableText;
        [TabGroup("生成Excel模板"), LabelText("List<T>集合模式"), Tooltip("用于一张表对应一个SO集合的方式")] public bool useListFied;
        [TabGroup("生成Excel模板"), LabelText("List<T>字段名称"), ShowIf("CheckGenerateExcelTemplateFieldName")] public string generateExcelTemplateFieldName;
        private bool CheckGenerateExcelTemplateFieldName => useListFied;

        [TabGroup("生成Excel模板"), Button(ButtonHeight = 30, Name = "生成Excel模版")]
        public void GenerateExcelTemplate()
        {
            GenerateExcelTemplateFile(generateExcelTemplatePath, fileName, generateExcelTemplateClassType, useLableText, useListFied, generateExcelTemplateFieldName);
        }

        private void GenerateExcelTemplateFile(string generateExcelTemplatePath, string fileName, Type generateExcelTemplateClassType, bool useLableText, bool useListFied, string generateExcelTemplateFieldName)
        {
            if (string.IsNullOrWhiteSpace(generateExcelTemplatePath) || generateExcelTemplateClassType == null) JKLog.Error("信息不全，无法生成！");

            string tName = fileName;
            if (string.IsNullOrEmpty(tName)) tName = generateExcelTemplateClassType.Name;
            string path = generateExcelTemplatePath + "/" + tName + ".xlsx";
            // 删除可能存在的当前模板文件
            File.Delete(path);
            FileInfo fileInfo = new FileInfo(path);
            using (ExcelPackage excelPackage = new ExcelPackage(fileInfo))
            {
                ExcelWorksheet mainSheet = excelPackage.Workbook.Worksheets.Add(generateExcelTemplateClassType.Name);
                FieldInfo[] fieldInfos = null;

                if (useListFied)
                {
                    // 泛型的实际类型
                    Type argType = generateExcelTemplateClassType.GetField(generateExcelTemplateFieldName).FieldType.GetInterfaces()[0].GetGenericArguments()[0];
                    // 获取ListT的部分
                    fieldInfos = argType.GetFields();
                }
                else
                {
                    fieldInfos = generateExcelTemplateClassType.GetFields();
                }

                for (int i = 0; i < fieldInfos.Length; i++)
                {
                    FieldInfo fieldInfo = fieldInfos[i];
                    int cellIndex = i + 1;
                    // 标记类型
                    mainSheet.Cells[typeIndex, cellIndex].Value = fieldInfo.FieldType.Name;
                    // 标记备注
                    if (useLableText)
                    {
                        LabelTextAttribute labelTextAttribute = fieldInfo.GetCustomAttribute<LabelTextAttribute>();
                        if (labelTextAttribute != null) mainSheet.Cells[remarkIndex, cellIndex].Value = labelTextAttribute.Text;
                    }
                    // 标记变量名
                    mainSheet.Cells[fieldNameIndex, cellIndex].Value = fieldInfo.Name;
                }
                excelPackage.Save();
                AssetDatabase.Refresh();
                Debug.Log($"生成Excel模板成功，路径为：{path}");
            }
        }
        #endregion
        #region 批量转换
        public enum Conversion
        {
            [LabelText("一个Excel生成一个SO")] Excel2OneSO,
            [LabelText("一个Excel生成多个SO")] Excel2SOs,
            [LabelText("一个SO生成一个Excel")] OneSo2Excel,
            [LabelText("多个SO生成一个Excel")] SOs2Excel,
        }

        [InfoBox("互转必须格式符合，1个SO生成一个Excel的前提是这个SO包含一个List")]
        [TabGroup("批量转换")][LabelText("转换方式")] public Conversion conversion;
        [TabGroup("批量转换")][LabelText("要转的类")][ShowIf("CheckConversionTye")] public Type conversionTye;
        [InfoBox("True:使用GUID进行资源记录（也就是资源位置不影响表格），False:使用路径进行记录")]
        [TabGroup("批量转换")][LabelText("GUIDMode")] public bool guidMode;

        [TabGroup("批量转换"), LabelText("Excel文件路径"), Sirenix.OdinInspector.FilePath(AbsolutePath = true, RequireExistingPath = true), ShowIf("CheckConversionExcelFilePath")]
        public string conversionExcelFilePath;
        [TabGroup("批量转换"), LabelText("保存到的Excel文件夹"), FolderPath(AbsolutePath = true, RequireExistingPath = true), ShowIf("CheckSaveExcelFileFolderPath")]
        public string saveExcelFileFolderPath;
        [TabGroup("批量转换"), LabelText("Excel的名称(无需后缀，留空则使用类型名)"), ShowIf("CheckSaveExcelName")]
        public string saveExcelName;

        [TabGroup("批量转换"), LabelText("SO文件夹路径"), FolderPath(AbsolutePath = false, RequireExistingPath = true), ShowIf("CheckConversionSoFolderPath")]
        public string conversionSoFolderPath;
        [TabGroup("批量转换"), LabelText("SO文件名采用前缀_数字的形式"), ShowIf("CheckUseSoFileNamePrefix")] public bool useSoFileNamePrefix;
        [TabGroup("批量转换"), LabelText("SO文件名使用第几个字段"), ShowIf("CheckSoFileNameUseFileIndex")] public int soFileNameUseFileIndex = 0;
        [TabGroup("批量转换"), LabelText("SO文件名前缀"), ShowIf("CheckSoFileNamePrefixName")] public string soFileNamePrefixName;

        [TabGroup("批量转换"), LabelText("SO文件"), ShowIf("CheckSoFile")] public ScriptableObject soFile;

        [TabGroup("批量转换"), LabelText("SO中List变量的名称"), ShowIf("CheckListFieldName")] public string listFieldName;
        [TabGroup("批量转换")][LabelText("转换前清空SO文件夹目录"), ShowIf("CheckDeleteFolder")] public bool deleteFolder;

        [Button("生成", ButtonHeight = 30), TabGroup("批量转换")]
        public void ConversionGeneate()
        {
            switch (conversion)
            {
                case Conversion.Excel2OneSO:
                    Excel2OneSO();
                    break;
                case Conversion.Excel2SOs:
                    Excel2SOs();
                    break;
                case Conversion.OneSo2Excel:
                    OneSo2Excel();
                    break;
                case Conversion.SOs2Excel:
                    SOs2Excel();
                    break;
            }
        }

        private void Excel2OneSO()
        {
            if (soFile == null)
            {
                Debug.LogError("SO不能为Null");
                return;
            }

            // 列表所属的字段
            FieldInfo listFieldInfo = soFile.GetType().GetField(listFieldName);
            if (listFieldInfo == null || (listFieldInfo as IList) != null)
            {
                Debug.LogError("SO中List变量的名称错误,或者其并不是一个List<T>");
                return;
            }

            // 得到泛型的实际类型
            Type argType = listFieldInfo.FieldType.GetInterfaces()[0].GetGenericArguments()[0];

            string excelFilepath = conversionExcelFilePath;

            // 创建列表的实际泛型 类型
            IList list = (IList)Activator.CreateInstance(listFieldInfo.FieldType);
            MethodInfo addMethodInfo = list.GetType().GetMethod("Add");
            // 读取表格数据，填充进SO
            // 根据表头找到所有的FieldInfo
            using (ExcelPackage excelPackage = new ExcelPackage(new FileStream(excelFilepath, FileMode.Open)))
            {
                ExcelWorksheet sheet = excelPackage.Workbook.Worksheets[1];
                int colCount = sheet.Dimension.End.Row;
                int rowCount = sheet.Dimension.End.Column;
                List<FieldInfo> fieldInfoList = new List<FieldInfo>();
                for (int i = 1; i <= rowCount; i++)
                {
                    FieldInfo fieldInfo = argType.GetField(sheet.Cells[fieldNameIndex, i].Text);
                    if (fieldInfo == null)
                    {
                        continue;
                    }
                    fieldInfoList.Add(fieldInfo);
                }

                int startColIndex = remarkIndex;
                // 验证起始行
                if (sheet.Cells[remarkIndex, 1].Text == fieldInfoList[0].Name)
                {
                    startColIndex = remarkIndex + 1;
                }
                // 行
                for (int x = 1; x <= colCount; x++)
                {
                    // 创建对象 so中的对象
                    object obj = Activator.CreateInstance(argType);
                    bool allEmpty = true; // 一行全是空则说明可以停止
                    // 列
                    for (int y = 1; y <= fieldInfoList.Count; y++)
                    {
                        string excelValue = sheet.Cells[x + startColIndex, y].Text;
                        if (!string.IsNullOrEmpty(excelValue))
                        {
                            // y-1意味着是第几个字段
                            FieldInfo fieldInfo = fieldInfoList[y - 1];
                            SetDataField(obj, fieldInfo, excelValue);
                            allEmpty = false;
                        }
                    }
                    if (!allEmpty)
                    {
                        addMethodInfo.Invoke(list, new object[] { obj });
                    }
                }
            }

            listFieldInfo.SetValue(soFile, list);
            EditorUtility.SetDirty(soFile);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void Excel2SOs()
        {
            string excelFilepath = conversionExcelFilePath;
            if (!conversionTye.IsSubclassOf(typeof(ScriptableObject)))
            {
                Debug.LogError("选择的类型没有继承自ScriptableObject");
                return;
            }

            // 删除文件
            if (deleteFolder)
            {
                Directory.Delete(Application.dataPath + conversionSoFolderPath.Replace("Assets", ""), true);
                AssetDatabase.Refresh();
            }

            using (ExcelPackage excelPackage = new ExcelPackage(new FileStream(excelFilepath, FileMode.Open)))
            {
                ExcelWorksheet sheet = excelPackage.Workbook.Worksheets[1];
                int colCount = sheet.Dimension.End.Row;
                int rowCount = sheet.Dimension.End.Column;
                // 获取字段信息
                List<FieldInfo> fieldInfoList = new List<FieldInfo>();
                for (int i = 1; i <= rowCount; i++)
                {
                    string d = sheet.Cells[fieldNameIndex, i].Text;
                    FieldInfo fieldInfo = conversionTye.GetField(sheet.Cells[fieldNameIndex, i].Text);
                    if (fieldInfo == null)
                    {
                        continue;
                    }
                    fieldInfoList.Add(fieldInfo);
                }

                int startColIndex = remarkIndex;
                // 验证起始行
                if (sheet.Cells[remarkIndex, 1].Text == fieldInfoList[0].Name) startColIndex = remarkIndex + 1;

                // 行
                for (int x = 1; x <= colCount; x++)
                {
                    ScriptableObject obj = ScriptableObject.CreateInstance(conversionTye);
                    bool allEmpty = true; // 一行全是空则说明可以停止
                    // 列
                    for (int y = 1; y <= fieldInfoList.Count; y++)
                    {
                        string excelValue = sheet.Cells[x + startColIndex, y].Text;

                        if (!useSoFileNamePrefix)
                        {
                            if (y == soFileNameUseFileIndex + 1)// 文件名称 索引从0开始，但是excel从1开始
                            {
                                obj.name = excelValue;
                            }
                        }

                        if (!string.IsNullOrEmpty(excelValue))
                        {
                            // y-1意味着是第几个字段
                            FieldInfo fieldInfo = fieldInfoList[y - 1];
                            SetDataField(obj, fieldInfo, excelValue);
                            allEmpty = false;
                        }
                    }
                    if (!useSoFileNamePrefix && string.IsNullOrEmpty(obj.name) || allEmpty)
                    {
                        continue;
                    }
                    if (useSoFileNamePrefix)
                    {
                        obj.name = soFileNamePrefixName + "_" + x;
                    }
                    AssetDatabase.CreateAsset(obj, $"{conversionSoFolderPath}/{obj.name}.asset");
                }
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
        private void OneSo2Excel()
        {
            if (soFile == null)
            {
                Debug.LogError("SO不能为Null");
                return;
            }
            string excelFilepath = "";
            if (string.IsNullOrEmpty(saveExcelName)) excelFilepath = saveExcelFileFolderPath + "/" + soFile.GetType().Name + ".xlsx";
            else excelFilepath = saveExcelFileFolderPath + "/" + saveExcelName + ".xlsx";
            string listName = listFieldName;
            // 列表所属的字段
            FieldInfo listFieldInfo = soFile.GetType().GetField(listName);
            if (listFieldInfo == null || (listFieldInfo as IList) != null)
            {
                Debug.LogError("SO中List变量的名称错误,或者其并不是一个List<T>");
                return;
            }

            // 得到泛型的实际类型
            Type argType = listFieldInfo.FieldType.GetInterfaces()[0].GetGenericArguments()[0];

            File.Delete(excelFilepath); // 确保删除
            FileStream fileStream = new FileStream(excelFilepath, FileMode.CreateNew);
            using (ExcelPackage excelPackage = new ExcelPackage(fileStream))
            {
                // 获取所有字段,建立表头
                ExcelWorksheet mainSheet = excelPackage.Workbook.Worksheets.Add(conversionTye.Name);
                FieldInfo[] fieldInfos = argType.GetFields();
                for (int i = 0; i < fieldInfos.Length; i++)
                {
                    FieldInfo fieldInfo = fieldInfos[i];
                    int cellIndex = i + 1;
                    // 标记类型
                    mainSheet.Cells[typeIndex, cellIndex].Value = fieldInfo.FieldType.Name;
                    // 标记备注
                    if (useLableText)
                    {
                        LabelTextAttribute labelTextAttribute = fieldInfo.GetCustomAttribute<LabelTextAttribute>();
                        if (labelTextAttribute != null) mainSheet.Cells[remarkIndex, cellIndex].Value = labelTextAttribute.Text;
                    }
                    // 标记变量名
                    mainSheet.Cells[fieldNameIndex, cellIndex].Value = fieldInfo.Name;
                }
                // list转为迭代器
                IEnumerable listObject = (IEnumerable)listFieldInfo.GetValue(soFile);
                int y = 0;
                foreach (object item in listObject)
                {
                    for (int x = 0; x < fieldInfos.Length; x++)
                    {
                        string excelString = GetDataField(item, fieldInfos[x], guidMode);
                        mainSheet.Cells[4 + y, x + 1].Value = excelString;
                    }
                    y++;
                }
                excelPackage.Save();
            }
            fileStream.Dispose();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void SOs2Excel()
        {
            string excelFilepath = "";
            if (string.IsNullOrEmpty(saveExcelName)) excelFilepath = saveExcelFileFolderPath + "/" + soFile.GetType().Name + ".xlsx";
            else excelFilepath = saveExcelFileFolderPath + "/" + saveExcelName + ".xlsx";

            File.Delete(excelFilepath); // 确保删除
            FileStream fileStream = new FileStream(excelFilepath, FileMode.CreateNew);
            using (ExcelPackage excelPackage = new ExcelPackage(fileStream))
            {
                // 获取所有字段,建立表头
                ExcelWorksheet mainSheet = excelPackage.Workbook.Worksheets.Add(conversionTye.Name);
                FieldInfo[] fieldInfos = conversionTye.GetFields();
                for (int i = 0; i < fieldInfos.Length; i++)
                {
                    FieldInfo fieldInfo = fieldInfos[i];
                    int cellIndex = i + 1;
                    // 标记类型
                    mainSheet.Cells[typeIndex, cellIndex].Value = fieldInfo.FieldType.Name;
                    // 标记备注
                    if (useLableText)
                    {
                        LabelTextAttribute labelTextAttribute = fieldInfo.GetCustomAttribute<LabelTextAttribute>();
                        if (labelTextAttribute != null) mainSheet.Cells[remarkIndex, cellIndex].Value = labelTextAttribute.Text;
                    }
                    // 标记变量名
                    mainSheet.Cells[fieldNameIndex, cellIndex].Value = fieldInfo.Name;
                }

                // 获取所有文件
                string[] soFilePaths = Directory.GetFiles(Application.dataPath + conversionSoFolderPath.Replace("Assets", ""));
                int y = 0;
                for (int i = 0; i < soFilePaths.Length; i++)
                {
                    string path = soFilePaths[i].Substring(soFilePaths[i].IndexOf("Assets")); // 绝对路径转相对路径
                                                                                              // 挨个写入
                    UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, conversionTye);
                    if (obj != null)
                    {
                        for (int x = 0; x < fieldInfos.Length; x++)
                        {
                            string excelString = GetDataField(obj, fieldInfos[x], guidMode);
                            mainSheet.Cells[4 + y, x + 1].Value = excelString;
                        }
                        y += 1;
                    }
                }
                excelPackage.Save();
            }
            fileStream.Dispose();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private bool CheckConversionTye => conversion == Conversion.Excel2SOs || conversion == Conversion.SOs2Excel;
        private bool CheckConversionExcelFilePath => conversion == Conversion.Excel2OneSO || conversion == Conversion.Excel2SOs;
        private bool CheckUseSoFileNamePrefix => conversion == Conversion.Excel2SOs;
        private bool CheckConversionSoFolderPath => conversion == Conversion.Excel2SOs || conversion == Conversion.SOs2Excel;
        private bool CheckDeleteFolder => conversion == Conversion.Excel2SOs;
        private bool CheckListFieldName => conversion == Conversion.Excel2OneSO || conversion == Conversion.OneSo2Excel;
        private bool CheckSoFileNameUseFileIndex => CheckConversionSoFolderPath && !useSoFileNamePrefix && conversion != Conversion.SOs2Excel;
        private bool CheckSoFileNamePrefixName => !CheckSoFileNameUseFileIndex && conversion == Conversion.Excel2SOs;
        private bool CheckSoFile => conversion == Conversion.Excel2OneSO || conversion == Conversion.OneSo2Excel;
        private bool CheckSaveExcelFilePath => conversion == Conversion.OneSo2Excel || conversion == Conversion.SOs2Excel;
        private bool CheckSaveExcelName => conversion == Conversion.OneSo2Excel || conversion == Conversion.SOs2Excel;
        private bool CheckSaveExcelFileFolderPath => conversion == Conversion.OneSo2Excel || conversion == Conversion.SOs2Excel;

        #endregion
        #region 工具函数
        private UnityEngine.Object GetSubAsset(string subAssetName, string assetPath, bool guidMode)
        {
            if (guidMode)
            {
                // 获取原始路径
                assetPath = AssetDatabase.GUIDToAssetPath(assetPath);
            }
            UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
            for (int i = 0; i < assets.Length; i++)
            {
                if (assets[i].name == subAssetName && AssetDatabase.IsSubAsset(assets[i]))
                {
                    return assets[i];
                }
            }
            return null;
        }

        private void SetDataField(object obj, FieldInfo fieldInfo, string excelValue)
        {
            // 对Unity类型特殊处理,这种情况保存的一般是路径
            if (fieldInfo.FieldType.IsSubclassOf(typeof(UnityEngine.Object)))
            {
                string path = excelValue;
                bool isSubAsset = path.Contains("$/");
                // 有可能是subAsset
                if (isSubAsset)
                {
                    string[] paths = path.Split("$/");
                    UnityEngine.Object asset = GetSubAsset(paths[1], paths[0], guidMode);
                    fieldInfo.SetValue(obj, asset);
                }
                else
                {
                    if (guidMode) path = AssetDatabase.GUIDToAssetPath(excelValue);
                    UnityEngine.Object unityeObject = AssetDatabase.LoadAssetAtPath(path, fieldInfo.FieldType);
                    fieldInfo.SetValue(obj, unityeObject);
                }
            }
            else
            {
                // 内置类型直接转
                if (fieldInfo.FieldType == typeof(string))
                {
                    fieldInfo.SetValue(obj, excelValue);
                }
                else if (fieldInfo.FieldType.IsEnum) // 枚举特殊转换
                {
                    fieldInfo.SetValue(obj, Enum.Parse(fieldInfo.FieldType, excelValue));
                }
                else if (fieldInfo.FieldType == typeof(bool))
                {
                    fieldInfo.SetValue(obj, excelValue.ToUpper() == "TRUE" || excelValue == "1");// 表格可能会对bool自动做变化，避免这种情况
                }
                else
                {
                    fieldInfo.SetValue(obj, Convert.ChangeType(excelValue, fieldInfo.FieldType));
                }
            }
        }

        private string GetDataField(object obj, FieldInfo fieldInfo, bool guidMode)
        {
            object data = fieldInfo.GetValue(obj);
            string res = "";
            // 对Unity类型特殊处理,获取资源的路径
            if (fieldInfo.FieldType.IsSubclassOf(typeof(UnityEngine.Object)))
            {
                if (data != null)
                {
                    UnityEngine.Object unityeObject = (UnityEngine.Object)data;
                    res = AssetDatabase.GetAssetPath(unityeObject);
                    if (guidMode)
                    {
                        res = AssetDatabase.GUIDFromAssetPath(res).ToString();
                    }
                    if (AssetDatabase.IsSubAsset(unityeObject))
                    {
                        res += "$/" + unityeObject.name;
                    }
                }
            }
            else
            {
                res = data.ToString();
            }
            return res;
        }
        #endregion
    }
}