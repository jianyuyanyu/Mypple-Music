﻿using Mypple_Music.Events;
using Mypple_Music.Models;
using Mypple_Music.Service;
using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Mypple_Music.ViewModels
{
    public class PlayListViewModel : NavigationViewModel
    {
        private bool isUpdating;
        private IMusicService musicService;

        private bool playOrPause;

        public bool PlayOrPause
        {
            get { return playOrPause; }
            set
            {
                playOrPause = value;
                RaisePropertyChanged();
            }
        }
        private bool isSearchVisible;

        public bool IsSearchVisible
        {
            get { return isSearchVisible; }
            set
            {
                isSearchVisible = value;
                RaisePropertyChanged();
            }
        }

        private int count;

        public int Count
        {
            get { return count; }
            set
            {
                count = value;
                RaisePropertyChanged();
            }
        }

        private double duration;

        public double Duration
        {
            get { return duration; }
            set
            {
                duration = value;
                RaisePropertyChanged();
            }
        }

        private Album album;

        public Album Album
        {
            get { return album; }
            set
            {
                album = value;
                RaisePropertyChanged();
            }
        }

        private Music selectedMusic;

        public Music SelectedMusic
        {
            get { return selectedMusic; }
            set
            {
                selectedMusic = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<Music> musicList;

        public ObservableCollection<Music> MusicList
        {
            get { return musicList; }
            set
            {
                musicList = value;
                RaisePropertyChanged();
            }
        }

        public DelegateCommand ChangeVisibilityCommand { set; get; }
        public DelegateCommand<Music> SelectedMusicChangedCommand { set; get; }
        public DelegateCommand<Music> PauseOrPlayCommand { set; get; }

        public PlayListViewModel(IContainerProvider containerProvider, IMusicService musicService)
            : base(containerProvider)
        {
            this.musicService = musicService;
            SelectedMusicChangedCommand = new DelegateCommand<Music>(SelectedMusicChanged);
            PauseOrPlayCommand = new DelegateCommand<Music>(PauseOrPlay);
            ChangeVisibilityCommand = new DelegateCommand(() =>
            {
                if (IsSearchVisible == true)
                    IsSearchVisible = false;
                else
                    IsSearchVisible = true;
            });
            eventAggregator.GetEvent<StopPlayEvent>().Subscribe(arg =>
            {
                isUpdating = true;
                PlayOrPause = !arg.isStop;
                isUpdating = false;
            },
            m =>
            {
                return m.filter == "PlayListView";
            });
        }

        private void PauseOrPlay(Music music)
        {
            if (isUpdating)
                return;
            switch (music.Status)
            {
                case Music.PlayStatus.StartPlay:
                    isUpdating = true;
                    music.Status = Music.PlayStatus.PausePlay;
                    eventAggregator.GetEvent<StopPlayEvent>().Publish(new StopModel(true, "MainView"));
                    isUpdating = false;
                    break;
                case Music.PlayStatus.PausePlay:
                    isUpdating = true;
                    music.Status = Music.PlayStatus.StartPlay;
                    eventAggregator.GetEvent<StopPlayEvent>().Publish(new StopModel(false, "MainView"));
                    isUpdating = false;
                    break;
                case Music.PlayStatus.StopPlay:
                    isUpdating = true;
                    SelectedMusicChanged(music);
                    isUpdating = false;
                    break;
                default:
                    break;
            }
        }

        private void SelectedMusicChanged(Music Music)
        {
            //设置播放状态
            var playingMusic = MusicList.FirstOrDefault(
                m => m.Status == Music.PlayStatus.StartPlay
            );
            if (playingMusic != null)
                playingMusic.Status = Music.PlayStatus.StopPlay;
            SelectedMusic = Music;
            SelectedMusic.Status = Music.PlayStatus.StartPlay;

            //把当前播放列表发送给播放器待播放
            eventAggregator
                .GetEvent<PlayListCreatedEvent>()
                .Publish(
                    new PlayListCreatedModel(
                        MusicList,
                        MusicList.IndexOf(SelectedMusic),
                        "MainView"
                    )
                );
            //发送歌曲给歌词界面
            eventAggregator
                .GetEvent<MusicPlayedEvent>()
                .Publish(new MusicPlayedModel(Music, "LyricView"));
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters.ContainsKey("Album"))
            {
                Album = navigationContext.Parameters.GetValue<Album>("Album");
                MusicList = new ObservableCollection<Music>(
                    await musicService.GetMusicsByAlbumIdAsync(Album.Id)
                );
                Count = MusicList.Count;
                Duration = MusicList.Sum(m => m.Duration);
            }
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            //设置播放状态
            var playingMusic = MusicList.FirstOrDefault(
                m => m.Status == Music.PlayStatus.StartPlay
            );
            if (playingMusic != null)
                playingMusic.Status = Music.PlayStatus.StopPlay;
        }
    }
}
