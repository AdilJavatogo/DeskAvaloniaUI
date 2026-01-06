using System;
using System.Collections.ObjectModel;
using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace DeskApp.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ParticipantViewModel? _activeSpeaker;

        // Fortæl at IsMutedIcon skal opdateres, når _isMuted ændres
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsMutedIcon))]
        private bool _isMuted;

        // VIGTIGT: Fortæl at IsVideoOnIcon skal opdateres, når _isVideoOn ændres
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsVideoOnIcon))]
        private bool _isVideoOn = true;

        private System.Threading.Timer? _videoTimer;

        public ObservableCollection<ParticipantViewModel> Participants { get; } = new();

        public string IsMutedIcon => IsMuted ? "🔴" : "🎙️";
        public string IsVideoOnIcon => IsVideoOn ? "📹" : "🚫";

        public MainWindowViewModel()
        {
            Participants.Add(new ParticipantViewModel("Lars Larsen", Colors.Blue));
            Participants.Add(new ParticipantViewModel("Mette Frederiksen", Colors.Red));
            Participants.Add(new ParticipantViewModel("Ole Opfinder", Colors.Green));

            // Opret dig selv med video tændt
            var me = new ParticipantViewModel("Dig (Mig)", Colors.Purple)
            {
                IsVideoOn = true
            };
            Participants.Add(me);

            ActiveSpeaker = Participants[0];

            _videoTimer = new System.Threading.Timer(SimulateIncomingVideoFrames, null, 0, 33);
        }

        [RelayCommand]
        public void ToggleMute()
        {
            IsMuted = !IsMuted;
        }

        [RelayCommand]
        public void ToggleVideo()
        {
            IsVideoOn = !IsVideoOn;

            // Opdater også "Dig" i listen, så det lille billede følger med
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