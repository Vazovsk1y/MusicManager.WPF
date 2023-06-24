namespace MusicManager.Domain.ValueObjects;

public record DirectorInfo(string Name, string Surname)
{
    public static readonly DirectorInfo Undefined = new("Undefined", "Undefined");
}

