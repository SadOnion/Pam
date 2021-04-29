using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pam.Model
{
    public class AppSettings
    {
        public string GifBlob { get; set; }
        public string SfxBlob {get; set;}
        public int DefaultDelay { get; set; } = 100;

        public List<Image> Images {get; set;} = new List<Image>();
        public List<Audio> Audios {get; set;} = new List<Audio>();
        public bool GifModeSelected {get; set;} = true;


    }
}
