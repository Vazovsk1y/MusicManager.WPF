namespace MusicManager.Domain.ValueObjects;

public class DirectorInfo
{
    public string Surname { get; init; }

    public string Name { get; init; } 

    public DirectorInfo(string name, string surname)
    {
        Name = name;
        Surname = surname;
    }
}

