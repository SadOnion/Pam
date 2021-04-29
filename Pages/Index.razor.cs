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
        public IJSRuntime js { get; set; }
        [Inject]
        NavigationManager navigationManager { get; set; }
        [Inject]
        public AppSettings appSettings { get; set; }
        private List<Image> list = new List<Image>();
        int focusedIndex = -1;
        string focusedImage = string.Empty;

        protected override void OnInitialized()
        {
            if(appSettings.Images.Count > 0)
            {
                list = appSettings.Images;
            }
        }
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
            ChangeFocus(list.Count-1);
        }
        [JSInvokable]
        public void Preview(string blob)
        {
            appSettings.GifBlob = blob;
            appSettings.Images = list;
            Console.WriteLine(blob);
            navigationManager.NavigateTo($"/Pam/preview");
            //navigationManager.NavigateTo($"/preview");
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
            List<int> delays = new List<int>();
            foreach (var item in list)
            {
                delays.Add(item.DelayInMs);
            }
            await js.InvokeVoidAsync("MakeGif", urls,delays, dotNetObjRef);
        }
        public void ChangeDelay(ChangeEventArgs args)
        {
            list[focusedIndex].DelayInMs = int.Parse((string)args.Value);
        }
        public void ChangeFocus(int index)
        {
            focusedIndex = index;
            focusedImage = list[index].ImageUrl;
        }
        public void ChangePosition(ChangeEventArgs args)
        {
            int newPosition = int.Parse((string)args.Value);
            Image img = list[focusedIndex];
            newPosition = Math.Clamp(newPosition,1,list.Count);
            list.RemoveAt(focusedIndex);
            list.Insert(newPosition - 1, img);
            ChangeFocus(newPosition - 1);
        }
        public void DeleteImage(int position)
        {
            list.RemoveAt(position);
            if (position == focusedIndex)
            {

                focusedIndex = -1;
                focusedImage = string.Empty;
            }
        }
    }
}
