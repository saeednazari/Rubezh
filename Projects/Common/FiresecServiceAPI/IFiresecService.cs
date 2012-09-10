﻿using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using FiresecAPI.Models;
using XFiresecAPI;

namespace FiresecAPI
{
    [ServiceContract(CallbackContract = typeof(IFiresecCallback), SessionMode = SessionMode.Required)]
    public interface IFiresecService : IFiresecServiceSKUD
    {
        #region Service
        [OperationContract]
        OperationResult<bool> Connect(ClientCredentials clientCredentials, bool isNew);

        [OperationContract]
        OperationResult<bool> Reconnect(string userName, string password);

        [OperationContract(IsOneWay = true)]
        void Disconnect();

        [OperationContract]
        string GetStatus();

        [OperationContract]
        string Ping();

        [OperationContract]
        [FaultContract(typeof(FiresecException))]
        string Test();
        #endregion

        #region Configuration
        [OperationContract]
        List<Driver> GetDrivers();

        [OperationContract]
        DeviceConfiguration GetDeviceConfiguration();

        [OperationContract]
        SystemConfiguration GetSystemConfiguration();

        [OperationContract]
        void SetSystemConfiguration(SystemConfiguration systemConfiguration);

        [OperationContract]
        LibraryConfiguration GetLibraryConfiguration();

        [OperationContract]
        void SetLibraryConfiguration(LibraryConfiguration libraryConfiguration);

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
        #endregion

        #region Convertation
        [OperationContract]
		void ConvertJournal(List<JournalRecord> journalRecords);
        #endregion

        #region XSystem
        [OperationContract]
        void SetXDeviceConfiguration(XDeviceConfiguration xDeviceConfiguration);

        [OperationContract]
        XDeviceConfiguration GetXDeviceConfiguration();
        #endregion

        #region OPC
        [OperationContract]
        void OPCRefresh();

        [OperationContract]
        void OPCRegister();

        [OperationContract]
        void OPCUnRegister();
        #endregion
    }

    public class FiresecException : Exception
    {
    }
}