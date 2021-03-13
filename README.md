# P41.OpenIddict.CouchDB

[![Github Build Status](https://github.com/panoukos41/couchdb-openiddict/actions/workflows/build.yaml/badge.svg)](https://github.com/panoukos41/couchdb-openiddict/actions/workflows/build.yaml)
[![Nuget](https://img.shields.io/nuget/v/P41.OpenIddict.CouchDB)](https://www.nuget.org/packages/P41.OpenIddict.CouchDB/)
[![Nuget Downloads](https://img.shields.io/nuget/dt/P41.OpenIddict.CouchDB)](https://www.nuget.org/packages/P41.OpenIddict.CouchDB/)
[![GitHub](https://img.shields.io/github/license/panoukos41/couchdb-openiddict)](https://github.com/panoukos41/couchdb-openiddict/blob/main/LICENSE.md)

[CouchDB](https://couchdb.apache.org/) store provider for the [Openiddict](https://github.com/openiddict/openiddict-core) using [CouchDB.NET](https://github.com/matteobortolazzo/couchdb-net).

The project was insipired from the existing [OpenIddict](https://github.com/openiddict/openiddict-core) MongoDB implementation.

# Before Getting started
This project was made in a hurry and was live tested on a really simple application so some things might not work as expected. Feel free to open an issue and even a pull request.

I will try my best to support the project and provide tests and samples whenever i find time.

Thanks for looking at this library 😄

# Getting Started

By default a database named `openiddict` is used but you can change it using the options overload.

Your [CouchDB](https://couchdb.apache.org/) database must include the following [design documents](ddocs/).

At your `Startup.cs` add your `ICouchClient` in DI and use the following code.
```csharp
// Configure OpenIddict stores and services.
services.AddOpenIddict()
    // Register the OpenIddict core components and to use the CouchDb stores and models.
    .AddCore(options => options.UseCouchDb())
```

Or you can provide your own instance of `ICouchClient` in the options.
```csharp
// Configure OpenIddict stores and services.
// With custom client.
services.AddOpenIddict()
    // Register the OpenIddict core components and to use the CouchDb stores and models.
    .AddCore(options => options
        .UseCouchDb(options => options
            .UseClient(new CouchClient("http://localhost:5984"))))
```

Services are *registered as Singleton* since `ICouchClient` is/should be registered as such and no other dependency is needed.

# LICENSE
The project is Licensed under the Apache-2.0 License see the [LICENSE](LICENSE.md) for more info.
