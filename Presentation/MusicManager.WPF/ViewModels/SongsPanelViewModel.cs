using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MusicManager.Services;
using MusicManager.Utils;
using MusicManager.WPF.Messages;
using MusicManager.WPF.ViewModels.Entities;
using MusicManager.WPF.Views.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace MusicManager.WPF.ViewModels;

internal partial class SongsPanelViewModel : 
    ObservableRecipient,
    IRecipient<SongCreatedMessage>
{
    public DiscsPanelViewModel DiscsPanelViewModel { get; }

    private readonly IUserDialogService<SongAddWindow> _dialogService;    

    public IReadOnlyCollection<SongViewModel> Songs => new ObservableCollection<SongViewModel>(DiscsPanelViewModel.Compilations
        .SelectMany(e => e.Songs)
        .Union(DiscsPanelViewModel.MovieReleases.SelectMany(e => e.Songs)));

    private SongViewModel? _selectedSong;

    public SongsPanelViewModel() 
    {
        InvalidOperationExceptionHelper.ThrowIfTrue(!App.IsInDesignMode, "Parametrless ctor is only for design time.");
    }

    public SongsPanelViewModel(
        DiscsPanelViewModel discsPanelViewModel, 
        IUserDialogService<SongAddWindow> dialogService)
    {
        DiscsPanelViewModel = discsPanelViewModel;
        _dialogService = dialogService;
    }

    public SongViewModel? SelectedSong
    {
        get => _selectedSong;
        set => SetProperty(ref _selectedSong, value);
    }

    [RelayCommand]
    private void AddSong()
    {
        _dialogService.ShowDialog();
    }

    public async void Receive(SongCreatedMessage message)
    {
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var disc = DiscsPanelViewModel.Discs.FirstOrDefault(e => e.DiscId == message.DiscId);
            disc?.Songs.AddRange(message.SongsViewsModels);
        });
    }
}


public static class CollectionExtensions
{
    public static void AddRange<T>(this IList<T> values, IEnumerable<T> collectionToAdd)
    {
        ArgumentNullException.ThrowIfNull(collectionToAdd);

        foreach (var item in collectionToAdd)
        {
            values.Add(item);
        }
    }
}
    
    


