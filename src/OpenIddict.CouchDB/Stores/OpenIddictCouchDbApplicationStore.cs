/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */

using CouchDB.Driver;
using CouchDB.Driver.Exceptions;
using CouchDB.Driver.Extensions;
using CouchDB.Driver.Query.Extensions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenIddict.Abstractions;
using OpenIddict.CouchDB.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using SR = OpenIddict.Abstractions.OpenIddictResources;

namespace OpenIddict.CouchDB
{
    /// <summary>
    /// Provides methods allowing to manage the applications stored in a database.
    /// </summary>
    /// <typeparam name="TApplication">The type of the Application entity.</typeparam>
    public class OpenIddictCouchDbApplicationStore<TApplication> : BaseOpenIddictCouchDbStore<TApplication>, IOpenIddictApplicationStore<TApplication>
        where TApplication : OpenIddictCouchDbApplication
    {
        public OpenIddictCouchDbApplicationStore(
            IServiceProvider provider,
            IOptionsMonitor<OpenIddictCouchDbOptions> options)
            : base(provider, options)
        {
        }

        protected override string Discriminator => Options.CurrentValue.ApplicationDiscriminator;

        /// <inheritdoc/>
        public virtual async ValueTask<long> CountAsync(CancellationToken cancellationToken)
        {
            var db = GetDatabase();
            var (design, view) = OpenIddictCouchDbViews.Application.Count;

            return (await db.GetViewAsync<TApplication, int>(design, view, cancellationToken: cancellationToken))
                .Rows.FirstOrDefault()?.Value ?? 0;
        }

        /// <inheritdoc/>
        public virtual ValueTask<long> CountAsync<TResult>(
            Func<IQueryable<TApplication>, IQueryable<TResult>> query, CancellationToken cancellationToken)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return new(query(QueryDb()).LongCount());
        }

        /// <inheritdoc/>
        public virtual async ValueTask CreateAsync(TApplication application,
            CancellationToken cancellationToken)
        {
            if (application is null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            application.Discriminator = Discriminator;

            var db = GetDatabase();

            await db.AddAsync(application, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async ValueTask DeleteAsync(TApplication application,
            CancellationToken cancellationToken)
        {
            if (application is null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            var db = GetDatabase();

            try
            {
                await db.RemoveAsync(application, cancellationToken: cancellationToken);
            }
            catch (CouchConflictException ex)
            {
                throw new OpenIddictExceptions.ConcurrencyException(SR.GetResourceString(SR.ID0239), ex);
            }

            var deleteDb = GetDatabase<CouchDocumentDelete>();

            //var auths = OpenIddictCouchViews.Application.Authorizations;
            var (design, view) = OpenIddictCouchDbViews.Authorization.ApplicationId;

            // Delete the authorizations associated with the application.
            var auths = await db.GetViewAsync<TApplication, string, CouchDocumentEmpty>(design, view);
            var authsDel = deleteDb.AddOrUpdateRangeAsync(
                auths.Rows.Select(x => new CouchDocumentDelete(x.Id, x.Doc.Rev)).ToArray());

            // Delete the tokens associated with the application.
            (design, view) = OpenIddictCouchDbViews.Token.ApplicationId;
            var tokens = await db.GetViewAsync<TApplication, string, CouchDocumentDelete>(design, view);
            var tokensDel = deleteDb.AddOrUpdateRangeAsync(tokens.Rows.Select(x => new CouchDocumentDelete(x.Id, x.Doc.Rev)).ToArray());

            await Task.WhenAll(authsDel, tokensDel);
        }

        /// <inheritdoc/>
        public virtual async ValueTask<TApplication?> FindByClientIdAsync(string identifier,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                throw new ArgumentException(SR.GetResourceString(SR.ID0195), nameof(identifier));
            }

            var db = GetDatabase();

            return (await QueryDb(db)
                .Where(x => x.ClientId == identifier)
                .Take(1)
                .ToCouchListAsync(cancellationToken))
                .FirstOrDefault();
        }

        /// <inheritdoc/>
        public virtual async ValueTask<TApplication?> FindByIdAsync(string identifier,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                throw new ArgumentException(SR.GetResourceString(SR.ID0195), nameof(identifier));
            }

            var db = GetDatabase();
            return await db.FindAsync(identifier, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public virtual IAsyncEnumerable<TApplication> FindByPostLogoutRedirectUriAsync(
            string address, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(address))
            {
                throw new ArgumentException(SR.GetResourceString(SR.ID0143), nameof(address));
            }

            return ExecuteAsync(cancellationToken);

            async IAsyncEnumerable<TApplication> ExecuteAsync([EnumeratorCancellation] CancellationToken cancellationToken)
            {
                var db = GetDatabase();

                foreach (var application in await QueryDb(db).Where(app =>
                    app.PostLogoutRedirectUris.Contains(address)).ToCouchListAsync(cancellationToken))
                {
                    yield return application;
                }
            }
        }

        /// <inheritdoc/>
        public virtual IAsyncEnumerable<TApplication> FindByRedirectUriAsync(
            string address, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(address))
            {
                throw new ArgumentException(SR.GetResourceString(SR.ID0143), nameof(address));
            }

            return ExecuteAsync(cancellationToken);

            async IAsyncEnumerable<TApplication> ExecuteAsync([EnumeratorCancellation] CancellationToken cancellationToken)
            {
                var db = GetDatabase();

                foreach (var application in await QueryDb(db).Where(app =>
                    app.RedirectUris.Contains(address)).ToCouchListAsync(cancellationToken))
                {
                    yield return application;
                }
            }
        }

        /// <inheritdoc/>
        public virtual async ValueTask<TResult> GetAsync<TState, TResult>(
            Func<IQueryable<TApplication>, TState, IQueryable<TResult>> query,
            TState state, CancellationToken cancellationToken)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var db = GetDatabase();

            return await query(QueryDb(db), state).FirstOrDefaultAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public virtual ValueTask<string?> GetClientIdAsync(TApplication application,
            CancellationToken cancellationToken)
        {
            if (application is null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            return new ValueTask<string?>(application.ClientId);
        }

        /// <inheritdoc/>
        public virtual ValueTask<string?> GetClientSecretAsync(TApplication application,
            CancellationToken cancellationToken)
        {
            if (application is null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            return new ValueTask<string?>(application.ClientSecret);
        }

        /// <inheritdoc/>
        public virtual ValueTask<string?> GetClientTypeAsync(TApplication application,
            CancellationToken cancellationToken)
        {
            if (application is null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            return new ValueTask<string?>(application.Type);
        }

        /// <inheritdoc/>
        public virtual ValueTask<string?> GetConsentTypeAsync(TApplication application,
            CancellationToken cancellationToken)
        {
            if (application is null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            return new ValueTask<string?>(application.ConsentType);
        }

        /// <inheritdoc/>
        public virtual ValueTask<string?> GetDisplayNameAsync(TApplication application,
            CancellationToken cancellationToken)
        {
            if (application is null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            return new ValueTask<string?>(application.DisplayName);
        }

        /// <inheritdoc/>
        public virtual ValueTask<ImmutableDictionary<CultureInfo, string>> GetDisplayNamesAsync(TApplication application,
            CancellationToken cancellationToken)
        {
            if (application is null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            if (application.DisplayNames is null || application.DisplayNames.Count == 0)
            {
                return new ValueTask<ImmutableDictionary<CultureInfo, string>>(ImmutableDictionary.Create<CultureInfo, string>());
            }

            return new ValueTask<ImmutableDictionary<CultureInfo, string>>(application.DisplayNames.ToImmutableDictionary());
        }

        /// <inheritdoc/>
        public virtual ValueTask<string?> GetIdAsync(TApplication application, CancellationToken cancellationToken)
        {
            if (application is null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            return new ValueTask<string?>(application.Id.ToString());
        }

        /// <inheritdoc/>
        public virtual ValueTask<ImmutableArray<string>> GetPermissionsAsync(TApplication application,
            CancellationToken cancellationToken)
        {
            if (application is null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            if (application.Permissions is null || application.Permissions.Count == 0)
            {
                return new ValueTask<ImmutableArray<string>>(ImmutableArray.Create<string>());
            }

            return new ValueTask<ImmutableArray<string>>(application.Permissions.ToImmutableArray());
        }

        /// <inheritdoc/>
        public virtual ValueTask<ImmutableArray<string>> GetPostLogoutRedirectUrisAsync(
            TApplication application, CancellationToken cancellationToken)
        {
            if (application is null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            if (application.PostLogoutRedirectUris is null || application.PostLogoutRedirectUris.Count == 0)
            {
                return new ValueTask<ImmutableArray<string>>(ImmutableArray.Create<string>());
            }

            return new ValueTask<ImmutableArray<string>>(application.PostLogoutRedirectUris.ToImmutableArray());
        }

        /// <inheritdoc/>
        public virtual ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(TApplication application,
            CancellationToken cancellationToken)
        {
            if (application is null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            if (application.Properties is null)
            {
                return new ValueTask<ImmutableDictionary<string, JsonElement>>(ImmutableDictionary.Create<string, JsonElement>());
            }

            using var document = JsonDocument.Parse(application.Properties.ToString(Formatting.None));
            var builder = ImmutableDictionary.CreateBuilder<string, JsonElement>();

            foreach (var property in document.RootElement.EnumerateObject())
            {
                builder[property.Name] = property.Value.Clone();
            }

            return new ValueTask<ImmutableDictionary<string, JsonElement>>(builder.ToImmutable());
        }

        /// <inheritdoc/>
        public virtual ValueTask<ImmutableArray<string>> GetRedirectUrisAsync(
            TApplication application, CancellationToken cancellationToken)
        {
            if (application is null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            if (application.RedirectUris is null || application.RedirectUris.Count == 0)
            {
                return new ValueTask<ImmutableArray<string>>(ImmutableArray.Create<string>());
            }

            return new ValueTask<ImmutableArray<string>>(application.RedirectUris.ToImmutableArray());
        }

        /// <inheritdoc/>
        public virtual ValueTask<ImmutableArray<string>> GetRequirementsAsync(TApplication application,
            CancellationToken cancellationToken)
        {
            if (application is null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            if (application.Requirements is null || application.Requirements.Count == 0)
            {
                return new ValueTask<ImmutableArray<string>>(ImmutableArray.Create<string>());
            }

            return new ValueTask<ImmutableArray<string>>(application.Requirements.ToImmutableArray());
        }

        /// <inheritdoc/>
        public virtual ValueTask<TApplication> InstantiateAsync(CancellationToken cancellationToken)
        {
            try
            {
                return new ValueTask<TApplication>(Activator.CreateInstance<TApplication>());
            }
            catch (MemberAccessException exception)
            {
                return new ValueTask<TApplication>(Task.FromException<TApplication>(
                    new InvalidOperationException(SR.GetResourceString(SR.ID0240), exception)));
            }
        }

        /// <inheritdoc/>
        public virtual async IAsyncEnumerable<TApplication> ListAsync(
            int? count, int? offset, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var db = GetDatabase();
            var (design, view) = OpenIddictCouchDbViews.Application.All;

            var result = await db.GetViewAsync<TApplication, int, TApplication>(design, view, new()
            {
                Limit = count,
                Skip = offset
            });

            foreach (var row in result.Rows)
            {
                yield return row.Doc;
            }
        }

        /// <inheritdoc/>
        public virtual IAsyncEnumerable<TResult> ListAsync<TState, TResult>(
            Func<IQueryable<TApplication>, TState, IQueryable<TResult>> query,
            TState state, CancellationToken cancellationToken)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return ExecuteAsync(cancellationToken);

            async IAsyncEnumerable<TResult> ExecuteAsync([EnumeratorCancellation] CancellationToken cancellationToken)
            {
                // todo: check
                //foreach (var element in await query(QueryDb(db), state).ToCouchListAsync(cancellationToken))
                foreach (var element in await Task.Run(() => query(QueryDb(), state)))
                {
                    yield return element;
                }
            }
        }

        /// <inheritdoc/>
        public virtual ValueTask SetClientIdAsync(TApplication application,
            string? identifier, CancellationToken cancellationToken)
        {
            if (application is null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            application.ClientId = identifier;

            return default;
        }

        /// <inheritdoc/>
        public virtual ValueTask SetClientSecretAsync(TApplication application,
            string? secret, CancellationToken cancellationToken)
        {
            if (application is null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            application.ClientSecret = secret;

            return default;
        }

        /// <inheritdoc/>
        public virtual ValueTask SetClientTypeAsync(TApplication application,
            string? type, CancellationToken cancellationToken)
        {
            if (application is null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            application.Type = type;

            return default;
        }

        /// <inheritdoc/>
        public virtual ValueTask SetConsentTypeAsync(TApplication application,
            string? type, CancellationToken cancellationToken)
        {
            if (application is null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            application.ConsentType = type;

            return default;
        }

        /// <inheritdoc/>
        public virtual ValueTask SetDisplayNameAsync(TApplication application,
            string? name, CancellationToken cancellationToken)
        {
            if (application is null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            application.DisplayName = name;

            return default;
        }

        /// <inheritdoc/>
        public virtual ValueTask SetDisplayNamesAsync(TApplication application,
            ImmutableDictionary<CultureInfo, string> names, CancellationToken cancellationToken)
        {
            if (application is null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            application.DisplayNames = names;

            return default;
        }

        /// <inheritdoc/>
        public virtual ValueTask SetPermissionsAsync(TApplication application,
            ImmutableArray<string> permissions, CancellationToken cancellationToken)
        {
            if (application is null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            if (permissions.IsDefaultOrEmpty)
            {
                application.Permissions = ImmutableList.Create<string>();

                return default;
            }

            application.Permissions = permissions.ToImmutableList();

            return default;
        }

        /// <inheritdoc/>
        public virtual ValueTask SetPostLogoutRedirectUrisAsync(TApplication application,
            ImmutableArray<string> addresses, CancellationToken cancellationToken)
        {
            if (application is null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            if (addresses.IsDefaultOrEmpty)
            {
                application.PostLogoutRedirectUris = ImmutableList.Create<string>();

                return default;
            }

            application.PostLogoutRedirectUris = addresses.ToImmutableList();

            return default;
        }

        /// <inheritdoc/>
        public virtual ValueTask SetPropertiesAsync(TApplication application,
            ImmutableDictionary<string, JsonElement> properties, CancellationToken cancellationToken)
        {
            if (application is null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            if (properties is null || properties.IsEmpty)
            {
                application.Properties = null;

                return default;
            }

            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                Indented = false
            });

            writer.WriteStartObject();

            foreach (var property in properties)
            {
                writer.WritePropertyName(property.Key);
                property.Value.WriteTo(writer);
            }

            writer.WriteEndObject();
            writer.Flush();

            application.Properties = JObject.Parse(Encoding.UTF8.GetString(stream.ToArray()));

            return default;
        }

        /// <inheritdoc/>
        public virtual ValueTask SetRedirectUrisAsync(TApplication application,
            ImmutableArray<string> addresses, CancellationToken cancellationToken)
        {
            if (application is null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            if (addresses.IsDefaultOrEmpty)
            {
                application.RedirectUris = ImmutableList.Create<string>();

                return default;
            }

            application.RedirectUris = addresses.ToImmutableList();

            return default;
        }

        /// <inheritdoc/>
        public virtual ValueTask SetRequirementsAsync(TApplication application,
            ImmutableArray<string> requirements, CancellationToken cancellationToken)
        {
            if (application is null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            if (requirements.IsDefaultOrEmpty)
            {
                application.Requirements = ImmutableList.Create<string>();

                return default;
            }

            application.Requirements = requirements.ToImmutableList();

            return default;
        }

        /// <inheritdoc/>
        public virtual async ValueTask UpdateAsync(TApplication application,
            CancellationToken cancellationToken)
        {
            if (application is null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            var db = GetDatabase();
            try
            {
                await db.AddOrUpdateAsync(application, cancellationToken: cancellationToken);
            }
            catch (CouchConflictException ex)
            {
                throw new OpenIddictExceptions.ConcurrencyException(SR.GetResourceString(SR.ID0239), ex);
            }
        }
    }
}