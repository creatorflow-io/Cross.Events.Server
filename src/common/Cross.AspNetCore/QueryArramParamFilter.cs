using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Cross.AspNetCore
{
	public class QueryArramParamFilter : IParameterFilter
	{
		public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
		{
			if (parameter.In.HasValue && parameter.In.Value == ParameterLocation.Query)
			{
				if (parameter.Schema?.Type == "array")
				{
					parameter.Schema.Extensions.Add("collectionFormat", new OpenApiString("multi"));
				}
			}
		}
	}
}
