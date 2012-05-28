﻿using System.Collections.Generic;
using System.ServiceModel;
using FiresecAPI.Models;

namespace FiresecAPI
{
	public interface IFiresecCallback
	{
		[OperationContract(IsOneWay = true)]
		void DeviceStateChanged(List<DeviceState> deviceStates);

		[OperationContract(IsOneWay = true)]
		void DeviceParametersChanged(List<DeviceState> deviceStates);

		[OperationContract(IsOneWay = true)]
		void ZoneStateChanged(ZoneState zoneState);

		[OperationContract(IsOneWay = true)]
		void NewJournalRecord(JournalRecord journalRecord);
	}
}