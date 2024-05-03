namespace Domain.ValueObjects;

public readonly record struct Party(int Id, string Name, string ShortName, string LeaderName, string Color, string Link)
{
    public static readonly int[] Allowed = [0, 1, 2, 5, 6, 7, 8, 9, 36, 41, 45];

    public static readonly IEnumerable<Party> All = new List<Party>
    {
        new(1, "გიორგი ვაშაძე - სტრატეგია აღმაშენებელი", "strategy-agmashenebeli", "გიორგი ვაშაძე", "#ff0000", "https://ka.wikipedia.org/wiki/სტრატეგია_აღმაშენებელი"),
        new(2, "ევროპული საქართველო - მოძრაობა თავისუფლებისთვის", "euro-georgia", "გიგა ბოკერია", "#013150", "https://ka.wikipedia.org/wiki/ევროპული_საქართველო"),
        new(5, "ერთიანი ნაციონალური მოძრაობა", "unm", "მიხეილ სააკაშვილი", "#ce2121", "https://ka.wikipedia.org/wiki/ერთიანი_ნაციონალური_მოძრაობა"),
        new(6, "ევროპელი დემოკრატები", "euro-democrats", "პაატა დავითაია", "#2a0e72", "https://ka.wikipedia.org/wiki/ევროპელი_დემოკრატები"),
        new(7, "ალეკო ელისაშვილი - მოქალაქეები", "citizens", "ალეკო ელისაშვილი", "#8bc43f", "https://ka.wikipedia.org/wiki/მოქალაქეები"),
        new(8, "კანონი და სამართალი", "law-and-order", "თაკო ჩარკვიანი", "#010173", "https://ka.wikipedia.org/wiki/კანონი_და_სამართალი"),
        new(9, "ლელო საქართველოსთვის", "lelo", "მამუკა ხარაძე", "#d4a700", "https://ka.wikipedia.org/wiki/ლელო_საქართველოსთვის"),
        new(36, "გირჩი - ახალი პოლიტიკური ცენტრი", "girchi-iago", "იაგო ხვიჩია", "#317e38", "https://ka.wikipedia.org/wiki/ახალი_პოლიტიკური_ცენტრი_-_გირჩი#ორგანიზაციული_სტრუქტურა"),
        new(41, "ქართული ოცნება - დემოკრატიული საქართველო", "gd", "ბიძინა ივანიშვილი", "#0b6abe", "https://ka.wikipedia.org/wiki/ქართული_ოცნება_—_დემოკრატიული_საქართველო"),
        new(45, "გირჩი - მეტი თავისუფლება", "girchi-zurab", "ზურაბ გირჩი ჯაფარიძე", "#317e38", "https://ka.wikipedia.org/wiki/გირჩი_—_მეტი_თავისუფლება"),
    };

    public int Id { get; } = Allowed.Contains(Id)
        ? Id
        : throw new ArgumentException($"The party with the id: {Id} is not yet registered", nameof(Id));

    public static implicit operator int(Party party) => party.Id;

    public static implicit operator Party(int id) => Allowed.Contains(id) ? All.First(p => p.Id == id)
        : throw new ArgumentException($"The party with the id: {id} is not yet registered", nameof(id));

    public override int GetHashCode() => Id.GetHashCode();
}