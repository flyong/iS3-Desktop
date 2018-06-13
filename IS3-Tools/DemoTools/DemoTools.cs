using System.Collections.Generic;
using IS3.Core;
using System.Windows.Forms;

namespace DemoTools
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
    public class DemoTools : Tools
    {
        //基本信息
        public override string name() { return "iS3.DemoTools"; }
        public override string provider() { return "Tongji iS3 team"; }
        public override string version() { return "1.0"; }

        //分析工具列表
        List<ToolTreeItem> items;
        public override IEnumerable<ToolTreeItem> treeItems()
        {
            return items;
        }

        MonitorDemoTool demoWindow;
        void CallMonitorDemoTool()
        {
            if (demoWindow != null)
            {
                demoWindow.Show();
                return;
            }

            demoWindow = new MonitorDemoTool();
            demoWindow.Closed += (o, args) =>
            {
                demoWindow = null;
            };
            demoWindow.Show();
        }

        void CallDomainTree()
        {
            DomainTree dt = new DomainTree();
            dt.DataContext = Globals.project.domains.Values;
            dt.Show();


        }
       


        //新建工具树
        public DemoTools()
        {
            items = new List<ToolTreeItem>();

            ToolTreeItem item = new ToolTreeItem("Demo|Basic", "MonitorDemoTool", CallMonitorDemoTool);
            items.Add(item);

            ToolTreeItem item1 = new ToolTreeItem("Demo|Basic", "DomainTree", CallDomainTree);
            items.Add(item1);

        }
    }
}
