using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Navigation
{
    public class MyItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string PictureUri { get; set; }

        public MyItem(string title, string description, string pictureUri)
        {
            Title = title;
            Description = description;
            PictureUri = pictureUri;
        }
    }
}
