﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using FiresecClient;
using FiresecClient.Models;

namespace DeviveModelManager
{
    public class TreeBuilder
    {
        public TreeItem RootTreeItem { get; private set; }

        public void Build()
        {
            FiresecManager.Connect("adm", "");

            RootTreeItem = RootHelper.CreateRoot();
            RootTreeItem.Name = "Компьютер";

            var rootDriverId = FiresecManager.Configuration.Drivers.FirstOrDefault(x => x.DriverName == "Компьютер").Id;
            AddDriver(rootDriverId, RootTreeItem);
            RootTreeItem.Children.Add(MonitorHelper.CreateMonitor());
            RootTreeItem.Children.Add(ZoneHelper.CreateZone());
            InitializeModelInfo(RootTreeItem);
            SaveToFile();
        }

        void AddDriver(string parentDriverId, TreeItem parentTreeItem)
        {
            Driver parentDriver = FiresecManager.Configuration.Drivers.FirstOrDefault(x => x.Id == parentDriverId);

            if (parentDriver != null)
            {
                if (parentDriver.DriverName == "Модуль пожаротушения")
                    return;

                foreach (var driver in parentDriver.Children)
                {
                    var _driver = FiresecManager.Configuration.Drivers.FirstOrDefault(x => x.Id == driver);
                    TreeItem childTree = new TreeItem();
                    childTree.SetDriver(_driver);
                    parentTreeItem.Children.Add(childTree);

                    AddDriver(driver, childTree);
                }
            }
        }

        void InitializeModelInfo(TreeItem parentTreeItem)
        {
            if (parentTreeItem.Children.Count > 0)
            {
                parentTreeItem.ModelInfo.type = new Assad.modelInfoType[parentTreeItem.Children.Count];
                for (int i = 0; i < parentTreeItem.Children.Count; i++)
                {
                    parentTreeItem.ModelInfo.type[i] = parentTreeItem.Children[i].ModelInfo;
                    InitializeModelInfo(parentTreeItem.Children[i]);
                }
            }
        }

        void SaveToFile()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Assad.modelInfoType));
            StreamWriter fileStream = File.CreateText("DeviceModel.xml");
            serializer.Serialize(fileStream, RootTreeItem.ModelInfo);
            fileStream.Close();
        }
    }
}
