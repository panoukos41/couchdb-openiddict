/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */

using CouchDB.Driver;
using CouchDB.Driver.Exceptions;
using CouchDB.Driver.Extensions;
using CouchDB.Driver.Views;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenIddict.Abstractions;
using OpenIddict.CouchDB.Internal;
using OpenIddict.CouchDB.Models;
using OpenIddict.CouchDB.Stores.Internal;
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

namespace OpenIddict.CouchDB.Stores
{
    /// <summary>
    /// Provides methods allowing to manage the scopes stored in a database.
    /// </summary>
    /// <typeparam name="TScope">The type of the Scope entity.</typeparam>
    public class OpenIddictCouchDbScopeStore<TScope> : StoreBase<TScope>, IOpenIddictScopeStore<TScope>
        where TScope : OpenIddictCouchDbScope
    {
        public OpenIddictCouchDbScopeStore(
            IServiceProvider provider,
            IOptionsMonitor<OpenIddictCouchDbOptions> options)
            : base(provider, options)
        {
            Discriminator = Options.CurrentValue.ScopeDiscriminator;
        }

        /// <inheritdoc/>
        protected override string Discriminator { get; }

        /// <inheritdoc/>
        public virtual async ValueTask<long> CountAsync(CancellationToken cancellationToken)
        {
            return (await GetDatabase()
                .GetViewAsync(Views.Scope<TScope>.Count, cancellationToken: cancellationToken))
                .FirstOrDefault()
                ?.Value ?? 0;
        }

        /// <inheritdoc/>
        public virtual async ValueTask<long> CountAsync<TResult>(
            Func<IQueryable<TScope>, IQueryable<TResult>> query, CancellationToken cancellationToken)
        {
            Check.NotNull(query, nameof(query));

            return await Task.Run(() => query(QueryDb()).LongCount()).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public virtual async ValueTask CreateAsync(TScope scope, CancellationToken cancellationToken)
        {
            Check.NotNull(scope, nameof(scope));

            await GetDatabase().AddAsync(scope, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async ValueTask DeleteAsync(TScope scope, CancellationToken cancellationToken)
        {
            Check.NotNull(scope, nameof(scope));

            var db = GetDatabase();

            try
            {
                await db.RemoveAsync(scope, cancellationToken: cancellationToken);
            }
            catch (CouchConflictException ex)
            {
                throw new OpenIddictExceptions.ConcurrencyException(SR.GetResourceString(SR.ID0245), ex);
            }
        }

        /// <inheritdoc/>
        public virtual async ValueTask<TScope?> FindByIdAsync(string identifier, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                throw new ArgumentException(SR.GetResourceString(SR.ID0195), nameof(identifier));
            }

            var db = GetDatabase();

            return await db.FindAsync(identifier, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async ValueTask<TScope?> FindByNameAsync(string name, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException(SR.GetResourceString(SR.ID0202), nameof(name));
            }

            return (await QueryDb()
                .Where(x => x.Name == name)
                .Take(1)
                .ToCouchListAsync(cancellationToken))
                .FirstOrDefault();
        }

        /// <inheritdoc/>
        public virtual IAsyncEnumerable<TScope> FindByNamesAsync(ImmutableArray<string> names, CancellationToken cancellationToken)
        {
            if (names.Any(name => string.IsNullOrEmpty(name)))
            {
                throw new ArgumentException(SR.GetResourceString(SR.ID0203), nameof(names));
            }

            return ExecuteAsync(cancellationToken);

            async IAsyncEnumerable<TScope> ExecuteAsync([EnumeratorCancellation] CancellationToken cancellationToken)
            {
                var options = new CouchViewOptions<string>
                {
                    IncludeDocs = true,
                    Keys = names
                };

                foreach (var row in await GetDatabase()
                    .GetViewAsync(Views.Scope<TScope>.Name, options, cancellationToken)
                    .ConfigureAwait(false))
                {
                    yield return row.Document;
                }
            }
        }

        /// <inheritdoc/>
        public virtual IAsyncEnumerable<TScope> FindByResourceAsync(string resource, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(resource))
            {
                throw new ArgumentException(SR.GetResourceString(SR.ID0062), nameof(resource));
            }

            return ExecuteAsync(cancellationToken);

            async IAsyncEnumerable<TScope> ExecuteAsync([EnumeratorCancellation] CancellationToken cancellationToken)
            {
                foreach (var scope in await QueryDb().Where(scope =>
                    scope.Resources.Contains(resource)).ToCouchListAsync(cancellationToken))
                {
                    yield return scope;
                }
            }
        }

        /// <inheritdoc/>
        public virtual async ValueTask<TResult> GetAsync<TState, TResult>(
            Func<IQueryable<TScope>, TState, IQueryable<TResult>> query,
            TState state, CancellationToken cancellationToken)
        {
            Check.NotNull(query, nameof(query));

            return await query(QueryDb(), state).FirstOrDefaultAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public virtual ValueTask<string?> GetDescriptionAsync(TScope scope, CancellationToken cancellationToken)
        {
            Check.NotNull(scope, nameof(scope));

            return new ValueTask<string?>(scope.Description);
        }

        /// <inheritdoc/>
        public virtual ValueTask<ImmutableDictionary<CultureInfo, string>> GetDescriptionsAsync(TScope scope, CancellationToken cancellationToken)
        {
            Check.NotNull(scope, nameof(scope));

            if (scope.Descriptions is null || scope.Descriptions.Count == 0)
            {
                return new ValueTask<ImmutableDictionary<CultureInfo, string>>(ImmutableDictionary.Create<CultureInfo, string>());
            }

            return new ValueTask<ImmutableDictionary<CultureInfo, string>>(scope.Descriptions.ToImmutableDictionary());
        }

        /// <inheritdoc/>
        public virtual ValueTask<string?> GetDisplayNameAsync(TScope scope, CancellationToken cancellationToken)
        {
            Check.NotNull(scope, nameof(scope));

            return new ValueTask<string?>(scope.DisplayName);
        }

        /// <inheritdoc/>
        public virtual ValueTask<ImmutableDictionary<CultureInfo, string>> GetDisplayNamesAsync(TScope scope, CancellationToken cancellationToken)
        {
            Check.NotNull(scope, nameof(scope));

            if (scope.DisplayNames is null || scope.DisplayNames.Count == 0)
            {
                return new ValueTask<ImmutableDictionary<CultureInfo, string>>(ImmutableDictionary.Create<CultureInfo, string>());
            }

            return new ValueTask<ImmutableDictionary<CultureInfo, string>>(scope.DisplayNames.ToImmutableDictionary());
        }

        /// <inheritdoc/>
        public virtual ValueTask<string?> GetIdAsync(TScope scope, CancellationToken cancellationToken)
        {
            Check.NotNull(scope, nameof(scope));

            return new ValueTask<string?>(scope.Id.ToString());
        }

        /// <inheritdoc/>
        public virtual ValueTask<string?> GetNameAsync(TScope scope, CancellationToken cancellationToken)
        {
            Check.NotNull(scope, nameof(scope));

            return new ValueTask<string?>(scope.Name);
        }

        /// <inheritdoc/>
        public virtual ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(TScope scope, CancellationToken cancellationToken)
        {
            Check.NotNull(scope, nameof(scope));

            if (scope.Properties is null)
            {
                return new ValueTask<ImmutableDictionary<string, JsonElement>>(ImmutableDictionary.Create<string, JsonElement>());
            }

            using var document = JsonDocument.Parse(scope.Properties.ToString(Formatting.None));
            var builder = ImmutableDictionary.CreateBuilder<string, JsonElement>();

            foreach (var property in document.RootElement.EnumerateObject())
            {
                builder[property.Name] = property.Value.Clone();
            }

            return new ValueTask<ImmutableDictionary<string, JsonElement>>(builder.ToImmutable());
        }

        /// <inheritdoc/>
        public virtual ValueTask<ImmutableArray<string>> GetResourcesAsync(TScope scope, CancellationToken cancellationToken)
        {
            Check.NotNull(scope, nameof(scope));

            if (scope.Resources is null || scope.Resources.Count == 0)
            {
                return new ValueTask<ImmutableArray<string>>(ImmutableArray.Create<string>());
            }

            return new ValueTask<ImmutableArray<string>>(scope.Resources.ToImmutableArray());
        }

        /// <inheritdoc/>
        public virtual ValueTask<TScope> InstantiateAsync(CancellationToken cancellationToken)
        {
            try
            {
                return new ValueTask<TScope>(Activator.CreateInstance<TScope>());
            }
            catch (MemberAccessException exception)
            {
                return new ValueTask<TScope>(Task.FromException<TScope>(
                    new InvalidOperationException(SR.GetResourceString(SR.ID0246), exception)));
            }
        }

        /// <inheritdoc/>
        public virtual async IAsyncEnumerable<TScope> ListAsync(
            int? count, int? offset, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var options = new CouchViewOptions<string>
            {
                IncludeDocs = true,
                Limit = count,
                Skip = offset ?? 0
            };

            foreach (var row in await GetDatabase()
                .GetViewAsync(Views.Scope<TScope>.All, options, cancellationToken)
                .ConfigureAwait(false))
            {
                yield return row.Document;
            }
        }

        /// <inheritdoc/>
        public virtual IAsyncEnumerable<TResult> ListAsync<TState, TResult>(
            Func<IQueryable<TScope>, TState, IQueryable<TResult>> query,
            TState state, CancellationToken cancellationToken)
        {
            Check.NotNull(query, nameof(query));

            return ExecuteAsync(cancellationToken);

            async IAsyncEnumerable<TResult> ExecuteAsync([EnumeratorCancellation] CancellationToken cancellationToken)
            {
                foreach (var element in await Task.Run(() => query(QueryDb(), state), cancellationToken))
                {
                    yield return element;
                }
            }
        }

        /// <inheritdoc/>
        public virtual ValueTask SetDescriptionAsync(TScope scope, string? description, CancellationToken cancellationToken)
        {
            Check.NotNull(scope, nameof(scope));

            scope.Description = description;

            return default;
        }

        /// <inheritdoc/>
        public virtual ValueTask SetDescriptionsAsync(TScope scope,
            ImmutableDictionary<CultureInfo, string> descriptions, CancellationToken cancellationToken)
        {
            Check.NotNull(scope, nameof(scope));

            scope.Descriptions = descriptions;

            return default;
        }

        /// <inheritdoc/>
        public virtual ValueTask SetDisplayNamesAsync(TScope scope,
            ImmutableDictionary<CultureInfo, string> names, CancellationToken cancellationToken)
        {
            Check.NotNull(scope, nameof(scope));

            scope.DisplayNames = names;

            return default;
        }

        /// <inheritdoc/>
        public virtual ValueTask SetDisplayNameAsync(TScope scope, string? name, CancellationToken cancellationToken)
        {
            Check.NotNull(scope, nameof(scope));

            scope.DisplayName = name;

            return default;
        }

        /// <inheritdoc/>
        public virtual ValueTask SetNameAsync(TScope scope, string? name, CancellationToken cancellationToken)
        {
            Check.NotNull(scope, nameof(scope));

            scope.Name = name;

            return default;
        }

        /// <inheritdoc/>
        public virtual ValueTask SetPropertiesAsync(TScope scope,
            ImmutableDictionary<string, JsonElement> properties, CancellationToken cancellationToken)
        {
            Check.NotNull(scope, nameof(scope));

            if (properties is null || properties.IsEmpty)
            {
                scope.Properties = null;

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

            scope.Properties = JObject.Parse(Encoding.UTF8.GetString(stream.ToArray()));

            return default;
        }

        /// <inheritdoc/>
        public virtual ValueTask SetResourcesAsync(TScope scope, ImmutableArray<string> resources, CancellationToken cancellationToken)
        {
            Check.NotNull(scope, nameof(scope));

            if (resources.IsDefaultOrEmpty)
            {
                scope.Resources = ImmutableList.Create<string>();

                return default;
            }

            scope.Resources = resources.ToImmutableList();

            return default;
        }

        /// <inheritdoc/>
        public virtual async ValueTask UpdateAsync(TScope scope, CancellationToken cancellationToken)
        {
            Check.NotNull(scope, nameof(scope));

            var db = GetDatabase();
            try
            {
                await db.AddOrUpdateAsync(scope, cancellationToken: cancellationToken);
            }
            catch (CouchConflictException ex)
            {
                throw new OpenIddictExceptions.ConcurrencyException(SR.GetResourceString(SR.ID0245), ex);
            }
        }
    }
}