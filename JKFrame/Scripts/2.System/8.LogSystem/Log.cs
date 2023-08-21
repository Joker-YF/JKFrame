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
    }
}