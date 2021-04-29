using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Pam.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pam.Pages
{
    public partial class PreviewSfx : ComponentBase
    {
        [Inject]
        public AppSettings appSettings { get; set; }
        [Inject]
        public IJSRuntime js { get; set; }
        [Inject]
        public NavigationManager navigation { get; set; }

        public TimeSpan Duration { get; set; }
        protected override void OnInitialized()
        {
            float secs = (float)appSettings.Audios.SelectMany(x => x.ByteArray).Count() / ((44100 * 16 * 1) / 8);
            Duration = TimeSpan.FromSeconds(secs);
        }
        public void BackToEdit()
        {
            //navigation.NavigateTo("/sfx");
            navigation.NavigateTo("/Pam/sfx");
        }
        public async Task Save()
        {
            await js.InvokeVoidAsync("SaveFile", appSettings.SfxBlob);
        }

        public async Task Play(EventArgs args)
        {
            await js.InvokeVoidAsync("PlayAudioFile", appSettings.SfxBlob, "Sfx.wav");
        }
    }
}
