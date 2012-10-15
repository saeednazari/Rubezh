﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Win32;

namespace Infrastructure.Common.Module
{
    public class ModuleHelper
    {
        public static List<string> DisableModules { get; set; }
        public static List<string> EnableModules { get; set; }
        public static void SetModuleIntoRegister()
        {
            try
            {
                RegistryKey saveKey = Registry.LocalMachine.CreateSubKey("software\\rubezh\\Modules");
                foreach (var enabledModule in EnableModules)
                {
                    saveKey.SetValue(enabledModule, "isEnabled");
                }
                foreach (var disabledModule in DisableModules)
                {
                    saveKey.SetValue(disabledModule, "isDisabled");
                }
                saveKey.Close();
            }
            catch (Exception)
            {
                { }
                throw;
            }
        }

        public static void LoadModulesFromRegister()
        {
            EnableModules = new List<string>();
            DisableModules = new List<string>();
            try
            {
                RegistryKey readKey = Registry.LocalMachine.OpenSubKey("software\\rubezh\\Modules");
                var Modules = readKey.GetValueNames();
                foreach (var module in Modules)
                {
                    var status = readKey.GetValue(module);
                    if(status.Equals("isEnabled"))
                        EnableModules.Add(module);
                    else
                        DisableModules.Add(module);
                }
                readKey.Close();
            }
            catch (Exception)
            {
                
            }
        }
    }
}
