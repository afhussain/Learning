using Platform;

namespace ASP.NET_Core_Routing
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            // Routes are handled by end points after all middleware has been processed regardless
            // of their order in source code - note to self: end points terminate the pipeline!
            app.MapGet("/", () => "Hello World!");

            // Class based middleware for start of pipeline and passing the request along the pipeline
            // (order of middleware components in source code defines the order in which they are invoked)
            // It is possible that each middleware is invoked twice per request giving the effect of the counter incrementing by 2
            // see https://stackoverflow.com/questions/66620965/why-are-middleware-components-called-twice-in-a-net-core-pipeline
            // hence requests for favicon.ico are ignored by middleware components
            app.UseMiddleware<MyRequestProcessor>("Class based middleware - starting AND continuing the pipeline");

            // Use same class to process an endpoint - note to self: this invocation does not fire in the next delegate
            // Note to self: new instance of class object is used - proved by observing counter increment independent of middleware
            app.MapGet("1", new MyRequestProcessor("Class based endpoint for route '/1' - terminates the pipeline").Invoke);

            // Use same class to process another endpoint - note to self: this invocation does not fire in the next delegate
            // Note to self: new instance of class object is used - proved by observing counter increment independent of route /1
            app.MapGet("2", new MyRequestProcessor("Class based endpoint for route '/2' - terminates the pipeline").Invoke);

            // Just another simple route handled inline
            app.MapGet("3", () => "Hello World for route '/3'");

            // End of pipeline middleware using the same class for start of pipeline
            // Note to self: new instance of class object is used - proved by observing counter value is in sync with first middlware
            app.UseMiddleware<MyRequestProcessor>("Class based middleware - last one in the pipeline before endpoints are processed");

            // Static class based endpoint handler for a simple route
            app.MapGet("endpoint/class", MyEndPointProcessor.EndPoint);

            // Use same static class to handle endpoint of the format x/y/... where the first two  
            // segments of the route are integers followed by any number of segments in any format
            app.MapGet("{first:int}/{second:int}/{*catchall}", MyEndPointProcessor.EndPoint);

            // Catch if no routes matched!
            app.MapFallback(() => "Routed to fallback endpoint");

            // Go, go, go
            app.Run();
        }
    }
}
