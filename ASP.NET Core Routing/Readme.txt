Sample web application following Chapter 13 of Adam Freeman's "Pro ASP.NET Core 6" book.

The application has been created using the "ASP.NET Core Empty" template provided by VS 2022. It is based upon the minimal API architecture and targets .NET 8, has no support for HTTPS and does not use "top level statements".

The application demonstrates the simplest parts of the URL routing capability provided by ASP.NET Core to process endpoints.

For further technical info see https://learn.microsoft.com/en-us/aspnet/core/fundamentals/routing?view=aspnetcore-8.0

Local test URLs (in all cases observe counter value - each instance of the request processor has its own copy):

http://localhost:5223/ -> displays Request Processor invoked for: Middleware Pipeline Start, Middleware Pipeline End
                                   Hello World
                                   ** NOTE: middleware counter in sync with pipleline start and pipeline end ** 

http://localhost:5223/1 -> displays Request Processor invoked for: Middleware Pipeline Start, Middleware Pipeline End, Endpoint route '/1'
                                   ** NOTE: endpoint counter independent of route '/2' ** 

http://localhost:5223/2 -> displays Request Processor invoked for: Middleware Pipeline Start, Middleware Pipeline End, Endpoint route '/2'
                                   ** NOTE: endpoint counter independent of route '/1' ** 

http://localhost:5223/3 -> displays Request Processor invoked for: Middleware Pipeline Start, Middleware Pipeline End
                                    Hello World route '/3'

http://localhost:5223/endpoint/class -> displays Request Processor invoked for: Middleware Pipeline Start, Middleware Pipeline End
                                                 Endpoint Processor invoked for: route 'endpoint,class'
                                                 ** NOTE: endpoint counter shared with routes below ** 

http://localhost:5223/10/12 -> displays Request Processor invoked for: Middleware Pipeline Start, Middleware Pipeline End
                                        Endpoint Processor invoked for: route segments '10,12'
                                        ** NOTE: counter shared with route above and below ** 

http://localhost:5223/45/99/x/y/x/123/456 -> displays Request Processor invoked for: Middleware Pipeline Start, Middleware Pipeline End
                                                      Endpoint Processor invoked for: route segments '45,99, x/y/x/123/456'
                                                      ** NOTE: counter shared with routes above ** 

http://localhost:5223/a45/99/x/y/x/123/456 -> displays Request Processor invoked for: Middleware Pipeline Start, Middleware Pipeline End
                                                       Routed to fallback endpoint
