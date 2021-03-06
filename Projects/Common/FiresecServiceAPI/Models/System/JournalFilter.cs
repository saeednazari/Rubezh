﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class JournalFilter
	{
		public JournalFilter()
		{
            UID = Guid.NewGuid();
			LastRecordsCount = 100;
			LastDaysCount = 10;
			StateTypes = new List<StateType>();
			Categories = new List<DeviceCategoryType>();
		}

        [DataMember]
        public Guid UID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public int LastRecordsCount { get; set; }

		[DataMember]
		public int LastDaysCount { get; set; }

		[DataMember]
		public bool IsLastDaysCountActive { get; set; }

		[DataMember]
		public List<StateType> StateTypes { get; set; }

		[DataMember]
		public List<DeviceCategoryType> Categories { get; set; }
	}
}