﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI.Models.Skud
{
    [DataContract]
    public class Document
	{
		[DataMember]
        public Guid Uid { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public DateTime? IssueDate { get; set; }
        [DataMember]
        public DateTime? LaunchDate { get; set; }
	}
}
