using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Pam.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pam.Pages
{
    public partial class Preview : ComponentBase
    {
        public string GifBlob { get; set; }
        [Inject]
        public AppSettings appSettings {get; set;}
        [Inject]
        public NavigationManager navigation {get; set;}
        [Inject]
        public IJSRuntime js { get; set; }
        protected override void OnInitialized()
        {
            GifBlob = appSettings.GifBlob;
        }

        public void BackToEdit()
        {
            navigation.NavigateTo("/");
        }

        public async Task Save()
        {
            await js.InvokeVoidAsync("SaveFile",GifBlob);
        }
    }
}
