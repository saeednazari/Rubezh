﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace WpfApplication5
{
    public class PlanViewModel : BaseViewModel
    {
        public PlanViewModel()
        {
            Children = new ObservableCollection<PlanViewModel>();
            ParentPlans = new ObservableCollection<PlanViewModel>();
            SelectCommand = new RelayCommand(Select);
        }

        public PlanViewModel Parent { get; set; }
        public ObservableCollection<PlanViewModel> Children { get; set; }

        public Plan plan { get; set; }
        public string Name { get; set; }
        public string Caption { get; set; }

        public ObservableCollection<PlanViewModel> ParentPlans { get; set; }

        public RelayCommand SelectCommand { get; private set; }
        public void Select(object obj)
        {
            IsSelected = true;
            ExpandParent();
        }

        bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                OnPropertyChanged("IsSelected");
                if (isSelected)
                {
                    MainViewModel.Current.SelectedPlanViewModel = this;
                    DrawPlan();
                }
            }
        }

        bool isExpanded;
        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                isExpanded = value;
                OnPropertyChanged("IsExpanded");
            }
        }

        void ExpandParent()
        {
            if (Parent != null)
            {
                Parent.IsExpanded = true;
                //ExpandParent();
            }
        }

        public void Initialize(Plan plan)
        {
            this.plan = plan;
            Name = plan.Name;
            Caption = plan.Caption;
            UpdateParentPlans();
        }

        public void DrawPlan()
        {
            Canvas MainCanvas = MainViewModel.Current.MainCanvas;
            MainCanvas.Children.Clear();

            foreach (Plan childPlan in plan.Children)
            {
                Rectangle rectanglePlan = new Rectangle();
                rectanglePlan.Name = childPlan.Name;
                rectanglePlan.MouseLeftButtonDown += new MouseButtonEventHandler(rectanglePlan_MouseLeftButtonDown);
                rectanglePlan.Width = childPlan.Width;
                rectanglePlan.Height = childPlan.Height;
                rectanglePlan.Fill = childPlan.Brush;
                rectanglePlan.RadiusX = 10;
                rectanglePlan.RadiusY = 10;
                Canvas.SetLeft(rectanglePlan, childPlan.Left);
                Canvas.SetTop(rectanglePlan, childPlan.Top);
                MainCanvas.Children.Add(rectanglePlan);
            }

            foreach (Element element in plan.Elements)
            {
                Ellipse ellipse = new Ellipse();
                ellipse.Width = 20;
                ellipse.Height = 20;
                ellipse.Fill = Brushes.GreenYellow;
                Canvas.SetLeft(ellipse, element.X);
                Canvas.SetTop(ellipse, element.Y);
                MainCanvas.Children.Add(ellipse);
            }
        }

        void UpdateParentPlans()
        {
            ParentPlans = new ObservableCollection<PlanViewModel>();
            foreach (PlanViewModel parentPlan in AllParents)
            {
                ParentPlans.Add(parentPlan);
            }
        }

        public List<PlanViewModel> AllParents
        {
            get
            {
                if (Parent == null)
                {
                    return new List<PlanViewModel>();
                }

                List<PlanViewModel> allParents = Parent.AllParents;
                allParents.Add(Parent);
                return allParents;
            }
        }

        void rectanglePlan_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string name = (sender as Rectangle).Name;
            if (e.ClickCount == 2)
            {
                if (Children.Count > 0)
                {
                    PlanViewModel SelectedPlanViewModel = Children.FirstOrDefault(x => x.Name == name);
                    SelectedPlanViewModel.Select(null);
                }
            }
        }
    }
}