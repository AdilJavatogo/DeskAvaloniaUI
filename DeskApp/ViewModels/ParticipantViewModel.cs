using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DeskApp.ViewModels
{
    // Husk 'partial' så CommunityToolkit kan generere koden
    public partial class ParticipantViewModel : ViewModelBase
    {
        public string Name { get; }
        public string Initials => string.IsNullOrEmpty(Name) ? "?" : Name.Substring(0, 1);
        public IBrush AvatarColor { get; }

        // Erstatter den gamle manuelle IsVideoOn property
        [ObservableProperty]
        private bool _isVideoOn = true;

        // Erstatter den gamle manuelle VideoFrame property
        [ObservableProperty]
        private WriteableBitmap? _videoFrame;

        // Privat variabel til animations-logik
        private double _animOffset = 0;

        public ParticipantViewModel(string name, Color color)
        {
            Name = name;
            AvatarColor = new SolidColorBrush(color);

            // Opret en tom bitmap (320x180 pixels)
            VideoFrame = new WriteableBitmap(new PixelSize(320, 180), new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Premul);
        }

        // Tegne-metoden (Unsafe for performance)
        public unsafe void UpdateVideoFrame()
        {
            // Sikkerhedstjek hvis VideoFrame er null
            if (VideoFrame is null) return;

            // Når vi bruger 'using' på .Lock(), fortæller Avalonia automatisk UI'et, 
            // at billedet er ændret, når blokken slutter.
            using (var buf = VideoFrame.Lock())
            {
                int w = buf.Size.Width;
                int h = buf.Size.Height;
                int stride = buf.RowBytes; // VIGTIGT: Stride håndtering

                byte* ptr = (byte*)buf.Address;

                // Animation logic
                _animOffset += 5;
                if (_animOffset > w) _animOffset = 0;

                for (int y = 0; y < h; y++)
                {
                    // Pointer aritmetik for at finde rækken korrekt
                    uint* rowPtr = (uint*)(ptr + y * stride);

                    for (int x = 0; x < w; x++)
                    {
                        uint color = 0xFF202020; // Mørkegrå baggrund

                        // Rød stribe
                        if (x > _animOffset && x < _animOffset + 20)
                        {
                            color = 0xFFFF0000;
                        }

                        rowPtr[x] = color;
                    }
                }
            }
            // Bemærk: Vi behøver ikke længere kalde OnPropertyChanged(nameof(VideoFrame)),
            // da WriteableBitmap selv signalerer ændringer, når låsen slippes.
        }
    }
}