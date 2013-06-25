﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using FiresecAPI.Models;
using FS2Api;
using Common;
using System.Windows.Forms;
using System.Collections;

namespace ServerFS2.Monitoring
{
	public partial class MonitoringUSB
	{
		public Device USBDevice { get; private set; }
		public List<MonitoringPanel> MonitoringPanels { get; private set; }
		public List<Device> MonitoringNonPanels { get; private set; }
		DateTime StartTime;

		public MonitoringUSB(Device usbDevice)
		{
			USBDevice = usbDevice;
			MonitoringPanels = new List<MonitoringPanel>();
			MonitoringNonPanels = new List<Device>();

			if (!usbDevice.IsParentMonitoringDisabled)
			{
				switch (usbDevice.Driver.DriverType)
				{
					case DriverType.MS_1:
					case DriverType.MS_2:
						foreach (var channelChild in usbDevice.Children)
						{
							foreach (var panelDevice in channelChild.Children)
							{
								if (!panelDevice.IsParentMonitoringDisabled)
								{
									switch (panelDevice.Driver.DriverType)
									{
										case DriverType.Rubezh_2AM:
										//case DriverType.Rubezh_2OP:
										//case DriverType.Rubezh_4A:
										//case DriverType.BUNS:
										//case DriverType.BUNS_2:
										//case DriverType.BlindPanel:
											MonitoringPanels.Add(new MonitoringPanel(panelDevice));
											break;
										case DriverType.IndicationBlock:
										case DriverType.PDU:
										case DriverType.PDU_PT:
										case DriverType.UOO_TL:
										case DriverType.MS_3:
										case DriverType.MS_4:
											MonitoringNonPanels.Add(panelDevice);
											break;
									}
								}
							}
						}
						break;
					case DriverType.USB_Rubezh_2AM:
					case DriverType.USB_Rubezh_2OP:
					case DriverType.USB_Rubezh_4A:
					case DriverType.USB_Rubezh_P:
					case DriverType.USB_BUNS:
					case DriverType.USB_BUNS_2:
						MonitoringPanels.Add(new MonitoringPanel(usbDevice));
						break;
				}
			}
			USBManager.NewResponse += new Action<Device, Response>(UsbRunner_NewResponse);
		}

		void OnRun()
		{
			try
			{
				SetAllInitializing();
				foreach (var monitoringPanel in MonitoringPanels)
				{
					if (CheckSuspending(false))
						return;

					monitoringPanel.Initialize();
				}
				RemoveAllInitializing();
			}
			catch (FS2StopMonitoringException)
			{
				return;
			}
			catch (Exception e)
			{
				Logger.Error(e, "MonitoringProcessor.OnRun");
			}

			while (true)
			{
				try
				{
					if (CheckSuspending(false))
						return;

					foreach (var monitoringDevice in MonitoringPanels)
					{
						if (CheckSuspending(false))
							return;
						monitoringDevice.CheckTasks();
					}

					MonitoringNonPanels.ForEach(x => NonPanelStatesManager.UpdatePDUPanelState(x));

					if (CheckSuspending(false))
						return;

					if (IsTimeSynchronizationNeeded)
					{
						MonitoringPanels.ForEach(x => x.SynchronizeTime());
						IsTimeSynchronizationNeeded = false;
					}
				}
				catch (FS2StopMonitoringException)
				{
					return;
				}
				catch (Exception e)
				{
					Logger.Error(e, "MonitoringProcessor.LoopMonitoring");
				}
			}
		}

		void UsbRunner_NewResponse(Device usbDevice, Response response)
		{
			try
			{
				if (USBDevice.UID == usbDevice.UID)
				{
					//Trace.WriteLine("response.Id=" + response.Id);
					foreach (var monitoringDevice in MonitoringPanels)
					{
						for (int i = 0; i < monitoringDevice.Requests.Count; i++)
						{
							var request = monitoringDevice.Requests[i];
							if (request != null && request.Id == response.Id)
							{
								monitoringDevice.OnResponceRecieved(request, response);
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "MonitoringUSB.UsbRunner_NewResponse");
			}
		}
	}
}