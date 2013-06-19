﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using ServerFS2.Monitoring;
using ServerFS2.Service;
using FS2Api;

namespace ServerFS2
{
	public class ReadPanelDatabaseOperationHelper
	{
		public int RomDBFirstIndex;
		public int FlashDBLastIndex;
		bool CheckMonitoringSuspend = false;

		public ReadPanelDatabaseOperationHelper(bool checkMonitoringSuspend)
		{
			CheckMonitoringSuspend = checkMonitoringSuspend;
		}

		void CheckSuspending()
		{
			if (CheckMonitoringSuspend)
				MonitoringProcessor.CheckSuspending();
		}

		public List<byte> GetRomDBBytes(Device device)
		{
			var packetLenght = USBManager.IsUsbDevice(device) ? 0x33 : 0xFF;
			var response = USBManager.SendCodeToPanel(device, 0x38, BitConverter.GetBytes(RomDBFirstIndex).Reverse(), packetLenght);
			var result = response.Bytes;
			var romDBLastIndex = BytesHelper.ExtractTriple(response.Bytes, 9);

			var numberOfPackets = romDBLastIndex - RomDBFirstIndex / packetLenght;

			for (var i = RomDBFirstIndex + packetLenght + 1; i < romDBLastIndex; i += packetLenght + 1)
			{
				var packetNo = (i - RomDBFirstIndex) / packetLenght;
				CheckSuspending();
				CallbackManager.AddProgress(new FS2ProgressInfo("Чтение базы ROM " + packetNo + " из " + numberOfPackets));
				var length = Math.Min(packetLenght, romDBLastIndex - i);
				response = USBManager.SendCodeToPanel(device, 0x38, BitConverter.GetBytes(i).Reverse(), length);
				result.AddRange(response.Bytes);
			}
			return result;
		}

		public List<byte> GetFlashDBBytes(Device device)
		{
			var packetLenght = USBManager.IsUsbDevice(device) ? 0x33 : 0xFF;
			var result = new List<byte>();

			var numberOfPackets = FlashDBLastIndex - 0x100 / packetLenght;

			for (var i = 0x100; i < FlashDBLastIndex; i += packetLenght + 1)
			{
				var packetNo = (i - FlashDBLastIndex) / packetLenght;
				CheckSuspending();
				CallbackManager.AddProgress(new FS2ProgressInfo("Чтение базы FLASH " + packetNo + " из " + numberOfPackets));
				var length = Math.Min(packetLenght, FlashDBLastIndex - i);
				var response = USBManager.SendCodeToPanel(device, 0x01, 0x52, BitConverter.GetBytes(i).Reverse(), length);
				result.AddRange(response.Bytes);
			}
			var nullbytes = new List<byte>();
			for (var i = 0; i < 0x100; i++)
				nullbytes.Add(0);
			result.InsertRange(0, nullbytes);
			return result;
		}

		public int GetRomFirstIndex(Device device)
		{
			CheckSuspending();
			CallbackManager.AddProgress(new FS2ProgressInfo("Запрос размера ROM части базы"));
			var response = USBManager.SendCodeToPanel(device, 0x01, 0x57);
			return BytesHelper.ExtractTriple(response.Bytes, 1);
		}

		public int GetFlashLastIndex(Device device)
		{
			CheckSuspending();
			CallbackManager.AddProgress(new FS2ProgressInfo("Запрос размера FLASH части базы"));
			var response = USBManager.SendCodeToPanel(device, 0x38, BitConverter.GetBytes(RomDBFirstIndex).Reverse(), 0x0B);
			return BytesHelper.ExtractTriple(response.Bytes, 6);
		}
	}
}