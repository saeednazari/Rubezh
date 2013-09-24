﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GKModule.ViewModels
{
	public static class HexHelper
	{
		public static List<byte> HexFileToBytesList(string filePath)
		{
			var bytes = new List<byte>();
			var strings = File.ReadAllLines(filePath).ToList();
			strings.RemoveAt(0);
			strings.RemoveRange(strings.Count - 2, 2);
			foreach (var str in strings)
			{
				var count = Convert.ToInt32(str.Substring(1, 2), 16);
				for (var i = 9; i < count * 2 + 9; i += 2)
				{
					bytes.Add(Convert.ToByte(str.Substring(i, 2), 16));
				}
			}
			return bytes;
		}
	}
}