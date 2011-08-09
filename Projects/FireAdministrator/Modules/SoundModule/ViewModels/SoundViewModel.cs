﻿using FiresecClient;
using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure.Common;
using System.Media;
using System.Windows;
using System;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using FiresecAPI.Models;

namespace SoundsModule.ViewModels
{
    public class SoundViewModel : BaseViewModel
    {
        public SoundViewModel(Sound sound)
        {
            Sound = sound;
        }

        Sound _sound;
        public Sound Sound
        {
            get { return _sound; }
            set
            {
                _sound = value;
            }
        }

        public StateType StateType
        {
            get { return Sound.StateType; }
        }

        public string SoundName
        {
            get
            {
                if (AvailableSounds.Any(x => x == Sound.SoundName))
                {
                    return Sound.SoundName;
                }
                else
                {
                    return DownloadHelper.DefaultName;
                }
            }
            set
            {
                if (value == DownloadHelper.DefaultName)
                {
                    Sound.SoundName = null;
                }
                else
                {
                    Sound.SoundName = value;
                }
                OnPropertyChanged("SoundName");
            }
        }

        public SpeakerType SpeakerType
        {
            get { return Sound.SpeakerType; }
            set 
            {
                Sound.SpeakerType = value; 
            }
        }

        public bool IsContinious
        {
            get { return Sound.IsContinious; }
            set
            {
                Sound.IsContinious = value;
                OnPropertyChanged("IsContinious");
            }
        }

        public List<string> AvailableSounds 
        {
            get
            {
                List<string> fileNames = new List<string>();
                fileNames.Add(DownloadHelper.DefaultName);
                foreach (string str in Directory.GetFiles(DownloadHelper.CurrentDirectory))
                {
                    fileNames.Add(Path.GetFileName(str));
                }
                return fileNames;
            } 
        }

        public Array AvailableSpeakers
        {
            get { return Enum.GetValues(typeof(SpeakerType)); }
        }

        //public List<string> AvailableSpeakers 
        //{
            //get
            //{
            //    List<string> speakerType = new List<string>();
            //    foreach (var speakertype in Enum.GetNames(typeof(SpeakerType)))
            //    {
            //        speakerType.Add(speakertype);
            //    }
            //    return speakerType;
            //}
        //}
    }
}
