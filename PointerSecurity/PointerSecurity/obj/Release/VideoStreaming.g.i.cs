﻿#pragma checksum "E:\Tomal Project\PointerSecurity\PointerSecurity\VideoStreaming.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "1A5AC91673A64923B4FD4D96BF0F77CC"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace PointerSecurity {
    
    
    public partial class VideoStreaming : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Image VideoStream;
        
        internal System.Windows.Controls.Image ScreenStream;
        
        internal System.Windows.Controls.TextBlock Loading;
        
        internal System.Windows.Controls.ProgressBar pgr;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/PointerSecurity;component/VideoStreaming.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.VideoStream = ((System.Windows.Controls.Image)(this.FindName("VideoStream")));
            this.ScreenStream = ((System.Windows.Controls.Image)(this.FindName("ScreenStream")));
            this.Loading = ((System.Windows.Controls.TextBlock)(this.FindName("Loading")));
            this.pgr = ((System.Windows.Controls.ProgressBar)(this.FindName("pgr")));
        }
    }
}

