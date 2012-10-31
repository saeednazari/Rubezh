﻿using System.Windows.Controls;

namespace DevicesModule.Plans.Views
{
    public partial class ZonePropertiesView : UserControl
    {
        public ZonePropertiesView()
        {
            InitializeComponent();
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listView = sender as ListView;
            if (listView.SelectedItem != null)
            {
                listView.ScrollIntoView(listView.SelectedItem);
            }
        }
    }
}