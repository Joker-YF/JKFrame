using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;
using System.Linq;
using System;
using JKFrame;
namespace JKFrame.Editor
{
    public class PoolSystemViewer : EditorWindow
    {
        private class LayerData
        {
            public string layerName;
            public int count;
        }
        private List<LayerData> objectLayerDatas;
        private List<LayerData> gameObjectLayerDatas;
        Dictionary<string, int> objectLayerDataIndexDic;
        Dictionary<string, int> gameObjectLayerDataIndexDic;

        private ListView objectLayerListView;
        private ListView gameObjectLayerListView;
        [MenuItem("JKFrame/PoolSystemViewer")]
        public static void ShowExample()
        {
            PoolSystemViewer wnd = GetWindow<PoolSystemViewer>();
            wnd.titleContent = new GUIContent("PoolSystemViewer");
        }

        public void CreateGUI()
        {
            objectLayerDatas = new List<LayerData>();
            gameObjectLayerDatas = new List<LayerData>();
            objectLayerDataIndexDic = new Dictionary<string, int>();
            gameObjectLayerDataIndexDic = new Dictionary<string, int>();

            VisualElement root = rootVisualElement;
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/JKFrame/Editor/Windows/PoolSystem/PoolSystemViewer.uxml");
            VisualElement labelFromUXML = visualTree.Instantiate();
            root.Add(labelFromUXML);

            if (EditorApplication.isPlayingOrWillChangePlaymode == false)
            {
                return;
            }
            #region 绘制普通实例
            objectLayerListView = root.Q<ListView>("LayerList");
            objectLayerListView.makeItem = MakeListItem;
            objectLayerListView.bindItem = BindClassListItem;
            objectLayerListView.itemsSource = objectLayerDatas;
            #endregion

            #region 绘制游戏物体
            gameObjectLayerListView = root.Q<ListView>("GameObjectLayerList");
            gameObjectLayerListView.makeItem = MakeListItem;
            gameObjectLayerListView.bindItem = BindGameObjectListItem;
            gameObjectLayerListView.itemsSource = gameObjectLayerDatas;

            #endregion

            #region 获取PoolSystem现有数据进行绘制

            Dictionary<string, ObjectPoolData> objectPoolData = PoolSystem.GetObjectLayerDatas();
            Dictionary<string, GameObjectPoolData> gameObjectPoolData = PoolSystem.GetGameObjectLayerDatas();
            foreach (var item in objectPoolData)
            {
                SetLayerData(objectLayerDataIndexDic, objectLayerDatas, item.Key, item.Value.PoolQueue.Count);
            }
            foreach (var item in gameObjectPoolData)
            {
                SetLayerData(gameObjectLayerDataIndexDic, gameObjectLayerDatas, item.Key, item.Value.PoolQueue.Count);
            }
            #endregion

            #region 绑定编辑器事件
            JKFrameRoot.EditorEventModule.AddEventListener<Action<string, int>>(nameof(OnPushObject), OnPushObject);
            JKFrameRoot.EditorEventModule.AddEventListener<Action<string, int>>(nameof(OnGetObject), OnGetObject);
            JKFrameRoot.EditorEventModule.AddEventListener<Action<string, int>>(nameof(OnPushGameObject), OnPushGameObject);
            JKFrameRoot.EditorEventModule.AddEventListener<Action<string, int>>(nameof(OnGetGameObject), OnGetGameObject);
            JKFrameRoot.EditorEventModule.AddEventListener<Action<string>>(nameof(OnClearnObject), OnClearnObject);
            JKFrameRoot.EditorEventModule.AddEventListener<Action<string>>(nameof(OnClearGameObject), OnClearGameObject);
            JKFrameRoot.EditorEventModule.AddEventListener(nameof(OnClearAllObject), OnClearAllObject);
            JKFrameRoot.EditorEventModule.AddEventListener(nameof(OnClearAllGameObject), OnClearAllGameObject);
            JKFrameRoot.EditorEventModule.AddEventListener(nameof(OnClearAll), OnClearAll);
            JKFrameRoot.EditorEventModule.AddEventListener<Action<string, int>>(nameof(OnInitObjectPool), OnInitObjectPool);
            JKFrameRoot.EditorEventModule.AddEventListener<Action<string, int>>(nameof(OnInitGameObjectPool), OnInitGameObjectPool);
            #endregion
            objectLayerListView.RefreshItems();
            gameObjectLayerListView.RefreshItems();

        }

        private VisualElement MakeListItem()
        {
            VisualTreeAsset template = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/JKFrame/Editor/Windows/PoolSystem/PoolSystemItem.uxml");
            return template.Instantiate();
        }
        private void BindClassListItem(VisualElement visualElement, int index)
        {
            Label name = visualElement.Q<Label>("Name");
            Label count = visualElement.Q<Label>("Count");
            name.text = objectLayerDatas[index].layerName;
            count.text = objectLayerDatas[index].count.ToString();
        }

        private void BindGameObjectListItem(VisualElement visualElement, int index)
        {
            Label name = visualElement.Q<Label>("Name");
            Label count = visualElement.Q<Label>("Count");
            name.text = gameObjectLayerDatas[index].layerName;
            count.text = gameObjectLayerDatas[index].count.ToString();
        }

        public void OnPushObject(string layerName, int addNum = 1)
        {
            SetLayerData(objectLayerDataIndexDic, objectLayerDatas, layerName, addNum);
            objectLayerListView.RefreshItems();
        }
        public void OnGetObject(string layerName, int removeNum = 1)
        {
            OnPushObject(layerName, -removeNum);
        }
        public void OnPushGameObject(string layerName, int addNum = 1)
        {
            SetLayerData(gameObjectLayerDataIndexDic, gameObjectLayerDatas, layerName, addNum);
            gameObjectLayerListView.RefreshItems();
        }
        public void OnGetGameObject(string layerName, int removeNum = 1)
        {
            OnPushGameObject(layerName, -removeNum);
        }
        private void SetLayerData(Dictionary<string, int> layerIndexDic, List<LayerData> layerDatas, string layerName, int addNum = 1)
        {
            if (layerIndexDic.TryGetValue(layerName, out int index))
            {
                LayerData layerData = layerDatas[index];
                layerData.count += addNum;
            }
            else
            {
                layerIndexDic.Add(layerName, layerDatas.Count);
                layerDatas.Add(new LayerData() { layerName = layerName, count = addNum });
            }
        }
        private void OnClearnObject(string layerName)
        {
            if (objectLayerDataIndexDic.TryGetValue(layerName, out int index))
            {
                objectLayerDatas.RemoveAt(index);
                objectLayerDataIndexDic.Remove(layerName);
                objectLayerListView.RefreshItems();
            }
        }
        private void OnClearAllObject()
        {
            objectLayerDatas.Clear();
            objectLayerDataIndexDic.Clear();
            gameObjectLayerListView.RefreshItems();
        }
        private void OnClearAllGameObject()
        {
            gameObjectLayerDatas.Clear();
            gameObjectLayerDataIndexDic.Clear();
            gameObjectLayerListView.RefreshItems();
        }
        private void OnClearGameObject(string layerName)
        {
            if (gameObjectLayerDataIndexDic.TryGetValue(layerName, out int index))
            {
                gameObjectLayerDatas.RemoveAt(index);
                gameObjectLayerDataIndexDic.Remove(layerName);
                gameObjectLayerListView.RefreshItems();
            }
        }
        private void OnClearAll()
        {
            OnClearAllObject();
            OnClearAllGameObject();
        }
        private void OnInitObjectPool(string layerName, int defaultCapacity = 0)
        {
            SetLayerDataOnInitPool(objectLayerDataIndexDic, objectLayerDatas, layerName, defaultCapacity);
            gameObjectLayerListView.RefreshItems();
        }
        private void OnInitGameObjectPool(string layerName, int defaultCapacity = 0)
        {
            SetLayerDataOnInitPool(gameObjectLayerDataIndexDic, gameObjectLayerDatas, layerName, defaultCapacity);
            gameObjectLayerListView.RefreshItems();
        }
        private void SetLayerDataOnInitPool(Dictionary<string, int> layerIndexDic, List<LayerData> layerDatas, string layerName, int defaultCapacity = 0)
        {
            if (defaultCapacity == 0) return;

            if (layerIndexDic.TryGetValue(layerName, out int index))
            {
                LayerData layerData = layerDatas[index];
                int addNum = defaultCapacity - layerData.count;
                layerData.count += addNum;
            }
            else
            {
                layerIndexDic.Add(layerName, layerDatas.Count);
                layerDatas.Add(new LayerData() { layerName = layerName, count = defaultCapacity });
            }


        }
    }
}
