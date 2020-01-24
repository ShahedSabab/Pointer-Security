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
    public partial class VideoStreaming : PhoneApplicationPage
    {

        public BitmapImage bmp;
        static DatagramSocket ab;
        static DatagramSocket scr;
        public BitmapImage scr_bmp;
        static bool SocketsAreConnected = false;
        public VideoStreaming()
        {
            InitializeComponent();
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                InitializeSockets();
            }
            catch
            {
                Loading.Text = "Can't Load The Video. Check network Connection";
                pgr.IsIndeterminate = false;
            }
        }
        public async void InitializeSockets()
        {
            
            if (SocketsAreConnected)
                return;

            ab = new DatagramSocket();
            scr = new DatagramSocket();

            ab.MessageReceived += ClientSocketMessageReceived;
            scr.MessageReceived += ClientSocketMessageReceived2;
            try
            {
                await ab.BindServiceNameAsync("11444");
                await scr.BindServiceNameAsync("11445");
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
                    VideoStream.Source = bmp;
                    Loading.Text = "";
                    pgr.IsIndeterminate = false;
                });

            }



        }


        public async void ClientSocketMessageReceived2(DatagramSocket socket, DatagramSocketMessageReceivedEventArgs eventArguments)
        {

            IBuffer MyBuffer = eventArguments.GetDataReader().DetachBuffer();
            // Convert the received IBuffer into a string.
            byte[] MessageArray = MyBuffer.ToArray();
            


            using (MemoryStream ms = new MemoryStream(MessageArray, 0, MessageArray.Length))
            {
                ms.Write(MessageArray, 0, MessageArray.Length);
                //showVideo(ms);
                Debug.WriteLine(ms.ToString());
                CoreDispatcher c = CoreApplication.MainView.CoreWindow.Dispatcher;
                await c.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    scr_bmp = new BitmapImage();
                    scr_bmp.SetSource(ms);
                    ScreenStream.Source = scr_bmp;

                });

                //Deployment.Current.Dispatcher.BeginInvoke(new Action(() => showVideo(ms)));

                //Deployment.Current.Dispatcher.BeginInvoke(() => MessageBox.Show(ms.ToString()));

            }
        }
        public void showVideo(MemoryStream s)
        {
            bmp = new BitmapImage();
            bmp.SetSource(s);
            VideoStream.Source = bmp;


        }
        public static void TearDownSockets()
        {
            if (ab != null)
            {
                ab.Dispose();
                ab = null;
               
                
            }

            if (scr != null)
            {
                scr.Dispose();
                scr = null;
                
            }

            SocketsAreConnected = false;
        }

        private void VideoStream_DoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            
            TearDownSockets();
            tcpdata();
            videorequest();
            NavigationService.Navigate(new Uri("/SingleVideoPage.xaml?vid=" +"Video" , UriKind.Relative));
        }

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

            result = client.Send("video stream");////////////////////////////

            result = client.Receive();

            client.Close();
        }

        private void ScreenStream_DoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            TearDownSockets();
            tcpdata();
            videorequest2();
            NavigationService.Navigate(new Uri("/SingleVideoPage.xaml?vid=" + "Screen", UriKind.Relative));
        }

        private void videorequest2()
        {
            SocketClient client = new SocketClient();
            string result = client.Connect(MainPage.ipaddress, 100);

            result = client.Send("screen stream");////////////////////////////

            result = client.Receive();

            client.Close();
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            TearDownSockets();
            tcpdata();
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
    }
}