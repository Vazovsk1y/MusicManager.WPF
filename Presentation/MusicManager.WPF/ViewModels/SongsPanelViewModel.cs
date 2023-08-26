using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using MusicManager.Domain.Shared;
using MusicManager.Services;
using MusicManager.Services.Contracts.Dtos;
using MusicManager.Utils;
using MusicManager.WPF.Messages;
using MusicManager.WPF.Tools;
using MusicManager.WPF.ViewModels.Entities;
using MusicManager.WPF.Views.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
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
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public SongsPanelViewModel() 
    {
        InvalidOperationExceptionHelper.ThrowIfTrue(!App.IsInDesignMode, "Parametrless ctor is only for design time.");
    }

    public SongsPanelViewModel(
        DiscsPanelViewModel discsPanelViewModel,
        IUserDialogService<SongAddWindow> dialogService,
        IServiceScopeFactory serviceScopeFactory)
    {
        DiscsPanelViewModel = discsPanelViewModel;
        _dialogService = dialogService;
        _serviceScopeFactory = serviceScopeFactory;
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

    [RelayCommand]
    private async Task Save()
    {
        if (SaveCommand.IsRunning)
        {
            return;
        }

        using var scope = _serviceScopeFactory.CreateScope();
        var songService = scope.ServiceProvider.GetRequiredService<ISongService>();
        var songsToUpdate = Songs.Where(e => e.IsModified);

        var results = new List<Result>();
        foreach (var item in songsToUpdate)
        {
            var dto = new SongUpdateDTO(
                item.SongId,
                item.Title,
                item.Number);

            var updateResult = await songService.UpdateAsync(dto);
            if (updateResult.IsFailure)
            {
                item.RollBackChanges();
            }
            else
            {
                item.SetCurrentAsPrevious();
            }

            results.Add(updateResult);
        }

        if (results.Any(e => e.IsFailure))
        {
            MessageBoxHelper.ShowErrorBox(string.Join(",", results.Where(e => e.IsFailure).Select(e => e.Error.Message)));
        }
        else
        {
            MessageBoxHelper.ShowInfoBox("Successfully updated.");
        }
    }

    public async void Receive(SongCreatedMessage message)
    {
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var disc = DiscsPanelViewModel.Discs.FirstOrDefault(e => e.DiscId == message.DiscId);
            foreach (var song in message.SongsViewsModels)
            {
                song.SetCurrentAsPrevious();
            }
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
    
    


