﻿using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class SystemConfiguration : VersionedConfiguration
	{
		public SystemConfiguration()
		{
			Sounds = new List<Sound>();
			JournalFilters = new List<JournalFilter>();
			Instructions = new List<Instruction>();
			Cameras = new List<Camera>();
			EmailData = new EmailData();
		}

		[DataMember]
		public List<Sound> Sounds { get; set; }

		[DataMember]
		public List<JournalFilter> JournalFilters { get; set; }

		[DataMember]
		public List<Instruction> Instructions { get; set; }

		[DataMember]
		public List<Camera> Cameras { get; set; }

		[DataMember]
		public EmailData EmailData { get; set; }

		public override bool ValidateVersion()
		{
			var result = true;
			if (Instructions == null)
			{
				Instructions = new List<Instruction>();
				result = false;
			}
			if (EmailData == null)
			{
				EmailData = new EmailData();
				result = false;
			}
			return result;
		}
	}
}