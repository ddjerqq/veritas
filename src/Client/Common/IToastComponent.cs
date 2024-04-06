using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;

namespace Client.Common;

public interface IToastComponent
{
    [Inject]
    public IToastService Toast { get; set; }
}