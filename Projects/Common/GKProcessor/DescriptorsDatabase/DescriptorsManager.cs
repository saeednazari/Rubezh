﻿using System.Collections.Generic;
using FiresecClient;
using XFiresecAPI;

namespace GKProcessor
{
	public static class DescriptorsManager
	{
		public static List<KauDatabase> KauDatabases { get; private set; }
		public static List<GkDatabase> GkDatabases { get; private set; }

		public static void Create()
		{
			XManager.UpdateConfiguration();
			XManager.Prepare();

			GkDatabases = new List<GkDatabase>();
			KauDatabases = new List<KauDatabase>();

			foreach (var device in XManager.Devices)
			{
				if (device.DriverType == XDriverType.GK)
				{
					var gkDatabase = new GkDatabase(device);
					GkDatabases.Add(gkDatabase);

					foreach (var kauDevice in device.Children)
					{
						if (kauDevice.Driver.IsKauOrRSR2Kau)
						{
							var kauDatabase = new KauDatabase(kauDevice);
							gkDatabase.KauDatabases.Add(kauDatabase);
							KauDatabases.Add(kauDatabase);
						}
					}
				}
			}

			KauDatabases.ForEach(x => x.BuildObjects());
			GkDatabases.ForEach(x => x.BuildObjects());
			CreateDynamicObjectsInXManager();
		}

		public static void CreateDynamicObjectsInXManager()
		{
			XManager.Delays = new List<XDelay>();
			XManager.Pims = new List<XPim>();
			foreach (var gkDatabase in GkDatabases)
			{
				foreach (var delay in gkDatabase.Delays)
				{
					delay.BaseState = new XDelayState(delay);
					delay.State = new XState(delay);
					XManager.Delays.Add(delay);
				}
				foreach (var pim in gkDatabase.Pims)
				{
					pim.BaseState = new XPimState(pim);
					pim.State = new XState(pim);
					XManager.Pims.Add(pim);
				}
			}
		}
	}
}