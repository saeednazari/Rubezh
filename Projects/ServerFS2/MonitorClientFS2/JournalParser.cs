﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using ServerFS2;
using ServerFS2.DataBase;

namespace MonitorClientFS2
{
	public class JournalParser
	{
		public static FSJournalItem FSParce(List<byte> allbytes)
		{
			List<byte> _allBytes = allbytes;
			List<byte> _bytes = new List<byte>(_allBytes);
			_bytes.RemoveRange(0, 7);
			FSJournalItem _fsjournalItem = new FSJournalItem();

			var timeBytes = _bytes.GetRange(1, 4);
			_fsjournalItem.DeviceTime = ParceDateTime(timeBytes);
			_fsjournalItem.SystemTime = DateTime.Now;

			_fsjournalItem.Description = GetEvent(_bytes);

			var ShleifNo = _bytes[6] + 1;
			if (_bytes[0] == 0x83)
				_fsjournalItem.Detalization += "Выход: " + ShleifNo + "\n";
			if (_bytes[0] == 0x0F)
				_fsjournalItem.Detalization += "АЛС: " + ShleifNo + "\n";

			_fsjournalItem.DeviceCategory = _bytes[7];

			var PanelAddress = _allBytes[5];
			var Panel = ConfigurationManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.AddressFullPath == PanelAddress.ToString());
			_fsjournalItem.PanelName = Panel.Driver.Name;
			_fsjournalItem.PanelUID = Panel.Driver.UID;

			var FirstAddress = _bytes[17] + 1;
			var Address = _bytes[8];

			var byteState = _bytes[9];
			_fsjournalItem.StateType = (StateType)byteState;

			// Системная неисправность
			if (_bytes[0] == 0x0D && byteState == 0x20)
			{
				_fsjournalItem.Detalization += "база (сигнатура) повреждена или отсутствует\n";
			}

			SetZone(_bytes, _fsjournalItem);

			//Охранные события (сброс тревоги, постановка, снятие)
			GuardEvents(_allBytes, _fsjournalItem);

			// Потеря связи с мониторинговой станцией (БИ, ПДУ, УОО-ТЛ, МС-1, МС-2)
			LostConnection(_bytes, _fsjournalItem);

			_fsjournalItem.DeviceUID = MetadataHelper.GetUidById((ushort)_fsjournalItem.DeviceCategory);
			var deviceUid = _fsjournalItem.DeviceUID.ToString().ToUpper();

			var device = MetadataHelper.Metadata.deviceTables.FirstOrDefault(x => ((x.deviceDriverID != null) && (x.deviceDriverID.Equals(deviceUid))));

			if (device != null)
			{
				if (_fsjournalItem.DeviceCategory == 1)
					_fsjournalItem.DeviceName = "АСПТ " + (ShleifNo - 1) + ".";
				else
					_fsjournalItem.DeviceName = device.shortName;
			}
			else
				_fsjournalItem.DeviceName = "Неизвестное устройство";

			int tableType = 99999;

			if (deviceUid != "00000000-0000-0000-0000-000000000000")
			{
				if (_fsjournalItem.DeviceCategory != 0)
				{
					tableType = Convert.ToInt32(MetadataHelper.Metadata.deviceTables.FirstOrDefault(x => ((x.deviceDriverID != null) && (x.deviceDriverID.Equals(deviceUid)))).tableType);
				}
			}

			_fsjournalItem.Detalization += "Устройство: " + _fsjournalItem.DeviceName + " " + FirstAddress + "." + Address + "\n";

			// Детализация событий
			if (tableType != 99999)
			{
				var even = MetadataHelper.Metadata.events.FirstOrDefault(x => x.rawEventCode == "$" + _bytes[0].ToString("X2"));
				if (even.detailsFor != null)
				{
					var details = even.detailsFor.FirstOrDefault(x => x.tableType == tableType.ToString());
					if (details != null)
					{
						var dictionaryName =
							even.detailsFor.FirstOrDefault(x => x.tableType == tableType.ToString()).dictionary;
						var dictionary = MetadataHelper.Metadata.dictionary.FirstOrDefault(x => x.name == dictionaryName);
						var bitState = new BitArray(new int[] { byteState });
						foreach (var bit in dictionary.bit)
						{
							if (bitState.Get(Convert.ToInt32(bit.no)))
								_fsjournalItem.Detalization += dictionary.bit.FirstOrDefault(x => x.no == bit.no).value + "\n";
						}
					}
				}
			}

			_fsjournalItem.UserName = "Usr";
			return _fsjournalItem;
		}

		// Потеря связи с мониторинговой станцией (БИ, ПДУ, УОО-ТЛ, МС-1, МС-2)
		private static void LostConnection(List<byte> _bytes, FSJournalItem _fsjournalItem)
		{
			if (_bytes[0] == 0x85)
			{
				switch (_bytes[7])
				{
					case 3:
						_fsjournalItem.Detalization += "Прибор: Рубеж-БИ Адрес:" + _bytes[6] + "\n";
						break;
					case 7:
						_fsjournalItem.Detalization += "Прибор: Рубеж-ПДУ Адрес:" + _bytes[6] + "\n";
						break;
					case 100:
						_fsjournalItem.Detalization += "Устройство: МС-3 Адрес:" + _bytes[6] + "\n";
						break;
					case 101:
						_fsjournalItem.Detalization += "Устройство: МС-4 Адрес:" + _bytes[6] + "\n";
						break;
					case 102:
						_fsjournalItem.Description += "Устройство: УОО-ТЛ Адрес:" + _bytes[6] + "\n";
						break;
					default:
						_fsjournalItem.Detalization += "Неизв. устр." + "(" + _bytes[7] + ") Адрес:" + _bytes[6] + "\n";
						break;
				}
			}
		}

		//Охранные события (сброс тревоги, постановка, снятие)
		private static void GuardEvents(List<byte> _allBytes, FSJournalItem _fsjournalItem)
		{
			if (_allBytes[7] == 0x28)
			{
				switch (_allBytes[24])
				{
					case 0:
						{
							_fsjournalItem.Detalization += "команда с компьютера\n";
							if (_allBytes[23] == 0)
								_fsjournalItem.Detalization += "через USB\n";
							else
								_fsjournalItem.Detalization += "через канал МС " + _allBytes[23] + "\n";
							break;
						}
					case 3:
						_fsjournalItem.Detalization += "Прибор: Рубеж-БИ Адрес:" + _allBytes[23] + "\n";
						break;
					case 7:
						_fsjournalItem.Detalization += "Прибор: Рубеж-ПДУ Адрес:" + _allBytes[23] + "\n";
						break;
					case 9:
						_fsjournalItem.Detalization += "Прибор: Рубеж-ПДУ-ПТ Адрес:" + _allBytes[23] + "\n";
						break;
					case 100:
						_fsjournalItem.Detalization += "Устройство: МС-3 Адрес:" + _allBytes[23] + "\n";
						break;
					case 101:
						_fsjournalItem.Detalization += "Устройство: МС-4 Адрес:" + _allBytes[23] + "\n";
						break;
					case 102:
						_fsjournalItem.Detalization += "Устройство: УОО-ТЛ Адрес:" + _allBytes[23] + "\n";
						break;
					default:
						_fsjournalItem.Detalization += "Неизв. устр." + "(" + _allBytes[24] + ") Адрес:" + _allBytes[23] + "\n";
						break;
				}
				if (_fsjournalItem.DeviceCategory == 0x00)
					_fsjournalItem.DeviceCategory = 0x75;
			}
		}

		private static void SetZone(List<byte> _bytes, FSJournalItem _fsjournalItem)
		{
			var zone = ConfigurationManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == _bytes[10] * 256 + _bytes[11]);
			if (zone == null)
			{
				_fsjournalItem.ZoneName = "";
			}
			else
			{
				_fsjournalItem.ZoneName = zone.Name;
				_fsjournalItem.Detalization += "Зона: " + zone.No + "\n";
			}
		}

		private static string GetEvent(List<byte> _bytes)
		{
			int flag = _bytes[5];
			var eventName = MetadataHelper.GetEventByCode(_bytes[0]);
			var firstIndex = eventName.IndexOf("[");
			var lastIndex = eventName.IndexOf("]");
			if (firstIndex != -1 && lastIndex != -1)
			{
				var firstPart = eventName.Substring(0, firstIndex);
				var secondPart = eventName.Substring(firstIndex + 1, lastIndex - firstIndex - 1);
				var thirdPart = eventName.Substring(lastIndex + 1);
				var secondParts = secondPart.Split('/');
				if (flag < secondParts.Count())
				{
					var choise = secondParts[flag];
					return firstPart + choise + thirdPart;
				}
			}
			return eventName;
		}

		private static DateTime ParceDateTime(List<byte> bytes)
		{
			var bitsExtracter = new BitsExtracter(bytes);
			var day = bitsExtracter.Get(0, 4);
			var month = bitsExtracter.Get(5, 8);
			var year = bitsExtracter.Get(9, 14);
			var hour = bitsExtracter.Get(15, 19);
			var min = bitsExtracter.Get(20, 25);
			var sec = bitsExtracter.Get(26, 31);

			var resultString = day.ToString() + "/" + month.ToString() + "/" + (year + 2000).ToString() + " " + hour.ToString() + ":" + min.ToString() + ":" + sec.ToString();
			return DateTime.Parse(resultString);
		}
	}
}