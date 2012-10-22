﻿using FiresecAPI.Models;
using Microsoft.Practices.Prism.Events;

namespace Infrastructure.Events
{
	public class ShowAlarmsEvent : CompositePresentationEvent<AlarmType?>
	{
	}
}