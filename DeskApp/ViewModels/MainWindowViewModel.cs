using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using DeskApp.ViewModels;

namespace DeskApp.ViewModels
{
    // Hoved ViewModel for mødet
    public class MainWindowViewModel : ViewModelBase
    {
        private ParticipantViewModel _activeSpeaker;
        private bool _isMuted;
        private bool _isVideoOn = true;
        private Timer _videoTimer;

        public ObservableCollection<ParticipantViewModel> Participants { get; } = new();

        public ParticipantViewModel ActiveSpeaker
        {
            get => _activeSpeaker;
            set { _activeSpeaker = value; OnPropertyChanged(); }
        }

        public string IsMutedIcon => _isMuted ? "🔴" : "🎙️";
        public string IsVideoOnIcon => _isVideoOn ? "📹" : "🚫";

        public MainWindowViewModel()
        {
            // Opret dummy deltagere
            Participants.Add(new ParticipantViewModel("Lars Larsen", Colors.Blue));
            Participants.Add(new ParticipantViewModel("Mette Frederiksen", Colors.Red));
            Participants.Add(new ParticipantViewModel("Ole Opfinder", Colors.Green));
            Participants.Add(new ParticipantViewModel("Dig (Mig)", Colors.Purple) { IsVideoOn = true });

            // Sæt den første som "Active Speaker" (den store skærm)
            ActiveSpeaker = Participants[0];

            // Start en "Fake Video Engine" for at simulere kamera-input
            // I en rigtig app ville dette være WebRTC frames der kommer ind
            _videoTimer = new Timer(SimulateIncomingVideoFrames, null, 0, 33); // Ca. 30 FPS
        }

        // --- Kommandoer til UI knapper ---
        public void ToggleMuteCommand()
        {
            _isMuted = !_isMuted;
            OnPropertyChanged(nameof(IsMutedIcon));
        }

        public void ToggleVideoCommand()
        {
            _isVideoOn = !_isVideoOn;
            OnPropertyChanged(nameof(IsVideoOnIcon));

            // Opdater også "Min" deltager i listen
            var me = Participants[3];
            me.IsVideoOn = _isVideoOn;
        }

        public void LeaveCallCommand()
        {
            Environment.Exit(0);
        }

        // --- SIMULERING AF VIDEO STREAM ---
        // Dette er den vigtige del teknisk set. Vi skriver rå bytes til grafikkortet.
        private void SimulateIncomingVideoFrames(object? state)
        {
            // Vi opdaterer kun UI på UI-tråden
            Dispatcher.UIThread.Post(() =>
            {
                // Opdater Active Speaker (Stor skærm)
                ActiveSpeaker?.UpdateVideoFrame();

                // Opdater alle små billeder i bunden
                foreach (var p in Participants)
                {
                    if (p.IsVideoOn) p.UpdateVideoFrame();
                }
            });
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}