using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace TruckLoadingApp.API.Configuration;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public class RateLimitingAttribute : Attribute
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
{
    public string? PolicyName { get; }

    public RateLimitingAttribute(string policyName = "fixed")
    {
        PolicyName = policyName;
    }
}
