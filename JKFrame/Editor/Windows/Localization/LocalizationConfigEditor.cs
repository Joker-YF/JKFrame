using OfficeOpenXml;
using Sirenix.OdinInspector.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

// [CustomEditor(typeof(LocalizationConfig))]
public class LocalizationConfigEditor : OdinEditor
{
    private LocalizationConfig config => (LocalizationConfig)target;
    private VisualElement root;
    public override VisualElement CreateInspectorGUI()
    {
        root = new VisualElement();
        IMGUIContainer container = new IMGUIContainer(() =>
        {
            base.DrawDefaultInspector();
        });
        root.Add(container);

        Button importExcelButton = new Button(ImportExcelButtonClick);
        importExcelButton.text = "导入Excel";
        Button saveExcelButton = new Button(SaveExcelButtonClick);
        saveExcelButton.text = "导出Excel";
        root.Add(importExcelButton);
        root.Add(saveExcelButton);
        return root;
    }

    private void ImportExcelButtonClick()
    {
        string excelFilePath = EditorUtility.OpenFilePanel("选择Excel文件", Application.dataPath, "");
    }
    private void SaveExcelButtonClick()
    {
        string excelFilePath = EditorUtility.SaveFilePanel("保存Excel文件", Application.dataPath, "newExcel", "xlsx");
        Dictionary<Type, ExcelWorksheet> sheetDic = new Dictionary<Type, ExcelWorksheet>();
        File.Delete(excelFilePath); // 确保删除
        FileStream fileStream = new FileStream(excelFilePath, FileMode.CreateNew);
        // 第一行为参数类型
        // 第二行为表头
        using (ExcelPackage excelPackage = new ExcelPackage(fileStream))
        {
            // 按照不同的类型建表
            foreach (KeyValuePair<string, Dictionary<LanguageType, LocalizationDataBase>> key in config.config)
            {
                foreach (KeyValuePair<LanguageType, LocalizationDataBase> keyDic in key.Value)
                {
                    ExcelWorksheet sheet = GetSheet(keyDic.Value.GetType(), excelPackage, sheetDic);

                }
            }
            ExcelWorksheet mainSheet = excelPackage.Workbook.Worksheets.Add("Sheet1");
            mainSheet.Cells[1, 1].Value = "Key";
        }
    }

    private ExcelWorksheet GetSheet(Type type, ExcelPackage excelPackage, Dictionary<Type, ExcelWorksheet> sheetDic)
    {
        if (!sheetDic.TryGetValue(type, out ExcelWorksheet excelWorksheet))
        {
            excelWorksheet = excelPackage.Workbook.Worksheets.Add(type.Name);
            sheetDic.Add(type, excelWorksheet);
        }
        return excelWorksheet;
    }
}
