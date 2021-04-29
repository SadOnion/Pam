using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Pam.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Pam.Pages
{
    public partial class Sfx : ComponentBase
    {
        [Inject]
        public IJSRuntime js { get; set; }
        [Inject]
        NavigationManager navigationManager { get; set; }
        [Inject]
        public AppSettings appSettings { get; set; }
        public float Tempo { get; set; }
        private List<Audio> audioList  = new List<Audio>();
        int focusedIndex = -1;
        string focusedAudio = string.Empty;
        protected override void OnAfterRender(bool firstRender)
        {
            appSettings.GifModeSelected = false;
        }
        protected override void OnInitialized()
        {
            
            if(appSettings.Audios.Count > 0)
            {
                audioList = appSettings.Audios;
            }
            Tempo = 1f;
        }
        public void ChangeTempo(ChangeEventArgs args)
        {
            Console.WriteLine(args.Value);
            string val = (string)args.Value;
            float newValue = float.Parse(val.Replace('.',','));
            newValue = MathF.Max(MathF.Min(2f,newValue),0.1f);
            Tempo = newValue;
        }
        public async Task AddFile(InputFileChangeEventArgs args)
        {
            IBrowserFile audioFile = args.File;
            var buffers = new byte[audioFile.Size];
            await audioFile.OpenReadStream(maxAllowedSize: 2097152).ReadAsync(buffers);
            string audioType = audioFile.ContentType;
            var base64 = Convert.ToBase64String(buffers);
            string blob = $"data:{audioType};base64,{base64}";
            Console.WriteLine(audioType);
            Console.WriteLine(blob);
            Audio audio = new Audio()
            {
                Blob = blob,
                ByteArray = buffers
            };
            audioList.Add(audio);
        }
        public void Preview(EventArgs args)
        {
            MakeSfx();
            appSettings.Audios = audioList;
            
            //navigationManager.NavigateTo($"/Pam/previewSfx");
            navigationManager.NavigateTo($"/previewSfx");
        }

        public void MakeSfx()
        {
            string combinedSfx = WaveCombiner.Combine(audioList.Select(x => x.ByteArray).ToList(),Tempo);
            appSettings.SfxBlob = $"data:audio/wav;base64,{combinedSfx}";
        }

        public void ChangeFocus(int index)
        {
            focusedIndex = index;
            focusedAudio = audioList[index].Blob;
        }
        public async Task Play(EventArgs args)
        {
            await js.InvokeVoidAsync("PlayAudioFile",audioList[focusedIndex].Blob,"audio/wav");
        }
        public void ChangePosition(ChangeEventArgs args)
        {
            int newPosition = int.Parse((string)args.Value);
            newPosition = Math.Clamp(newPosition,1,audioList.Count);
            Audio audio = audioList[focusedIndex];
            audioList.RemoveAt(focusedIndex);
            audioList.Insert(newPosition - 1, audio);
            ChangeFocus(newPosition - 1);
        }
        public void DeleteImage(int position)
        {
            audioList.RemoveAt(position);
            if (position == focusedIndex)
            {
                
                focusedIndex = -1;
                focusedAudio = string.Empty;
            }
        }
    }
}
