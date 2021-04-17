using System;
namespace Pam
{
    public class Image
    {
        public byte[] ImageData { get; set; }
        public string ImageType {get; set;}
        public string ImageUrl => $"data:{ImageType};base64,{Convert.ToBase64String(ImageData)}";
        public int DelayInMs { get; set; } = 100;
    }
}
