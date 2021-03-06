﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class Driver
	{
		public Driver()
		{
			Properties = new List<DriverProperty>();
			Parameters = new List<Parameter>();
			States = new List<DriverState>();
			Children = new List<Guid>();
			AvaliableChildren = new List<Guid>();
			AutoCreateChildren = new List<Guid>();
		}

		[DataMember]
		public DriverType DriverType { get; set; }

		[DataMember]
		public List<DriverProperty> Properties { get; set; }

		[DataMember]
		public List<Parameter> Parameters { get; set; }

		[DataMember]
		public List<DriverState> States { get; set; }

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public string StringUID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string ShortName { get; set; }

		[DataMember]
		public string DeviceClassName { get; set; }

		[DataMember]
		public bool HasAddress { get; set; }

		[DataMember]
		public bool CanEditAddress { get; set; }

		[DataMember]
		public string ChildAddressMask { get; set; }

		[DataMember]
		public int ShleifCount { get; set; }

		[DataMember]
		public bool IsDeviceOnShleif { get; set; }

		[DataMember]
		public bool HasShleif { get; set; }

		[DataMember]
		public bool UseParentAddressSystem { get; set; }

		[DataMember]
		public bool IsChildAddressReservedRange { get; set; }

		[DataMember]
		public int ChildAddressReserveRangeCount { get; set; }

		[DataMember]
		public bool DisableAutoCreateChildren { get; set; }

		[DataMember]
		public List<Guid> Children { get; set; }

		[DataMember]
		public List<Guid> AvaliableChildren { get; set; }

		[DataMember]
		public bool CanAddChildren { get; set; }

		[DataMember]
		public List<Guid> AutoCreateChildren { get; set; }

		[DataMember]
		public Guid AutoChild { get; set; }

		[DataMember]
		public int AutoChildCount { get; set; }

		[DataMember]
		public bool IsRangeEnabled { get; set; }

		[DataMember]
		public int MinAddress { get; set; }

		[DataMember]
		public int MaxAddress { get; set; }

		[DataMember]
		public bool IsAutoCreate { get; set; }

		[DataMember]
		public int MinAutoCreateAddress { get; set; }

		[DataMember]
		public int MaxAutoCreateAddress { get; set; }

		[DataMember]
		public bool HasAddressMask { get; set; }

		[DataMember]
		public bool IsZoneDevice { get; set; }

		[DataMember]
		public bool IsZoneLogicDevice { get; set; }

		[DataMember]
		public bool CanDisable { get; set; }

		[DataMember]
		public bool IsPlaceable { get; set; }

		[DataMember]
		public bool IsSingleInParent { get; set; }

		[DataMember]
		public bool IsSingleInZone { get; set; }

		[DataMember]
		public bool IsNotValidateZoneAndChildren { get; set; }

		[DataMember]
		public bool IsBUtton { get; set; }

		[DataMember]
		public bool IsOutDevice { get; set; }

		[DataMember]
		public DeviceCategoryType Category { get; set; }

		[DataMember]
		public string CategoryName { get; set; }

		[DataMember]
		public DeviceType DeviceType { get; set; }

		[DataMember]
		public string DeviceTypeName { get; set; }

		[DataMember]
		public bool IsIgnore { get; set; }

		[DataMember]
		public bool IgnoreInZoneState { get; set; }

		[DataMember]
		public bool IsAssadIgnore { get; set; }

		[DataMember]
		public bool CanWriteDatabase { get; set; }

		[DataMember]
		public bool CanReadDatabase { get; set; }

		[DataMember]
		public bool CanReadJournal { get; set; }

		[DataMember]
		public bool CanAutoDetect { get; set; }

		[DataMember]
		public bool CanSynchonize { get; set; }

		[DataMember]
		public bool CanReboot { get; set; }

		[DataMember]
		public bool CanGetDescription { get; set; }

		[DataMember]
		public bool CanSetPassword { get; set; }

		[DataMember]
		public bool CanUpdateSoft { get; set; }

		[DataMember]
		public bool CanExecuteCustomAdminFunctions { get; set; }

		[DataMember]
		public bool IsAlternativeUSB { get; set; }

		[DataMember]
		public bool CanMonitoringDisable { get; set; }

		[DataMember]
		public bool HasConfigurationProperties { get; set; }

		[DataMember]
		public string PresenterKeyPropertyName { get; set; }

		public string ImageSource
		{
			get { return "/Controls;component/FSIcons/" + this.DriverType.ToString() + ".png"; }
		}

		public bool IsPanel
		{
			get { return DeviceClassName == "ППКП"; }
		}

		public bool HasControlProperties
		{
			get { return Properties.Any(x => x.IsControl); }
		}

		public bool IsGroupDevice
		{
			get
			{
				switch (DriverType)
				{
					case DriverType.AM4:
					case DriverType.AM4_P:
					case DriverType.AMT_4:
					case DriverType.RM_2:
					case DriverType.RM_3:
					case DriverType.RM_4:
					case DriverType.RM_5:
						return true;
				}
				return false;
			}
		}

		public bool IsPump()
		{
			return
				(DriverType == DriverType.PumpStation ||
				DriverType == DriverType.Pump ||
				DriverType == DriverType.JokeyPump ||
				DriverType == DriverType.Compressor ||
				DriverType == DriverType.DrenazhPump ||
				DriverType == DriverType.CompensationPump);
		}

		public DriverProperty PresenterKeyProperty
		{
			get { return Properties.FirstOrDefault(item => item.Name == PresenterKeyPropertyName); }
		}
	}
}