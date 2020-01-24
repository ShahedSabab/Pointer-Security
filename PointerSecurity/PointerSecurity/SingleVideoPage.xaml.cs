using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.Networking;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Data;
using System.Windows.Media.Imaging;
using Windows.UI.Core;
using Windows.ApplicationModel.Core;


namespace PointerSecurity
{
    public partial class SingleVideoPage : PhoneApplicationPage
    {
        public BitmapImage bmp;
        static DatagramSocket ab;
        static bool SocketsAreConnected = false;
        string type;
        public SingleVideoPage()
        {
           
            InitializeComponent();
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationContext.QueryString.TryGetValue("vid", out type);

            try
            {
                InitializeSockets();
            }
            catch
            {
                Loading.Text = "Can't Load Video. Check Your Connection";
                pgr.IsIndeterminate = false;
            }
        }

        public async void InitializeSockets()
        {

            if (SocketsAreConnected)
                return;

            ab = new DatagramSocket();
           

            ab.MessageReceived += ClientSocketMessageReceived;
           
            try
            {
                if (type == "Video")
                {
                    await ab.BindServiceNameAsync("11444");
                }
                else if(type == "Screen")
                {
                    await ab.BindServiceNameAsync("11445");
                }
            }
            catch (Exception exception)
            {
                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                {
                    throw;
                }

                // DebugLogOut("Error: Start Listen failed: " + exception.Message);
                return;
            }

            SocketsAreConnected = true;
        }

        public async void ClientSocketMessageReceived(DatagramSocket socket, DatagramSocketMessageReceivedEventArgs eventArguments)
        {

            IBuffer MyBuffer = eventArguments.GetDataReader().DetachBuffer();
            byte[] MessageArray = MyBuffer.ToArray();



            using (MemoryStream ms = new MemoryStream(MessageArray, 0, MessageArray.Length))
            {
                ms.Write(MessageArray, 0, MessageArray.Length);
                //showVideo(ms);
                Debug.WriteLine(ms.ToString());
                CoreDispatcher c = CoreApplication.MainView.CoreWindow.Dispatcher;
                await c.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    bmp = new BitmapImage();
                    bmp.SetSource(ms);
                    video.Source = bmp;
                    Loading.Text = "";
                    pgr.IsIndeterminate = false;
                });

            }

        }

        public void showVideo(MemoryStream s)
        {
            bmp = new BitmapImage();
            bmp.SetSource(s);
            video.Source = bmp;


        }
        public static void TearDownSockets()
        {
            if (ab != null)
            {
                ab.Dispose();
                ab = null;


            }

            SocketsAreConnected = false;
        }
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            TearDownSockets();
            tcpdata();
            videorequest();
            NavigationService.Navigate(new Uri("/VideoStreaming.xaml", UriKind.Relative));
        }
        //protected override void OnNavigatedFrom(NavigationEventArgs e)
        //{
        //    TearDownSockets();
        //    tcpdata();
        //    videorequest();
        //}

        private void tcpdata()
        {
            SocketClient client = new SocketClient();
            string result = client.Connect(MainPage.ipaddress, 100);

            result = client.Send("stop stream");////////////////////////////

            result = client.Receive();

            client.Close();

            //NavigationService.Navigate(new Uri("/VideoStreaming.xaml", UriKind.Relative));
        }
        private void videorequest()
        {
            SocketClient client = new SocketClient();
            string result = client.Connect(MainPage.ipaddress, 100);

            result = client.Send("start spy");////////////////////////////

            result = client.Receive();

            client.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TearDownSockets();
            tcpdata();
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
    }
}