using Com.ChinaSoft.Devinfo.ViewModels;
using Prism.Commands;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Com.ChinaSoft.Devinfo.UserControls
{
    /// <summary>
    /// MediaPlayer.xaml 的交互逻辑
    /// </summary>
    public partial class MediaPlayer : UserControl
    {
        public MediaPlayer()
        {
            InitializeComponent();
            this.DataContext = new MediaPlayerViewModel(this.mplayer); ;
        }

        public string Source
        {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(string), typeof(MediaPlayer), new PropertyMetadata(null, (d, e) =>
            {
                var vm = (d as MediaPlayer)?.DataContext as MediaPlayerViewModel;
                if (vm == null)
                    return;
                vm.VideoSource = e.NewValue?.ToString();
                if (e.NewValue == null)
                    vm.StopCommand.Execute();
            }));
    }

    class MediaPlayerViewModel : BaseViewModel
    {
        TimeSpan minDuration = TimeSpan.Zero.Add(TimeSpan.FromTicks(1));
        public MediaPlayerViewModel(MediaElement player)
        {
            this.player = player;
            this.Volume = 0.5;
            this.Duration = minDuration;//防止0/0时进度条显示在末端

            timer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += UpdateProgress;

            refreshTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(10) };
            refreshTimer.Tick += async (s, e) => { await this.PlayCommand.Execute(); refreshTimer.Stop(); };
            Refresh();
        }

        /// <summary>
        /// 播放500ms视频，刷新MediaElement(MediaElement设置Position不播放并不会更新界面)
        /// </summary>
        async void Refresh()
        {
            await this.PlayCommand.Execute();
            refreshTimer.Start();
        }

        /// <summary>
        /// 更新进度条
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void UpdateProgress(object sender, EventArgs e)
        {
            this.Position = player.Position;

            if (Duration <= minDuration && player.NaturalDuration.HasTimeSpan)
                Duration = player.NaturalDuration.TimeSpan;
        }

        public MediaPlayerViewModel Current { get { return this; } }

        DispatcherTimer timer, refreshTimer;

        MediaElement player;

        private string source;
        /// <summary>
        /// 视频源
        /// </summary>
        public string VideoSource
        {
            get { return source; }
            set { SetProperty<string>(ref source, value); }
        }

        private TimeSpan duration;
        /// <summary>
        /// 持续时间
        /// </summary>
        public TimeSpan Duration
        {
            get { return duration; }//<= TimeSpan.Zero ? TimeSpan.Zero.Add(TimeSpan.FromTicks(1)) : duration; }
            set { SetProperty<TimeSpan>(ref duration, value); }
        }

        private TimeSpan position;
        /// <summary>
        /// 当前进度
        /// </summary>
        public TimeSpan Position
        {
            get { return position; }
            set
            {
                SetProperty<TimeSpan>(ref position, value > Duration ? Duration : value);
                OnPropertyChanged("Progress");
                OnPropertyChanged("Current");
            }
        }

        /// <summary>
        /// 播放进度
        /// </summary>
        public double Progress
        {
            get { return Position.TotalSeconds; }
            set
            {
                this.Position = TimeSpan.FromSeconds(value);
                player.Position = this.Position;
                OnPropertyChanged("Progress");
                if (!IsPlaying)
                    Refresh();
            }
        }

        private double volume;
        /// <summary>
        /// 音量
        /// </summary>
        public double Volume
        {
            get { return volume; }
            set { SetProperty<double>(ref volume, value); }
        }

        private bool isPlaying;
        /// <summary>
        /// 是否正在播放
        /// </summary>
        public bool IsPlaying
        {
            get { return isPlaying; }
            set { SetProperty<bool>(ref isPlaying, value); }
        }

        private bool isMute;
        /// <summary>
        /// 是否静音
        /// </summary>
        public bool IsMute
        {
            get { return isMute; }
            set { SetProperty<bool>(ref isMute, value); }
        }

        private bool canStop;
        /// <summary>
        /// 是否可以停止
        /// </summary>
        public bool CanStop
        {
            get { return canStop; }
            set { SetProperty<bool>(ref canStop, value); }
        }


        /// <summary>
        /// 播放暂停
        /// </summary>
        public DelegateCommand PlayCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    if (IsPlaying)
                    {
                        player.Pause();
                        timer.Stop();
                    }
                    else
                    {
                        player.Play();
                        timer.Start();
                    }

                    IsPlaying = !IsPlaying;
                    this.CanStop = true;
                });
            }
        }

        /// <summary>
        /// 停止播放
        /// </summary>
        public DelegateCommand StopCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    player.Stop();
                    timer.Stop();
                    this.CanStop = false;
                    this.IsPlaying = false;
                    this.Position = TimeSpan.Zero;
                    player.Position = TimeSpan.Zero;
                    this.Duration = minDuration;

                    Refresh();
                });
            }
        }

        /// <summary>
        /// 快退
        /// </summary>
        public DelegateCommand BackwardCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    var newValue = this.Position.Add(TimeSpan.FromSeconds(-5));
                    this.Position = newValue < TimeSpan.Zero ? TimeSpan.Zero : newValue;
                    player.Position = this.Position;

                    if (!IsPlaying)
                        Refresh();
                });
            }
        }

        /// <summary>
        /// 快进
        /// </summary>
        public DelegateCommand ForwardCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    var newValue = this.Position.Add(TimeSpan.FromSeconds(5));
                    this.Position = newValue > this.Duration ? this.Duration : newValue;
                    player.Position = this.Position;

                    if (!IsPlaying)
                        Refresh();
                });
            }
        }

        /// <summary>
        /// 静音
        /// </summary>
        public DelegateCommand MuteCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    player.IsMuted = !player.IsMuted;
                    this.IsMute = player.IsMuted;
                });
            }
        }
    }
}
