namespace Domain.ValueObjects;

public readonly record struct Party(int Id)
{
    public static readonly int[] Allowed = [5, 9, 36, 42];

    public int Id { get; } = Allowed.Contains(Id)
        ? Id
        : throw new ArgumentException("The party with this id is not registered", nameof(Id));

    public string Name => Id switch
    {
        5 => "ნაციონალური მოძრაობა",
        9 => "ლელო საქართველოსთვის",
        36 => "გირჩი",
        42 => "ქართული ოცნება",
        _ => throw new ArgumentOutOfRangeException(nameof(Id)),
    };

    public string ShortName => Id switch
    {
        5 => "unm",
        9 => "lelo",
        36 => "girchi",
        42 => "gd",
        _ => throw new ArgumentOutOfRangeException(nameof(Id)),
    };

    public string LeaderName => Id switch
    {
        5 => "მიხეილ სააკაშვილი",
        9 => "მამუკა ხარაძე",
        36 => "ზურაბ გირჩი ჯაფარიძე",
        42 => "ბიძინა ივანიშვილი",
        _ => throw new ArgumentOutOfRangeException(nameof(Id)),
    };

    public static implicit operator int(Party party) => party.Id;
    public static implicit operator Party(int id) => new(id);

    public override int GetHashCode() => Id.GetHashCode();
}