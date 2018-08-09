﻿using System;
using System.Collections.Generic;
using System.Xml.Linq;
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
    //     objects definition
    // Remarks:
    //     Object non-graphic infos are usually stored in tables of a database.
    //     Object graphic infos are usually stored in GIS layers.
    //     These infos are defined in objects definition.
    //
    public class DGObjectsDefinition
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public bool HasGeometry { get; set; }
        public string GISLayerName { get; set; }
        public string ConditionSQL { get; set; }
        public string OrderSQL { get; set; }
        public bool Has3D { get; set; }
        public string Layer3DName { get; set; }

        public static DGObjectsDefinition LoadDefinition(XElement root)
        {
            DGObjectsDefinition def = new DGObjectsDefinition();
            XAttribute attr;

            def.Type = root.Name.ToString();

            attr = root.Attribute("Name");
            if (attr != null)
                def.Name = (string)attr;
            attr = root.Attribute("HasGeometry");
            if (attr != null)
                def.HasGeometry = (bool)attr;
            attr = root.Attribute("GISLayerName");
            if (attr != null)
                def.GISLayerName = (string)attr;

            attr = root.Attribute("ConditionSQL");
            if (attr != null)
                def.ConditionSQL = (string)attr;
            attr = root.Attribute("OrderSQL");
            if (attr != null)
                def.OrderSQL = (string)attr;

            attr = root.Attribute("Has3D");
            if (attr != null)
                def.Has3D = (bool)attr;

            attr = root.Attribute("Layer3DName");
            if (attr != null)
                def.Layer3DName = (string)attr;

            return def;
        }

        public override string ToString()
        {
            string str = string.Format(
                "Object definition: Type={0}, Name={1}, HasGeometry={2}, GISLayerName={3}," +
                "ConditionSQL={4}, OrderSQL={5}",
                Type, Name, HasGeometry, GISLayerName, ConditionSQL, OrderSQL);
            return str;
        }
    }

    // Summary:
    //     DGObjects: a collection of DGObject
    // Remarks:
    //   1.DGObjects is typically a collection of DGObject which is loaded from database.
    //     DGObjects must have a defintion (DGObjectsDefinition) which defines the 
    //     table name in database, GIS layer name, etc.
    //   2.The objects in a DGObjects should be the same type.
    //   3.DGObjects is different from a list of DGObject.
    //   4.The DGObjects has the following index for fast visit:
    //     (1) name of DGObject -> DGObject
    //     (2) id of DGObject -> DGObject
    //     (3) DataRow of DataTable -> DGObject
    //     (4) DGObject -> DataRow of DataTable
    //
    public class DGObjects
    {

        // Parent of the objects
        public Domain parent { get; set; }
        // Objects definition
        public DGObjectsDefinition definition { get; set; }

        public List<DGObject> objContainer { get; set; }

        public string filter { get; set; }

        public string GetFilter()
        {
            string result = filter == null ? "" : filter;
            result += ((filter != null) && (filter.Length > 0) && (definition.ConditionSQL != null) && (definition.ConditionSQL.Length > 0)) ? "and" + definition.ConditionSQL : definition.ConditionSQL;
            return result;
        }

        // Summary:
        //     Constructors
        public DGObjects(DGObjectsDefinition def)
        {
            definition = def;
        }

        // Summary:
        //     Get object by a key
        public async Task<DGObject> QueryObjByID(int objID)
        {
            DGObjectRepository repository = DGObjectRepository.Instance(
                     Globals.project.projDef.ID,parent.name, definition.Type);
            DGObject obj = await repository.Retrieve(objID);
            obj.parent = this;
            return obj;
        }

        public async Task<List<DGObject>> QueryAllByObjs()
        {
            DGObjectRepository repository = DGObjectRepository.Instance(
         Globals.project.projDef.ID, parent.name, definition.Type);
            List<DGObject> list= await repository.GetAllByObjs(GetFilter());
            list.ForEach(x => x.parent = this);
            return list;
        }

        public async Task<List<DGObject>> QueryAll()
        {
            DGObjectRepository repository = DGObjectRepository.Instance(
         Globals.project.projDef.ID, parent.name, definition.Type);
            List<DGObject> list = await repository.GetAllAsync();
            list.ForEach(x => x.parent = this);
            return list;
        }
    }
}
