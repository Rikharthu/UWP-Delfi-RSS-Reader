using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Delfi_RSS_Reader.Annotations;

namespace Delfi_RSS_Reader
{
    class RSSFeedViewModel : INotifyPropertyChanged
    {

        public HashSet<string> Categories;
        public string Title { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public string Language { get; set; }
        public string LastBuildDate { get; set; }

        public List<FeedItem> Items { get; set; }
        public ObservableCollection<FeedItem> FilteredItems { get; set; }

        private List<string> selectedCategories;
        public IList<object> SelectedCategories { get; set; }

        public RSSFeedViewModel()
        {
            Categories = new HashSet<string>();
            Items = new List<FeedItem>();
            FilteredItems = new ObservableCollection<FeedItem>();
        }

        public void FilterItems()
        {
            FilteredItems.Clear();
            if (SelectedCategories != null && SelectedCategories.Count > 0)
            {
               //var tempItems = Items.Where(item => item.Category.Equals("Европа"));
                var tempItems = from item in Items
                    join cat in SelectedCategories on item.Category equals cat
                    select item;
                //var tempItems = Items;
                foreach (var item in tempItems)
                {
                    FilteredItems.Add(item);
                }
            }
            else
            {
                foreach (var item in Items)
                {
                    FilteredItems.Add(item);
                }
            }
        }

        // GENERATED
        public event PropertyChangedEventHandler PropertyChanged;

        // GENERATED
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}