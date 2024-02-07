using Platform;
using System.Reflection;

namespace ASP.NET_Core_DI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Register our service as a singleton so it is shared everywhere
        bool htmlOutput = false;
        if (htmlOutput)
            builder.Services.AddSingleton<IMyService, MyServiceHTML>();
        else
            builder.Services.AddSingleton<IMyService, MyServiceText>();

        var app = builder.Build();

        // Recap of notes from Routing sample app:
        //  - routes are handled by end points after all middleware has been processed regardless of their order in source code
        //  - end points terminate the pipeline
        //  - order of middleware components in source code defines the order in which they are invoked
        //  - classes processing middleware and endpoints have their own object instance
        //  - it is possible that each middleware is invoked twice per request giving the effect of the counters incrementing by 2
        //    (see https://stackoverflow.com/questions/66620965/why-are-middleware-components-called-twice-in-a-net-core-pipeline)
        //    hence requests for favicon.ico are ignored by middleware components

        // This app demonstrates a service shared across:
        // - inline middleware
        // - class based middleware
        // - inline endpoint
        // - class based endpoint

        app.MapGet("/", () => "Hello World!");

        // Inline middleware component that requires our service
        app.Use(async (context, next) => {
            IMyService service = context.RequestServices.GetRequiredService<IMyService>();
            await service.InvocationCounter(context, "Inline middleware - start of pipeline");
            await next();
        });

        // Class based middleware that requires our service
        app.UseMiddleware<MyRequestProcessor>("Class based middleware - continuing pipeline");

        // End of pipeline middleware using the same class as above middleware
        // (new instance of class object is used - proved by observing counter value is in sync with above middleware)
        app.UseMiddleware<MyRequestProcessor>("Class based middleware - end of pipeline");

        // Inline endpoint handler that requires our service 
        app.MapGet("endpoint/inline", async (HttpContext context, IMyService service) => {
            await service.InvocationCounter(context, "Inline endpoint for route '/endpoint/inline'");
        });

        // Endpoint handler using same class as for middleware
        // (new instance of class object is used - proved by observing counter increment is independent of middleware)
        app.MapGet("endpoint/class", ActivatorUtilities.CreateInstance<MyRequestProcessor>(app.Services, "Class based endpoint for route '/endpoint/class'").Invoke);

        // Class based end point route that requires our service - using a different class from middleware
        app.MapGet("endpoint/class1", ActivatorUtilities.CreateInstance<MyEndPointProcessor>(app.Services, "Class based endpoint for route '/endpoint/class1'").EndPoint);

        // Class based end point route using same class as above
        // (new instance of class object is used - proved by observing counter increment is independent of above)
        app.MapGet("endpoint/class2", ActivatorUtilities.CreateInstance<MyEndPointProcessor>(app.Services, "Class based endpoint for route '/endpoint/class2'").EndPoint);

        app.Run();
    }
}
