using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityCore.ModelSys;
using System;

namespace UnityCore.MessageSys
{
    public class SendUnityLayerMessage : iS3UnityMessage
    {
        public override MessageType type { get { return MessageType.SendUnityLayer; } }

        public UnityLayer MyUnityLayer
        {
            get
            {
                lock (obj)
                {
                    if (null == myUnityLayer)
                    {
                        myUnityLayer = new UnityLayer();
                    }
                    return myUnityLayer;
                }
                
            }

            set
            {
                myUnityLayer = value;
            }
        }
        //锁住
        UnityEngine.Object obj = new UnityEngine.Object();
        UnityLayer myUnityLayer;
        private Dictionary<string, string> objWithParent = new Dictionary<string, string>();
        private Dictionary<string, int> objWithLayer = new Dictionary<string, int>();

        public override string SerializeObject()
        {
            //清空objWithParent和objWithLayer
            objWithParent.Clear();
            objWithLayer.Clear();

            UnityTreeModel treeModel = MyUnityLayer.UnityLayerModel;
            
            for (int i = 0; i < treeModel.childs.Count; i++)
            {
                HandleObj(treeModel, treeModel.childs[i]);
            }
            //写入父子关系和节点关系
            string strValue = string.Empty;
            //添加第一组特殊数据
            strValue += treeModel.Name + "," + treeModel.layer+"@";
            foreach (KeyValuePair<string,string> item in objWithParent)
            {
                strValue += string.Format("{0},{1};",item.Key,item.Value);
            }
            strValue += "@";
            foreach (KeyValuePair<string, int> item in objWithLayer)
            {
                strValue += string.Format("{0},{1};",item.Key,item.Value);
            }
            return strValue;
        }
        public override void DeSerializeObject(string message)
        {
            //清空objWithParent和objWithLayer
            objWithParent.Clear();
            objWithLayer.Clear();

            //先解析特殊数据和objWithParent和objWithLayer
            String[] strs = message.Split(new String[] {"@"},StringSplitOptions.RemoveEmptyEntries);
            if (strs.Length<3)
            {
                Debug.Log("设置出错");
                return;
            }
            //特殊处理
            UnityTreeModel treeModel = MyUnityLayer.UnityLayerModel;
            String[] specialStr = strs[0].Split(new String[] {","},StringSplitOptions.RemoveEmptyEntries);
            treeModel.Name = specialStr[0];
            treeModel.layer = int.Parse(specialStr[1]);


            //第一个是父子关系
            String[] parentRealtive = strs[1].Split(new String[] {";"},StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < parentRealtive.Length; i++)
            {
                String[] twoParams = parentRealtive[i].Split(new String[] {","},StringSplitOptions.RemoveEmptyEntries);
                if (twoParams.Length>=2)
                {
                    if (!objWithParent.ContainsKey(twoParams[0]))
                    {
                        objWithParent.Add(twoParams[0], twoParams[1]);
                    }
                }
                else
                {
                    Debug.Log("设置出错");
                    return;
                }
            }
            //第二个是节点关系
            String[] layerRealtive = strs[2].Split(new String[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < layerRealtive.Length; i++)
            {
                String[] twoParams = layerRealtive[i].Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (twoParams.Length >= 2)
                {
                    if (!objWithLayer.ContainsKey(twoParams[0]))
                    {
                        objWithLayer.Add(twoParams[0], int.Parse(twoParams[1]));
                    }
                }
                else
                {
                    Debug.Log("设置出错");
                    return;
                }
            }

            //开始填充
            SetLayerAndChild(treeModel);
        }

        //反解析设置层和子物体
        public void SetLayerAndChild(UnityTreeModel parent)
        {
            List<String> strs = FindAllValueByStr(parent.Name);
            if (strs.Count<=0)
            {
                return;
            }
            else
            {
                for (int i = 0; i < strs.Count; i++)
                {
                    UnityTreeModel model = new UnityTreeModel();
                    model.Name = strs[i];
                    if (objWithLayer.ContainsKey(model.Name))
                    {
                        model.layer = objWithLayer[model.Name];
                    }
                    parent.childs.Add(model);
                    SetLayerAndChild(model);
                }
            }
        }


        private List<String> FindAllValueByStr(string value)
        {
            List<String> strs = new List<string>();
            foreach (KeyValuePair<string,string> item in objWithParent)
            {
                if (item.Value == value)
                {
                    strs.Add(item.Key);
                }
            }
            return strs;
        }
        //处理单行obj
        public void HandleObj(UnityTreeModel parent,UnityTreeModel child)
        {
            AddLayer(parent.Name, parent.layer);
            AddLayer(child.Name, child.layer);
            AddParent(child.Name, parent.Name);
            if (child.layer == LayerMask.NameToLayer("UnityLayer"))
            {
                //应该继续往下走
                if (null == child.childs && child.childs.Count<=0)
                {
                    return;
                }
                else
                {
                    List<UnityTreeModel> newChild = child.childs;
                    for (int i = 0; i < newChild.Count; i++)
                    {
                        HandleObj(child,newChild[i]);
                    }
                }
            }
        }

        private void AddLayer(string name,int layer)
        {
            if (!objWithLayer.ContainsKey(name))
            {
                objWithLayer.Add(name,layer);
            }
        }
        private void AddParent(string childName,string parentName)
        {
            if (!objWithParent.ContainsKey(childName))
            {
                objWithParent.Add(childName, parentName);
            }
        }
    }
}