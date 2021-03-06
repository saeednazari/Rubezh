﻿using XFiresecAPI;

namespace GKProcessor
{
	public static class GKDriversHelper
	{
		public static XDriverProperty AddPlainEnumProprety(XDriver driver, byte no, byte offset, string propertyName, string parameter1Name, string parameter2Name)
		{
			var property = new XDriverProperty()
			{
				No = no,
				Name = propertyName,
				Caption = propertyName,
				Default = (ushort)(1 << offset),
				Mask = (short)((1 << offset) + (1 << (offset + 1)))
			};
			var parameter1 = new XDriverPropertyParameter()
			{
				Name = parameter1Name,
				Value = (ushort)(1 << offset)
			};
			var parameter2 = new XDriverPropertyParameter()
			{
				Name = parameter2Name,
				Value = (ushort)(2 << offset)
			};
			property.Parameters.Add(parameter1);
			property.Parameters.Add(parameter2);
			driver.Properties.Add(property);
			return property;
		}

		public static XDriverProperty AddPlainEnumProprety2(XDriver driver, byte no, string propertyName, byte offset, string parameter1Name, string parameter2Name, int mask)
		{
			var property = new XDriverProperty()
			{
				No = no,
				Name = propertyName,
				Caption = propertyName,
				Default = 0,
				Mask = (byte)mask
			};
			var parameter1 = new XDriverPropertyParameter()
			{
				Name = parameter1Name,
				Value = (ushort)0
			};
			var parameter2 = new XDriverPropertyParameter()
			{
				Name = parameter2Name,
				Value = (ushort)mask
			};
			property.Parameters.Add(parameter1);
			property.Parameters.Add(parameter2);
			driver.Properties.Add(property);
			return property;
		}

		public static void AddBoolProprety(XDriver driver, byte no, string propertyName)
		{
			var property = new XDriverProperty()
			{
				No = no,
				Name = propertyName,
				Caption = propertyName,
				DriverPropertyType = XDriverPropertyTypeEnum.BoolType
			};
			driver.Properties.Add(property);
		}

		public static XDriverProperty AddIntProprety(XDriver driver, byte no, string propertyName, int defaultValue, int min, int max)
		{
			var property = new XDriverProperty()
			{
				No = no,
				Name = propertyName,
				Caption = propertyName,
				DriverPropertyType = XDriverPropertyTypeEnum.IntType,
				Default = (ushort)defaultValue,
				Min = (ushort)min,
				Max = (ushort)max
			};
			driver.Properties.Add(property);
			return property;
		}

		public static void AddPropertyParameter(XDriverProperty property, string name, int value)
		{
			var parameter = new XDriverPropertyParameter()
			{
				Name = name,
				Value = (ushort)value
			};
			property.Parameters.Add(parameter);
		}

		public static void AddAvailableStateBits(XDriver driver, XStateBit stateType)
		{
			if (driver.AvailableStateBits.Count == 0)
			{
				driver.AvailableStateBits.Add(XStateBit.Norm);
				driver.AvailableStateBits.Add(XStateBit.Failure);
				driver.AvailableStateBits.Add(XStateBit.Ignore);
			}
			driver.AvailableStateBits.Add(stateType);
		}

		public static void AddControlAvailableStates(XDriver driver)
		{
			AddAvailableStateBits(driver, XStateBit.On);
		}

		public static void AddAvailableStateClasses(XDriver driver, XStateClass stateClass)
		{
			if (driver.AvailableStateClasses.Count == 0)
			{
				driver.AvailableStateClasses.Add(XStateClass.No);
				driver.AvailableStateClasses.Add(XStateClass.Norm);
				driver.AvailableStateClasses.Add(XStateClass.Failure);
				driver.AvailableStateClasses.Add(XStateClass.Ignore);
				driver.AvailableStateClasses.Add(XStateClass.Unknown);
			}
			driver.AvailableStateClasses.Add(stateClass);
		}

		public static void AddDefaultStateBitsClasses(XDriver driver)
		{
			driver.AvailableStateBits.Add(XStateBit.Norm);
			driver.AvailableStateBits.Add(XStateBit.Failure);
			driver.AvailableStateBits.Add(XStateBit.Ignore);
			driver.AvailableStateClasses.Add(XStateClass.No);
			driver.AvailableStateClasses.Add(XStateClass.Norm);
			driver.AvailableStateClasses.Add(XStateClass.Failure);
			driver.AvailableStateClasses.Add(XStateClass.Ignore);
			driver.AvailableStateClasses.Add(XStateClass.Unknown);
		}
	}
}