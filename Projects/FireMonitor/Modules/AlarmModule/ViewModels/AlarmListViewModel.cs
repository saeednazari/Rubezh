﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Infrastructure;
using Infrastructure.Common;
using AlarmModule.Events;

namespace AlarmModule.ViewModels
{
    public class AlarmListViewModel : RegionViewModel
    {
        public AlarmListViewModel()
        {
            ServiceFactory.Events.GetEvent<ResetAlarmEvent>().Subscribe(OnResetAlarm);
            ServiceFactory.Events.GetEvent<AlarmAddedEvent>().Subscribe(OnAlarmAdded);
            ServiceFactory.Events.GetEvent<MoveAlarmToEndEvent>().Subscribe(OnMoveAlarmToEnd);
        }

        void OnResetAlarm(Alarm alarm)
        {
            AlarmViewModel alarmViewModel = Alarms.FirstOrDefault(x => x.alarm == alarm);
            Alarms.Remove(alarmViewModel);
            if (Alarms.Count == 0)
            {
                ServiceFactory.Layout.Close();
            }
        }

        void OnAlarmAdded(Alarm alarm)
        {
            AlarmViewModel alarmViewModel = new AlarmViewModel();
            alarmViewModel.Initialize(alarm);
            Alarms.Insert(0, alarmViewModel);
        }

        void OnMoveAlarmToEnd(AlarmViewModel alarmViewModel)
        {
            int oldIndex = Alarms.IndexOf(alarmViewModel);
            int newIndex = Alarms.Count;
            Alarms.Move(oldIndex, newIndex - 1);
        }

        public void Initialize(List<Alarm> alarms)
        {
            Alarms = new ObservableCollection<AlarmViewModel>();
            foreach (var alarm in alarms)
            {
                AlarmViewModel alarmViewModel = new AlarmViewModel();
                alarmViewModel.Initialize(alarm);
                Alarms.Add(alarmViewModel);
            }
        }

        ObservableCollection<AlarmViewModel> alarms;
        public ObservableCollection<AlarmViewModel> Alarms
        {
            get { return alarms; }
            set
            {
                alarms = value;
                OnPropertyChanged("Alarms");
            }
        }

        AlarmViewModel selectedAlarm;
        public AlarmViewModel SelectedAlarm
        {
            get { return selectedAlarm; }
            set
            {
                selectedAlarm = value;
                ServiceFactory.Layout.ShowAlarm(value);
                OnPropertyChanged("SelectedAlarm");
            }
        }

        public override void Dispose()
        {
            //ServiceFactory.Events.GetEvent<ResetAlarmEvent>().Unsubscribe(OnResetAlarm);
            //ServiceFactory.Events.GetEvent<AlarmAddedEvent>().Unsubscribe(OnAlarmAdded);
        }
    }
}
