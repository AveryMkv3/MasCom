using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.IconPacks;
using System.Windows.Input;

namespace MasCom.Clientv2.ViewModels
{
    public class VisioWindowVM : ViewModelBase
    {
        private PackIconFontAwesome _videoIcon = new PackIconFontAwesome() { Height = 35, Width = 35, Kind = PackIconFontAwesomeKind.VideoSolid };
        private PackIconFontAwesome _audioIcon = new PackIconFontAwesome() { Height = 35, Width = 35, Kind = PackIconFontAwesomeKind.MicrophoneSolid };
        public PackIconFontAwesome AudioIcon
        {
            get { return _audioIcon; }
            set { _audioIcon = value; RaisePropertyChanged(nameof(AudioIcon)); }
        }
        public PackIconFontAwesome VideoIcon
        {
            get { return _videoIcon; }
            set { _videoIcon = value; RaisePropertyChanged(nameof(VideoIcon)); }
        }

        public VisioWindowVM()
        {
        }
        public ICommand EndVideoCallCommand => new RelayCommand(EndVideoCall);
        public ICommand ToggleAudioCommand => new RelayCommand(ToggleAudio);
        public ICommand ToggleVideoCommand => new RelayCommand(ToggleVideo);

        private void EndVideoCall()
        {
            Messenger.Default.Send(new NotificationMessage(Models.AppNotifications.EndVisio.ToString()));
        }
        private void ToggleAudio()
        {
            if (AudioIcon.Kind == PackIconFontAwesomeKind.MicrophoneSolid)
                AudioIcon = new PackIconFontAwesome() { Height = 35, Width = 35, Kind = PackIconFontAwesomeKind.MicrophoneSlashSolid };
            if (AudioIcon.Kind == PackIconFontAwesomeKind.MicrophoneSlashSolid)
                AudioIcon = new PackIconFontAwesome() { Height = 35, Width = 35, Kind = PackIconFontAwesomeKind.MicrophoneSolid };
        }
        private void ToggleVideo()
        {
            if (VideoIcon.Kind == PackIconFontAwesomeKind.VideoSolid)
                VideoIcon.Kind = PackIconFontAwesomeKind.VideoSlashSolid;
            if (VideoIcon.Kind == PackIconFontAwesomeKind.VideoSlashSolid)
                VideoIcon.Kind = PackIconFontAwesomeKind.VideoSolid;
        }
    }
}
