Sample web application following Chapter 14 of Adam Freeman's "Pro ASP.NET Core 7" book.

The application has been created using the "ASP.NET Core Empty" template provided by VS 2022. It is based upon the minimal API architecture and targets .NET 8, has no support for HTTPS and does not use "top level statements".

The application demonstrates using the ASP.NET Core dependency injection feature to create and consume a shared singleton service.

For further technical info see https://learn.microsoft.com/en-us/aspnet/core/fundamentals/xxxx

For each request the following behaviour is always observed: 

  - displays Inline Middleware Pipeline Start
    ** Shared service counter +1

  - displays Class Middleware Pipeline Continue - request processor invoked
    ** Shared service counter +1
    ** Own copy of request counter +1

  - displays Class Middleware Pipeline End - request processor invoked
    ** Shared service counter +1
    ** Own copy of request counter +1

The service is a shared singleton and its counter is incremented for each middleware that is processed i.e. count is advanced by 3. However, the class middleware components have their own instance so their counters remain in sync and increment by 1.

In addition to the above, specific behaviour for each test URL is as follows:

http://localhost:5199/ -> displays Hello World

http://localhost:5199/endpoint/inline -> displays Inline Endpoint 
                                         ** Shared service counter +1

http://localhost:5199/endpoint/class -> displays Class Endpoint - request processor invoked
                                        ** Shared service counter +1
                                        ** Own copy of request counter +1 (independent of counters for pipeline continue and pipeline end)

http://localhost:5199/endpoint/class1 -> displays Class Endpoint - endpoint processor invoked
                                        ** Shared service counter +1
                                        ** Own copy of processor counter +1

http://localhost:5199/endpoint/class2 -> displays Class Endpoint - endpoint processor invoked
                                        ** Shared service counter +1
                                        ** Own copy of processor counter +1

http://localhost:5199/xxx -> anything apart from the above is not supported so no further behaviour observed

