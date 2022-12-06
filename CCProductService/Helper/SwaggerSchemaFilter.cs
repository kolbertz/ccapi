using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Runtime.Serialization;

namespace CCProductService.Helper
{
    public class SwaggerSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema?.Properties != null)
            {
                IEnumerable<PropertyInfo> ignoreDataMemberProperties = context.Type.GetProperties()
                .Where(t => t.GetCustomAttribute<IgnoreDataMemberAttribute>() != null);

                foreach (var ignoreDataMemberProperty in ignoreDataMemberProperties)
                {
                    var propertyToHide = schema.Properties.Keys
                        .SingleOrDefault(x => x.ToLower() == ignoreDataMemberProperty.Name.ToLower());

                    if (propertyToHide != null)
                    {
                        schema.Properties.Remove(propertyToHide);
                    }
                }
            }
        }
    }
}
