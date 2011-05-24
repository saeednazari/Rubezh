﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using DeviceLibrary.Models;
using Frame = DeviceLibrary.Models.Frame;

namespace DeviceControls
{
    public class StateViewModel:IDisposable
    {
        #region Private Fields
        private readonly List<Frame> _frames;
        private readonly List<Canvas> _canvases;
        private int _tick;
        private int _startTick;
        #endregion

        public StateViewModel(State state, ICollection<Canvas> stateCanvases)
        {
            _canvases = new List<Canvas>();
            _frames = state.Frames;
            foreach (var frame in _frames)
            {
                var canvas = Helper.Str2Canvas(frame.Image, frame.Layer);
                canvas.Visibility = Visibility.Collapsed;
                _canvases.Add(canvas);
                stateCanvases.Add(canvas);
            }
            _canvases[0].Visibility = Visibility.Visible;
            if (_frames.Count <= 1) return;
            Timer.Tick += OnTick;
        }

        static StateViewModel()
        {
            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromMilliseconds(94);
            Timer.Start();
        }

        public static DispatcherTimer Timer { get; set; }
        void OnTick(object sender, EventArgs e)
        {
            _startTick++;
            if (_startTick * 100 < _frames[_tick].Duration) return;
            _startTick = 0;
            _tick = (_tick + 1) % _frames.Count;
            foreach (var canvas in _canvases)
            {
                canvas.Visibility = Visibility.Collapsed;
            }
            _canvases[_tick].Visibility = Visibility.Visible;
        }

        public void Dispose()
        {
            Timer.Tick -= OnTick;
        }
    }
}