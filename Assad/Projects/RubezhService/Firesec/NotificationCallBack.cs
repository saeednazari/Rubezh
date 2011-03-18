﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Firesec
{
    public class NotificationCallBack : FS_Types.IFS_CallBack
    {
        public void NewEventsAvailable(int EventMask)
        {
            Trace.WriteLine("NewEventsAvailable " + EventMask.ToString());

            bool evmNewEvents = ((EventMask & 1) == 1);
            bool evmStateChanged = ((EventMask & 2) == 2);
            bool evmConfigChanged = ((EventMask & 4) == 4);
            bool evmDeviceParamsUpdated = ((EventMask & 8) == 8);
            bool evmPong = ((EventMask & 16) == 16);
            bool evmDatabaseChanged = ((EventMask & 32) == 32);
            bool evmReportsChanged = ((EventMask & 64) == 64);
            bool evmSoundsChanged = ((EventMask & 128) == 128);
            bool evmLibraryChanged = ((EventMask & 256) == 256);
            bool evmPing = ((EventMask & 512) == 512);
            bool evmIgnoreListChanged = ((EventMask & 1024) == 1024);
            bool evmEventViewChanged = ((EventMask & 2048) == 2048);

            if (evmStateChanged)
            {
                Trace.WriteLine("evmStateChanged " + EventMask.ToString());
                CoreState.config coreState = ComServer.GetCoreState();
                FiresecEventAggregator.OnStateChanged(ComServer.CoreStateString, coreState);
            }
            if (evmDeviceParamsUpdated)
            {
                Trace.WriteLine("evmDeviceParamsUpdated " + EventMask.ToString());
                DeviceParams.config coreParameters = ComServer.GetDeviceParams();
                FiresecEventAggregator.OnParametersChanged(ComServer.DeviceParametersString, coreParameters);
            }
            if (evmNewEvents)
            {
                int lastEvent = 24423;

                ReadEvents.document journal = ComServer.ReadEvents(0, 100);
                string journalString = ComServer.JournalString;

                int EventId = Convert.ToInt32(journal.Journal[0].IDEvents);
                Trace.WriteLine("Last Event Id: " + EventId.ToString());
                if (EventId > lastEvent)
                    Trace.WriteLine("NEW EVENT: " + EventId.ToString());
            }
        }
    }
}

  //evmNewEvents           = $0001;
  //evmStateChanged        = $0002;
  //evmConfigChanged       = $0004;
  //evmDeviceParamsUpdated = $0008;
  //evmPong                = $0010;
  //evmDatabaseChanged     = $0020;
  //evmReportsChanged      = $0040;
  //evmSoundsChanged       = $0080;
  //evmLibraryChanged      = $0100;
  //evmPing                = $0200;
  //evmIgnoreListChanged   = $0400;
  //evmEventViewChanged    = $0800;