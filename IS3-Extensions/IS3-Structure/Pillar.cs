using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows;

using IS3.Core;
using IS3.Core.Serialization;
using IS3.Structure.Serialization;

namespace IS3.Structure
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
    //    Monitoring Point
    public class Pillar : DGObject
    {
       public int ObjID { get; set; }
        public Pillar()
        {
           
        }
        public Pillar(DataRow rawData)
            : base(rawData)
        {
         
        }

        public override bool LoadObjs(DGObjects objs)
        {
            StructureDGObjectLoader loader =
                new StructureDGObjectLoader();
            bool success = loader.LoadPillar(objs);
            return success;
        }
        public override string ToString()
        {
            string str = base.ToString();


            return str;
        }

        private bool isDetail = false;
        public override List<DataView> tableViews(IEnumerable<DGObject> objs)
        {

            List<DataView> dataViews = base.tableViews(objs);

            return dataViews;
        }


        public override List<FrameworkElement> chartViews(
            IEnumerable<DGObject> objs, double width, double height)
        {
            List<FrameworkElement> charts = new List<FrameworkElement>();
            return charts;
        }
    }
}
