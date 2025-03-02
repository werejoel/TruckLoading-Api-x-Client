using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TruckLoadingApp.API.Configuration
{
    public class SwaggerFileOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var fileParameters = context.MethodInfo.GetParameters()
                .Where(p => p.ParameterType == typeof(IFormFile) || 
                           (p.ParameterType.IsGenericType && p.ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>) && 
                            p.ParameterType.GetGenericArguments()[0] == typeof(IFormFile)))
                .ToList();

            if (fileParameters.Count == 0) return;

            // Add consumes
            operation.RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = new Dictionary<string, OpenApiSchema>(),
                            Required = new HashSet<string>()
                        }
                    }
                }
            };

            // Add form parameters for each method parameter
            foreach (var parameter in context.MethodInfo.GetParameters())
            {
                var schema = context.SchemaGenerator.GenerateSchema(parameter.ParameterType, context.SchemaRepository);

                if (parameter.ParameterType == typeof(IFormFile))
                {
                    operation.RequestBody.Content["multipart/form-data"].Schema.Properties.Add(parameter.Name, new OpenApiSchema
                    {
                        Type = "string",
                        Format = "binary"
                    });

                    if (parameter.HasDefaultValue == false)
                    {
                        operation.RequestBody.Content["multipart/form-data"].Schema.Required.Add(parameter.Name);
                    }
                }
                else if (parameter.ParameterType.IsGenericType && 
                         parameter.ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>) && 
                         parameter.ParameterType.GetGenericArguments()[0] == typeof(IFormFile))
                {
                    operation.RequestBody.Content["multipart/form-data"].Schema.Properties.Add(parameter.Name, new OpenApiSchema
                    {
                        Type = "array",
                        Items = new OpenApiSchema
                        {
                            Type = "string",
                            Format = "binary"
                        }
                    });

                    if (parameter.HasDefaultValue == false)
                    {
                        operation.RequestBody.Content["multipart/form-data"].Schema.Required.Add(parameter.Name);
                    }
                }
                else if (parameter.ParameterType.IsClass && parameter.ParameterType != typeof(string))
                {
                    // For complex types (like your DriverDocument), add their properties as form fields
                    var properties = parameter.ParameterType.GetProperties();
                    foreach (var prop in properties)
                    {
                        var propSchema = context.SchemaGenerator.GenerateSchema(prop.PropertyType, context.SchemaRepository);
                        operation.RequestBody.Content["multipart/form-data"].Schema.Properties.Add(
                            $"{parameter.Name}.{prop.Name}", propSchema);
                    }
                }
                else
                {
                    // For simple types, add them as form fields
                    operation.RequestBody.Content["multipart/form-data"].Schema.Properties.Add(parameter.Name, schema);
                }
            }

            // Remove the original parameters
            foreach (var fileParameter in fileParameters)
            {
                var paramToRemove = operation.Parameters.FirstOrDefault(p => p.Name == fileParameter.Name);
                if (paramToRemove != null)
                {
                    operation.Parameters.Remove(paramToRemove);
                }
            }
        }
    }
} 