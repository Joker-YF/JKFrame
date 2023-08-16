using System;
using System.Text.RegularExpressions;
using JK.Log;
using UnityEngine;
using static JKFrame.JKFrameSetting;

namespace JKFrame
{
    public static class JKLog
    {
        static JKLog()
        {
            Init();
        }
        public static void Init()
        {
            if (JKFrameRoot.Setting != null)
            {
                Init(JKFrameRoot.Setting.LogConfig);
            }
        }
        public static void Init(LogSetting logSetting)
        {
#if ENABLE_LOG
            JK.Log.JKLog.Init(LoggerType.Unity, logSetting.writeTime, logSetting.writeThreadID, logSetting.writeTrace, logSetting.enableSave, Application.persistentDataPath + logSetting.savePath + "/", logSetting.customSaveFileName, logSetting.saveLogTypes, 5);
#endif
        }

        public static void Log(string msg)
        {
# if ENABLE_LOG
            JK.Log.JKLog.Log(msg);
#endif
        }
        public static void Warning(string msg)
        {
#if ENABLE_LOG
            JK.Log.JKLog.Warning(msg);
#endif
        }
        public static void Error(string msg)
        {
#if ENABLE_LOG
            JK.Log.JKLog.Error(msg);
#endif
        }
        public static void Succeed(string msg)
        {
#if ENABLE_LOG
            JK.Log.JKLog.Succeed(msg);
#endif
        }

        public static void Close()
        {
#if ENABLE_LOG
            JK.Log.JKLog.Close();
#endif
        }
        
        #if UNITY_EDITOR
        [UnityEditor.Callbacks.OnOpenAssetAttribute(0)]
        static bool OnOpenAsset(int instanceID, int line)
        {
            var stackTrace = GetStackTrace();

            //非JKLog日志则跳出
            if (string.IsNullOrEmpty(stackTrace) || !stackTrace.Contains("JK.Log.JKLog")) return false;
            //正则表达式匹配
            string pattern = @"\(at Assets(/.+\.cs):(\d+)\)";
            var match = Regex.Match(stackTrace, pattern);
            while (match.Success)
            {
                var path = match.Groups[1].Value;
                var l = match.Groups[2].Value;
                if (!path.Contains("Log.cs"))
                {
                    var fullPath = Application.dataPath + path;
                    if (Int32.TryParse(l, out var row))
                    {
                        UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(fullPath, row);
                        return true;
                    }
                }

                match = match.NextMatch();
            }

            return false;
        }

        /// <summary>
        /// 获取当前日志窗口选中的日志的堆栈信息
        /// </summary>
        /// <returns>堆栈文本</returns>
        private static string GetStackTrace()
        {
            var consoleWindowType = typeof(UnityEditor.EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow");
            var consoleWindowFieldInfo = consoleWindowType.GetField("ms_ConsoleWindow",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            if (consoleWindowFieldInfo != null)
            {
                var consoleWindow = consoleWindowFieldInfo.GetValue(null) as UnityEditor.EditorWindow;

                if (consoleWindow != UnityEditor.EditorWindow.focusedWindow) return null;

                var activeTextFieldInfo = consoleWindowType.GetField(
                    "m_ActiveText", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

                if (activeTextFieldInfo != null) return activeTextFieldInfo.GetValue(consoleWindow).ToString();
            }

            return null;
        }
#endif
    }
}

