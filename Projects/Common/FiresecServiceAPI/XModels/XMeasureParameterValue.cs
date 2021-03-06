﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class XMeasureParameterValue
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public double Value { get; set; }

		[DataMember]
		public string StringValue { get; set; }
	}
}