using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using IS3.Core;
using Telerik.Windows.Controls;

namespace IS3.Control.ViewModels
{
    public class ToolsPanelViewModel
    {
        public ObservableCollection<Tab> Tabs { get; set; } = new ObservableCollection<Tab>();

        public ToolsPanelViewModel()
        {
            var doc = new XmlDocument();
            doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Tools.xml");
            var xn = doc.SelectSingleNode("Tabs");
            var tabs = xn.ChildNodes;
            foreach (var tabnode in tabs)
            {
                var tabElement = (XmlElement)tabnode;
                var tab = new Tab(tabElement.GetAttribute("Text"));

                foreach (var itemNode in tabElement.ChildNodes)
                {
                    var group = new Group();
                    var itemElement = (XmlElement)itemNode;
                    var item = new Item(itemElement.GetAttribute("Text"));
                    item.ImagePath = AppDomain.CurrentDomain.BaseDirectory + itemElement.GetAttribute("ImagePath");
                    item.DllPath = AppDomain.CurrentDomain.BaseDirectory + itemElement.GetAttribute("DllPath");
                    item.ClassName = itemElement.GetAttribute("ClassName");
                    group.Items.Add(item);
                    tab.Groups.Add(group);
                }
                Tabs.Add(tab);
            }
        }
    }

    public class Tab
    {
        public string Text { get; set; }
        public ObservableCollection<Group> Groups { get; set; } = new ObservableCollection<Group>();

        public Tab(string text)
        {
            Text = text;
        }
    }

    public class Group
    {
        public ObservableCollection<Item> Items { get; set; } = new ObservableCollection<Item>();
        public string Text { get; set; }
    }

    public class Item
    {
        public ICommand CustomCommand { get; set; }
        public string Text { get; set; }
        public string ImagePath { get; set; }
        public string DllPath { get; set; }
        public string ClassName { get; set; }

        public Item(string text)
        {
            Text = text;
            CustomCommand = new DelegateCommand(Excuted);
        }

        private void Excuted(object obj)
        {


        }
    }

    public class HierarchicalDataTemplate : System.Windows.HierarchicalDataTemplate
    {
    }
}
