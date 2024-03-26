using Domain.Common;
using Domain.ValueObjects;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Presentation.Auth;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Presentation.Filters;

public class PublicKeyAndSignatureOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();

        var alice = Voter.NewVoter();
        var bob = Voter.NewVoter();
        var exampleVoters = new Dictionary<string, (string Addr, string Sig)>
        {
            ["alice"] = (alice.PublicKey.ToHexString(), alice.Sign(alice.Address.ToBytesFromHex()).ToHexString()),
            ["bob"] = (bob.PublicKey.ToHexString(), bob.Sign(bob.Address.ToBytesFromHex()).ToHexString()),
        };

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = PublicKeyBearerAuthHandler.PubKeyHeaderName,
            In = ParameterLocation.Header,
            Description = "Public key encoded in hex",
            Required = true,
            Examples = exampleVoters
                .ToDictionary(
                    x => x.Key,
                    x => new OpenApiExample
                    {
                        Value = new OpenApiString(x.Value.Addr),
                    }),
            Schema = new OpenApiSchema
            {
                Title = "Public key",
                Type = "string",
            },
        });

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = PublicKeyBearerAuthHandler.SignatureHeaderName,
            In = ParameterLocation.Header,
            Description = "The signed address encoded in hex",
            Required = true,
            Examples = exampleVoters
                .ToDictionary(
                    x => x.Key,
                    x => new OpenApiExample
                    {
                        Value = new OpenApiString(x.Value.Sig),
                    }),
            Schema = new OpenApiSchema
            {
                Title = "Signed address",
                Type = "string",
            },
        });
    }
}