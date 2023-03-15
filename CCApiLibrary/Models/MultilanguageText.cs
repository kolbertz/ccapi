using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCApiLibrary.Models
{
    public class MultilanguageText
    {
        public MultilanguageText() { }

        public MultilanguageText(string culture, string text) { 
            Culture= culture;
            Text= text;
        }

        public string Culture { get; set; }
        public string Text { get; set; }
    }
}
