﻿using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrustructure.Plans.Elements;
using XFiresecAPI;
using GKModule.Plans.Designer;

namespace GKModule.Plans
{
	internal class PlanMonitor
	{
		Plan Plan;
		Action CallBack;
		List<XState> DeviceStates;
		List<XState> ZoneStates;
		List<XState> DirectionStates;

		public PlanMonitor(Plan plan, Action callBack)
		{
			Plan = plan;
			CallBack = callBack;
			DeviceStates = new List<XState>();
			ZoneStates = new List<XState>();
			DirectionStates = new List<XState>();
			Initialize();
		}
		private void Initialize()
		{
			Plan.ElementXDevices.ForEach(item => Initialize(item));
			Plan.ElementRectangleXZones.ForEach(item => Initialize(item));
			Plan.ElementPolygonXZones.ForEach(item => Initialize(item));
			Plan.ElementRectangleXDirections.ForEach(item => Initialize(item));
			Plan.ElementPolygonXDirections.ForEach(item => Initialize(item));
		}
		private void Initialize(ElementXDevice element)
		{
			var device = Helper.GetXDevice(element);
			if (device != null)
			{
				DeviceStates.Add(device.State);
				device.State.StateChanged += CallBack;
			}
		}
		private void Initialize(IElementZone element)
		{
			if (element.ZoneUID != Guid.Empty)
			{
				var zone = Helper.GetXZone(element);
				if (zone != null)
				{
					ZoneStates.Add(zone.State);
					zone.State.StateChanged += CallBack;
				}
			}
		}
		private void Initialize(IElementDirection element)
		{
			if (element.DirectionUID != Guid.Empty)
			{
				var direction = Helper.GetXDirection(element);
				if (direction != null)
				{
					DirectionStates.Add(direction.State);
					direction.State.StateChanged += CallBack;
				}
			}
		}

		public XStateClass GetState()
		{
			var result = XStateClass.No;
			foreach (var deviceState in DeviceStates)
			{
				var stateClass = deviceState.StateClass;
				if (deviceState.Device.DriverType == XDriverType.AM1_T && stateClass == XStateClass.Fire2)
				{
					stateClass = XStateClass.Info;
				}
				if (stateClass < result)
					result = stateClass;
			}
			foreach (var zoneState in ZoneStates)
			{
				if (zoneState.StateClass < result)
					result = zoneState.StateClass;
			}
			foreach (var directionState in DirectionStates)
			{
				if (directionState.StateClass < result)
					result = directionState.StateClass;
			}
			return result;
		}
	}
}