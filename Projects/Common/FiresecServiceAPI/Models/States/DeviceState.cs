﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class DeviceState
	{
		[DataMember]
		public Guid DeviceUID { get; set; }

		[DataMember]
		public List<DeviceDriverState> SerializableStates { get; set; }

		[DataMember]
		public List<ParentDeviceState> ParentStates { get; set; }

		[DataMember]
		public List<ChildDeviceState> ChildStates { get; set; }

		[DataMember]
		public List<Parameter> Parameters { get; set; }

		[DataMember]
		public float Smokiness { get; set; }

		[DataMember]
		public float Dustiness { get; set; }

		[DataMember]
		public float Temperature { get; set; }

		[DataMember]
		public bool IsInitializing { get; set; }

		[DataMember]
		public bool IsPanelConnectionLost { get; set; }

		[DataMember]
		public bool IsUsbConnectionLost { get; set; }

		[DataMember]
		public bool IsDBMissmatch { get; set; }

		[DataMember]
		public bool IsWrongPanel { get; set; }

		public Device Device { get; set; }

		static object locker = new object();
		List<DeviceDriverState> _states;
		public List<DeviceDriverState> States
		{
			get
			{
				lock (locker)
				{
					return _states;
				}
			}
			set
			{
				lock (locker)
				{
					if (_states == null)
					{
						_states = new List<DeviceDriverState>();
					}
					_states = value;
				}
			}
		}
        public List<DeviceDriverState> ThreadSafeStates
        {
            get
            {
                if (States == null)
                {
                    States = new List<DeviceDriverState>();
                }
                return States.ToList();
            }
        }

		public List<ParentDeviceState> ThreadSafeParentStates
		{
			get
			{
				if (ParentStates == null)
				{
					ParentStates = new List<ParentDeviceState>();
				}
				return ParentStates.ToList();
			}
		}
		public List<ChildDeviceState> ThreadSafeChildStates
		{
			get
			{
				if (ChildStates == null)
				{
					ChildStates = new List<ChildDeviceState>();
				}
				return ChildStates.ToList();
			}
		}
		public List<Parameter> ThreadSafeParameters
		{
			get
			{
				if (Parameters == null)
				{
					Parameters = new List<Parameter>();
				}
				return Parameters.ToList();
			}
		}

		public List<string> StateParameters
		{
			get
			{
				var parameters = new List<string>();
				foreach (var parameter in ThreadSafeParameters)
				{
					var mustShowParameter = false;
					if (!parameter.IsIgnore && parameter.Visible)
					{
						mustShowParameter = true;
					}
					else
					{
						switch (parameter.Name)
						{
							case "FailureType":
							case "AlarmReason":
							case "OtherMessage":
							case "FailureType5":
							case "AlarmReason5":
							case "OtherMessage5":
							case "FailureType6":
							case "AlarmReason6":
							case "OtherMessage6":
							case "VoltageBattery1":
							case "VoltageBattery2":
							case "VoltageOutput1":
							case "VoltageOutput2":
							case "VoltageInput":
								mustShowParameter = true;
								break;
						}
					}

					if (mustShowParameter && !string.IsNullOrEmpty(parameter.Value) && parameter.Value != "<NULL>")
					{
						parameters.Add(parameter.Caption + ": " + parameter.Value);
					}
				}
				return parameters;
			}
		}

		public DeviceState()
		{
			SerializableStates = new List<DeviceDriverState>();
			States = new List<DeviceDriverState>();
			ParentStates = new List<ParentDeviceState>();
			ChildStates = new List<ChildDeviceState>();
			Parameters = new List<Parameter>();
		}

		public StateType StateType
		{
			get
			{
				var stateTypes = new List<StateType>() { StateType.Norm };

                foreach (var deviceDriverState in ThreadSafeStates)
				{
					if (deviceDriverState.DriverState != null)
					{
						stateTypes.Add(deviceDriverState.DriverState.StateType);
					}
				}

				foreach (var parentDeviceState in ThreadSafeParentStates)
				{
					if (parentDeviceState.DriverState != null)
					{
						stateTypes.Add(parentDeviceState.DriverState.StateType);
					}
				}

				foreach (var childDeviceState in ThreadSafeChildStates)
				{
					stateTypes.Add(childDeviceState.StateType);
				}

				return stateTypes.Min();
			}
		}

		public List<string> StringStates
		{
			get
			{
				var stringStates = new List<string>();
                foreach (var state in ThreadSafeStates)
				{
					stringStates.Add(state.DriverState.Name);
				}
				return stringStates;
			}
		}

		public List<string> ParentStringStates
		{
			get
			{
				var parentStringStates = new List<string>();
				foreach (var parentDeviceState in ThreadSafeParentStates)
				{
					if (parentDeviceState.ParentDevice.Driver != null)
						parentStringStates.Add(parentDeviceState.ParentDevice.Driver.ShortName + " - " + parentDeviceState.DriverState.Name);
				}
				return parentStringStates;
			}
		}

		public bool IsDisabled
		{
            get { return ThreadSafeStates.Any(x => x.DriverState.StateType == StateType.Off); }
		}

		public event Action StateChanged;
		public void OnStateChanged()
		{
			if (StateChanged != null)
				StateChanged();
		}

		public event Action ParametersChanged;
		public void OnParametersChanged()
		{
			if (ParametersChanged != null)
				ParametersChanged();
		}
	}
}