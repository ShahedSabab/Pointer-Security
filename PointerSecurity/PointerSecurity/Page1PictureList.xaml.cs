using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Net.Http;
using Newtonsoft.Json;

namespace PointerSecurity
{
    public partial class Page1PictureList : PhoneApplicationPage
    {

        public RootObject root = new RootObject();
        bool data = false;
        bool data2 = true;
        public Page1PictureList()
        {
            InitializeComponent();
        }

        private async void InitializeList()
        {

            HttpClient client = new HttpClient();
            string jsonString = await client.GetStringAsync("http://www.eyepointer.tk/sabab/connection/pic_details.php");
            string[] separator = new string[] { "<!-- www.1freehosting.com Analytics Code -->" };
            string[] result = jsonString.Split(separator, StringSplitOptions.None);
            root = JsonConvert.DeserializeObject<RootObject>(result[0]);
            data = true;
        }

        private IEnumerable<PicList> GetPicList()
        {
            List<PicList> pl = new List<PicList>();

            for (int i = 0; i < root.pics.Count; i++)
            {
                DateTime Date1 = Convert.ToDateTime(root.pics[i].date);
                Date1 = Date1.Date;
                string Date2 = Date1.ToString("d");
                DateTime Date3 = Convert.ToDateTime(Date2);

                pl.Add(new PicList() { id = root.pics[i].id, loc = root.pics[i].loc, DateTime = root.pics[i].date, DateGroup = Date2, date = Date3 });
            }
            return pl;


        }

        private List<Group<PicList>> GetPicGroups()
        {
            IEnumerable<PicList> pl = GetPicList();
            return GetItemGroups(pl, c => c.date);
        }

        private static List<Group<T>> GetItemGroups<T>(IEnumerable<T> itemlist, Func<T, DateTime> getkeyfunc)
        {
            IEnumerable<Group<T>> grouplist = from item in itemlist
                                              group item by getkeyfunc(item) into g
                                              orderby g.Key descending
                                              select new Group<T>(g.Key, g);

            return grouplist.ToList();
        }

        private void longListSelectorState_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PicList item = e.AddedItems[0] as PicList;

            if (longListSelectorState.SelectedItem == null)
                return;

            
            NavigationService.Navigate(new Uri("/ViewPhoto.xaml?loc=" +item.loc, UriKind.Relative));

            
            longListSelectorState.SelectedItem = null;
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            try
            {
                System.Threading.ThreadPool.QueueUserWorkItem(obj =>
                {

                    InitializeList();
                    System.Threading.Thread.Sleep(10000);

                    Dispatcher.BeginInvoke(() =>
                    {
                        Loading.Text = "";
                        PB.IsIndeterminate = false;
                        this.longListSelectorState.ItemsSource = this.GetPicGroups();
                    });
                });
            }
            catch
            {
                Loading.Text = "Can't Load. Check Your Connection";
                PB.IsIndeterminate = false;
            }
        }
    }
}