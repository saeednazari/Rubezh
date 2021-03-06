﻿using System;

namespace Common
{
	public static class GuidHelper
	{
		public static string ToString(Guid guid)
		{
			if (guid == Guid.Empty)
				return null;
			return guid.ToString();
		}

		public static Guid ToGuid(string val)
		{
			if (val == null)
				return Guid.Empty;
			return new Guid(val);
		}

		public static Guid CreateOn(Guid guid)
		{
			var stringUID = guid.ToString();
			var firstChar = stringUID[0];
			if (firstChar == '0')
				firstChar = '1';
			else
				firstChar = '0';

			var newStringUID = firstChar + stringUID.Substring(1);
			var newGuid = new Guid(newStringUID);
			return newGuid;
		}
	}
}