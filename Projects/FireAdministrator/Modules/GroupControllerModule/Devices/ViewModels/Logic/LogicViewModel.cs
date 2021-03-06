﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;
using Infrastructure.Common;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class LogicViewModel : BaseViewModel
	{
		public XDevice Device { get; private set; }

		public LogicViewModel(XDevice device, List<XClause> clauses)
		{
			Device = device;
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand<ClauseViewModel>(OnRemove);
			ChangeJoinOperatorCommand = new RelayCommand(OnChangeJoinOperator);

			Clauses = new ObservableCollection<ClauseViewModel>();
			if (clauses != null)
			{
				foreach (var clause in clauses)
				{
					var clauseViewModel = new ClauseViewModel(clause, device);
					Clauses.Add(clauseViewModel);
					JoinOperator = clause.ClauseJounOperationType;
				}
			}
			UpdateJoinOperatorVisibility();
		}

		public ObservableCollection<ClauseViewModel> Clauses { get; private set; }

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var clause = new XClause();
			var clauseViewModel = new ClauseViewModel(clause, Device);
			Clauses.Add(clauseViewModel);
			UpdateJoinOperatorVisibility();
		}

		public void UpdateJoinOperatorVisibility()
		{
			foreach (var clause in Clauses)
				clause.ShowJoinOperator = false;

			if (Clauses.Count > 1)
			{
				foreach (var clause in Clauses)
					clause.ShowJoinOperator = true;

				Clauses.Last().ShowJoinOperator = false;
			}
		}

		ClauseJounOperationType _joinOperator;
		public ClauseJounOperationType JoinOperator
		{
			get { return _joinOperator; }
			set
			{
				_joinOperator = value;
				OnPropertyChanged("JoinOperator");
			}
		}

		public List<XClause> GetClauses()
		{
			var clauses = new List<XClause>();
			foreach (var clauseViewModel in Clauses)
			{
				var clause = new XClause()
				{
					ClauseConditionType = clauseViewModel.SelectedClauseConditionType,
					StateType = clauseViewModel.SelectedStateType,
					ClauseJounOperationType = JoinOperator,
					ClauseOperationType = clauseViewModel.SelectedClauseOperationType
				};
				switch (clause.ClauseOperationType)
				{
					case ClauseOperationType.AllDevices:
					case ClauseOperationType.AnyDevice:
						clause.Devices = clauseViewModel.Devices.ToList();
						clause.DeviceUIDs = clauseViewModel.Devices.Select(x => x.UID).ToList();
						break;

					case ClauseOperationType.AllZones:
					case ClauseOperationType.AnyZone:
						clause.Zones = clauseViewModel.Zones.ToList();
						clause.ZoneUIDs = clauseViewModel.Zones.Select(x => x.UID).ToList();
						break;

					case ClauseOperationType.AllDirections:
					case ClauseOperationType.AnyDirection:
						clause.Directions = clauseViewModel.Directions.ToList();
						clause.DirectionUIDs = clauseViewModel.Directions.Select(x => x.UID).ToList();
						break;
				}
				if (clause.ZoneUIDs.Count > 0 || clause.DeviceUIDs.Count > 0 || clause.DirectionUIDs.Count > 0)
					clauses.Add(clause);
			}
			return clauses;
		}

		public RelayCommand ChangeJoinOperatorCommand { get; private set; }
		void OnChangeJoinOperator()
		{
			if (JoinOperator == ClauseJounOperationType.And)
				JoinOperator = ClauseJounOperationType.Or;
			else
				JoinOperator = ClauseJounOperationType.And;
		}

		public RelayCommand<ClauseViewModel> RemoveCommand { get; private set; }
		void OnRemove(ClauseViewModel clauseViewModel)
		{
			Clauses.Remove(clauseViewModel);
			UpdateJoinOperatorVisibility();
		}
	}
}