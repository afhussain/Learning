using Microsoft.Extensions.Options;

// Our class based middleware component checking for querystring "customer=true"
// The key thing to note here is the "next" delegate is called after this component does its work
// BUT only if the pipeline is not to be terminated!

namespace Platform {

    public class QueryStringMiddleWare {
        private RequestDelegate? next;

        public QueryStringMiddleWare() {
            // do nothing
        }

        public QueryStringMiddleWare(RequestDelegate nextDelegate) {
            next = nextDelegate;
        }

        public async Task Invoke(HttpContext context) {
            if (context.Request.Method == HttpMethods.Get && context.Request.Query["custom"] == "true") {
                if (!context.Response.HasStarted) context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync("Class-based Middleware (invoked when querystring custom=true) \n");
            }
            
            if (next != null) {
                await next(context);
            }
        }
    }

    public class ConfigMiddleware
    {
        private RequestDelegate next;
        private MessageOptions options;

        public ConfigMiddleware(RequestDelegate nextDelegate, IOptions<MessageOptions> opts)
        {
            next = nextDelegate;
            options = opts.Value;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path == "/config3") {
                if (!context.Response.HasStarted) context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync($"Class-based Middleware (invoked for path /config3) - config values are {options.CityName}, {options.CountryName} \n");
            }
            await next(context);
        }
    }
}
