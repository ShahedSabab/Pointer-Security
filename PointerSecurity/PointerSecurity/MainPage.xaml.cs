using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PointerSecurity.Resources;
using Windows.Phone.Speech.Synthesis;
using Windows.Phone.Speech.Recognition;
using Windows.Foundation;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Net.NetworkInformation;

namespace PointerSecurity
{
    

    public partial class MainPage : PhoneApplicationPage
    {
        const int ECHO_PORT = 100;
        const int QOTD_PORT = 100;
        public static string ipaddress = "";
        ///////////////for voice command/////////////////////////////
        SpeechSynthesizer _synthesizer;
        SpeechRecognizer _recognizer;
        IAsyncOperation<SpeechRecognitionResult> _recoOperation;
        bool _recoEnabled = true;
        bool _listnEnable = false;
        string[] genre = { "Shut Down", "Lock Screen", "Stop Listening", "Start Listening", "Start Spy","Show Uploaded Photo" };
        bool serverRunning = false;
        bool LiveStream = false;
        bool ScreenStream = false;
        public MainPage()
        {
            InitializeComponent();
            //private List<LocalHostItem> localHostItems = new List<LocalHostItem>();
            
            
        }

        private void ApplyAction_Click(object sender, RoutedEventArgs e)
        {
            ClearLog();
            if (ValidateRemoteHost())
            {
                ipaddress = txtRemoteHost.Text;
                SocketClient client = new SocketClient();

                

                if (Lockscreenbtn.IsChecked.Value && !Shutdownbtn.IsChecked.Value)
                {
                    Log(String.Format("Connecting to server '{0}' over port {1} (echo) ...", txtRemoteHost.Text, ECHO_PORT), true);
                    string result = client.Connect(txtRemoteHost.Text, ECHO_PORT);
                    Log(result, false);

                    // Attempt to send our message to be echoed to the echo server
                    Log(String.Format("Sending '{0}' to server ...", "Lock Screen"), true);//////////////////
                    result = client.Send("Lock Screen");////////////////////////////
                    Log(result, false);

                    // Receive a response from the echo server
                    Log("Requesting Acknowledgement", true);
                    result = client.Receive();
                    Log(result, false);
                    if (result == "NotConnected")
                    {
                        serverRunning = false;
                    }
                    else serverRunning = true;
                    // Close the socket connection explicitly
                    client.Close();
                    Lockscreenbtn.IsChecked = false;
                    Shutdownbtn.IsChecked = false;
                }

                else if ((!Lockscreenbtn.IsChecked.Value && Shutdownbtn.IsChecked.Value) || (Lockscreenbtn.IsChecked.Value && Shutdownbtn.IsChecked.Value))
                {
                    Log(String.Format("Connecting to server '{0}' over port {1} (echo) ...", txtRemoteHost.Text, ECHO_PORT), true);
                    string result = client.Connect(txtRemoteHost.Text, ECHO_PORT);
                    Log(result, false);

                    // Attempt to send our message to be echoed to the echo server
                    Log(String.Format("Sending '{0}' to server ...","Shut Down" ), true);//////////////////
                    result = client.Send("Shut Down");////////////////////////////
                    Log(result, false);

                    // Receive a response from the echo server
                    Log("Requesting Acknowledgement", true);
                    result = client.Receive();
                    Log(result, false);
                    if (result == "NotConnected")
                    {
                        serverRunning = false;
                    }
                    else serverRunning = true;
                    // Close the socket connection explicitly
                    client.Close();
                    Lockscreenbtn.IsChecked = false;
                    Shutdownbtn.IsChecked = false;
                }

                else if (!Lockscreenbtn.IsChecked.Value && !Shutdownbtn.IsChecked.Value)
                {
                    MessageBox.Show("No Actions Selected");

                }

            }

            else MessageBox.Show("Enter Host name");
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            _listnEnable = false;
            serverRunning = false;
            LiveStream = false;
            ScreenStream = false;
            _recoEnabled = true;
            txtRemoteHost.Text = ipaddress;
            try
            {

                if (_synthesizer == null)
                {
                    _synthesizer = new SpeechSynthesizer();
                }
                if (_recognizer == null)
                {
                    _recognizer = new SpeechRecognizer();


                    _recognizer.Grammars.AddGrammarFromList("genres", genre);
                }

            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }

            SpeachInput();

            base.OnNavigatedTo(e);
        }


        #region UI Validation
   
        private bool ValidateRemoteHost()
        {
            // The txtRemoteHost must contain some text
            if (String.IsNullOrWhiteSpace(txtRemoteHost.Text))
            {
                MessageBox.Show("Please enter a host name");
                return false;
            }

            return true;
        }
        #endregion

        #region Logging
        private void Log(string message, bool isOutgoing)
        {
            string direction = (isOutgoing) ? ">> " : "<< ";
            txtOutput.Text += Environment.NewLine + direction + message;
        }

        private void ClearLog()
        {
            txtOutput.Text = String.Empty;
        }
        #endregion

        private async void SpeachInput()
        {
            
            while (this._recoEnabled)
            {
                try
                {
                    // Perform speech recognition.  
                    _recoOperation = _recognizer.RecognizeAsync();
                    var recoResult = await this._recoOperation;

                    // Check the confidence level of the speech recognition attempt.
                    if (recoResult.TextConfidence < SpeechRecognitionConfidence.Medium)
                    {
                        //await _synthesizer.SpeakTextAsync("Not sure what you said, please try again");
                    }
                    else
                    {
                        string say = recoResult.Text;
                        
                        if (say == "Shut Down" && _listnEnable == true)
                        {
                            Shutdownbtn.IsChecked = true;
                            await _synthesizer.SpeakTextAsync("Action Recognized" + say);
                            
                            ApplyAction_Click(this, null);
                             if (serverRunning == false)
                             {
                                 await _synthesizer.SpeakTextAsync("Server Program is not running");
                             }
                        }

                        else if (say == "Lock Screen" && _listnEnable == true)
                        {
                            Lockscreenbtn.IsChecked = true;
                            await _synthesizer.SpeakTextAsync("Action Recognized" + say);
                            
                            ApplyAction_Click(this, null);
                            if (serverRunning == false)
                            {
                                await _synthesizer.SpeakTextAsync("Server Program is not running");
                            }
                        }

                        else if(say == "Show Uploaded Photo" && _listnEnable == true)
                        {
                            if (NetworkInterface.GetIsNetworkAvailable())
                            {
                                _recoEnabled = false;
                                _recoOperation.Cancel();
                                NavigationService.Navigate(new Uri("/Page1PictureList.xaml", UriKind.Relative));
                            }
                            else await _synthesizer.SpeakTextAsync("No Internet Connection");
                        }

                        else if (say == "Stop Listening" && _listnEnable == true)
                        {
                            await _synthesizer.SpeakTextAsync("Listening Stopped");
                            _listnEnable = false;
                        }

                        else if (say == "Start Listening" && _listnEnable == false)
                        {
                            await _synthesizer.SpeakTextAsync("Listening Started");
                            _listnEnable = true;
                        }

                        else if (say == "Start Spy" && _listnEnable == true)
                        {
                            await _synthesizer.SpeakTextAsync("Action Recognized" + say);
                           
                            LiveStream = true;
                            VideoRequest();
                             if (serverRunning == true)
                            {
                                _recoEnabled = false;
                                _recoOperation.Cancel();
                                NavigationService.Navigate(new Uri("/VideoStreaming.xaml", UriKind.Relative));
                            }
                            else
                            {
                                await _synthesizer.SpeakTextAsync("Server Program is not running");
                                LiveStream = false;
                                //return;
                            }
                        }
                    }
                }
                catch (System.Threading.Tasks.TaskCanceledException)
                {

                }
                catch (Exception err)
                {
                    // Handle the speech privacy policy error.
                    const int privacyPolicyHResult = unchecked((int)0x80045509);

                    if (err.HResult == privacyPolicyHResult)
                    {
                        MessageBox.Show("To run this sample, you must first accept the speech privacy policy. To do so, navigate to Settings -> speech on your phone and check 'Enable Speech Recognition Service' ");
                        _recoEnabled = false;

                    }
                }
            }


        }

        private void VideoRequest()
        {
            ClearLog();

            if (ValidateRemoteHost())
            {
                ipaddress = txtRemoteHost.Text;
                SocketClient client = new SocketClient();
                
                if(LiveStream == true)
                {
                    Log(String.Format("Connecting to server '{0}' over port {1} (echo) ...", txtRemoteHost.Text, ECHO_PORT), true);
                    string result = client.Connect(txtRemoteHost.Text, ECHO_PORT);
                    Log(result, false);

                    // Attempt to send our message to be echoed to the echo server
                    Log(String.Format("Sending '{0}' to server ...", "start spy"), true);//////////////////
                    result = client.Send("start spy");////////////////////////////
                    Log(result, false);

                    // Receive a response from the echo server
                    Log("Requesting Acknowledgement", true);
                    result = client.Receive();
                    Log(result, false);
                    if (result == "NotConnected")
                    {
                        serverRunning = false;
                    }
                    else serverRunning = true;
                    // Close the socket connection explicitly
                    client.Close();
                    
                    
                }

            }

        }

        private void ShowPhoto_Click(object sender, RoutedEventArgs e)
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                _recoEnabled = false;
                _recoOperation.Cancel();
                NavigationService.Navigate(new Uri("/Page1PictureList.xaml", UriKind.Relative));
            }
            else MessageBox.Show("No Internet Connection");
        }

        private void ScrollViewer_DoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Lockscreenbtn.IsChecked = false;
            Shutdownbtn.IsChecked = false;
        }

        private void VideoStream_Click(object sender, RoutedEventArgs e)
        {
            LiveStream = true;
            VideoRequest();
            if (serverRunning == true)
            {
                _recoEnabled = false;
                _recoOperation.Cancel();
                NavigationService.Navigate(new Uri("/VideoStreaming.xaml", UriKind.Relative));
            }
            else
            {
                MessageBox.Show("Server Program is not running");
                LiveStream = false;
                //return;
            }
        }

        private void txtRemoteHost_LostFocus(object sender, RoutedEventArgs e)
        {
            ipaddress = txtRemoteHost.Text;
        }
    }
}