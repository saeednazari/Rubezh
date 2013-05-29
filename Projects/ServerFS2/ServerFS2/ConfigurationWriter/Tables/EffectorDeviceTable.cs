﻿using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecAPI.Models.Binary;

namespace ServerFS2.ConfigurationWriter
{
	public class EffectorDeviceTable : TableBase
	{
		public Device Device;
		bool IsRemote;

		public override Guid UID
		{
			get { return Device.UID; }
		}

		BinaryDevice BinaryDevice;
		public List<ZoneTable> ZonesInLogic = new List<ZoneTable>();

		public EffectorDeviceTable(FlashDatabase panelDatabase, BinaryDevice binaryDevice, bool isOuter)
			: base(panelDatabase, isOuter ? "Внешнее устройство " : "" + binaryDevice.Device.DottedPresentationAddressAndName)
		{
			binaryDevice.TableBase = this;
			BinaryDevice = binaryDevice;
			Device = binaryDevice.Device;
			IsRemote = isOuter;
		}

		public override void Create()
		{
			if (IsRemote)
			{
				var deviceCode = FiresecAPI.Models.DriversHelper.GetCodeForFriver(Device.Driver.DriverType);
				BytesDatabase.AddByte(deviceCode, "Тип внешнего ИУ");
			}
			var outerPanelAddress = 0;
			if (IsRemote)
			{
				outerPanelAddress = Device.ParentPanel.IntAddress;
			}
			BytesDatabase.AddByte(outerPanelAddress, "Адрес прибора привязки в сети");
			BytesDatabase.AddByte((Device.AddressOnShleif), "Адрес");
			BytesDatabase.AddByte(Math.Max(Device.ShleifNo - 1, 0), "Номер шлейфа");
			BytesDatabase.AddShort(0, "Внутренние параметры", true);
			BytesDatabase.AddByte(0, "Динамические параметры для базы", true);

			var description = Device.Description;
			if (string.IsNullOrEmpty(description))
			{
				description = Device.Driver.ShortName + " ";
				if (IsRemote)
				{
					description += Device.ParentPanel.IntAddress.ToString() + ".";
				}
				else
				{
					description += "0.";
				}
				var shleifNo = Device.ShleifNo;
				if (Device.Driver.DriverType == DriverType.PumpStation)
					shleifNo = 1;
				if (Device.Driver.DriverType == DriverType.Exit)
					shleifNo = Device.AddressOnShleif;
				description += shleifNo.ToString() + "." + Device.AddressOnShleif.ToString();
			}
			BytesDatabase.AddString(description, "Описание");

			var configLengtByteDescription = BytesDatabase.AddByte(0, "Длина переменной части блока с конфигурацией и сырыми параметрами");
			var lengtByteDescription = BytesDatabase.AddShort(0, "Общая длина записи");
			var configLengt1 = BytesDatabase.ByteDescriptions.Count;
			AddDynamicBlock();
			AddConfig();
			var configLengt2 = BytesDatabase.ByteDescriptions.Count;
			configLengtByteDescription.Value = configLengt2 - configLengt1;

			if (Device.Driver.DriverType != DriverType.MPT)
			{
				AddLogic();
			}
			else
			{
				BytesDatabase.AddByte(0, "Пустая логика для МПТ");
				BytesDatabase.AddByte(0, "Пустая логика для МПТ");
				BytesDatabase.AddByte(0, "Пустая логика для МПТ");
				BytesDatabase.AddByte(0, "Пустая логика для МПТ");
				BytesDatabase.AddByte(0, "Пустая логика для МПТ");
			}
			var length = BytesDatabase.ByteDescriptions.Count;
			if (IsRemote)
			{
				length -= 1;
			}
			BytesDatabase.SetShort(lengtByteDescription, length);

			if (Device.Driver.DriverType == DriverType.PumpStation &&
				(ParentPanel.Driver.DriverType == DriverType.BUNS || ParentPanel.Driver.DriverType == DriverType.BUNS_2 || ParentPanel.Driver.DriverType == DriverType.USB_BUNS))
			{
				AddPumpStation();
			}
		}

		void AddDynamicBlock()
		{
			var count = 2;
			switch (Device.Driver.DriverType)
			{
				case DriverType.Valve:
					count = 6;
					break;

				case DriverType.MPT:
					count = 5;
					break;

				case DriverType.PumpStation:
					count = 0;
					break;

				case DriverType.MDU:
					count = 4;
					break;

				case DriverType.MRO_2:
					count = 3;
					break;

				case DriverType.Exit:
					count = 1;
					break;
			}

			for (int i = 0; i < count; i++)
			{
				BytesDatabase.AddByte(0, "Сырой параметр устройства " + (i + 1).ToString());
			}
		}

		void AddConfig()
		{
			var config = 0;
			switch (Device.Driver.DriverType)
			{
				case DriverType.MPT:
					var isAutoBlock = false;
					var isAutoBlockProperty = Device.Properties.FirstOrDefault(x => x.Name == "Config");
					if ((isAutoBlockProperty == null) || (isAutoBlockProperty.Value == null))
						isAutoBlock = false;
					else
						isAutoBlock = true;
					if (isAutoBlock)
						config += 2;
					break;

				case DriverType.Valve:
					var valveAction = 0;
					var actionProperty = Device.Properties.FirstOrDefault(x => x.Name == "Action");
					if ((actionProperty == null) || (actionProperty.Value == null))
						valveAction = 0;
					else
						valveAction = 1;
					config = valveAction;
					break;

				case DriverType.MDU:
					var mduType = 0;
					var mduTypeProperty = Device.Properties.FirstOrDefault(x => x.Name == "Начальное положение для привода пружинный ДУ");
					if ((mduTypeProperty == null) || (mduTypeProperty.Value == null))
						mduType = 0;
					else
						mduType = Int32.Parse(mduTypeProperty.Value);
					config = mduType;
					break;

				case DriverType.RM_1:
					if (Device.IsRmAlarmDevice)
						config += 0x80;
					var childCount = GetRmChildCount();
					if (childCount > 0)
					{
						var childIndex = Device.Parent.Children.IndexOf(Device);
						config += (childIndex << 1);
						config += 16;
					}
					break;
			}
			BytesDatabase.AddByte(config, "Конфиг");

			switch (Device.Driver.DriverType)
			{
				case DriverType.MPT:
					var mptParentAddress = 0;
					var mptParentShleif = 0;
					if (Device.Parent.Driver.DriverType == DriverType.MPT)
					{
						mptParentAddress = Device.Parent.AddressOnShleif;
						mptParentShleif = Device.Parent.ShleifNo - 1;
					}
					BytesDatabase.AddByte(mptParentAddress, "Адрес родителя");
					BytesDatabase.AddByte(mptParentShleif, "Шлейф родителя");
					int delay = 0;
					var delayProperty = Device.Properties.FirstOrDefault(x => x.Name == "RunDelay");
					if ((delayProperty == null) || (delayProperty.Value == null))
						delay = 0;
					else
						delay = int.Parse(delayProperty.Value);
					BytesDatabase.AddShort(delay, "Задержка запуска");
					var localZoneNo = 0;
					if (Device.Zone != null)
					{
						var binaryZone = PanelDatabase.BinaryPanel.BinaryLocalZones.FirstOrDefault(x => x.Zone == Device.Zone);
						if (binaryZone != null)
						{
							localZoneNo = binaryZone.LocalNo;
						}
					}
					BytesDatabase.AddShort(localZoneNo, "Номер привязанной зоны");

					BytesDatabase.AddByte(0, "Пустой байт");
					BytesDatabase.AddByte(0, "Пустой байт");
					BytesDatabase.AddByte(0, "Пустой байт");

					break;

				case DriverType.MRO_2:
					var mroParentAddress = 0;
					var mroParentShleif = 0;
					if (Device.Parent.Driver.DriverType == DriverType.MRO_2)
					{
						mroParentAddress = Device.Parent.AddressOnShleif;
						mroParentShleif = Device.Parent.ShleifNo - 1;
					}
					BytesDatabase.AddByte(mroParentAddress, "Адрес родителя");
					BytesDatabase.AddByte(mroParentShleif, "Шлейф родителя");
					break;

				case DriverType.MDU:
					//var outDevicesCount = 0;
					//BytesDatabase.AddShort(outDevicesCount, "Общее количество связанных ИУ");
					//if (outDevicesCount > 0)
					//{
					//    for (int i = 0; i < ParentPanel.ShleifNo; i++)
					//    {
					//        BytesDatabase.AddByte(0, "Количество связанных ИУ шлейфа " + (i + 1).ToString());
					//        BytesDatabase.AddReference((ByteDescription)null, "Указатель на размещение абсолютного адреса размещения первого саиска связанного ИУ шлейфа " + (i + 1).ToString());
					//    }
					//}
					break;
			}
		}

        void AddLogic()
        {
            var clauses = new List<Clause>();
            {
                foreach (var clause in Device.ZoneLogic.Clauses)
                {
                    var actualZoneTables = new List<TableBase>();
                    foreach (var zone in clause.Zones)
                    {
                        var binaryPanels = new HashSet<Device>();
                        foreach (var zoneDevice in zone.DevicesInZone)
                        {
                            binaryPanels.Add(zoneDevice.ParentPanel);
                        }

						if (!IsRemote)
						{
							foreach (var binaryPanel in binaryPanels)
							{
								foreach (var table in PanelDatabase.RemoteZonesTableGroup.Tables)
								{
									if ((table as RemoteZoneTable).Zone.No == zone.No && (table as RemoteZoneTable).BinaryZone.ParentPanel.UID == binaryPanel.UID)
									{
										actualZoneTables.Add(table);
									}
								}
							}
						}

                        foreach (var binaryPanel in binaryPanels)
                        {
                            foreach (var table in PanelDatabase.LocalZonesTableGroup.Tables)
                            {
                                if ((table as ZoneTable).Zone.No == zone.No && (table as ZoneTable).BinaryZone.ParentPanel.UID == binaryPanel.UID)
                                {
                                    actualZoneTables.Add(table);
                                }
                            }
                        }
                    }

                    var zonesOrDevicesCount = 0;
                    zonesOrDevicesCount = actualZoneTables.Count;
					if(clause.State == ZoneLogicState.Failure)
						zonesOrDevicesCount = 1;

                    if (zonesOrDevicesCount > 0)
                    {
                        clauses.Add(clause.Clone());
                    }
                }
            }

            if (clauses.Count == 0)
            {
                BytesDatabase.AddByte(0, "Пустая логика");
                BytesDatabase.AddByte(0, "Пустая логика");
                BytesDatabase.AddByte(0, "Пустая логика");
                BytesDatabase.AddByte(0, "Пустая логика");
                BytesDatabase.AddByte(0, "Пустая логика");
                return;
            }

            foreach (var clause in clauses)
            {
                var actualZoneTables = new HashSet<TableBase>();
                foreach (var zone in clause.Zones)
                {
                    var binaryPanels = new HashSet<Device>();
                    foreach (var zoneDevice in zone.DevicesInZone)
                    {
                        binaryPanels.Add(zoneDevice.ParentPanel);
                    }

					if (!IsRemote)
					{
						foreach (var binaryPanel in binaryPanels)
						{
							foreach (var table in PanelDatabase.RemoteZonesTableGroup.Tables)
							{
								var remoteZoneTable = table as RemoteZoneTable;
								if (remoteZoneTable.Zone.No == zone.No && remoteZoneTable.BinaryZone.ParentPanel.UID == binaryPanel.UID)
								//if (remoteZoneTable.Zone.No == zone.No && remoteZoneTable.Zone.DevicesInZone.Any(x=>x.ParentPanel.UID == binaryPanel.UID))
								{
									actualZoneTables.Add(table);
								}
							}
						}
					}

                    foreach (var binaryPanel in binaryPanels)
                    {
                        foreach (var table in PanelDatabase.LocalZonesTableGroup.Tables)
                        {
							var zoneTable = table as ZoneTable;
							if (zoneTable.Zone.No == zone.No && zoneTable.BinaryZone.ParentPanel.UID == binaryPanel.UID)
							//if (zoneTable.Zone.No == zone.No && zoneTable.Zone.DevicesInZone.Any(x => x.ParentPanel.UID == binaryPanel.UID))
                            {
                                actualZoneTables.Add(table);
                            }
                        }
                    }
                }

                var zonesOrDevicesCount = 0;
                zonesOrDevicesCount = actualZoneTables.Count;
                if (clause.Device != null)
                    zonesOrDevicesCount = 1;

                var joinOperation = clause.Operation.Value == ZoneLogicOperation.All ? 1 : 2;
                var mroLogic = 0;
                joinOperation += mroLogic;
                BytesDatabase.AddByte(joinOperation, "Логика внутри группы зон");

                var state = 0;
                switch (clause.State)
                {
                    case ZoneLogicState.MPTAutomaticOn:
                        state = 0x01;
                        break;

                    case ZoneLogicState.Alarm:
                        state = 0x02;
                        break;

                    case ZoneLogicState.GuardSet:
                        state = 0x03;
                        break;

                    case ZoneLogicState.GuardUnSet:
                        state = 0x05;
                        break;

                    case ZoneLogicState.PCN:
                        state = 0x06;
                        break;

                    case ZoneLogicState.Fire:
                        state = 0x04;
                        break;

                    case ZoneLogicState.Failure:
                        state = 0x08;
                        break;

                    case ZoneLogicState.PumpStationOn:
                        state = 0x09;
                        break;

                    case ZoneLogicState.PumpStationAutomaticOff:
                        state = 0x0A;
                        break;

                    case ZoneLogicState.Attention:
                        state = 0x20;
                        break;

                    case ZoneLogicState.MPTOn:
                        state = 0x40;
                        break;

                    case ZoneLogicState.Firefighting:
                        state = 0x80;
                        break;

                    case ZoneLogicState.AM1TOn:
                        state = 0x0B;
                        break;
                }
                BytesDatabase.AddByte(state, "Состояние");

                var joinOperator = 0;
                if (clauses.IndexOf(clause) < clauses.Count - 1)
                {
                    switch (Device.ZoneLogic.JoinOperator)
                    {
                        case ZoneLogicJoinOperator.And:
                            joinOperator = 1;
                            break;

                        case ZoneLogicJoinOperator.Or:
                            joinOperator = 2;
                            break;
                    }
                }
                BytesDatabase.AddByte(joinOperator, "Логика операции со следующей логической группой");

                if (zonesOrDevicesCount > 0 || clauses.IndexOf(clause) == 0)
                {
                    BytesDatabase.AddShort(zonesOrDevicesCount, "Количество зон в этой группе или ИУ, по активации которого должно включится");
                    foreach (var tableBase in actualZoneTables)
                    {
                        BytesDatabase.AddReferenceToTable(tableBase, tableBase.BytesDatabase.Name + " Указатель на участвующую в логике зону, в которой не локальные ИП управляют данным локальным ИУ по логике межприборное И или ИУ, по активации которого должно включится");
                    }
                }
            }
        }

		int GetRmChildCount()
		{
			switch(Device.Parent.Driver.DriverType)
			{
				case DriverType.RM_2:
					return 2;

				case DriverType.RM_3:
					return 3;

				case DriverType.RM_4:
					return 4;

				case DriverType.RM_5:
					return 5;
			}
			return 0;
		}

		void AddPumpStation()
		{
			BytesDatabase.AddByte(GetProperty("StartDelay"), "Тайм-аут пуска");
			BytesDatabase.AddShort(GetProperty("DistTime"), "Время тушения");
			BytesDatabase.AddByte(GetProperty("PumpTime"), "Время разновременного пуска насосов");
			BytesDatabase.AddByte(GetProperty("PumpCount"), "Количество насосов, необходимых для тушения");
			BytesDatabase.AddByte(Device.Children.Count, "Число записей в переменной части");

			foreach (var pumpDevice in Device.Children.OrderBy(x => x.PumpAddress))
			{
				var timeoutValue = 0;
				try
				{
					var property = pumpDevice.Properties.FirstOrDefault(x => x.Name == "Time");
					if (property != null)
						timeoutValue = Int32.Parse(property.Value);
				}
				catch { }

				BytesDatabase.AddByte(0, "Битовое поле признаков РУ");
				BytesDatabase.AddByte(pumpDevice.PumpAddress, "Адрес насоса " + pumpDevice.PumpAddress);
				BytesDatabase.AddShort(0, "№ аппаратной версии");
				BytesDatabase.AddShort(0, "Заводской номер");
				BytesDatabase.AddByte(timeoutValue % 256, "Параметр 1");
				BytesDatabase.AddByte(timeoutValue / 256, "Параметр 1");
				BytesDatabase.AddShort(0, "Параметр 2");
				BytesDatabase.AddShort(0, "Параметр 3");
			}
		}

		int GetProperty(string name)
		{
			var propertyValue = 0;
			try
			{
				var property = Device.Parent.Properties.FirstOrDefault(x => x.Name == name);
				if (property != null)
					propertyValue = Int32.Parse(property.Value);
			}
			catch { }
			return propertyValue;
		}
	}
}