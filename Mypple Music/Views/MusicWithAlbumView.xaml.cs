﻿using Mypple_Music.Events;
using Mypple_Music.Extensions;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Mypple_Music.Views
{
    /// <summary>
    /// MusicWithAlbumView.xaml 的交互逻辑
    /// </summary>
    public partial class MusicWithAlbumView : UserControl
    {
        private readonly IEventAggregator eventAggregator;

        public MusicWithAlbumView(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            InitializeComponent();


            this.eventAggregator = eventAggregator;
        }
    }
}