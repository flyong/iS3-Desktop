﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iS3.Core;

namespace iS3.SimpleGeologyTools
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
    //     Inherits from iS3.Core.Tools to add geology tools.
    //     This is the entry point for the tools.
    // Remarks:
    //     Overide treeItems() function to return a list of 
    //     ToolTreeItem, which defines tool path, name and function.
    //     ToolTreeItem example:
    //         displayPath: "Geology|Basic"
    //         displayName: "MakeProfile"
    //         func: void test();
    //
    public class GeologyTools : Tools
    {
        public override string name() { return "iS3.SimpleGeologyTools"; }
        public override string provider() { return "Tongji iS3 team"; }
        public override string version() { return "1.0"; }

        List<ToolTreeItem> items;
        public override IEnumerable<ToolTreeItem> treeItems()
        {
            return items;
        }

        SimpleProfileAnalysisWindow spaWindow;
        public void makeProfile()
        {
            if (spaWindow != null)
            {
                spaWindow.Show();
                return;
            }

            spaWindow = new SimpleProfileAnalysisWindow();
            spaWindow.Closed += (o, args) =>
                {
                    spaWindow = null;
                };
            spaWindow.Show();
        }

        public GeologyTools()
        {
            items = new List<ToolTreeItem>();

            ToolTreeItem item = new ToolTreeItem("Geology|Basic", "MakeProfile", makeProfile);
            items.Add(item);
        }
    }
}
