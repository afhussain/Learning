namespace Platform;

// Used for processing middleware and route endpoints
public class MyRequestProcessor
{
    private int _counter = 0;
    private string _message;
    private RequestDelegate? _next;

    public MyRequestProcessor(string message) { _message = message; }

    public MyRequestProcessor(RequestDelegate nextDelegate, string message)
    {
        _message = message;
        _next = nextDelegate;
    }

    public async Task Invoke(HttpContext context)
    {
        // Ignore favicon request - see https://stackoverflow.com/questions/66620965/why-are-middleware-components-called-twice-in-a-net-core-pipeline
        if (context.Request.Path != "/favicon.ico") {
            _counter++;
            if (!context.Response.HasStarted) context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync($"Counter {_counter} - Request processor invoked (by either middleware or endpoint) - {_message} \n");
        }

        if (_next != null)
            await _next(context);
    }
}

// Can only be used for processing route endpoints and not middleware 
// as there is a dependency on data provided by ASP.NET core routing 
// Also note there is no provision to continue the pipeline!
public class MyEndPointProcessor
{
    private static int _counter = 0;

    // Unlike in class MyRequestProcessor where we must have a method named Invoke
    // here we could name the method to anything we want so calling it EndPoint
    // makes it clear what its purpose is
    public static async Task EndPoint(HttpContext context)
    {
        _counter++;
        if (!context.Response.HasStarted) context.Response.ContentType = "text/plain";
        await context.Response.WriteAsync($"Counter {_counter} - Endpoint processor invoked for route '{context.Request.Path}' - segments are: \n");
        foreach (var kvp in context.Request.RouteValues)
            await context.Response.WriteAsync($"    {kvp.Key}: {kvp.Value} \n");
    }
}
