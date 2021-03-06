﻿using System;
using Common;
using Firesec;
using FiresecAPI;
using FSAgentClient;
using Infrastructure.Common;

namespace FiresecClient
{
    public partial class FiresecManager
    {
        public static FiresecDriver FiresecDriver { get; private set; }
		public static FSAgent FSAgent { get; private set; }

        static public OperationResult<bool> InitializeFiresecDriver(bool isPing)
        {
            try
            {
				FSAgent = new FSAgent(ConnectionSettingsManager.FSAgentServerAddress);
                FiresecDriver = new FiresecDriver();
                var result = FiresecDriver.Connect(FSAgent, isPing);
                return result;
            }
            catch (Exception e)
            {
                Logger.Error(e, "FiresecManager.InitializeFiresecDriver");
                LoadingErrorManager.Add(e);
                return new OperationResult<bool>(e.Message);
            }
        }

        public static void SynchrinizeJournal()
        {
            try
            {
                var result = FiresecService.FiresecService.GetJournalLastId();
                if (result.HasError)
                {
                    LoadingErrorManager.Add("Ошибка при получении индекса последней записи с сервера");
                }

                var journalRecords = FiresecDriver.Watcher.SynchrinizeJournal(result.Result);
                if (journalRecords.Count > 0)
                {
                    FiresecService.AddJournalRecords(journalRecords);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "FiresecManager.SynchrinizeJournal");
                LoadingErrorManager.Add(e);
            }
        }
    }
}