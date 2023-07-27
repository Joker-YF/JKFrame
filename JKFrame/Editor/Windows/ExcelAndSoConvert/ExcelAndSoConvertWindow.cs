using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace JKFrame.Editor
{
    public class ExcelAndSoConvertWindow : OdinEditorWindow
    {
        public int num;
        public ExcelAndSoConvertSetting setting;     // 通过面板拖拽赋值
        public VisualTreeAsset editorUIAsset;   // 通过面板拖拽赋值
        [MenuItem("JKFrame/Excel和SO互转")]
        public static void ShowExample()
        {
            ExcelAndSoConvertWindow wnd = GetWindow<ExcelAndSoConvertWindow>();
            wnd.titleContent = new GUIContent("Excel和SO互转");
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            root.Add(new IMGUIContainer(base.OnGUI));
            root.Add(editorUIAsset.Instantiate());
        }

        protected override object GetTarget()
        {
            return setting;
        }
    }
}
