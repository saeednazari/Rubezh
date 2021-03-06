﻿using System;
using FiresecClient;
using FSAgentAPI;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace FireAdministrator.ViewModels
{
	public class ProgressViewModel : DialogViewModel
	{
		public ProgressViewModel(string title)
		{
			Title = title;
			CancelText = "Отменить";
			CloseOnEscape = false;
			AllowClose = false;
			StopCommand = new RelayCommand(OnStop);
			FiresecManager.FSAgent.Progress -= new Action<FSProgressInfo>(Progress);
			FiresecManager.FSAgent.Progress += new Action<FSProgressInfo>(Progress);
		}

		public void CloseProgress()
		{
			FiresecManager.FSAgent.Progress -= new Action<FSProgressInfo>(Progress);
			Close(true);
		}

		public void Progress(FSProgressInfo fsProgressInfo)
		{
			ApplicationService.Invoke(() => OnProgress(fsProgressInfo));
		}

		void OnProgress(FSProgressInfo fsProgressInfo)
		{
			Description = fsProgressInfo.Comment;
			if (fsProgressInfo.PercentComplete >= 0)
			{
				Percent = fsProgressInfo.PercentComplete;
			}

			if (fsProgressInfo.Stage == -100)
				CancelText = "Остановить";

			if (fsProgressInfo.Stage > 0)
			{
				int stageNo = fsProgressInfo.Stage / (256 * 256);
				int stageCount = fsProgressInfo.Stage - stageNo * 256 * 256;
			}
		}

		int _percent;
		public int Percent
		{
			get { return _percent; }
			set
			{
				_percent = value;
				OnPropertyChanged("Percent");
			}
		}

		string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				OnPropertyChanged("Description");
			}
		}

		string _cancelText;
		public string CancelText
		{
			get { return _cancelText; }
			set
			{
				_cancelText = value;
				OnPropertyChanged("CancelText");
			}
		}

		public RelayCommand StopCommand { get; private set; }
		void OnStop()
		{
			FiresecManager.FSAgent.CancelProgress();
		}
	}
}