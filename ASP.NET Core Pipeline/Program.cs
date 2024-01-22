using Microsoft.Extensions.Options;
using Platform;
using System;

namespace ASP.NET_Core_Pipeline
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Set up the configuration class to be made available as a service that will be injected in as a dependency by the framework
            builder.Services.Configure<MessageOptions>(options => {
                options.CityName = "Albany";
            });

            var app = builder.Build();

            // Let other middleware components do their work before this inline middleware component modifies the response
            // The key thing to note here is the "next" delegate is called before this component does its work
            // For more technical info https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-6.0#middleware-order
            app.Use(async (context, next) => {
                await next();
                if (!context.Response.HasStarted) context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync($"Inline Middleware (adding to end of pipeline) \n");
            });

            // Inline middleware component terminating the pipeline
            // The key thing to note here is the "next" delegate is not called if the pipeline is to be terminated
            app.Use(async (context, next) => {
                if (context.Request.Path == "/short") {
                    if (!context.Response.HasStarted) context.Response.ContentType = "text/plain";
                    await context.Response.WriteAsync($"Inline Middleware - Request Short Circuited (invoked for path /short) \n");
                }
                else
                    await next();
            });

            // Seperate branch of the pipeline 
            ((IApplicationBuilder)app).Map("/branch", branch => {
                // Class based middleware component checking for querystring "customer=true"
                // (for this invocation the framework fires in the "next" delegate to the constructor for QueryStringMiddleWare so the pipeline is continued)
                branch.UseMiddleware<QueryStringMiddleWare>();

                // Inline middleware component terminating the pipeline
                branch.Use(async (HttpContext context, Func<Task> next) => {
                    if (!context.Response.HasStarted) context.Response.ContentType = "text/plain";
                    await context.Response.WriteAsync($"Inline Branch Middleware - Request Terminated (invoked for path /branch)  \n");
                });

                // Convenience shorthand for above! Makes it clear that the pipeline is to terminated because there is no "next" parameter
                //branch.Run(async (context) => {
                // if (!context.Response.HasStarted) context.Response.ContentType = "text/plain";
                //    await context.Response.WriteAsync($"Inline Branch Middleware (invoked for path /branch) \");
                //});
            });

            // Seperate branch of the pipeline 
            ((IApplicationBuilder)app).Map("/branch2", branch => {
                // Inline middleware component doing nothing much (just informing us of a new pipeline branch)
                branch.Use(async (HttpContext context, Func<Task> next) => {
                    if (!context.Response.HasStarted) context.Response.ContentType = "text/plain";
                    await context.Response.WriteAsync($"Inline Branch2 Middleware (invoked for path /branch2)  \n");
                    await next();
                });

                // Class based middleware component checking for querystring "customer=true"
                // (this invocation does not provide the "next" delegate to the constructor for QueryStringMiddleWare so the pipeline is terminated)
                branch.Run(new QueryStringMiddleWare().Invoke);
            });

            // Inline middleware component checking for querystring "custom=true"
            // The key thing to note here is the "next" delegate is called after this component does its work to ensure pipeline processing is continued
            app.Use(async (context, next) => {
                if (context.Request.Method == HttpMethods.Get && context.Request.Query["custom"] == "true") {
                    if (!context.Response.HasStarted) context.Response.ContentType = "text/plain";
                    await context.Response.WriteAsync("Inline Middleware (invoked when querystring custom=true) \n");
                }
                await next();
            });

            // Class based middleware component checking for querystring "customer=true"
            // (this invocation fires in the "next" delegate to the constructor for QueryStringMiddleWare so the pipeline is continued)
            app.UseMiddleware<QueryStringMiddleWare>();

            // Endpoint handling default root URL - this middleware component was set up by the "ASP.NET Core Empty" template
            app.MapGet("/", () => "Hello World! \n");

            // Endpoint handling specific URL with the registered config class being injected in
            app.MapGet("/config", (IOptions<MessageOptions> msgOpts) => {
                var opts = msgOpts.Value;
                return $"Config value are {opts.CityName}, {opts.CountryName} \n";
            });

            // As above but using the overload that takes in a RequestDelegate hence use of async and await
            app.MapGet("/config2", async (HttpContext context, IOptions<MessageOptions> msgOpts) => {
                var opts = msgOpts.Value;
                if (!context.Response.HasStarted) context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync($"Config values are {opts.CityName}, {opts.CountryName} \n");
            });

            // Inline middleware component checking for querystring "config=true" and picking up config data - this middleware is executed before the end points are handled
            // The key thing to note here is the "next" delegate is called after this component does its work to ensure pipeline processing is continued
            app.Use(async (context, next) => {
                if (context.Request.Method == HttpMethods.Get && context.Request.Query["config"] == "true") {
                    var opts = context.RequestServices.GetService<IOptions<MessageOptions>>()?.Value;
                    if (!context.Response.HasStarted) context.Response.ContentType = "text/plain";
                    await context.Response.WriteAsync($"Inline Middleware (invoked when querystring config=true) - config values are {opts?.CityName}, {opts?.CountryName} \n");
                }
                await next();
            });

            // Class based middleware component checking for path "/config3"
            // (this invocation fires in the "next" delegate to the constructor for ConfigMiddleWare so the pipeline is continued)
            app.UseMiddleware<ConfigMiddleware>();

            app.Run();
        }
    }
}
