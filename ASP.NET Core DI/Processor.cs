using Platform;

namespace Platform;

// Used for processing middleware and route endpoints
public class MyRequestProcessor
{
    private int _counter = 0;
    private IMyService _service;
    private string _message;
    private RequestDelegate? _next;

    public MyRequestProcessor(IMyService service, string message) { _service = service; _message = message; }

    public MyRequestProcessor(RequestDelegate nextDelegate, IMyService service, string message)
    {
        _next = nextDelegate;
        _service = service;
        _message = message;
    }

    public async Task Invoke(HttpContext context)
    {
        // Ignore favicon request - see https://stackoverflow.com/questions/66620965/why-are-middleware-components-called-twice-in-a-net-core-pipeline
        if (context.Request.Path != "/favicon.ico")
            await _service.InvocationCounter(context, _message, $"Request processor invoked (by either middleware or endpoint) - counter is {++_counter}");

        if (_next != null)
            await _next(context);
    }
}

// Used for processing route endpoints only as there is no provision to continue the pipeline!
public class MyEndPointProcessor
{
    private int _counter = 0;
    private string _message;
    private IMyService _service;

    public MyEndPointProcessor(string message, IMyService service)
    {
        _message = message;
        _service = service;
    }

    public async Task EndPoint(HttpContext context)
    {
        await _service.InvocationCounter(context, _message, $"Endpoint processor invoked - counter is {++_counter}");
    }
}
