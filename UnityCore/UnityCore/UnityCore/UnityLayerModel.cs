using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UnityCore.ModelSys
{
    public class UnityLayer
    {
        public string rootName = "iS3Project";
        public UnityTreeModel UnityLayerModel { get; set; }
        public UnityLayer()
        {
            if (null == UnityLayerModel)
            {
                UnityLayerModel = new UnityTreeModel();
                UnityLayerModel.Name = "iS3Project";
                UnityLayerModel.layer = LayerMask.NameToLayer("UnityLayer");
                Transform curModel = GameObject.Find("iS3Project").transform;
                if (null == UnityLayerModel.childs)
                {
                    UnityLayerModel.childs = new List<UnityTreeModel>();
                }
                for (int i = 0; i < curModel.childCount; i++)
                {
                    GameObject obj = curModel.GetChild(i).gameObject;
                    string strLog = "iS3Project"+"/"+obj.name;
                    UnityTreeModel treeModel = new UnityTreeModel();
                    UnityLayerModel.childs.Add(treeModel);
                    SetCatalog(strLog,treeModel);
                }
            }
        }
        void SetCatalog(string curObj,UnityTreeModel treeModel)
        {
            GameObject curModel = GameObject.Find(curObj);
            treeModel.Name = curModel.name;
            treeModel.layer = curModel.layer;
            if (curModel.transform.childCount > 0)
            {
                if (null == treeModel.childs)
                {
                    treeModel.childs = new List<UnityTreeModel>();
                }
                for (int i = 0; i < curModel.transform.childCount; i++)
                {
                    UnityTreeModel childModel = new UnityTreeModel();
                    treeModel.childs.Add(childModel);
                    GameObject child = curModel.transform.GetChild(i).gameObject;
                    string str = curObj + "/"+child.name;
                    SetCatalog(str,childModel);
                }
            }
        }
    }
    public class UnityTreeModel
    {
        public string Name { get; set; }
        public int layer { get; set; }
        public List<UnityTreeModel> childs { get; set; }
    }
    
}