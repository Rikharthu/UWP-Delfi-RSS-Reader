using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Delfi_RSS_Reader
{   
    /** Model object */
    class FeedItem
    {

        public static HashSet<string> Categories=new HashSet<string>();
        public string Title { get; set; }
        public string Link { get; set; }
        public string ImageUrl { get; set; }
        /** HTML-friendly link for WebView*/
        public string HtmlLink {
            get{
                // target="_blank" позаботиться о том, чтобы ссылка открывалась в новой вкладе =>Не наша программа будет хэндлить
                String newLink = "<a href=\"" + Link + "\" target=\"_blank\">"+"Больше..."+"</a>";
                return newLink;
            }
        }
        // TODO сделать метод, который вернёт описание БЕЗ html тэгов
        public string Description { get; set; }

        public string PlainTextDescription
        {
            get
            {
                return Regex.Replace(Description, "<.*?>", String.Empty);
            }
        }

        // TODO parse to normal date and get return according to locale
        private string pubDate;
        public string PubDate {
            get
            {
                DateTime dt = DateTime.Parse(pubDate);
                return dt.ToString("dd.MM, hh:mm");
            }
            set { pubDate = value; }
        }

        private string category;
        public string Category
        {
            get { return category; }
            set
            {
                category = value;
                Categories.Add(category);
            }
        }

        public virtual string ToString()
        {
            string str = "Title: " + Title + "\nDescription: " + Description
                         + "\nLink: " + Link + "\nPubDate: " + PubDate
                         + "\nCategory: " + Category;
            return str;
        }

        public virtual string ToHtmlString()
        {
            string str = "Title: " + Title + "</br>Description: " + Description
                         + "</br>Link: " + HtmlLink + "</br>PubDate: " + PubDate
                         + "</br>Category: " + Category;
            return str;
        }
    }

    

}
