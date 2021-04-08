/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */

using CouchDB.Driver;
using OpenIddict.Core;
using OpenIddict.CouchDB;
using OpenIddict.CouchDB.Models;
using System;
using System.ComponentModel;
using SR = OpenIddict.Abstractions.OpenIddictResources;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Exposes the necessary methods required to configure the OpenIddict CouchDB services.
    /// </summary>
    public class CouchDbOpenIddictBuilder
    {
        /// <summary>
        /// Initializes a new instance of <see cref="CouchDbOpenIddictBuilder"/>.
        /// </summary>
        /// <param name="services">The services collection.</param>
        public CouchDbOpenIddictBuilder(IServiceCollection services)
            => Services = services ?? throw new ArgumentNullException(nameof(services));

        /// <summary>
        /// Gets the services collection.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IServiceCollection Services { get; }

        /// <summary>
        /// Amends the default OpenIddict CouchDB configuration.
        /// </summary>
        /// <param name="configuration">The delegate used to configure the OpenIddict options.</param>
        /// <remarks>This extension can be safely called multiple times.</remarks>
        /// <returns>The <see cref="CouchDbOpenIddictBuilder"/>.</returns>
        public CouchDbOpenIddictBuilder Configure(Action<CouchDbOpenIddictOptions> configuration)
        {
            Check.NotNull(configuration, nameof(configuration));

            Services.Configure(configuration);

            return this;
        }

        /// <summary>
        /// Configures OpenIddict to use the specified entity as the default application entity.
        /// </summary>
        /// <returns>The <see cref="CouchDbOpenIddictBuilder"/>.</returns>
        public CouchDbOpenIddictBuilder ReplaceDefaultApplicationEntity<TApplication>()
            where TApplication : CouchDbOpenIddictApplication
        {
            Services.Configure<OpenIddictCoreOptions>(options => options.DefaultApplicationType = typeof(TApplication));

            return this;
        }

        /// <summary>
        /// Configures OpenIddict to use the specified entity as the default authorization entity.
        /// </summary>
        /// <returns>The <see cref="CouchDbOpenIddictBuilder"/>.</returns>
        public CouchDbOpenIddictBuilder ReplaceDefaultAuthorizationEntity<TAuthorization>()
            where TAuthorization : CouchDbOpenIddictAuthorization
        {
            Services.Configure<OpenIddictCoreOptions>(options => options.DefaultAuthorizationType = typeof(TAuthorization));

            return this;
        }

        /// <summary>
        /// Configures OpenIddict to use the specified entity as the default scope entity.
        /// </summary>
        /// <returns>The <see cref="CouchDbOpenIddictBuilder"/>.</returns>
        public CouchDbOpenIddictBuilder ReplaceDefaultScopeEntity<TScope>()
            where TScope : CouchDbOpenIddictScope
        {
            Services.Configure<OpenIddictCoreOptions>(options => options.DefaultScopeType = typeof(TScope));

            return this;
        }

        /// <summary>
        /// Configures OpenIddict to use the specified entity as the default token entity.
        /// </summary>
        /// <returns>The <see cref="CouchDbOpenIddictBuilder"/>.</returns>
        public CouchDbOpenIddictBuilder ReplaceDefaultTokenEntity<TToken>()
            where TToken : CouchDbOpenIddictToken
        {
            Services.Configure<OpenIddictCoreOptions>(options => options.DefaultTokenType = typeof(TToken));

            return this;
        }

        /// <summary>
        /// Replaces the default application discriminator (by default, openiddict.applications).
        /// </summary>
        /// <param name="discriminator">The discriminator name.</param>
        /// <returns>The <see cref="CouchDbOpenIddictBuilder"/>.</returns>
        public CouchDbOpenIddictBuilder SetApplicationDiscriminator(string discriminator)
        {
            if (string.IsNullOrEmpty(discriminator))
            {
                throw new ArgumentException(SR.GetResourceString(SR.ID0261), nameof(discriminator));
            }

            return Configure(options => options.ApplicationDiscriminator = discriminator);
        }

        /// <summary>
        /// Replaces the default authorization discriminator (by default, openiddict.authorizations).
        /// </summary>
        /// <param name="discriminator">The discriminator name.</param>
        /// <returns>The <see cref="CouchDbOpenIddictBuilder"/>.</returns>
        public CouchDbOpenIddictBuilder SetAuthorizationDiscriminator(string discriminator)
        {
            if (string.IsNullOrEmpty(discriminator))
            {
                throw new ArgumentException(SR.GetResourceString(SR.ID0261), nameof(discriminator));
            }

            return Configure(options => options.AuthorizationDiscriminator = discriminator);
        }

        /// <summary>
        /// Replaces the default scope discriminator (by default, openiddict.scopes).
        /// </summary>
        /// <param name="discriminator">The discriminator name.</param>
        /// <returns>The <see cref="CouchDbOpenIddictBuilder"/>.</returns>
        public CouchDbOpenIddictBuilder SetScopeDiscriminator(string discriminator)
        {
            if (string.IsNullOrEmpty(discriminator))
            {
                throw new ArgumentException(SR.GetResourceString(SR.ID0261), nameof(discriminator));
            }

            return Configure(options => options.ScopeDiscriminator = discriminator);
        }

        /// <summary>
        /// Replaces the default token discriminator (by default, openiddict.tokens).
        /// </summary>
        /// <param name="discriminator">The discriminator name.</param>
        /// <returns>The <see cref="CouchDbOpenIddictBuilder"/>.</returns>
        public CouchDbOpenIddictBuilder SetTokenDiscriminator(string discriminator)
        {
            if (string.IsNullOrEmpty(discriminator))
            {
                throw new ArgumentException(SR.GetResourceString(SR.ID0261), nameof(discriminator));
            }

            return Configure(options => options.TokenDiscriminator = discriminator);
        }

        /// <summary>
        /// Change the database name that will be used on the provided couch client
        /// to retrieve a database.
        /// </summary>
        /// <param name="name">The name of the database.</param>
        /// <returns>The <see cref="CouchDbOpenIddictBuilder"/>.</returns>
        public CouchDbOpenIddictBuilder SetDatabaseName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Database name must not be null or whitespace.", nameof(name));
            }

            return Configure(options => options.DatabaseName = name);
        }

        /// <summary>
        /// Configures the CouchDB design document that will be used.
        /// </summary>
        /// <param name="viewOptions">The new options to use.</param>
        /// <returns>The <see cref="CouchDbIdentityBuilder"/>.</returns>
        public CouchDbOpenIddictBuilder UseViewOptions(CouchDbOpenIddictViewOptions viewOptions)
        {
            Check.NotNull(viewOptions, nameof(viewOptions));

            return Configure(options => options.ViewOptions = viewOptions);
        }

        /// <summary>
        /// Configures the CouchDB stores to use the specified client
        /// instead of retrieving it from the dependency injection container.
        /// </summary>
        /// <param name="client">The <see cref="ICouchClient"/>.</param>
        /// <returns>The <see cref="CouchDbOpenIddictBuilder"/>.</returns>
        public CouchDbOpenIddictBuilder UseClient(ICouchClient client)
        {
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            return Configure(options => options.CouchClient = client);
        }

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object? obj) => base.Equals(obj);

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => base.GetHashCode();

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string? ToString() => base.ToString();
    }
}