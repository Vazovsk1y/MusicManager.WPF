using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using MusicManager.Domain.Shared;
using MusicManager.Services;
using MusicManager.Services.Contracts.Dtos;
using MusicManager.Utils;
using MusicManager.WPF.Messages;
using MusicManager.WPF.Infrastructure;
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

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DeleteSongCommand))]
	private SongViewModel? _selectedSong;


    [RelayCommand]
    private async Task Save()
    {
		var songsToUpdate = Songs.Where(e => e.IsModified).ToList();
		if (SaveCommand.IsRunning || songsToUpdate.Count == 0)
        {
            return;
        }

        using var scope = _serviceScopeFactory.CreateScope();
        var songService = scope.ServiceProvider.GetRequiredService<ISongService>();

        var results = new List<Result>();
        foreach (var song in songsToUpdate)
        {
            var dto = new SongUpdateDTO(
                song.SongId,
                song.Title,
                song.Number);

            var updateResult = await songService.UpdateAsync(dto);
            if (updateResult.IsFailure)
            {
                song.RollBackChanges();
            }
            else
            {
                song.SaveState();
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

	[RelayCommand(CanExecute = nameof(CanDeleteSong))]
	private async Task DeleteSong()
	{
		var dialog = MessageBoxHelper.ShowDialogBoxYesNo($"Delete {SelectedSong!.Title} from list?");
		if (dialog == MessageBoxResult.Yes)
		{
			using var scope = _serviceScopeFactory.CreateScope();
			var service = scope.ServiceProvider.GetRequiredService<ISongService>();
			var result = await service.DeleteAsync(SelectedSong!.SongId);
			if (result.IsFailure)
			{
				MessageBoxHelper.ShowErrorBox(result.Error.Message);
				return;
			}

			await App.Current.Dispatcher.InvokeAsync(() =>
			{
                var discs = DiscsPanelViewModel.Discs.Where(e => e.Songs.Contains(SelectedSong));
                foreach (var disc in discs)
                {
                    disc.Songs.Remove(SelectedSong);
                }
			});
		}
	}

	private bool CanDeleteSong() => SelectedSong is not null;

	public async void Receive(SongCreatedMessage message)
    {
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var disc = DiscsPanelViewModel.Discs.FirstOrDefault(e => e.DiscId == message.DiscId);
            foreach (var song in message.SongsViewsModels)
            {
                song.SaveState();
            }
            disc!.Songs.AddRange(message.SongsViewsModels);
            disc.Songs = new(disc.Songs.OrderBy(e => e.Title));
        });
    }
}
