using MusicManager.Domain.Entities;
using MusicManager.Domain.Errors;
using MusicManager.Domain.Models;
using MusicManager.Domain.Shared;
using MusicManager.Domain.ValueObjects;

namespace MusicManager.Domain.Common;

public class Disc : IAggregateRoot
{
    #region --Fields--

    protected readonly List<Song> _songs = new();

    protected readonly List<Cover> _covers = new();

    #endregion

    #region --Properties--

    public DiscId Id { get; }

    public DiscType Type { get; protected set; }

    public ProductionInfo ProductionInfo { get; protected set; }

    public string Identifier { get; protected set; }

    public EntityDirectoryInfo? AssociatedFolderInfo { get; protected set; }

    public IReadOnlyCollection<Song> Songs => _songs.ToList();

    public IReadOnlyCollection<Cover> Covers => _covers.ToList();

	#endregion

	#region --Constructors--

#pragma warning disable CS8618 
	protected Disc()
	{
        Id = DiscId.Create();
    }
#pragma warning restore CS8618 


	#endregion

	#region --Methods--

	public virtual Result AddCover(string coverPath)
    {
		var coverCreationResult = Cover.Create(Id, coverPath);
        if (coverCreationResult.IsFailure)
        {
            return Result.Failure(coverCreationResult.Error);
        }

        var cover = coverCreationResult.Value;
		if (IsCoverAlreadyExists())
		{
			return Result.Failure(DomainErrors.Disc.CoverWithPassedPathAlreadyAdded(coverPath));
		}

		_covers.Add(cover);
        return Result.Success();

        bool IsCoverAlreadyExists()
        {
            return _covers.SingleOrDefault(e => e.Path == cover.Path) is not null;
		}
    }

    public virtual Result AddSong(Song song)
    {
        if (song is null)
        {
            return Result.Failure(DomainErrors.NullPassed(nameof(song)));
        }

        if (IsSongAlreadyExists())
        {
            return Result.Failure(DomainErrors.PassedEntityAlreadyAdded(nameof(song)));
        }

        _songs.Add(song);
        return Result.Success();

        bool IsSongAlreadyExists() => _songs.SingleOrDefault(s => s.Id == song.Id || s.PlaybackInfo == song.PlaybackInfo) is not null; 
	}

    public virtual Result SetAssociatedFolder(string path)
    {
        var result = EntityDirectoryInfo.Create(path);

        if (result.IsFailure)
        {
            return result;
        }

		AssociatedFolderInfo = result.Value;
		return Result.Success();
    }

    public virtual Result SetIdentifier(string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            return Result.Failure(DomainErrors.NullOrEmptyStringPassed(nameof(identifier)));
        }

        Identifier = identifier;
        return Result.Success();
    }

    public virtual Result SetProductionInfo(string? productionCountry, int? productionYear)
    {
        if (productionYear is null)
        {
            if (Type != DiscType.Bootleg && Type != DiscType.Unknown)
            {
				return Result.Failure(DomainErrors.Disc.ProductionYearCanNotBeNullForOtherTypesExceptBootlegAndUnknown);
			}
		}

        var result = ProductionInfo.Create(productionCountry, productionYear);
        if (result.IsFailure)
        {
            return result;
        }

		ProductionInfo = result.Value;
		return Result.Success();
	}

    public virtual Result SetDiscType(DiscType discType)
    {
        if (discType == null)
        {
            return Result.Failure(DomainErrors.NullPassed("disc type"));
        }

        Type = discType;
        return Result.Success();
    }

    #endregion
}

public record DiscId(Guid Value)
{
    public static DiscId Create() => new(Guid.NewGuid());
}
