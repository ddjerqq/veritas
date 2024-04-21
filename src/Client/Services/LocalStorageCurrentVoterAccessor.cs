using Application.Common.Abstractions;
using Blazored.LocalStorage;
using Domain.Entities;

namespace Client.Services;

public class LocalStorageCurrentVoterAccessor(ILocalStorageService localStorage) : ICurrentVoterAccessor
{
    private Voter Voter { get; set; } = default!;

    public Voter TryGetCurrentVoter()
    {
        // localStorage.GetItemAsync<>()
        // try load from local storage, else create new
        throw new NotImplementedException();
    }

    private static Voter LoadFromLocalStorage(ILocalStorageService localStorage)
    {
        throw new NotImplementedException();
    }
}