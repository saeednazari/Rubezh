﻿using System.Collections.Generic;
using FiresecAPI.Models;
using Infrastructure.Common;

namespace PlansModule.ViewModels
{
    public class ZoneViewModel : BaseViewModel
    {
        public ZoneViewModel(Plan plan)
        {
            Parent = plan;
        }

        public void Initialize(List<PolygonPoint> polygonPoints)
        {
            if (PolygonPoints == null) PolygonPoints = new List<PolygonPoint>();
            PolygonPoints = polygonPoints;
        }

        public Plan Parent { get; private set; }
        public List<PolygonPoint> PolygonPoints { get; set; }
        public string ZoneNo { get; set; }

        public void Update()
        {
            OnPropertyChanged("Plan");
        }
    }
}