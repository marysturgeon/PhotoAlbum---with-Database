using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoAlbum
{
    public class Photograph
    {
        private int id;
        private string title;
        private DateTime datetaken;
        private DateTime datemod;
        private string description;
        private string photog;
        private string keywords;
        private string location;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        public DateTime DateTaken
        {
            get { return datetaken; }
            set { datetaken = value; }
        }
        public DateTime DateMod
        {
            get { return datemod; }
            set { datemod = value; }
        }
        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        public string Photog
        {
            get { return photog; }
            set { photog = value; }
        }
        public string Keywords
        {
            get { return keywords; }
            set { keywords = value; }
        }
        public string Location
        {
            get { return location; }
            set { location = value; }
        }

        public Photograph(string title, DateTime datetaken, DateTime datemod, string desc, 
            string photog, string keywords, string location)
        {
            Title = title;
            DateTaken = datetaken;
            DateMod = datemod;
            Description = desc;
            Photog = photog;
            Keywords = keywords;
            Location = location;
        }

        public override string ToString()
        {
            return Title.ToString() + "\t" + DateTaken.ToShortDateString() + "\t" + Description.ToString();
        }
    }

    
}
