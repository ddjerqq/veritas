using System.ComponentModel;
using System.Reflection;
using Domain.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace Client.Common;

public abstract class QueryParameterBinderComponentBase : ComponentBase
{
    private IDictionary<string, PropertyInfo> QueryProperties { get; set; } = new Dictionary<string, PropertyInfo>();

    [Inject]
    protected NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    protected ILogger<QueryParameterBinderComponentBase> Logger { get; set; } = null!;

    public override Task SetParametersAsync(ParameterView parameters)
    {
        NavigationManager.LocationChanged += (_, _) => ReadProperties();

        QueryProperties = GetType()
            .GetProperties()
            .Select(prop => new
            {
                prop,
                propName = prop.Name.ToSnakeCase(),
                attr = prop.GetCustomAttribute<QueryParameterBoundAttribute>(),
                attrName = prop.GetCustomAttribute<QueryParameterBoundAttribute>()?.Name?.ToSnakeCase(),
            })
            .Where(x => x.attr is not null)
            .Select(x => new
            {
                x.prop,
                name = string.IsNullOrWhiteSpace(x.attrName) ? x.propName : x.attrName,
            })
            .ToDictionary(x => x.name, x => x.prop);

        ReadProperties();

        return base.SetParametersAsync(parameters);
    }

    protected void ReadProperties()
    {
        var queryString = NavigationManager.ToAbsoluteUri(NavigationManager.Uri).Query;
        var queryParameters = QueryHelpers.ParseQuery(queryString);

        foreach (var prop in QueryProperties)
            if (queryParameters.TryGetValue(prop.Key, out var value))
            {
                var propType = prop.Value.PropertyType;
                try
                {
                    var convertedValue = TypeDescriptor.GetConverter(propType).ConvertFromString(value.ToString());
                    prop.Value.SetValue(this, convertedValue);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "failed converting value from query parameter: {}", prop);
                }
            }

        StateHasChanged();
    }

    protected void SyncPropertiesWithQuery()
    {
        var queryString = new Dictionary<string, string>();

        foreach (var queryProperty in QueryProperties)
        {
            var propValue = queryProperty.Value.GetValue(this)?.ToString();
            if (!string.IsNullOrEmpty(propValue))
                queryString[queryProperty.Key.ToSnakeCase()] = propValue;
        }

        var uri = new Uri(NavigationManager.Uri);
        var newUri = QueryHelpers.AddQueryString(uri.GetLeftPart(UriPartial.Path), queryString!);

        NavigationManager.NavigateTo(newUri, false);
    }
}