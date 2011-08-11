﻿using System.Collections.ObjectModel;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;

namespace PlansModule.ViewModels
{
    public class PlansViewModel : RegionViewModel
    {
        public PlansViewModel()
        {
            AddCommand = new RelayCommand(OnAdd);
            RemoveCommand = new RelayCommand(OnRemove, CanEditRemove);
            EditCommand = new RelayCommand(OnEdit, CanEditRemove);
            Plans = new ObservableCollection<PlanDetailsViewModel>();
        }

        public void Initialize()
        {
            if (FiresecManager.SystemConfiguration.Plans != null)
            {
                foreach (var plan in FiresecManager.SystemConfiguration.Plans)
                {
                    PlanDetailsViewModel planViewModel = new PlanDetailsViewModel();
                    Plans.Add(planViewModel);
                }
            }
        }

        public ObservableCollection<PlanDetailsViewModel> Plans { get; set; }

        PlanDetailsViewModel _selectedPlan;

        public PlanDetailsViewModel SelectedPlan
        {
            get { return _selectedPlan; }
            set
            {
                _selectedPlan = value;
                OnPropertyChanged("SelectedPlan");
            }
        }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            var planDetailsViewModel = new PlanDetailsViewModel(SelectedPlan);
            bool result = ServiceFactory.UserDialogs.ShowModalWindow(planDetailsViewModel);
            if (result)
            {
                if (planDetailsViewModel.Parent == null)
                {
                    FiresecManager.SystemConfiguration.Plans.Add(planDetailsViewModel.Plan);
                    Plans.Add(planDetailsViewModel);
                }
            }
        }

        bool CanEditRemove(object obj)
        {
            return SelectedPlan != null;
        }

        public RelayCommand RemoveCommand { get; private set; }

        bool RemovePlan(ObservableCollection<PlanDetailsViewModel> level)
        {
            bool res = false;
            if (level.Remove(SelectedPlan))
            {
                FiresecManager.SystemConfiguration.Plans.Remove(SelectedPlan.Plan);
                res = true;
            }
            else
            {
                for (int i = 0; i < level.Count; i++)
                {
                    if (level[i].Children != null)
                    {
                        if (RemovePlan(level[i].Children))
                        {
                            FiresecManager.SystemConfiguration.Plans.Remove(SelectedPlan.Plan);
                            res = true;
                            break;
                        }
                    }
                }
            }
            return res;
        }

        void OnRemove()
        {
            RemovePlan(Plans);
            SelectedPlan = null;
        }

        public RelayCommand EditCommand { get; private set; }

        void OnEdit()
        {
            PlanDetailsViewModel planDetailsViewModel = new PlanDetailsViewModel();
            planDetailsViewModel.Initialize(SelectedPlan);
            bool result = ServiceFactory.UserDialogs.ShowModalWindow(planDetailsViewModel);
            if (result)
            {
                EditPlan(Plans, planDetailsViewModel, SelectedPlan);
                SelectedPlan = null;
            }
        }

        bool EditPlan(ObservableCollection<PlanDetailsViewModel> level, PlanDetailsViewModel _newPlan, PlanDetailsViewModel _oldPlan)
        {
            bool res = false;
            int index = level.IndexOf(SelectedPlan);
            if (index != -1)
            {
                level[index].Name = _newPlan.Name;
                level[index].Height = _newPlan.Height;
                level[index].Width = _newPlan.Width;
                res = true;
            }
            else
            {
                for (int i = 0; i < level.Count; i++)
                {
                    if (level[i].Children != null)
                    {
                        index = level[i].Children.IndexOf(SelectedPlan);
                        if (index != -1)
                        {
                            ObservableCollection<PlanDetailsViewModel> _level = level[i].Children;
                            index = level[i].Children.IndexOf(_oldPlan);
                            _level[index].Name = _newPlan.Name;
                            _level[index].Height = _newPlan.Height;
                            _level[index].Width = _newPlan.Width;
                            res = true;
                            break;
                        }
                        else
                        {
                            EditPlan(level[i].Children, _newPlan, _oldPlan);
                        }
                    }
                }
            }
            return res;
        }

        public override void OnShow()
        {
            PlansContextMenuViewModel plansContextMenuViewModel = new PlansContextMenuViewModel(AddCommand, EditCommand, RemoveCommand);
            ServiceFactory.Layout.ShowMenu(plansContextMenuViewModel);
        }

        public override void OnHide()
        {
            ServiceFactory.Layout.ShowMenu(null);
        }
    }
}