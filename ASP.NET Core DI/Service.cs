namespace Platform;

// Simple service that outputs value of invocation counter to the response

public interface IMyService
{
    Task InvocationCounter(HttpContext context, string title, string content = "");
}

public class MyServiceText : IMyService
{
    private int counter = 0;

    public async Task InvocationCounter(HttpContext context, string title, string content = "")
    {
        // Ignore favicon request - see https://stackoverflow.com/questions/66620965/why-are-middleware-components-called-twice-in-a-net-core-pipeline
        if (context.Request.Path != "/favicon.ico") {
            if (!context.Response.HasStarted) context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync($"{title}\n" + (content == "" ? "" : $"{content}\n") + $"Shared service invoked - counter is {++counter}\n\n");
        }
    }
}

public class MyServiceHTML : IMyService
{
    private int counter = 0;

    public async Task InvocationCounter(HttpContext context, string title, string content = "")
    {
        // Ignore favicon request - see https://stackoverflow.com/questions/66620965/why-are-middleware-components-called-twice-in-a-net-core-pipeline
        if (context.Request.Path != "/favicon.ico") {
            if (!context.Response.HasStarted) context.Response.ContentType = "text/html";

            // Let ASP.NET add in HTML head and body tags, if we do it ourselves as per commented out section below
            // then response has mutiple head and body sections if the service is invoked mutiple times for a request
            await context.Response.WriteAsync($@"
                <b>{title}</b><br>"
                + (content == "" ? "" : $"<span>{content}</span><br>") +
                $@"<span>Shared service invoked - counter is {++counter}</span><br><br>");

            //await context.Response.WriteAsync($@"
            //<!DOCTYPE html>
            //<html lang=""en"">
            //<head><title>MyService</title></head>
            //<body>
            //    <h3>{title}</h3>"
            //    + (content == "" ? "" : $"<span>{content}</span><br>") +
            //    $@"<span>Shared service invoked - counter is {++counter}</span>
            //</body>
            //</html>");
        }
    }
}
