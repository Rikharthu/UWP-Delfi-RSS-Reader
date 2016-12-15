using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Delfi_RSS_Reader
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DetailsPage : Page
    {
        public DetailsPage()
        {
            this.InitializeComponent();
        }

        private FeedItem item;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            item = (FeedItem)e.Parameter;
            TitleTb.Text = item.Title;
            DatePublishedTb.Text = item.PubDate;
            DescriptionWv.NavigateToString(item.Description);
            //DescriptionTb.Text = item.Description;
            OpenInBrowserBtn.Click += OpenInBrowserBtn_Click;
            // TODO implement
            // http://stackoverflow.com/questions/35304615/pass-some-parameters-between-pages-in-uwp
        }

        private void OpenInBrowserBtn_Click(object sender, RoutedEventArgs e)
        {
            var uri = new Uri(item.Link);

            var success = Windows.System.Launcher.LaunchUriAsync(uri);

        }
    }
}
