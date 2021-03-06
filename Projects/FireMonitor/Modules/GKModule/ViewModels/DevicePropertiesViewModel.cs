﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using System.Collections.ObjectModel;

namespace GKModule.ViewModels
{
	public class DevicePropertiesViewModel : BaseViewModel
	{
		public DevicePropertiesViewModel(XDevice device)
		{
			Properties = new ObservableCollection<DeviceProperty>();

			foreach (var driverProperty in device.Driver.Properties)
			{
				if (driverProperty.IsAUParameter)
				{
					var deviceProperty = new DeviceProperty()
					{
						Name = driverProperty.Caption
					};

					var property = device.Properties.FirstOrDefault(x => x.Name == driverProperty.Name);
					if (property != null)
					{
						switch (driverProperty.DriverPropertyType)
						{
							case XDriverPropertyTypeEnum.EnumType:
								var parameter = driverProperty.Parameters.FirstOrDefault(x => x.Value == property.Value);
								if (parameter != null)
									deviceProperty.Value = parameter.Name;
								break;

							case XDriverPropertyTypeEnum.IntType:
								deviceProperty.Value = property.Value.ToString();
								break;

							case XDriverPropertyTypeEnum.BoolType:
								var isTrue = property.Value > 0;
								deviceProperty.Value = isTrue ? "Есть" : "Нет";
								break;
						}
					}

					Properties.Add(deviceProperty);
				}
			}
		}

		public ObservableCollection<DeviceProperty> Properties { get; private set; }

		public bool HasAUParameters
		{
			get { return Properties.Count > 0; }
		}
	}

	public class DeviceProperty
	{
		public string Name { get; set; }
		public string Value { get; set; }
	}
}