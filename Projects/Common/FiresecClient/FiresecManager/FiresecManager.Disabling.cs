﻿using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using Common;

namespace FiresecClient
{
    public partial class FiresecManager
    {
        public static bool CanDisable(DeviceState deviceState)
        {
            try
            {
                if ((deviceState != null) && (deviceState.Device.Driver.CanDisable))
                {
                    if (deviceState.IsDisabled)
                        return CheckPermission(PermissionType.Oper_RemoveFromIgnoreList);
                    return CheckPermission(PermissionType.Oper_AddToIgnoreList);
                }
                return false;
            }
            catch (Exception e)
            {
                Logger.Error(e, "FiresecManager.CanDisable");
                return false;
            }
        }

        public static void ChangeDisabled(DeviceState deviceState)
        {
            try
            {
                if ((deviceState != null) && (CanDisable(deviceState)))
                {
                    if (deviceState.IsDisabled)
                        FiresecDriver.RemoveFromIgnoreList(new List<Device>() { deviceState.Device });
                    else
                        FiresecDriver.AddToIgnoreList(new List<Device>() { deviceState.Device });
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "FiresecManager.ChangeDisabled");
            }
        }
    }
}