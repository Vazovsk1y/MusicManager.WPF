using MusicManager.Domain.Enums;

namespace MusicManager.Domain.ValueObjects;

public record SongFileInfo(string Name, string FullPath, SongFileType FileType);
