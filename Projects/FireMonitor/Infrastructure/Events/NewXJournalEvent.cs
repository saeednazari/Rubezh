﻿using System.Collections.Generic;
using Microsoft.Practices.Prism.Events;
using XFiresecAPI;

namespace Infrastructure.Events
{
	public class NewXJournalEvent : CompositePresentationEvent<List<JournalItem>>
	{
	}
}