﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using XFiresecAPI;

namespace FiresecAPI.Models
{
	[DataContract]
	public class Camera
	{
		public Camera()
		{
            UID = Guid.NewGuid();
			ZoneUIDs = new List<Guid>();
			Width = 300;
			Height = 300;
		}

        [DataMember]
        public Guid UID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Address { get; set; }

		[DataMember]
		public int Left { get; set; }

		[DataMember]
		public int Top { get; set; }

		[DataMember]
		public int Width { get; set; }

		[DataMember]
		public int Height { get; set; }

		[DataMember]
		public bool IgnoreMoveResize { get; set; }

		[DataMember]
		public XStateClass StateClass { get; set; }

		[DataMember]
		public List<Guid> ZoneUIDs { get; set; }
	}
}