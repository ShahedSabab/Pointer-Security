using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;

namespace PointerSecurity
{
    public partial class ViewPhoto : PhoneApplicationPage
    {
        string location;
        public ViewPhoto()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                NavigationContext.QueryString.TryGetValue("loc", out location);
                BitmapImage image = new BitmapImage(new Uri("http://www.eyepointer.tk/sabab/" + location));
                Picture.Source = image;
                Loading.Text = "";
                pgr.IsIndeterminate = false;
            }
            catch
            {
                Loading.Text = "Can't Load. Check Your Connection";
                pgr.IsIndeterminate = false;
            }
        }

        private void Actionbtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
    }
}