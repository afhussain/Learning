Sample web application following Chapter 12 of Adam Freeman's "Pro ASP.NET Core 6" book.

The application has been created using the "ASP.NET Core Empty" template provided by VS 2022. It is based upon the minimal API architecture and targets .NET 8, has no support for HTTPS and does not use "top level statements".

The application demonstrates the key features of the ASP.NET Core request pipeline and middleware components. It also touches on DI and the options pattern for configuring middleware.

For further technical info see // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-6.0#middleware-order

Local test URLs:

http://localhost:5033/ -> displays Hello World, End of Pipeline
http://localhost:5033/?custom=true -> displays Inline Middleware, Class Middleware, Hello World, End of Pipeline 

http://localhost:5033/short -> displays Request Short Circuited, End of pipeline 
http://localhost:5033/short?custom=true -> displays Request Short Circuited, End of pipeline 

http://localhost:5033/dummy -> displays End of pipeline
http://localhost:5033/dummy?custom=true -> displays Inline Middleware, Class Middleware, End of Pipeline 

http://localhost:5033/branch -> displays Inline Branch Middleware, End of pipeline
http://localhost:5033/branch?custom=true -> displays Class Middleware, Inline Branch Middleware, End of pipeline

http://localhost:5033/branch2 -> displays Inline Branch2 Middleware, End of pipeline
http://localhost:5033/branch2?custom=true -> displays Inline Branch2 Middleware, Class Middleware, End of pipeline

http://localhost:5033/config -> displays Config Data (Albany, USA), End of pipeline
http://localhost:5033/config2 -> displays Config Data (Albany, USA), End of pipeline

http://localhost:5033/?config=true -> displays Inline Config Middleware (Albany, USA), Hello World, End of pipeline
http://localhost:5033/config2?config=true -> displays Inline Config Middleware (Albany, USA), Config Data (Albany, USA), End of pipeline

http://localhost:5033/config3 -> displays Class Config Middleware (Albany, USA), End of pipeline
http://localhost:5033/config3?config=true -> displays Inline Config Middleware (Albany, USA), Class Config Middleware (Albany, USA), End of pipeline

http://localhost:5033/?custom=true&config=true -> displays Inline Middleware, Class Middleware, Inline Config Middleware (Albany, USA), Hello World, End of Pipeline 
