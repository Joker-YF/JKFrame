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
    public class ConfigWindowSetting : ConfigBase
    {
        private static List<Type> builtInTypes = new List<Type>() {
            typeof(bool),
            typeof(byte),
            typeof(sbyte),
            typeof(char),
            typeof(decimal),
            typeof(double),
            typeof(float),
            typeof(int),
            typeof(uint),
            typeof(nint),
            typeof(nuint),
            typeof(nint),
            typeof(long),
            typeof(ulong),
            typeof(short),
            typeof(ushort),
            typeof(string),
        };
        #region 生成Excel模板
        [TabGroup("生成Excel模板"), LabelText("生成路径"), FolderPath(AbsolutePath = true, RequireExistingPath = true)]
        public string generateExcelTemplatePath;
        [TabGroup("生成Excel模板"), LabelText("生成类型"), EnumToggleButtons] public Type generateExcelTemplateClassType;
        [TabGroup("生成Excel模板"), LabelText("使用OdinLabelText进行备注")] public bool useLableText;
        [TabGroup("生成Excel模板"), LabelText("List<T>集合模式"), Tooltip("用于一张表对应一个SO集合的方式")] public bool useListFied;
        [TabGroup("生成Excel模板"), LabelText("List<T>字段名称"), ShowIf("CheckGenerateExcelTemplateFieldName")] public string generateExcelTemplateFieldName;

        private bool CheckGenerateExcelTemplateFieldName => useListFied;

        const int typeIndex = 1;  // 类型
        const int fieldNameIndex = 2; // 变量名
        const int remarkIndex = 3;    // 备注

        [TabGroup("生成Excel模板"), Button(ButtonHeight = 30), LabelText("生成Excel模板文件")]
        public void GenerateExcelTemplate()
        {
            if (string.IsNullOrWhiteSpace(generateExcelTemplatePath) || generateExcelTemplateClassType == null) JKLog.Error("信息不全，无法生成！");

            string path = generateExcelTemplatePath + "/" + generateExcelTemplateClassType.Name + ".xlsx";
            // 删除可能存在的当前模板文件
            if (File.Exists(path)) File.Delete(path);
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
            [LabelText("一个Excel生成一个SO")] OneExcel2OneSO,
            [LabelText("一个Excel生成多个SO")] One2Excels,
            [LabelText("一个SO生成一个Excel")] OneSo2OneExcel,
            [LabelText("多个SO生成一个Excel")] Sos2Excel,
        }

        [InfoBox("互转必须格式符合，1个SO生成一个Excel的前提是这个SO包含一个List")]
        [TabGroup("批量转换")][LabelText("转换方式")] public Conversion conversion;
        [TabGroup("批量转换")][LabelText("要转的类")] public Type conversionTye;

        [TabGroup("批量转换"), LabelText("Excel文件夹路径"), FolderPath(AbsolutePath = true, RequireExistingPath = true), ShowIf("CheckConversionExcelFolderPath")]
        public string conversionExcelFolderPath;
        [TabGroup("批量转换"), LabelText("Excel文件路径"), Sirenix.OdinInspector.FilePath(AbsolutePath = true, RequireExistingPath = true), ShowIf("CheckConversionExcelFilePath")]
        public string conversionExcelFilePath;

        [TabGroup("批量转换"), LabelText("SO文件夹路径"), FolderPath(AbsolutePath = false, RequireExistingPath = true), ShowIf("CheckConversionSoFolderPath")]
        public string conversionSoFolderPath;
        [TabGroup("批量转换"), LabelText("SO文件")] public ScriptableObject soObject;

        [TabGroup("批量转换"), LabelText("SO中List变量的名称")] public string listFieldName;
        [TabGroup("批量转换")][LabelText("转换前清空目录")] public bool deleteFolder;

        [Button("生成", ButtonHeight = 30)]
        public void ConversionGeneate()
        {
            switch (conversion)
            {
                case Conversion.OneExcel2OneSO:
                    if (soObject == null)
                    {
                        Debug.LogError("SO不能为Null");
                        return;
                    }
                    // 1个excel文件转为1个SO：需要excel文件路径、so文件、so类型、so中的list变量名
                    string excelFilepath = conversionExcelFilePath;
                    Type type = conversionTye;
                    string listName = listFieldName;

                    // 列表所属的字段
                    FieldInfo listFieldInfo = conversionTye.GetField(listName);
                    if (listFieldInfo == null || (listFieldInfo as IList) != null)
                    {
                        Debug.LogError("SO中List变量的名称错误,或者其并不是一个List<T>");
                        return;
                    }

                    // 得到泛型的实际类型
                    Type argType = listFieldInfo.FieldType.GetInterfaces()[0].GetGenericArguments()[0];

                    // 创建列表的实际泛型 类型
                    IList list = (IList)Activator.CreateInstance(listFieldInfo.FieldType);
                    MethodInfo addMethodInfo = list.GetType().GetMethod("Add");
                    // addMethodInfo.Invoke(list,new object[] { 10086 });
                    // 读取表格数据，填充进SO
                    // 根据表头找到所有的FieldInfo
                    // FileInfo fileInfo = new FileInfo(excelFilepath);
                    using (ExcelPackage excelPackage = new ExcelPackage(new FileStream(excelFilepath, FileMode.Open)))
                    {
                        ExcelWorksheet sheet = excelPackage.Workbook.Worksheets[1];
                        int colCount = sheet.Dimension.End.Row;
                        int rowCount = sheet.Dimension.End.Column;
                        List<FieldInfo> fieldInfoList = new List<FieldInfo>();
                        Debug.Log(sheet.Cells[1, 1].Text);
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
                        bool isBreak = false;
                        // 行
                        for (int x = 1; x <= colCount; x++)
                        {
                            // 创建对象
                            object obj = Activator.CreateInstance(argType);
                            // 列
                            for (int y = 1; y <= fieldInfoList.Count; y++)
                            {
                                string value = sheet.Cells[x + startColIndex, y].Text;
                                if (string.IsNullOrEmpty(value))
                                {
                                    isBreak = true;
                                    continue;
                                }
                                FieldInfo fieldInfo = fieldInfoList[y - 1];
                                // y-1也意味着是第几个字段
                                // TODO:对Unity类型特殊处理
                                fieldInfo.SetValue(obj, Convert.ChangeType(value, fieldInfo.FieldType)); // 强转类型
                            }
                            if (isBreak) continue;
                            addMethodInfo.Invoke(list, new object[] { obj });
                        }
                    }

                    listFieldInfo.SetValue(soObject, list);
                    EditorUtility.SetDirty(soObject);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    break;
                case Conversion.One2Excels:
                    break;
                case Conversion.OneSo2OneExcel:
                    break;
                case Conversion.Sos2Excel:
                    break;
            }
        }

        private bool CheckConversionExcelFolderPath => conversion != Conversion.OneExcel2OneSO;
        private bool CheckConversionExcelFilePath => conversion == Conversion.OneExcel2OneSO;

        private bool CheckConversionSoFolderPath => conversion != Conversion.OneExcel2OneSO;

        private bool CheckDeleteFolder => conversion != Conversion.OneExcel2OneSO;

        #endregion

        #region 工具函数


        #endregion

    }
}