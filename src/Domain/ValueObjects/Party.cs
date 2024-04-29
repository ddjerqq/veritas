namespace Domain.ValueObjects;

public readonly record struct Party(int Id)
{
    public static readonly int[] Allowed = [0, 5, 9, 42, 45];

    public int Id { get; } = Allowed.Contains(Id)
        ? Id
        : throw new ArgumentException($"The party with the id: {Id} is not yet registered", nameof(Id));

    public string Name => Id switch
    {
        0 => "no_party",
        5 => "ნაციონალური მოძრაობა",
        9 => "ლელო საქართველოსთვის",
        42 => "ქართული ოცნება",
        45 => "გირჩი - მეტი თავისუფლება",
        _ => throw new ArgumentOutOfRangeException(nameof(Id), Id, "The party with this id is not registered yet"),
    };

    public string ShortName => Id switch
    {
        0 => "no_party",
        5 => "unm",
        9 => "lelo",
        42 => "gd",
        45 => "girchi",
        _ => throw new ArgumentOutOfRangeException(nameof(Id), Id, "The party with this id is not registered yet"),
    };

    public string LeaderName => Id switch
    {
        0 => "no_party",
        5 => "მიხეილ სააკაშვილი",
        9 => "მამუკა ხარაძე",
        42 => "ბიძინა ივანიშვილი",
        45 => "ზურაბ გირჩი ჯაფარიძე",
        _ => throw new ArgumentOutOfRangeException(nameof(Id), Id, "The party with this id is not registered yet"),
    };

    public static implicit operator int(Party party) => party.Id;

    public static implicit operator Party(int id) => new(id);

    public override int GetHashCode() => Id.GetHashCode();
}