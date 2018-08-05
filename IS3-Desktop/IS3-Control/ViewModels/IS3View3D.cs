using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Controls;
using System.Collections;
using System.IO;

using IS3.Core;
using IS3.Core.Geometry;
using IS3.Core.Graphics;

using UnityCore.MessageSys;
namespace IS3.Control
{
    //************************  Notice  **********************************
    //** This file is part of iS3
    //**
    //** Copyright (c) 2015 Tongji University iS3 Team. All rights reserved.
    //**
    //** This library is free software; you can redistribute it and/or
    //** modify it under the terms of the GNU Lesser General Public
    //** License as published by the Free Software Foundation; either
    //** version 3 of the License, or (at your option) any later version.
    //**
    //** This library is distributed in the hope that it will be useful,
    //** but WITHOUT ANY WARRANTY; without even the implied warranty of
    //** MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    //** Lesser General Public License for more details.
    //**
    //** In addition, as a special exception,  that plugins developed for iS3,
    //** are allowed to remain closed sourced and can be distributed under any license .
    //** These rights are included in the file LGPL_EXCEPTION.txt in this package.
    //**
    //**************************************************************************
    public class IS3View3D : IS3ViewBase,IView3D
    {
        #region IView interface
        public ViewType type { get { return ViewType.General3DView; } }

        public ViewBaseType baseType
        {
            get { return ViewBaseType.D3; }
        }

        public UserControl parent => throw new NotImplementedException();

        public async Task initializeView() {
            await Load3DScene();
        }

        public void onClose() { }

        public void highlightObject(DGObject obj, bool on = true)
        {
            //if (obj == null || obj.parent == null || (obj.parent.definition.Has3D==false))
            //    return;

            SetObjSelectStateMessage message = new SetObjSelectStateMessage();
            message.path = "iS3Project/Borehole/" + obj.id;
            message.iSSelected = on;
            ExcuteCommand(message);
        }
        public void highlightObjects(IEnumerable<DGObject> objs, bool on = true)
        {
            if (objs == null)
                return;
            foreach (DGObject obj in objs)
                highlightObject(obj, on);
        }
        public void highlightObjects(IEnumerable<DGObject> objs,
            string layerID, bool on = true)
        { }
        public void highlightAll(bool on = true) { }

        public void objSelectionChangedListener(object sender,
            ObjSelectionChangedEventArgs e)
        {
            if (sender == this)
                return;


        }
        public event EventHandler<DrawingGraphicsChangedEventArgs>
            drawingGraphicsChangedTrigger;
        public event EventHandler<DGObjectsSelectionChangedEventArgs> DGObjectsSelectionChangedTriggerOuter;
        public event EventHandler<ObjSelectionChangedEventArgs> objSelectionChangedTriggerOuter;
        public event EventHandler<DGObjectsSelectionChangedEventArgs> DGObjectsSelectionChangedTriggerInner;
        public event EventHandler<ObjSelectionChangedEventArgs> objSelectionChangedTriggerInner;
        #endregion


        U3DPlayerAxLib.U3DPlayerControl _u3dPlayerControl;

        public IS3View3D(UserControl parent,
            U3DPlayerAxLib.U3DPlayerControl u3dPlayerControl)
        {
            _parent = parent;
            _u3dPlayerControl = u3dPlayerControl;
        }
        public bool IsValidFileName(string filename)
        {

            if (filename == null || filename.Count() == 0)
                return false;
            else
                return true;
        }

        int _count = 0;
        public EventHandler<IS3ToUnityArgs> sendMessageEventHandler;
        public EventHandler<UnityToIS3Args> receiveMessageHandler;


        public async Task Load3DScene()
        {
            // check file exists
            string filePath = _prj.projDef.LocalFilePath + "\\"
                + _eMap.LocalMapFileName;
            if (File.Exists(filePath))
            {
                _u3dPlayerControl.LoadScence(filePath);
            }
            _u3dPlayerControl.UnityCall += new U3DPlayerAxLib.U3DPlayerControl.ExternalCallHandler(_u3dPlayerControl_UnityCall);
            receiveMessageHandler += new EventHandler<UnityToIS3Args>(ReceiveMessageListener);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _u3dPlayerControl_UnityCall(object sender, AxUnityWebPlayerAXLib._DUnityWebPlayerAXEvents_OnExternalCallEvent e)
        {
            try
            {
                string message = e.value;
                string[] list = message.Split('"');
                for (int i = 0; i < list.Length; i++)
                {
                    if (list[i].StartsWith("@"))
                    {
                        iS3UnityMessage myMessage = UnityCore.MessageSys. MessageConverter.DeSerializeMessage(list[i]);
                        switch (myMessage.type)
                        {
                            case MessageType.SendUnityLayer:
                                break;
                            case MessageType.SetObjSelectState:
                                SetObjSelectStateMessage _message = myMessage as SetObjSelectStateMessage;
                                string _path = _message.path;
                                int id = int.Parse(_path.Split('/')[_path.Split('/').Length - 1]);
                                string layer3d = _path.Substring(0, _path.Length - 1 - (id).ToString().Length);
                                bool isSelected = _message.iSSelected;
                                if (prj.objs3DIndex.ContainsKey(layer3d))
                                {
                                    foreach (DGObjects objs in prj.objs3DIndex[layer3d])
                                    {
                                        ShowSelect(objs,id, isSelected);
                                    }
  
                                }
                                break;
                            case MessageType.SetObjShowState:
                                break;
                            default: break;
                        }
                    }
                }
            }
            catch { }

        }
        DGObject lastOne = null;
        public async Task ShowSelect(DGObjects objs, int id, bool isselected)
        {
            try
            {
                List<DGObject> addedObjs = new List<DGObject>();
                List<DGObject> removedObjs = new List<DGObject>();
                DGObjectRepository repository = DGObjectRepository.Instance(
                                     Globals.project.projDef.ID, objs.parent.name, objs.definition.Type);
                DGObject obj = await repository.Retrieve(id);
                obj.parent = objs;
                addedObjs.Add(obj);
                if (lastOne != null)
                {
                    removedObjs.Add(lastOne);
                }
                lastOne = obj;
                string layerName = objs.definition.Name;

                if (objSelectionChangedTriggerOuter != null)
                {
                    Dictionary<string, IEnumerable<DGObject>> addedObjsDict = null;
                    Dictionary<string, IEnumerable<DGObject>> removedObjsDict = null;
                    if (addedObjs.Count > 0)
                    {
                        addedObjsDict = new Dictionary<string, IEnumerable<DGObject>>();
                        addedObjsDict[layerName] = addedObjs;
                    }
                    if (removedObjs.Count > 0)
                    {
                        removedObjsDict = new Dictionary<string, IEnumerable<DGObject>>();
                        removedObjsDict[layerName] = removedObjs;
                    }
                    ObjSelectionChangedEventArgs args =
                        new ObjSelectionChangedEventArgs();
                    args.addedObjs = addedObjsDict;
                    args.removedObjs = removedObjsDict;
                    if (objSelectionChangedTriggerOuter != null)
                    {
                        objSelectionChangedTriggerOuter(this, args);
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }
        public void ExcuteCommand(iS3UnityMessage message)
        {
            ExcuteCommand(UnityCore.MessageSys.MessageConverter.SerializeMessage(message));
        }
        public void ExcuteCommand(string command)
        {
            _u3dPlayerControl.SendMessage("Main Camera", "ReceiveMessage", command);
        }

        private void ReceiveMessageListener(object sender, UnityToIS3Args args)
        {
            //switch (args.methodType)
            //{
            //    case UnityToIS3Method.LoadComplete: break;
            //    case UnityToIS3Method.Select:
            //        SelectObjByName(args.info);
            //        break;
            //    default: break;
            //}
        }


        #region  receive function
        public void SelectObjByName(string message)
        {
            try
            {
                string nameInfo = message.Split(',')[0];
                bool _state = (message.Split(',')[1].ToUpper()) == "TRUE" ? true : false;
                DGObject obj = TurnNameToObj(nameInfo);
                if (obj != null && objSelectionChangedTriggerOuter != null)
                {
                    ObjSelectionChangedEventArgs args = new ObjSelectionChangedEventArgs();
                    if (_state)
                    {
                        args.addedObjs = new Dictionary<string, IEnumerable<DGObject>>();
                        List<DGObject> objs = new List<DGObject>() { obj };
                        args.addedObjs.Add(obj.parent.definition.GISLayerName, objs);
                    }
                    else
                    {
                        args.removedObjs = new Dictionary<string, IEnumerable<DGObject>>();
                        List<DGObject> objs = new List<DGObject>() { obj };
                        args.removedObjs.Add(obj.parent.definition.GISLayerName, objs);
                    }
                    objSelectionChangedTriggerOuter(this, args);
                }
            }
            catch { }

        }
        public DGObject TurnNameToObj(string nameInfo)
        {
            try
            {
                //string[] nameList = nameInfo.Split('+');
                //string projectName = nameList[0];
                //string domainName = nameList[1];
                //string objDefName = nameList[2];
                //string objName = nameList[3];
                //DGObject obj = Globals.project.domains[domainName].objsContainer[objDefName][objName];
                //return obj;
                return null;
            }
            catch
            {
                return null;
            }
        }
        #endregion
        #region send function
        public string TurnObjToName(DGObject obj)
        {
            string result = obj.name;
            result = obj.parent.definition.Name + "+" + result;
            result = obj.parent.parent.name + "+" + result;
            result = Globals.project.projDef.ID + "+" + result;
            return result;
        }

        public int syncObjects(string layerID, List<DGObject> objs)
        {
            return 0;
        }

        public void load()
        {
           
        }


        public void DGObjectsSelectionChangedListenerOuter(object sender, DGObjectsSelectionChangedEventArgs e)
        {

        }

        public void objSelectionChangedListenerOuter(object sender, ObjSelectionChangedEventArgs e)
        {
            if (e.addedObjs != null)
            {
                foreach (string layerID in e.addedObjs.Keys)
                    highlightObjects(e.addedObjs[layerID], true);

            }
            if (e.removedObjs != null)
            {
                foreach (string layerID in e.removedObjs.Keys)
                    highlightObjects(e.removedObjs[layerID], false);
            }
        }

        public void DGObjectsSelectionChangedListenerInner(object sender, DGObjectsSelectionChangedEventArgs e)
        {
           
        }

        public void objSelectionChangedListenerInner(object sender, ObjSelectionChangedEventArgs e)
        {
           
        }




        #endregion
    }

    #region 方法枚举
    public enum IS3ToUnityMethod
    {
        SetObjShowByName,
        SetObjShowByType,
        SetObjSelectByName,
        SetObjSelectByType,
        SetAllObjSelectState,
        SetObjPosByName,
        SetObjPosByType,
        MoveObjPosByName,
        MoveObjPosByType,
        QueryPosByName
    }
    public enum UnityToIS3Method
    {
        LoadComplete,
        Select,
    }
    #endregion
    #region 事件定义
    /// <summary>
    /// Reveive Message From Unity Event
    /// </summary>
    public class UnityToIS3Args : EventArgs
    {
        public UnityToIS3Method methodType;
        public string info;
    }
    /// <summary>
    /// unity success load
    /// </summary>
    public class IS3ToUnityArgs : EventArgs
    {
        public string obj { get; set; }
        public IS3ToUnityMethod method { get; set; }
        public string para { get; set; }
    }
    #endregion
}
