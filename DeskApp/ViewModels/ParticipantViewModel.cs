using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace DeskApp.ViewModels
{
    // Model for en enkelt person i mødet
    public class ParticipantViewModel : ViewModelBase
    {
        public string Name { get; }
        public string Initials => Name.Substring(0, 1);
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
        private readonly Random _rnd = new Random();

        public ParticipantViewModel(string name, Color color)
        {
            Name = name;
            AvatarColor = new SolidColorBrush(color);

            // Opret en tom bitmap (f.eks. 320x180 pixels for thumbnails)
            // DPI 96 er standard
            VideoFrame = new WriteableBitmap(new PixelSize(320, 180), new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Premul);
        }

        // Denne metode kaldes 30 gange i sekundet for at tegne nyt "kamera" billede
        public unsafe void UpdateVideoFrame()
        {
            using (var buf = VideoFrame.Lock())
            {
                var ptr = (uint*)buf.Address;
                int w = buf.Size.Width;
                int h = buf.Size.Height;

                // Demo Animation: En bevægende bar der skifter farve (Simulerer video)
                _animOffset += 5;
                if (_animOffset > w) _animOffset = 0;

                // Fyld pixels (Simpel rendering loop)
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        // Baggrundsfarve (Mørkegrå støj)
                        uint color = 0xFF202020;

                        // Tegn en bevægende stribe
                        if (x > _animOffset && x < _animOffset + 20)
                        {
                            color = 0xFFFF0000; // Rød
                        }

                        // Skriv til hukommelsen
                        ptr[y * w + x] = color;
                    }
                }
            }
            // Fortæl UI at dette billede skal tegnes igen
            OnPropertyChanged(nameof(VideoFrame));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
