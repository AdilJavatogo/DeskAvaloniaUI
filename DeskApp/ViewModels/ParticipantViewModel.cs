using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DeskApp.ViewModels
{
    // Model for en enkelt person i mødet
    public class ParticipantViewModel : ViewModelBase
    {
        public string Name { get; }
        public string Initials => string.IsNullOrEmpty(Name) ? "?" : Name.Substring(0, 1);
        public IBrush AvatarColor { get; }

        private bool _isVideoOn = true;
        public bool IsVideoOn
        {
            get => _isVideoOn;
            set { _isVideoOn = value; OnPropertyChanged(); }
        }

        // Dette er billedet vi viser i UI
        private WriteableBitmap _videoFrame;
        public WriteableBitmap VideoFrame
        {
            get => _videoFrame;
            set { _videoFrame = value; OnPropertyChanged(); }
        }

        // Variabler til animation (kun for demo skyld)
        private double _animOffset = 0;

        public ParticipantViewModel(string name, Color color)
        {
            Name = name;
            AvatarColor = new SolidColorBrush(color);

            // Opret en tom bitmap (320x180 pixels for thumbnails)
            // DPI 96 er standard
            VideoFrame = new WriteableBitmap(new PixelSize(320, 180), new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Premul);
        }

        // Denne metode kaldes 30 gange i sekundet for at tegne nyt "kamera" billede
        public unsafe void UpdateVideoFrame()
        {
            // Lås bufferen
            using (var buf = VideoFrame.Lock())
            {
                int w = buf.Size.Width;
                int h = buf.Size.Height;

                // VIGTIGT: Hent Stride (RowBytes) fra bufferen. 
                // Dette er antal bytes per række, som kan være større end bredde * 4.
                int stride = buf.RowBytes;

                // Start-pointer til hukommelsen (byte* for at kunne bruge stride korrekt)
                byte* ptr = (byte*)buf.Address;

                // Demo Animation logic
                _animOffset += 5;
                if (_animOffset > w) _animOffset = 0;

                // Render loop
                for (int y = 0; y < h; y++)
                {
                    // Beregn pointer til starten af denne række
                    // Vi tager start-adressen og lægger (y * stride) til.
                    // Så caster vi til uint* for at skrive hele pixels (4 bytes) ad gangen.
                    uint* rowPtr = (uint*)(ptr + y * stride);

                    for (int x = 0; x < w; x++)
                    {
                        // Baggrundsfarve (Mørkegrå) - Format BGRA: AA RR GG BB (Little Endian)
                        // 0xFF202020 -> Alpha=FF, Red=20, Green=20, Blue=20
                        uint color = 0xFF202020;

                        // Tegn rød stribe
                        if (x > _animOffset && x < _animOffset + 20)
                        {
                            // Rød: 0xFFFF0000 -> Alpha=FF, Red=FF, Green=00, Blue=00
                            color = 0xFFFF0000;
                        }

                        // Skriv pixel til hukommelsen
                        // Da rowPtr allerede peger på starten af rækken, bruger vi bare [x]
                        rowPtr[x] = color;
                    }
                }
            }

            // Fortæl UI at dette billede skal tegnes igen
            OnPropertyChanged(nameof(VideoFrame));
        }
    }
}