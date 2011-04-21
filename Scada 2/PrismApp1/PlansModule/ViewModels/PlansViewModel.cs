﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Media;
using PlansModule.Events;
using System.Diagnostics;
using PlansModule.Models;

namespace PlansModule.ViewModels
{
    public class PlansViewModel : RegionViewModel
    {
        List<PlanViewModel> AllPlanViewModels;
        public PlanViewModel RootPlanViewModel;

        public PlansViewModel()
        {
            MainCanvas = new Canvas();
            MainCanvas.Background = Brushes.Yellow;
            AllPlanViewModels = new List<PlanViewModel>();
            ServiceFactory.Events.GetEvent<SelectPlanEvent>().Subscribe(OnSelectPlan);
            ServiceFactory.Events.GetEvent<PlanDeviceSelectedEvent>().Subscribe(OnPlanDeviceSelected);
            ServiceFactory.Events.GetEvent<PlanZoneSelectedEvent>().Subscribe(OnPlanZoneSelected);
        }

        public void Initialize()
        {
            Plan rootPlan = PlanLoader.Load();

            RootPlanViewModel = new PlanViewModel();
            RootPlanViewModel.Parent = null;
            AllPlanViewModels.Add(RootPlanViewModel);
            AddPlan(rootPlan, RootPlanViewModel);
            RootPlanViewModel.Initialize(rootPlan, MainCanvas);

            RootPlanViewModels = new ObservableCollection<PlanViewModel>();
            RootPlanViewModels.Add(RootPlanViewModel);

            SelectedPlanViewModel = RootPlanViewModel;
        }

        void AddPlan(Plan parentPlan, PlanViewModel parentPlanViewModel)
        {
            if (parentPlan.Children != null)
                foreach (Plan plan in parentPlan.Children)
                {
                    PlanViewModel planViewModel = new PlanViewModel();
                    planViewModel.Parent = parentPlanViewModel;
                    parentPlanViewModel.Children.Add(planViewModel);
                    planViewModel.Initialize(plan, MainCanvas);
                    AllPlanViewModels.Add(planViewModel);
                    AddPlan(plan, planViewModel);
                }
        }

        Canvas mainCanvas;
        public Canvas MainCanvas
        {
            get { return mainCanvas; }
            set
            {
                mainCanvas = value;
                OnPropertyChanged("MainCanvas");
            }
        }

        ObservableCollection<PlanViewModel> rootPlanViewModels;
        public ObservableCollection<PlanViewModel> RootPlanViewModels
        {
            get { return rootPlanViewModels; }
            set
            {
                rootPlanViewModels = value;
                OnPropertyChanged("RootPlanViewModels");
            }
        }

        PlanViewModel selectedPlanViewModel;
        public PlanViewModel SelectedPlanViewModel
        {
            get { return selectedPlanViewModel; }
            set
            {
                selectedPlanViewModel = value;
                OnPropertyChanged("SelectedPlanViewModel");
                selectedPlanViewModel.Select();
            }
        }

        public void OnSelectPlan(string name)
        {
            PlanViewModel planViewModel = AllPlanViewModels.FirstOrDefault(x => x.Name == name);
            if (planViewModel != null)
            {
                SelectedPlanViewModel = planViewModel;
            }
        }

        public void OnPlanDeviceSelected(string path)
        {
            foreach (ElementDeviceViewModel elementDeviceViewModel in SelectedPlanViewModel.Devices)
            {
                if (elementDeviceViewModel.elementDevice.Path == path)
                {
                    elementDeviceViewModel.IsSelected = true;
                }
                else
                {
                    elementDeviceViewModel.IsSelected = false;
                }
            }

            SelectedDeviceViewModel = this.SelectedPlanViewModel.Devices.FirstOrDefault(x => x.elementDevice.Path == path);

            SelectedDeviceViewModel.IsActive = true;

            if (SelectedZoneViewModel != null)
                SelectedZoneViewModel.IsActive = false;
        }

        public void OnPlanZoneSelected(string zoneNo)
        {
            SelectedZoneViewModel = this.SelectedPlanViewModel.Zones.FirstOrDefault(x => x.elementZone.ZoneNo == zoneNo);

            if (SelectedDeviceViewModel != null)
                SelectedDeviceViewModel.IsActive = false;
            SelectedZoneViewModel.IsActive = true;
            //SelectedZoneViewModel.Name = zoneNo;
        }

        ElementDeviceViewModel selectedDeviceViewModel;
        public ElementDeviceViewModel SelectedDeviceViewModel
        {
            get { return selectedDeviceViewModel; }
            set
            {
                selectedDeviceViewModel = value;
                OnPropertyChanged("SelectedDeviceViewModel");
            }
        }

        ElementZoneViewModel selectedZoneViewModel;
        public ElementZoneViewModel SelectedZoneViewModel
        {
            get { return selectedZoneViewModel; }
            set
            {
                selectedZoneViewModel = value;
                OnPropertyChanged("SelectedZoneViewModel");
            }
        }

        public void ShowDevice(string path)
        {
            foreach (PlanViewModel planViewModel in AllPlanViewModels)
            {
                foreach (ElementDevice planDevice in planViewModel.plan.ElementDevices)
                {
                    if (planDevice.Path == path)
                    {
                        planViewModel.IsSelected = true;
                        return;
                    }
                }
            }
        }

        public override void Dispose()
        {
        }
    }
}
