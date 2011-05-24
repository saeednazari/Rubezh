﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Modularity;
using Infrastructure.Common;
using Infrastructure.Events;
using SecurityModule.ViewModels;
using Infrastructure;

namespace SecurityModule
{
    public class SequrityModule : IModule
    {
        public SequrityModule()
        {
            ServiceFactory.Events.GetEvent<ShowSecurityEvent>().Subscribe(OnShowSecurity);
        }

        public void Initialize()
        {
            RegisterResources();
            CreateViewModels();
        }

        void RegisterResources()
        {
            var resourceService = ServiceFactory.Get<IResourceService>();
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/DataGrid.xaml"));
        }

        static void CreateViewModels()
        {
            securityViewModel = new SecurityViewModel();
            securityViewModel.Initialize();
        }

        static SecurityViewModel securityViewModel;

        static void OnShowSecurity(string obj)
        {
            ServiceFactory.Layout.Show(securityViewModel);
        }
    }
}