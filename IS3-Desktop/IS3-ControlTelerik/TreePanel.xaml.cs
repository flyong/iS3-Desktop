using System;
using System.Windows;
using System.Windows.Controls;
using IS3.Core;

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

    /// <summary>
    /// Interaction logic for TreePanel.xaml
    /// </summary>
    public partial class TreePanel : UserControl,IViewHolder
    {
        public EventHandler<DGObjectsSelectionChangedEventArgs> dGObjectsSelectionChangedEventTriggle;
        protected IView _view;
        public IView view { get { return _view; } }

        public TreePanel(Tree rootTree)
        {
            InitializeComponent();
            DomainTreeView.ItemsSource = rootTree.Children;
            _view = new IS3ViewNormal(this);
            _view.DGObjectsSelectionChangedTriggerInner += DGObjectsSelectionChangedListener;
            dGObjectsSelectionChangedEventTriggle += _view.DGObjectsSelectionChangedListenerInner;

        }

        private void DGObjectsSelectionChangedListener(object sender, DGObjectsSelectionChangedEventArgs e)
        {
           
        }

        public event EventHandler<Tree> OnTreeSelected;
        private void DomainTreeView_SelectedItemChanged(object sender,
            RoutedPropertyChangedEventArgs<object> e)
        {
            //if (OnTreeSelected != null)
            //    OnTreeSelected(this, e.NewValue as Tree);

            if (dGObjectsSelectionChangedEventTriggle != null)
            {
                DGObjectsSelectionChangedEventArgs args = new DGObjectsSelectionChangedEventArgs();
                if (((e.NewValue as Tree) == null) || ((e.NewValue as Tree).RefObjsName == null))
                {
                    args.newOne = null;
                }
                else
                {
                    string dgObjectsName = (e.NewValue as Tree).RefObjsName;
                    if (Globals.project.objsDefIndex.ContainsKey(dgObjectsName))
                    {
                        args.newOne = Globals.project.objsDefIndex[dgObjectsName];
                    }
                }
                dGObjectsSelectionChangedEventTriggle(this, args);
            }
        }

        public void setCoord(string coord)
        {
            
        }
    }
}
