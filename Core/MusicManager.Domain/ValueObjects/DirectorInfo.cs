namespace MusicManager.Domain.ValueObjects;

public class DirectorInfo
{
    public string Surname { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public DirectorInfo(string name, string surname)
    {
        Name = name;
        Surname = surname;
    }

    public static readonly DirectorInfo Undefined = new("Undefined", "Undefined");
}

