using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Delfi_RSS_Reader
{
    public sealed partial class MainPage : Page
    {

        private bool isTitle = false;
        private bool isDescription = false;
        private bool isLink = false;
        private bool isPubDate = false;
        private bool isCategory = false;
        private bool isTopTitle = true;
        private bool isImageTitle = true;
        private bool isItem = false;
        private bool isChannel = false;
        private bool isImageUrl = false;
        private FeedItem feedItem;
        private RSSFeedViewModel _rssFeedViewModel;
        private List<string> selectedCategories;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void button1_Click(object sender, RoutedEventArgs e)
        {
            LoadingProgressRing.IsActive = true;
            listBox.Visibility = Visibility.Collapsed;
            DataContext = null;
            // симуляция загрузки
            await Task.Delay(2000);

            bool isInternetConnected = NetworkInterface.GetIsNetworkAvailable();

            if (isInternetConnected)
            {

                HttpClient client = new HttpClient();

                // open response stream 
                //var response = await client.GetStreamAsync("http://feeds.bbci.co.uk/news/world/rss.xml");
                var response = await client.GetStreamAsync("http://www.delfi.lv/rss.php");
                // convert stream to ling
                XDocument xmlDoc = XDocument.Load(response);

                StringBuilder output = new StringBuilder();
                
                // convert linq data to string
                String xmlString = xmlDoc.Document.ToString();

                // Create an XmlReader from a string
                using (XmlReader reader = XmlReader.Create(new StringReader(xmlString)))
                {
                    XmlWriterSettings ws = new XmlWriterSettings();
                    ws.Indent = true;
                    using (XmlWriter writer = XmlWriter.Create(output, ws))
                    {
                        // Parse the file and display each of the nodes.
                        while (reader.Read()) // read a single node
                        {
                            switch (reader.NodeType)
                            {                           
                                case XmlNodeType.Element:
                                    // this node is XML element
                                    switch (reader.Name)
                                    {
                                        case "title":
                                            isTitle = true;
                                            break;
                                        case "link":
                                            isLink = true;
                                            break;
                                        case "category":
                                            isCategory = true;
                                            break;
                                        case "description":
                                            isDescription = true;
                                            break;
                                        case "img":
                                            isImageUrl = true;
                                            break;
                                        case "pubDate":
                                            isPubDate = true;
                                            break;
                                        case "item":
                                            isItem = true;
                                            isChannel = false;
                                            feedItem = new FeedItem();
                                            break;
                                        case "channel":
                                            isChannel = true;
                                            _rssFeedViewModel = new RSSFeedViewModel();
                                            break;
                                    }
                                    break;
                                // tag contents
                                case XmlNodeType.CDATA: // because some attributes are CDATA
                                case XmlNodeType.Text:
                                    // Типа имитируем работу SAX парсера в андроиде
                                    // now parse text
                                    if (isChannel)
                                    {
                                        // set feed properties
                                        if (isTitle)
                                        {
                                            isTitle = false;
                                            _rssFeedViewModel.Title = reader.Value;
                                        }
                                        else if (isDescription)
                                        {
                                            isDescription = false;
                                            _rssFeedViewModel.Description = reader.Value;
                                        }
                                    }
                                    else if (isItem)
                                    {
                                        if (isTitle)
                                        {
                                            isTitle = false;
                                            feedItem.Title = reader.Value;
                                        }
                                        else if (isCategory)
                                        {
                                            isCategory = false;
                                            feedItem.Category = reader.Value;
                                            // attempt to extract imageUrl from the description 
                                            
                                            _rssFeedViewModel.Categories.Add(feedItem.Category);
                                        }
                                        else if (isDescription)
                                        {
                                            isDescription = false;
                                            feedItem.Description = reader.Value;
                                        }
                                        else if (isLink)
                                        {
                                            isLink = false;
                                            feedItem.Link = reader.Value;
                                        }
                                        else if (isPubDate)
                                        {
                                            // TODO в первый раз не сэтит
                                            isPubDate = false;
                                            feedItem.PubDate = reader.Value;
                                        }else if (isImageUrl) // images
                                        {
                                            feedItem.ImageUrl = reader.Value;
                                        }
                                    }
                                    break;
                                case XmlNodeType.EndElement:
                                    if (reader.Name == "item")
                                    {
                                        // finish item and add to list
                                        //textBlock.Text += feedItem.ToString()+"\n\n";
                                        isItem = false;
                                        _rssFeedViewModel.Items.Add(feedItem);
                                    }
                                    else if (reader.Name == "channel")
                                    {
                                        //isChannel = false;
                                        // finished
                                    }
                                    break;
                                case XmlNodeType.XmlDeclaration:
                                case XmlNodeType.ProcessingInstruction:
                                    writer.WriteProcessingInstruction(reader.Name, reader.Value);
                                    break;
                                case XmlNodeType.Comment:
                                    break;
                            }


                        }
                        String data = "";
                        //foreach (var rssFeedItem in _rssFeedViewModel.Items)
                        //{
                        //    // TODO сделать всем метод ToHtmlString() для WebView
                        //    //textBlock.Text += rssFeedItem.ToString() + "\n\n";
                        //    data += rssFeedItem.ToString()+"\n";
                            
                        //}
                        listBox.Visibility = Visibility.Visible;
                        //textBlock.Text = data;
                        DataContext = _rssFeedViewModel;
                        _rssFeedViewModel.FilterItems();
                        listBox.ItemsSource = FeedItem.Categories.ToList();
                    }
                }
                //textBlock.Text = feedItem.ToString();
                ShowToastNotification("Finished!", "Finished loading and parsing RSS content!");
            }
            else
            {
                // no internet
                // show dialog
                var dialog = new Windows.UI.Popups.MessageDialog(
                "Please check your connection.",
                "No internet connection!");

                var okCommand = new Windows.UI.Popups.UICommand("Ok") {Id = 0};
                var retryCommand = new Windows.UI.Popups.UICommand("Retry") {Id = 1};

                dialog.Commands.Add(okCommand);
                dialog.Commands.Add(retryCommand);

                //if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily != "Windows.Mobile")
                //{
                //    // Adding a 3rd command will crash the app when running on Mobile !!!
                //    dialog.Commands.Add(new Windows.UI.Popups.UICommand("Maybe later") { Id = 2 });
                //}

                dialog.DefaultCommandIndex = 1;
                dialog.CancelCommandIndex = 0;

                var result = await dialog.ShowAsync();
                if (result == retryCommand)
                {
                    // retry
                }
            }
            LoadingProgressRing.IsActive = false;
        }

        private void ShowToastNotification(string title, string stringContent)
        {
            ToastNotifier ToastNotifier = ToastNotificationManager.CreateToastNotifier();
            Windows.Data.Xml.Dom.XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            Windows.Data.Xml.Dom.XmlNodeList toastNodeList = toastXml.GetElementsByTagName("text");
            toastNodeList.Item(0).AppendChild(toastXml.CreateTextNode(title));
            toastNodeList.Item(1).AppendChild(toastXml.CreateTextNode(stringContent));
            Windows.Data.Xml.Dom.IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
            Windows.Data.Xml.Dom.XmlElement audio = toastXml.CreateElement("audio");
            audio.SetAttribute("src", "ms-winsoundevent:Notification.SMS");

            ToastNotification toast = new ToastNotification(toastXml);
            toast.ExpirationTime = DateTime.Now.AddSeconds(4);
            ToastNotifier.Show(toast);
        }

        public static string ReverseString(string s)
        {
            char[] arr = s.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _rssFeedViewModel.SelectedCategories = ((ListBox) sender).SelectedItems;
            _rssFeedViewModel.FilterItems();
        }

        private void FeedListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FeedItem item = (sender as ListView).SelectedItem as FeedItem;
            detailsFrame.Navigate(typeof (DetailsPage), item);
        }
    }

}
