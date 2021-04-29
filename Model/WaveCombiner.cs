using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Components.Forms;

namespace Pam.Model
{
    public class WaveCombiner
    {
        //https://www.codeproject.com/Articles/15187/Concatenating-Wave-Files-Using-C-2005
        public async Task<string> Merge(Audio[] files)
        {

            WaveFile wa_out = new WaveFile();

            wa_out.DataLength = 0;
            wa_out.Length = 0;

            List<WaveFile> waveFiles = new List<WaveFile>();
            //Gather header data
            foreach (Audio file in files)
            {
                WaveFile wa_IN = new WaveFile();
                await wa_IN.WaveHeaderIN(file);
                wa_out.DataLength += wa_IN.DataLength;
                wa_out.Length += wa_IN.Length;
                waveFiles.Add(wa_IN);
            }

            //Reconstruct new header
            wa_out.BitsPerSample = waveFiles[0].BitsPerSample;
            wa_out.Channels = waveFiles[0].Channels;
            wa_out.SampleRate = waveFiles[0].SampleRate;
            //wa_out.WaveHeaderOUT();

            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(new char[4] { 'R', 'I', 'F', 'F' });

            bw.Write(wa_out.Length);

            bw.Write(new char[8] { 'W', 'A', 'V', 'E', 'f', 'm', 't', ' ' });

            bw.Write((int)16);

            bw.Write((short)1);
            bw.Write(wa_out.Channels);

            bw.Write(wa_out.SampleRate);

            bw.Write((int)(wa_out.SampleRate * ((wa_out.BitsPerSample * wa_out.Channels) / 8)));

            bw.Write((short)((wa_out.BitsPerSample * wa_out.Channels) / 8));

            bw.Write(wa_out.BitsPerSample);

            bw.Write(new char[4] { 'd', 'a', 't', 'a' });
            bw.Write(wa_out.DataLength);
            foreach (WaveFile file in waveFiles)
            {
                bw.Write(file.NoHeaderArray);
            }
            byte[] newWav = new byte[ms.Length];
            await ms.ReadAsync(newWav);
            bw.Close();
            FileStream fileStream = new FileStream("cojest.wav", FileMode.Create);
            fileStream.Write(newWav);
            Console.WriteLine(newWav.Length);

            fileStream.Close();
            //Console.WriteLine($"data:audio/wav;base64,{Convert.ToBase64String(newWav)}");
            return $";base64,{Convert.ToBase64String(newWav)}";
        }
        public static string Combine(byte[] buffer1, byte[] buffer2)
        {
            int dataSize = buffer1.Length + buffer2.Length - 44;
            byte[] buffer = new byte[dataSize];
            MemoryStream ms = new MemoryStream(buffer);
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(buffer1);
            ms.Position = 41;
            bw.Write(dataSize);
            ms.Position = buffer1.Length;
            bw.Write(buffer2.Skip(44).ToArray());
            return Convert.ToBase64String(buffer);
        }
        public static string Combine(List<byte[]> buffers, float pitch = 1f)
        {
            int dataSize = buffers[0].Length;
            float scale = pitch;
            for (int i = 1; i < buffers.Count; i++)
            {
                dataSize += buffers[i].Length - 44;
            }

            byte[] buffer = new byte[dataSize];
            MemoryStream ms = new MemoryStream(buffer);
            BinaryWriter bw = new BinaryWriter(ms);
            BinaryReader br = new BinaryReader(ms);
            bw.Write(buffers[0]);
            ms.Position = 41;
            bw.Write(dataSize);
            ms.Position= 25;
            int sampleRate = br.ReadInt32();
            ms.Position = 25;
            bw.Write((int)(sampleRate * scale));
            ms.Position = buffers[0].Length;
            for (int i = 1; i < buffers.Count; i++)
            {
                bw.Write(buffers[i].Skip(44).ToArray());
            }
            return Convert.ToBase64String(buffer);
        }
    }

    public class WaveFile
    {
        public int Length { get; set; }
        public short Channels { get; set; }
        public int SampleRate { get; set; }
        public short BitsPerSample { get; set; }
        public int DataLength { get; set; }

        public byte[] ByteArray { get; set; }
        public byte[] NoHeaderArray { get; set; }
        public async Task WaveHeaderIN(Audio file)
        {
            ByteArray = file.ByteArray;
            byte[] arrfile = new byte[file.ByteArray.Length - 44];
            Array.Copy(file.ByteArray, 44, arrfile, 0, file.ByteArray.Length - 44);
            NoHeaderArray = arrfile;
            MemoryStream ms = new MemoryStream();
            await ms.WriteAsync(file.ByteArray);
            BinaryReader br = new BinaryReader(ms);
            Length = (int)ms.Length - 8;
            ms.Position = 22;
            Channels = br.ReadInt16();
            ms.Position = 24;
            SampleRate = br.ReadInt32();
            ms.Position = 34;

            BitsPerSample = br.ReadInt16();
            DataLength = (int)ms.Length - 44;
            br.Close();
            ms.Close();
        }

    }
}
