/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */

using Microsoft.Extensions.DependencyInjection.Extensions;
using OpenIddict.CouchDB;
using OpenIddict.CouchDB.Models;
using OpenIddict.CouchDB.Resolvers;
using OpenIddict.CouchDB.Stores;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Exposes extensions allowing to register the OpenIddict MongoDB services.
    /// </summary>
    public static class OpenIddictCouchDbBuilderExtensions
    {
        /// <summary>
        /// Registers the MongoDB stores services in the DI container and
        /// configures OpenIddict to use the MongoDB entities by default.
        /// </summary>
        /// <param name="builder">The services builder used by OpenIddict to register new services.</param>
        /// <remarks>This extension can be safely called multiple times.</remarks>
        /// <returns>The <see cref="OpenIddictCouchDbBuilder"/>.</returns>
        public static OpenIddictCouchDbBuilder UseCouchDb(this OpenIddictCoreBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            // Note: Mongo uses simple binary comparison checks by default so the additional
            // query filtering applied by the default OpenIddict managers can be safely disabled.
            builder.DisableAdditionalFiltering();

            builder.SetDefaultApplicationEntity<OpenIddictCouchDbApplication>()
                   .SetDefaultAuthorizationEntity<OpenIddictCouchDbAuthorization>()
                   .SetDefaultScopeEntity<OpenIddictCouchDbScope>()
                   .SetDefaultTokenEntity<OpenIddictCouchDbToken>();

            // Note: the Mongo stores/resolvers don't depend on scoped/transient services and thus
            // can be safely registered as singleton services and shared/reused across requests.
            builder.ReplaceApplicationStoreResolver<OpenIddictCouchDbApplicationStoreResolver>(ServiceLifetime.Singleton)
                   .ReplaceAuthorizationStoreResolver<OpenIddictCouchDbAuthorizationStoreResolver>(ServiceLifetime.Singleton)
                   .ReplaceScopeStoreResolver<OpenIddictCouchDbScopeStoreResolver>(ServiceLifetime.Singleton)
                   .ReplaceTokenStoreResolver<OpenIddictCouchDbTokenStoreResolver>(ServiceLifetime.Singleton);

            builder.Services.TryAddSingleton(typeof(OpenIddictCouchDbApplicationStore<>));
            builder.Services.TryAddSingleton(typeof(OpenIddictCouchDbAuthorizationStore<>));
            builder.Services.TryAddSingleton(typeof(OpenIddictCouchDbScopeStore<>));
            builder.Services.TryAddSingleton(typeof(OpenIddictCouchDbTokenStore<>));

            return new OpenIddictCouchDbBuilder(builder.Services);
        }

        /// <summary>
        /// Registers the MongoDB stores services in the DI container and
        /// configures OpenIddict to use the MongoDB entities by default.
        /// </summary>
        /// <param name="builder">The services builder used by OpenIddict to register new services.</param>
        /// <param name="configuration">The configuration delegate used to configure the MongoDB services.</param>
        /// <remarks>This extension can be safely called multiple times.</remarks>
        /// <returns>The <see cref="OpenIddictCoreBuilder"/>.</returns>
        public static OpenIddictCoreBuilder UseCouchDb(
            this OpenIddictCoreBuilder builder, Action<OpenIddictCouchDbBuilder> configuration)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(configuration, nameof(configuration));

            configuration(builder.UseCouchDb());

            return builder;
        }
    }
}