using Newtonsoft.Json;
using System.Reflection;

namespace CCProductService.Helper
{
    public class Wrapper<TModel>
    {
        public TModel? Model { get; set; }
        public Wrapper(TModel? model) { 
            Model= model;
        }

        public static async ValueTask<Wrapper<TModel>?> BindAsync(HttpContext context, ParameterInfo parameter)
        {
            if (!context.Request.HasJsonContentType())
            {
                throw new BadHttpRequestException(
                    "Request content type was not a recognized JSON content type.",
                    StatusCodes.Status415UnsupportedMediaType);
            }

            using var sr = new StreamReader(context.Request.Body);
            var str = await sr.ReadToEndAsync();

            return new Wrapper<TModel>(JsonConvert.DeserializeObject<TModel>(str));
        }
    }
}
