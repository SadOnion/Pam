using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Pam.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Pam.Pages
{
    public partial class Index : ComponentBase
    {
        [Inject]
        public IJSRuntime js{ get; set; }
        [Inject]
        NavigationManager navigationManager {get; set;}
        [Inject]
        public AppSettings appSettings {get; set;}
        private List<Image> list = new List<Image>();
        string focusedImage = string.Empty;
        public async Task AddFile(InputFileChangeEventArgs args)
        {
            IBrowserFile imgFile = args.File;
            var buffers = new byte[imgFile.Size];
            await imgFile.OpenReadStream(maxAllowedSize: 2097152).ReadAsync(buffers);
            string imageType = imgFile.ContentType;
            Image img = new Image() { ImageData = buffers, ImageType = imageType };

            Console.WriteLine(img.ImageType);
            Console.WriteLine(img.ImageData.Length);
            Console.WriteLine(img.ImageUrl);
            list.Add(img);
        }
        [JSInvokable]
        public void Preview(string blob)
        {
            appSettings.GifBlob = blob;
            Console.WriteLine(blob);
            navigationManager.NavigateTo($"/{navigationManager.BaseUri}/preview");
        }

        public async Task MakeGif()
        {
            var dotNetObjRef = DotNetObjectReference.Create(this);
            //await using var jsModule = await js.InvokeAsync<IJSObjectReference>("import", "./js/Audio.js");
            List<byte[]> urls = new List<byte[]>();
            foreach (var item in list)
            {
                urls.Add(item.ImageData);
            }
            await js.InvokeVoidAsync("MakeGif", urls,dotNetObjRef);
        }

        public void ChangeFocus(int position)
        {
            focusedImage = list[position].ImageUrl;
        }
    }
}
