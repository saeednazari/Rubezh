﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using System.Data.SqlServerCe;
using FiresecAPI;
using FiresecAPI.Models;

namespace ServerFS2.DataBase
{
	public static class DBJournalHelper
	{
		public static string ConnectionString { get; set; }

		static DBJournalHelper()
		{
			ConnectionString = @"Data Source=" + AppDataFolderHelper.GetDBFile("FSDB.sdf") + ";Password=adm;Max Database Size=4000";
		}

		public static int GetLastId(Guid deviceUID)
		{
			var lastIndex = 0;
			try
			{
				using (var dataContext = new SqlCeConnection(ConnectionString))
				{
					dataContext.ConnectionString = ConnectionString;
					dataContext.Open();
					var sqlCeCommand = new SqlCeCommand();
					sqlCeCommand.Connection = dataContext;
					sqlCeCommand.CommandText = @"SELECT LastID FROM LastIndexes WHERE DeviceUID = @p1";
					sqlCeCommand.Parameters.AddWithValue("@p1", deviceUID);
					lastIndex = (int)sqlCeCommand.ExecuteScalar();
					dataContext.Close();
				}
			}
			catch { }
			return lastIndex;
		}

		public static void SetLastId(Guid deviceUID, int lastIndex)
		{
			try
			{
				using (var dataContext = new SqlCeConnection(ConnectionString))
				{
					dataContext.ConnectionString = ConnectionString;
					dataContext.Open();
					var sqlCeCommand = new SqlCeCommand();
					sqlCeCommand.Connection = dataContext;
					sqlCeCommand.CommandText = @"Insert Into LastIndexes" +
						"(DeviceUID,LastID) Values" +
						"(@p1,@p2)";
					sqlCeCommand.Parameters.AddWithValue("@p1", deviceUID);
					sqlCeCommand.Parameters.AddWithValue("@p2", lastIndex);
					sqlCeCommand.ExecuteNonQuery();
					dataContext.Close();
				}
			}
			catch { }
		}

		public static void AddJournalItems(List<FSJournalItem> fsJournalItems)
		{
			using (var dataContext = new SqlCeConnection(ConnectionString))
			{
				dataContext.ConnectionString = ConnectionString;
				dataContext.Open();
				foreach (var fsJournalItem in fsJournalItems)
				{
					var sqlCeCommand = new SqlCeCommand();
					sqlCeCommand.Connection = dataContext;
					sqlCeCommand.CommandText = @"Insert Into Journal" +
						"(Description, Detalization, DeviceCategory, DeviceUID, DeviceName, DeviceTime, PanelUID, PanelName, StateType, SubsystemType, SystemTime, UserName, ZoneName) Values" +
						"(@p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12, @p13)";
					sqlCeCommand.Parameters.AddWithValue("@p1", (object)fsJournalItem.Description ?? DBNull.Value);
					sqlCeCommand.Parameters.AddWithValue("@p2", (object)fsJournalItem.Detalization ?? DBNull.Value);
					sqlCeCommand.Parameters.AddWithValue("@p3", (object)fsJournalItem.DeviceCategory ?? DBNull.Value);
					sqlCeCommand.Parameters.AddWithValue("@p4", (object)fsJournalItem.DeviceUID ?? DBNull.Value);
					sqlCeCommand.Parameters.AddWithValue("@p5", (object)fsJournalItem.DeviceName ?? DBNull.Value);
					sqlCeCommand.Parameters.AddWithValue("@p6", (object)fsJournalItem.DeviceTime ?? DBNull.Value);
					sqlCeCommand.Parameters.AddWithValue("@p7", (object)fsJournalItem.PanelUID ?? DBNull.Value);
					sqlCeCommand.Parameters.AddWithValue("@p8", (object)fsJournalItem.PanelName ?? DBNull.Value);
					sqlCeCommand.Parameters.AddWithValue("@p9", (object)fsJournalItem.StateType ?? DBNull.Value);
					sqlCeCommand.Parameters.AddWithValue("@p10", (object)fsJournalItem.SubsystemType ?? DBNull.Value);
					sqlCeCommand.Parameters.AddWithValue("@p11", (object)fsJournalItem.SystemTime ?? DBNull.Value);
					sqlCeCommand.Parameters.AddWithValue("@p12", (object)fsJournalItem.UserName ?? DBNull.Value);
					sqlCeCommand.Parameters.AddWithValue("@p13", (object)fsJournalItem.ZoneName ?? DBNull.Value);
					sqlCeCommand.ExecuteNonQuery();
				}
				dataContext.Close();
			}
		}

		public static List<FSJournalItem> GetJournalItems(Guid deviceUID)
		{
			var result = new List<FSJournalItem>();

			using (var dataContext = new SqlCeConnection(ConnectionString))
			{
				dataContext.ConnectionString = ConnectionString;
				dataContext.Open();
				var sqlCeCommand = new SqlCeCommand();
				sqlCeCommand.Connection = dataContext;
				sqlCeCommand.CommandText = @"SELECT TOP (100) Description, Detalization, DeviceCategory, DeviceUID, DeviceName, DeviceTime, PanelUID, PanelName, StateType, SubsystemType, SystemTime, UserName, ZoneName FROM Journal";
				var reader = sqlCeCommand.ExecuteReader();
				while (reader.Read())
				{
					try
					{
						var fsJournalItem = new FSJournalItem();
						fsJournalItem.Description = reader.GetString(0);
						fsJournalItem.Detalization = reader.GetString(1);
						fsJournalItem.DeviceCategory = reader.GetInt32(2);
						fsJournalItem.DeviceUID = reader.GetGuid(3);
						fsJournalItem.DeviceName = reader.GetString(4);
						fsJournalItem.DeviceTime = reader.GetDateTime(5);
						fsJournalItem.PanelUID = reader.GetGuid(6);
						fsJournalItem.PanelName = reader.GetString(7);
						fsJournalItem.StateType = (StateType)reader.GetInt32(8);
						fsJournalItem.SubsystemType = (SubsystemType)reader.GetInt32(9);
						fsJournalItem.SystemTime = reader.GetDateTime(10);
						fsJournalItem.UserName = reader.GetString(11);
						fsJournalItem.ZoneName = reader.GetString(12);
						result.Add(fsJournalItem);
					}
					catch { ;}
				}
				dataContext.Close();
			}
			return result;
		}
	}
}