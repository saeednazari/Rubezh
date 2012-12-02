﻿using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using FiresecAPI.Models;
using XFiresecAPI;

namespace FiresecAPI
{
	[ServiceContract]
    public interface IFiresecService : IFiresecServiceSKUD
    {
        #region Service
        [OperationContract]
        OperationResult<bool> Connect(Guid uid, ClientCredentials clientCredentials, bool isNew);

        [OperationContract]
        OperationResult<bool> Reconnect(Guid uid, string userName, string password);

        [OperationContract(IsOneWay = true)]
		void Disconnect(Guid uid);

        [OperationContract]
        List<CallbackResult> Poll(Guid uid);

        [OperationContract]
        string Test(string arg);

        [OperationContract(IsOneWay = true)]
        void NotifyClientsOnConfigurationChanged();
        #endregion

        #region Configuration
        [OperationContract]
        DriversConfiguration GetDriversConfiguration();

        [OperationContract]
        DeviceConfiguration GetDeviceConfiguration();

        [OperationContract]
        SystemConfiguration GetSystemConfiguration();

        [OperationContract]
        void SetSystemConfiguration(SystemConfiguration systemConfiguration);

        [OperationContract]
        DeviceLibraryConfiguration GetDeviceLibraryConfiguration();

        [OperationContract]
        void SetDeviceLibraryConfiguration(DeviceLibraryConfiguration deviceLibraryConfiguration);

        [OperationContract]
        PlansConfiguration GetPlansConfiguration();

        [OperationContract]
        void SetPlansConfiguration(PlansConfiguration plansConfiguration);

        [OperationContract]
        SecurityConfiguration GetSecurityConfiguration();

        [OperationContract]
        void SetSecurityConfiguration(SecurityConfiguration securityConfiguration);
        #endregion

        #region Devices
        [OperationContract]
        OperationResult<bool> SetDeviceConfiguration(DeviceConfiguration deviceConfiguration);
        #endregion

        #region Journal
        [OperationContract]
        OperationResult<int> GetJournalLastId();

        [OperationContract]
        OperationResult<List<JournalRecord>> GetFilteredJournal(JournalFilter journalFilter);

        [OperationContract]
        OperationResult<List<JournalRecord>> GetFilteredArchive(ArchiveFilter archiveFilter);

        [OperationContract]
        void BeginGetFilteredArchive(ArchiveFilter archiveFilter);

        [OperationContract]
        OperationResult<List<JournalDescriptionItem>> GetDistinctDescriptions();

        [OperationContract]
        OperationResult<DateTime> GetArchiveStartDate();

        [OperationContract()]
        void AddJournalRecords(List<JournalRecord> journalRecords);
        #endregion

        #region Files
        [OperationContract]
        List<string> GetFileNamesList(string directory);

        [OperationContract]
        Dictionary<string, string> GetDirectoryHash(string directory);

        [OperationContract]
        Stream GetFile(string dirAndFileName);

        [OperationContract]
        Stream GetConfig();
        #endregion

        #region Convertation
        [OperationContract]
        void SetJournal(List<JournalRecord> journalRecords);
        #endregion

        #region XSystem
        [OperationContract]
        void SetXDeviceConfiguration(XDeviceConfiguration xDeviceConfiguration);

        [OperationContract]
        XDeviceConfiguration GetXDeviceConfiguration();

        [OperationContract]
        void SetXDeviceLibraryConfiguration(XDeviceLibraryConfiguration xDeviceLibraryConfiguration);

        [OperationContract]
        XDeviceLibraryConfiguration GetXDeviceLibraryConfiguration();
        #endregion
    }
}