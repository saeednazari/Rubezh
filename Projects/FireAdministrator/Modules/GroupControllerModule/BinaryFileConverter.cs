﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GKModule.Database;
using System.IO;
using FiresecClient;

namespace GKModule
{
	public static class BinaryFileConverter
	{
		public static void Convert3()
		{
			DatabaseProcessor.Convert();

			var stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("<congig>");

			var gkDatabase = DatabaseProcessor.DatabaseCollection.GkDatabases.First();
			CreateConfigurationFile(gkDatabase, @"D:\GKConfig\GK.GKBIN");
			stringBuilder.AppendLine("<gk name=\"GK.GKBIN\" description=\"описание ГК\"/>");

			foreach (var kauDatabase in DatabaseProcessor.DatabaseCollection.KauDatabases)
			{
				var kauDevice = kauDatabase.Devices[0];
				short lineNo = XManager.GetKauLine(kauDevice);
				string lineTypeName = lineNo == 0 ? "baseline" : "reserveline";
				string fileName = "Kau" + kauDevice.Address + ".GKBIN";
				CreateConfigurationFile(kauDatabase, @"D:\GKConfig\" + fileName);
				stringBuilder.AppendLine("<kau name=\"" + fileName + "\" line=\"" + lineTypeName + "\" address=\"" + kauDevice.Address + "\" description=\"описание КАУ\"/>");
			}

			stringBuilder.AppendLine("</congig>");

			using (var streamWriter = new StreamWriter(@"D:\GKConfig\GK.gkprj"))
			{
				streamWriter.Write(stringBuilder.ToString());
			}
		}

		static void CreateConfigurationFile(CommonDatabase commonDatabase, string fileName)
		{
			var fileBytes = new List<byte>();
			fileBytes.Add(0x25);
			fileBytes.Add(0x08);
			fileBytes.Add(0x19);
			fileBytes.Add(0x65);
			fileBytes.AddRange(BytesHelper.ShortToBytes((short)(commonDatabase.BinaryObjects.Count + 1)));
			fileBytes.AddRange(BytesHelper.ShortToBytes((short)0));
			fileBytes.AddRange(BytesHelper.ShortToBytes((short)0));
			fileBytes.AddRange(BytesHelper.ShortToBytes((short)0));
			fileBytes.AddRange(BytesHelper.ShortToBytes((short)0));

			foreach (var binaryObject in commonDatabase.BinaryObjects)
			{
				fileBytes.AddRange(CreateDescriptor(binaryObject, true));
			}
			fileBytes.AddRange(CreateEndDescriptor((short)commonDatabase.BinaryObjects.Count));

			using (var fileStream = new FileStream(fileName, FileMode.Create))
			{
				fileStream.Write(fileBytes.ToArray(), 0, fileBytes.Count);
			}
		}

		public static void Convert1()
		{
			DatabaseProcessor.Convert();
			var gkDatabase = DatabaseProcessor.DatabaseCollection.GkDatabases.First();

			var fileBytes = new List<byte>();
			fileBytes.Add(0x25);
			fileBytes.Add(0x08);
			fileBytes.Add(0x19);
			fileBytes.Add(0x65);
			fileBytes.AddRange(BytesHelper.ShortToBytes((short)(gkDatabase.BinaryObjects.Count + 1)));
			fileBytes.AddRange(BytesHelper.ShortToBytes((short)0));
			fileBytes.AddRange(BytesHelper.ShortToBytes((short)0));
			fileBytes.AddRange(BytesHelper.ShortToBytes((short)0));
			fileBytes.AddRange(BytesHelper.ShortToBytes((short)0));

			foreach (var binaryObject in gkDatabase.BinaryObjects)
			{
				fileBytes.AddRange(CreateDescriptor(binaryObject, true));
			}
			fileBytes.AddRange(CreateEndDescriptor((short)gkDatabase.BinaryObjects.Count));

			using (var fileStream = new FileStream(@"D:\GKConfig\GK.GKBIN", FileMode.Create))
			{
				fileStream.Write(fileBytes.ToArray(), 0, fileBytes.Count);
			}

			using (var streamWriter = new StreamWriter(@"D:\GKConfig\GK.gkprj"))
			{
				var stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("<congig>");
				stringBuilder.AppendLine("<gk name=\"GK.GKBIN\" description=\"описание ГК\"/>");
				stringBuilder.AppendLine("</congig>");

				streamWriter.Write(stringBuilder.ToString());
			}
		}

		public static void Convert2()
		{
			DatabaseProcessor.Convert();
			var gkDatabase = DatabaseProcessor.DatabaseCollection.GkDatabases.First();

			foreach (var binaryObject in gkDatabase.BinaryObjects)
			{
				var fileName = "dsc" + String.Format("{0:00000}", binaryObject.GetNo()) + ".gk";
				var bytes = binaryObject.AllBytes;
				using (var fileStream = new FileStream(@"D:\GKConfig\" + fileName, FileMode.Create))
				{
					fileStream.Write(bytes.ToArray(), 0, bytes.Count);
				}
			}
		}

		public static List<byte> CreateDescriptor(BinaryObjectBase binaryObject, bool incluteDescription)
		{
			var resultBytes = new List<byte>();
			var bytes = binaryObject.AllBytes;

			resultBytes.AddRange(BytesHelper.ShortToBytes((short)(binaryObject.GetNo())));
			resultBytes.Add(1);
			//resultBytes.AddRange(BytesHelper.ShortToBytes((short)bytes.Count));
			if (incluteDescription)
			{
				if (binaryObject.Device != null)
					resultBytes.AddRange(BytesHelper.StringDescriptionToBytes("Устройство " + binaryObject.Device.Driver.DriverType.ToString(), 33));
				if (binaryObject.Zone != null)
					resultBytes.AddRange(BytesHelper.StringDescriptionToBytes("Зона " + binaryObject.Device.Driver.DriverType.ToString(), 33));
			}
			resultBytes.AddRange(bytes);
			//var resultButesCount = resultBytes.Count;
			//for (int i = 0; i < 256 - resultButesCount; i++)
			//{
			//    resultBytes.Add(0);
			//}
			return resultBytes;
		}

		public static List<byte> CreateEndDescriptor(short descriptorNo)
		{
			var resultBytes = new List<byte>();
			resultBytes.AddRange(BytesHelper.ShortToBytes(descriptorNo));
			resultBytes.Add(1);
			//resultBytes.AddRange(BytesHelper.ShortToBytes((short)2));
			//resultBytes.AddRange(BytesHelper.StringDescriptionToBytes("Завершающий дескриптор", 33));
			resultBytes.Add(255);
			resultBytes.Add(255);
			//for (int i = 0; i < 256 - 2; i++)
			//{
			//    resultBytes.Add(0);
			//}
			return resultBytes;
		}
	}
}