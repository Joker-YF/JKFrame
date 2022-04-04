using UnityEditor;
using UnityEngine;

namespace JKFrame
{
    public class JKMenuItem
    {
        [MenuItem("JKFrame/打开存档路径")]
        public static void OpenArchivedDirPath()
        {
            EditorUtility.RevealInFinder(Application.persistentDataPath);
        }
        [MenuItem("JKFrame/打开框架文档")]
        public static void OpenDoc()
        {
            Application.OpenURL("http://www.yfjoker.com/JKFrame/index.html");
        }
    }
}
