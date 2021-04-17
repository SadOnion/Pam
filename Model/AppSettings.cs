using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pam.Model
{
    public class AppSettings
    {
        public string GifBlob { get; set; }
        public int DefaultDelay { get; set; } = 100;

        public List<Image> Images {get; set;} = new List<Image>();
        public bool GifModeSelected {get; set;} = true;

        public string CssBackgroundClass
        {
            get => GifModeSelected? "red-gradient" : "blue-gradient";
        }

    }
}
