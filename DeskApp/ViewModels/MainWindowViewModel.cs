using System;
using System.Collections.ObjectModel;
using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace DeskApp.ViewModels
{
    // Vi arver fra ViewModelBase som allerede er en ObservableObject
    public partial class MainWindowViewModel : ViewModelBase
    {
        // [ObservableProperty] laver automatisk 'ActiveSpeaker' property
        // og håndterer OnPropertyChanged for dig.
        [ObservableProperty]
        private ParticipantViewModel? _activeSpeaker;

        // Når IsMuted ændres, beder vi den også om at opdatere 'IsMutedIcon'
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsMutedIcon))]
        private bool _isMuted;

        // VIGTIGT: Vi sætter default til 'true' her
        // Når IsVideoOn ændres, opdateres 'IsVideoOnIcon' automatisk
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsVideoOnIcon))]
        private bool _isVideoOn = true;

        private System.Threading.Timer? _videoTimer;

        public ObservableCollection<ParticipantViewModel> Participants { get; } = new();

        // Computed properties (afhænger af variablerne ovenfor)
        public string IsMutedIcon => IsMuted ? "🔴" : "🎙️";
        public string IsVideoOnIcon => IsVideoOn ? "📹" : "🚫";

        public MainWindowViewModel()
        {
            // Opret dummy deltagere
            Participants.Add(new ParticipantViewModel("Lars Larsen", Colors.Blue));
            Participants.Add(new ParticipantViewModel("Mette Frederiksen", Colors.Red));
            Participants.Add(new ParticipantViewModel("Ole Opfinder", Colors.Green));

            // "Dig" starter med video tændt (matcher _isVideoOn = true)
            var me = new ParticipantViewModel("Dig (Mig)", Colors.Purple)
            {
                IsVideoOn = true
            };
            Participants.Add(me);

            ActiveSpeaker = Participants[0];

            // Start fake video stream
            _videoTimer = new System.Threading.Timer(SimulateIncomingVideoFrames, null, 0, 33);
        }

        // [RelayCommand] laver automatisk en 'ToggleMuteCommand' som du kan binde til i XAML
        [RelayCommand]
        public void ToggleMute()
        {
            IsMuted = !IsMuted;
        }

        [RelayCommand]
        public void ToggleVideo()
        {
            IsVideoOn = !IsVideoOn;

            // Find "Dig" i listen (index 3) og opdater også dens status
            // Så det lille billede i bunden også slukker/tænder
            if (Participants.Count > 3)
            {
                Participants[3].IsVideoOn = IsVideoOn;
            }
        }

        [RelayCommand]
        public void LeaveCall()
        {
            Environment.Exit(0);
        }

        private void SimulateIncomingVideoFrames(object? state)
        {
            Dispatcher.UIThread.Post(() =>
            {
                ActiveSpeaker?.UpdateVideoFrame();
                foreach (var p in Participants)
                {
                    if (p.IsVideoOn) p.UpdateVideoFrame();
                }
            });
        }
    }
}