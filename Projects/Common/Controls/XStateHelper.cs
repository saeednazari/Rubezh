﻿using XFiresecAPI;

namespace Controls
{
	public static class StateHelper
	{
		public static string ToIconSource(this XStateClass stateClass)
		{
			if (stateClass == XStateClass.Norm)
				return null;
			if (stateClass == XStateClass.Off)
				return null;

			return "/Controls;component/StateClassIcons/" + stateClass.ToString() + ".png";
		}
	}
}