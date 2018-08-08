using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using System.Data;
using System.Threading.Tasks;

//using iS3.Core.Serialization;

namespace iS3.Core
{
    #region Copyright Notice
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
    #endregion

    // Summary:
    //      Project: stores all non-graphic infos.
    // Remarks:
    //      (1) Project is in the core of the MVC design pattern.
    //          It is the model, so it knows nothing about the controller
    //          (IMainframe) and the view (IView).
    //      (2) The Project class is one of the core rooms for IS3, there is
    //          only one active project in iS3.
    //      (3) Project data is typically loaded from a database.
    //      (4) PrejectDefinition gives the information such as where the
    //          database is located, how many domains it has, etc.
    //      (5) Project data is stored in domains, such as geology domain,
    //          structure domain, etc.
    //      (6) Project data can be assessed with index, such as layer index.
    //
    public class Project
    {
        public string projectID { get; set; }
        // Summary:
        //     Project definition: gives the information such as where the
        //     database is located, how many domains it has, etc.
        public ProjectDefinition projDef { get; set; }

        // Summary:
        //     Domains of the project, such as geology domain, etc.
        //     The domain is indexed by its name.
        protected Dictionary<string, Domain> _domains;
        public Dictionary<string, Domain> domains
        {
            get { return _domains; }
        }

        public Domain this[string key]
        {
            get { return domains[key]; }
        }

        // Summary: 
        //     Get the first domain of DomainType.
        public Domain getDomain(DomainType type)
        {
            return _domains.Values.FirstOrDefault(x => x.type == type);
        }

        // Summary:
        //      Objects layer index class
        // Remarks:
        //      Objects can be access with a layer ID which is specified
        //      in the DGObjectsDefinition.GISLayerName
       // public Dictionary<string, DGObjects> objsLayerIndex { get; set; }
        public Dictionary<string, DGObjects> objsDefIndex { get; set; }

        public Dictionary<string,List<DGObjects>> objs2DIndex { get; set; }
        public Dictionary<string,List<DGObjects>> objs3DIndex { get; set; }

        public Dictionary<string,IEnumerable<int>> objSelectedIndex { get; set; }
        // Summary:
        //      Objects table index class
        // Remarks:
        //      Objects can be access with a DataSet (DGObjects.RawDataSet).
        //  public Dictionary<DataSet, DGObjects> dataSetIndex { get; set; }

        public Project()
        {
            projDef = null;
            _domains = new Dictionary<string, Domain>();
            objsDefIndex = new Dictionary<string, DGObjects>();
            objSelectedIndex = new Dictionary<string, IEnumerable<int>>();
            objs2DIndex = new Dictionary<string, List<DGObjects>>();
            objs3DIndex = new Dictionary<string, List<DGObjects>>();
        }

        // Summary:
        //     Load project defintion from a XML file
        //
        public bool loadDefinition(string file)
        {
            string filePath = Runtime.dataPath + "\\" +file.Substring(0,file.Length-4)+"\\"+ file;
            if (File.Exists(filePath) == false)
            {
                ErrorReport.Report("Error: defintion file doesn't exist: " + filePath);
                return false;
            }
            StreamReader reader = new StreamReader(filePath);

            // Load root element from xml
            XElement root = XElement.Load(reader);
            if (root.Name != "Project")
            {
                ErrorReport.Report("Error: defintion file root must be <Project>.");
                return false;
            }

            // Load project definition
            projDef = ProjectDefinition.Load(root);
            if (projDef == null)
                return false;

            // get rid of ".xml", and use as default local file path
            string shortName = file.Substring(0, file.Length - 4);

            if (projDef.LocalFilePath == null)
            {
                Runtime.projPath = Runtime.dataPath + "\\" + shortName;
                projDef.LocalFilePath = Runtime.projPath;
            }
            else
                Runtime.projPath = projDef.LocalFilePath;

            if (projDef.LocalTilePath == null)
                projDef.LocalTilePath = Runtime.tilePath;

            // set dataservice path
        //    Globals.iS3Service.DataService.initializeDataService(projDef.DatabaseName1);
            // Load domain definition
            IEnumerable<XElement> nodes = root.Elements("Domain");
            foreach (XElement node in nodes)
            {
                Domain domain = Domain.loadDefinition(node);
                if (domain == null)
                    continue;
                domain.parent = this;
                _domains.Add(domain.name, domain);
            }

            return true;
        }

        public override string ToString()
        {
            string domainStr = "";
            foreach (string name in _domains.Keys)
                domainStr += "'" + name + "',";

            string str = string.Format(
                "Project: ID={0}, Domains={1}",
                projDef == null ? null : projDef.ID,
                domainStr);
            return str;
        }

      

        // Summary:
        //     Synchronize objects on the tree.
        // Remarks:
        //     After synchronization, each tree will have an a DataView of objects.
        //
        public int syncObjectsOnTree(Tree inputTree, bool readSubTree = true)
        {
            int nSync = 0;

            List<Tree> trees = null;
            if (readSubTree == true)
                trees = inputTree.ToList();
            else
            {
                trees = new List<Tree>();
                trees.Add(inputTree);
            }

            foreach (Tree tree in trees)
            {
                if (tree.RefDomainName == null || tree.RefObjsName == null)
                    continue;

                Domain domain = null;
                if (domains.ContainsKey(tree.RefDomainName))
                    domain = domains[tree.RefDomainName];
                if (domain == null)
                    continue;

                DGObjects objs = null;
                if (domain.objsContainer.ContainsKey(tree.RefObjsName))
                    objs = domain.objsContainer[tree.RefObjsName];
                if (objs == null)
                    continue;

                //if (objs.rawDataSet.Tables.Count == 0)
                //    continue;

                //// Open a view on the table, and apply filter and sort rule on the table
                ////
                //DataTable dt = objs.rawDataSet.Tables[0];
                //DataView dv = new DataView(dt, tree.Filter, tree.Sort, DataViewRowState.CurrentRows);
                //tree.ObjectsView = dv;
                tree.RefObjs = objs;
            }
            return nSync;
        }

        // Summary:
        //      Load project from a definition file.
        // Remarks:
        //      It involoved two steps:
        //      (1) Load defintion at first
        //      (2) Load project domain data specifiled in the definition file
        //
        //public static DbContext dbContext;
        public static async Task<Project> load(string definitionFile)
        {
            Project prj = new Project();
            iS3.Core.Globals.project = prj;

            // Load project definition first
            // 
            prj.loadDefinition(definitionFile);

            // Load project data
            foreach (Domain domain in prj.domains.Values)
            {
                // load all objects into domain
                //domain.loadAllObjects(dbContext);
                domain.loadAllObjects();
                // sync objects on the tree
                prj.syncObjectsOnTree(domain.root);
                // build objects index based on layer ID
                // which is specified in the DGObjectsDefinition.GISLayerName
                foreach (var def in domain.objsDefinitions)
                {
                    string defName = def.Key;
                    DGObjects objs = domain.objsContainer[defName];
                    prj.objsDefIndex[defName] = objs;
                    if ((objs.definition.GISLayerName!=null)&&(objs.definition.GISLayerName.Length>0))
                    {
                        if (!prj.objs2DIndex.ContainsKey(objs.definition.GISLayerName))
                        {
                            prj.objs2DIndex[objs.definition.GISLayerName] = new List<DGObjects>();
                        }
                        prj.objs2DIndex[objs.definition.GISLayerName].Add(objs);
                    }
                    if ((objs.definition.Layer3DName != null) && (objs.definition.Layer3DName.Length > 0))
                    {
                        if (!prj.objs3DIndex.ContainsKey(objs.definition.Layer3DName))
                        {
                            prj.objs3DIndex[objs.definition.Layer3DName] = new List<DGObjects>();
                        }
                        prj.objs3DIndex[objs.definition.Layer3DName].Add(objs);
                    }
                }
            }
            //dbContext.Close();

            return prj;
        }

        // Summary:
        //     Object selection event listener (function).
        //     It will set DataTable's IsSelected property according to
        //     object's selection state.
        // Remarks:
        // (1) The IsSelected property is dynamically injected into
        //     DataTable when object data is read from database.
        //     See ReadRawData() function in the 
        //     iS3.Core.Serialization.DbDataLoader class for more information.
        // (2) The DataTable is used to display object data in
        //     the datagrid (class IS3DataGrid). When IsSelected property
        //     is set to true, it will be displayed as selected state 
        //     in the datagrid.
        //
        public void objSelectionChangedListener(object sender,
            ObjSelectionChangedEventArgs e)
        {
            if (sender == this)
                return;

            if (e.addedObjs != null)
            {
                setObjSelectionState(e.addedObjs, true);
            }
            if (e.removedObjs != null)
            {
                setObjSelectionState(e.removedObjs, false);
            }
        }

        // Summary:
        //     Set object selection state.
        // Remarks:
        //     For more info on IsSelected property, 
        //     see remarks of objSelectionChangedListener() function.
        void setObjSelectionState(
            Dictionary<string, IEnumerable<DGObject>> selectedObjs,
            bool isSelected)
        {
            // Method1:
            //     set object selection state through layer index
            //
            //foreach (string layerID in selectedObjs.Keys)
            //{
            //    if (objsLayerIndex.ContainsKey(layerID))
            //    {
            //        DGObjects objs = objsLayerIndex[layerID];
            //        foreach (DGObject obj in selectedObjs[layerID])
            //        {
            //            if (objs.obj2RowView.Keys.Contains(obj))
            //            {
            //                DataRow dr = objs.obj2RowView[obj];
            //                dr.SetField<bool>("IsSelected", isSelected);
            //            }
            //        }
            //    }
            //}

            // Method 2: 
            //     set object selection state from object's parent
            //     Method 2 is robust than method 1 because it bypass layer index.
            //


                //if (objs.Count() == 0)
                //    continue;
                //DGObjects parent = objs.First().parent;
                //foreach (var obj in objs)
                //{
                //    if (parent.obj2RowView.Keys.Contains(obj))
                //    {
                //        DataRow dr = parent.obj2RowView[obj];
                //        dr.SetField<bool>("IsSelected", isSelected);
                //    }
                //}
           // }
        }

        // Summary:
        //     Get selected objects, i.e., objects with IsSelected == true
        // Return value:
        //     A dictionary of selected objects. The key of dictionary is 
        //     layer ID
        // Remarks:
        //     For more info on IsSelected property, 
        //     see remarks of objSelectionChangedListener() function.
        public Dictionary<string, IEnumerable<int>>
            getSelectedObjs()
        {

            return objSelectedIndex; ;
        }
      

        // Summary:
        //     Get selected objects of specified object type 
        //     in the specified domain name.
        // Return value:
        //     A dictionary of selected objects. The key of dictionary is 
        //     layer ID
        // Remarks:
        //     For more info on IsSelected property, 
        //     see remarks of objSelectionChangedListener() function.
        public Dictionary<string, IEnumerable<int>>
            getSelectedObjs(Domain domain, string objType)
        {
            Dictionary<string, IEnumerable<int>> selectedObjsDict =
                new Dictionary<string, IEnumerable<int>>();

            foreach (string objdefName in objSelectedIndex.Keys)
            {
                DGObjectsDefinition definition = objsDefIndex[objdefName].definition;
                if ((objsDefIndex[objdefName].parent.name == domain.name) && (definition.Type == objType))
                {
                    selectedObjsDict.Add(objdefName, objSelectedIndex[objdefName]);
                }

            }

            return selectedObjsDict;
        }

    }
}
