﻿using System.Data;
using CodeReason.Reports;
using Infrastructure.Common;
using Infrastructure.Common.Reports;
using Infrastructure.Common.Windows;
using GKModule.ViewModels;
using FiresecAPI;

namespace GKModule.Reports
{
    internal class JournalReport : ISingleReportProvider, IFilterableReport
    {
        private ReportArchiveFilter ReportArchiveFilter { get; set; }
        public JournalReport()
        {
            ReportArchiveFilter = new ReportArchiveFilter();
        }

        #region IFilterableReport Members
        public void Filter(RelayCommand refreshCommand)
        {
            var archiveFilterViewModel = new ArchiveFilterViewModel(ReportArchiveFilter.ArchiveFilter);
            if (DialogService.ShowModalWindow(archiveFilterViewModel))
            {
                ReportArchiveFilter = new ReportArchiveFilter(archiveFilterViewModel);
                refreshCommand.Execute();
            }
        }
        #endregion

        #region ISingleReportProvider Members
        public ReportData GetData()
        {
            ReportArchiveFilter.LoadArchive();
            var data = new ReportData();
            data.ReportDocumentValues.Add("StartDate", ReportArchiveFilter.StartDate);
            data.ReportDocumentValues.Add("EndDate", ReportArchiveFilter.EndDate);

            DataTable table = new DataTable("Journal");
            table.Columns.Add("DateTime");
            table.Columns.Add("Name");
            table.Columns.Add("YesNo");
            table.Columns.Add("Description");
            table.Columns.Add("DeviceName");
            table.Columns.Add("ZoneName");
            table.Columns.Add("DirectionName");
            table.Columns.Add("StateClass");
            foreach (var journalItem in ReportArchiveFilter.JournalItems)
            {
                var journalItemViewModel = new JournalItemViewModel(journalItem);
                var deviceName = "";
                if (journalItemViewModel.DeviceState != null)
                {
                    deviceName = journalItemViewModel.DeviceState.Device.PresentationAddressAndDriver;
                }
                var zoneName = "";
                if (journalItemViewModel.ZoneState != null)
                {
                    zoneName = journalItemViewModel.ZoneState.Zone.PresentationName;
                }
                var directionName = "";
                if (journalItemViewModel.DirectionState != null)
                {
                    directionName = journalItemViewModel.DirectionState.Direction.PresentationName;
                }
                table.Rows.Add(
                    journalItem.DateTime,
                    journalItem.Name,
                    journalItem.YesNo,
                    journalItem.Description,
                    deviceName,
                    zoneName,
                    directionName,
                    journalItem.StateClass.ToDescription());
            }
            data.DataTables.Add(table);
            return data;
        }
        #endregion

        #region IReportProvider Members
        public string Template
        {
            get { return "Reports/JournalReport.xaml"; }
        }

        public string Title
        {
            get { return "Журнал событий ГК"; }
        }

        public bool IsEnabled
        {
            get { return true; }
        }
        #endregion
    }
}