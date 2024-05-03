namespace Domain.ValueObjects;

public readonly record struct Party(int Id, string Name, string ShortName, string LeaderName, string Color)
{
    public static readonly int[] Allowed = [0, 5, 9, 42, 45];

    public static readonly IEnumerable<Party> All = new List<Party>
    {
        new(5, "ნაციონალური მოძრაობა", "unm", "მიხეილ სააკაშვილი", "#ce2121"),
        new(9, "ლელო საქართველოსთვის", "lelo", "მამუკა ხარაძე", "#d4a700"),
        new(42, "ქართული ოცნება", "gd", "ბიძინა ივანიშვილი", "#0b6abe"),
        new(45, "გირჩი - მეტი თავისუფლება", "girchi", "ზურაბ გირჩი ჯაფარიძე", "#317e38"),
    };

    public int Id { get; } = Allowed.Contains(Id)
        ? Id
        : throw new ArgumentException($"The party with the id: {Id} is not yet registered", nameof(Id));

    public static implicit operator int(Party party) => party.Id;

    public static implicit operator Party(int id) => Allowed.Contains(id) ? All.First(p => p.Id == id)
        : throw new ArgumentException($"The party with the id: {id} is not yet registered", nameof(id));

    public override int GetHashCode() => Id.GetHashCode();
}