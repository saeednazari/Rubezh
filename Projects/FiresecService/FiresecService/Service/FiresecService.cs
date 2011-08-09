﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecService.Converters;
using System;

namespace FiresecService
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Single)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single, MaxItemsInObjectGraph = 2147483647)]
    public class FiresecService : IFiresecService
    {
        IFiresecCallback _currentCallback;

        public void Connect()
        {
            _currentCallback = OperationContext.Current.GetCallbackChannel<IFiresecCallback>();
            CallbackManager.Add(_currentCallback);
        }

        public void Disconnect()
        {
            CallbackManager.Remove(_currentCallback);
        }

        public List<Driver> GetDrivers()
        {
            return FiresecManager.Drivers;
        }

        public DeviceConfiguration GetDeviceConfiguration()
        {
            return FiresecManager.DeviceConfiguration;
        }

        public DeviceConfigurationStates GetStates()
        {
            return FiresecManager.DeviceConfigurationStates;
        }

        public void SetDeviceConfiguration(DeviceConfiguration deviceConfiguration)
        {
            FiresecManager.DeviceConfiguration = deviceConfiguration;
            FiresecManager.SetNewConfig();
        }

        public void WriteConfiguration(string id)
        {
            var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.Id == id);
            FiresecInternalClient.DeviceWriteConfig(FiresecManager.CoreConfig, device.PlaceInTree);
        }

        public SecurityConfiguration GetSecurityConfiguration()
        {
            return FiresecManager.SecurityConfiguration;
        }

        public void SetSecurityConfiguration(SecurityConfiguration securityConfiguration)
        {
            FiresecManager.SecurityConfiguration = securityConfiguration;
        }

        public SystemConfiguration GetSystemConfiguration()
        {
            try
            {
                DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(SystemConfiguration));
                FileStream fileStream = new FileStream("D:/SystemConfiguration.xml", FileMode.Open);
                FiresecManager.SystemConfiguration = (SystemConfiguration)dataContractSerializer.ReadObject(fileStream);
                fileStream.Close();
            }
            catch
            {
            }

            return FiresecManager.SystemConfiguration;
        }

        public void SetSystemConfiguration(SystemConfiguration systemConfiguration)
        {
            FiresecManager.SystemConfiguration = systemConfiguration;

            DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(SystemConfiguration));
            FileStream fileStream = new FileStream("D:/SystemConfiguration.xml", FileMode.Create);
            dataContractSerializer.WriteObject(fileStream, FiresecManager.SystemConfiguration);
            fileStream.Close();
        }

        public List<JournalRecord> ReadJournal(int startIndex, int count)
        {
            var internalJournal = FiresecInternalClient.ReadEvents(startIndex, count);

            var journalRecords = new List<JournalRecord>();
            if (internalJournal != null && internalJournal.Journal != null)
            {
                foreach (var innerJournaRecord in internalJournal.Journal)
                {
                    journalRecords.Add(JournalConverter.Convert(innerJournaRecord));
                }
            }

            return journalRecords;
        }

        public void AddToIgnoreList(List<string> ids)
        {
            List<string> devicePaths = new List<string>();
            foreach (var id in ids)
            {
                var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.Id == id);
                devicePaths.Add(device.PlaceInTree);
            }
            FiresecInternalClient.AddToIgnoreList(devicePaths);
        }

        public void RemoveFromIgnoreList(List<string> ids)
        {
            List<string> devicePaths = new List<string>();
            foreach (var id in ids)
            {
                var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.Id == id);
                devicePaths.Add(device.PlaceInTree);
            }
            FiresecInternalClient.RemoveFromIgnoreList(devicePaths);
        }

        public void ResetStates(List<ResetItem> resetItems)
        {
            FiresecResetHelper.ResetMany(resetItems);
        }

        public void AddUserMessage(string message)
        {
            FiresecInternalClient.AddUserMessage(message);
        }

        public void ExecuteCommand(string id, string methodName)
        {
            var device = FiresecManager.DeviceConfigurationStates.DeviceStates.FirstOrDefault(x => x.Id == id);
            FiresecInternalClient.ExecuteCommand(device.PlaceInTree, methodName);
        }

        public List<string> GetSoundsFileName()
        {
            List<string> listSoungsFileName = new List<string>();
            var soungsFileName = Directory.GetFiles(@"C:\Program Files\Firesec\Sounds");
            foreach (string str in soungsFileName)
            {
                listSoungsFileName.Add(Path.GetFileName(str));
            }
            return listSoungsFileName;
        }

        public Dictionary<string, string> GetHashAndNameSoundFiles()
        {
            Dictionary<string, string> hashTable = new Dictionary<string, string>();
            List<string> HashListSoundFiles = new List<string>();
            DirectoryInfo dir = new DirectoryInfo(@"C:\Program Files\Firesec\Sounds");
            FileInfo[] files = dir.GetFiles();
            byte[] hash;
            StringBuilder sBuilder = new StringBuilder();
            foreach (FileInfo fInfo in files)
            {
                sBuilder.Clear();
                using (FileStream fileStream = fInfo.Open(FileMode.Open))
                {
                    hash = MD5.Create().ComputeHash(fileStream);
                    for (int i = 0; i < hash.Length; i++)
                    {
                        sBuilder.Append(hash[i].ToString());
                    }
                }
                hashTable.Add(sBuilder.ToString(), fInfo.Name);
            }
            return hashTable;
        }

        public Stream GetFile(string filename)
        {
            string filePath = @"C:\Program Files\Firesec\Sounds\" + filename;

            if (!File.Exists(filePath))
                throw new FileNotFoundException("File was not found", Path.GetFileName(filePath));

            return new FileStream(filePath, FileMode.Open, FileAccess.Read);
        }
    }
}
