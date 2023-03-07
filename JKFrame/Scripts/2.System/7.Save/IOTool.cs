using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace JKFrame
{
    public static class IOTool
    {
        private static BinaryFormatter binaryFormatter = new BinaryFormatter();

        /// <summary>
        /// 保存Json
        /// </summary>
        /// <param name="jsonString">Json的字符串</param>
        /// <param name="path">路径</param>
        public static void SaveJson(string jsonString, string path)
        {
            File.WriteAllText(path, jsonString);
        }

        /// <summary>
        /// 读取Json为指定的类型对象
        /// </summary>
        public static T LoadJson<T>(string path) where T : class
        {
            if (!File.Exists(path))
            {
                return null;
            }
            return JsonUtility.FromJson<T>(File.ReadAllText(path));
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="saveObject">保存的对象</param>
        /// <param name="path">保存的路径</param>
        public static void SaveFile(object saveObject, string path)
        {
            FileStream f = new FileStream(path, FileMode.OpenOrCreate);
            // 二进制的方式把对象写进文件
            binaryFormatter.Serialize(f, saveObject);
            f.Dispose();
        }

        /// <summary>
        /// 加载文件
        /// </summary>
        /// <typeparam name="T">加载后要转为的类型</typeparam>
        /// <param name="path">加载路径</param>
        public static T LoadFile<T>(string path) where T : class
        {
            if (!File.Exists(path))
            {
                return null;
            }
            FileStream file = new FileStream(path, FileMode.Open);
            // 将内容解码成对象
            T obj = (T)binaryFormatter.Deserialize(file);
            file.Dispose();
            return obj;
        }


    }
}

