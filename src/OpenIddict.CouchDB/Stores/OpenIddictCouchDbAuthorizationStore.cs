﻿/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */

using CouchDB.Driver;
using CouchDB.Driver.Exceptions;
using CouchDB.Driver.Extensions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenIddict.Abstractions;
using OpenIddict.CouchDB.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
    /// Provides methods allowing to manage the authorizations stored in a database.
    /// </summary>
    /// <typeparam name="TAuthorization">The type of the Authorization entity.</typeparam>
    public class OpenIddictCouchDbAuthorizationStore<TAuthorization> : BaseOpenIddictCouchDbStore<TAuthorization>, IOpenIddictAuthorizationStore<TAuthorization>
        where TAuthorization : OpenIddictCouchDbAuthorization
    {
        public OpenIddictCouchDbAuthorizationStore(
            IServiceProvider provider,
            IOptionsMonitor<OpenIddictCouchDbOptions> options)
            : base(provider, options)
        {
        }

        protected override string Discriminator => Options.CurrentValue.AuthorizationDiscriminator;

        /// <inheritdoc/>
        public virtual async ValueTask<long> CountAsync(CancellationToken cancellationToken)
        {
            var db = GetDatabase();
            var (design, view) = OpenIddictCouchDbViews.Authorization.Count;

            return (await db.GetViewAsync<TAuthorization, int>(design, view, cancellationToken: cancellationToken))
                .Rows.FirstOrDefault()?.Value ?? 0;
        }

        /// <inheritdoc/>
        public virtual ValueTask<long> CountAsync<TResult>(
            Func<IQueryable<TAuthorization>, IQueryable<TResult>> query, CancellationToken cancellationToken)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var db = GetDatabase();

            return new(query(QueryDb(db)).LongCount());
        }

        /// <inheritdoc/>
        public virtual async ValueTask CreateAsync(TAuthorization authorization, CancellationToken cancellationToken)
        {
            if (authorization is null)
            {
                throw new ArgumentNullException(nameof(authorization));
            }
            authorization.Discriminator = Discriminator;

            var db = GetDatabase();

            await db.AddAsync(authorization, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async ValueTask DeleteAsync(TAuthorization authorization, CancellationToken cancellationToken)
        {
            if (authorization is null)
            {
                throw new ArgumentNullException(nameof(authorization));
            }

            var db = GetDatabase();

            try
            {
                await db.RemoveAsync(authorization, cancellationToken: cancellationToken);
            }
            catch (CouchConflictException ex)
            {
                throw new OpenIddictExceptions.ConcurrencyException(SR.GetResourceString(SR.ID0239), ex);
            }

            var deleteDb = GetDatabase<CouchDocumentDelete>();

            // Delete the tokens associated with the authorization.
            var (design, view) = OpenIddictCouchDbViews.Token.AuthorizationId;
            var tokens = await db.GetViewAsync<TAuthorization, string, CouchDocumentDelete>(design, view);
            await deleteDb.AddOrUpdateRangeAsync(tokens.Rows.Select(x => new CouchDocumentDelete(x.Id, x.Doc.Rev)).ToArray());
        }

        /// <inheritdoc/>
        public virtual IAsyncEnumerable<TAuthorization> FindAsync(
            string subject, string client, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(subject))
            {
                throw new ArgumentException(SR.GetResourceString(SR.ID0198), nameof(subject));
            }

            if (string.IsNullOrEmpty(client))
            {
                throw new ArgumentException(SR.GetResourceString(SR.ID0124), nameof(client));
            }

            return ExecuteAsync(cancellationToken);

            async IAsyncEnumerable<TAuthorization> ExecuteAsync([EnumeratorCancellation] CancellationToken cancellationToken)
            {
                var db = GetDatabase();

                foreach (var authorization in await QueryDb(db).Where(authorization =>
                     authorization.Subject == subject &&
                     authorization.ApplicationId == client).ToCouchListAsync(cancellationToken))
                {
                    yield return authorization;
                }
            }
        }

        /// <inheritdoc/>
        public virtual IAsyncEnumerable<TAuthorization> FindAsync(
            string subject, string client, string status,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(subject))
            {
                throw new ArgumentException(SR.GetResourceString(SR.ID0198), nameof(subject));
            }

            if (string.IsNullOrEmpty(client))
            {
                throw new ArgumentException(SR.GetResourceString(SR.ID0124), nameof(client));
            }

            if (string.IsNullOrEmpty(status))
            {
                throw new ArgumentException(SR.GetResourceString(SR.ID0199), nameof(status));
            }

            return ExecuteAsync(cancellationToken);

            async IAsyncEnumerable<TAuthorization> ExecuteAsync([EnumeratorCancellation] CancellationToken cancellationToken)
            {
                var db = GetDatabase();

                foreach (var authorization in await QueryDb(db).Where(authorization =>
                    authorization.Subject == subject &&
                    authorization.ApplicationId == client &&
                    authorization.Status == status).ToCouchListAsync(cancellationToken))
                {
                    yield return authorization;
                }
            }
        }

        /// <inheritdoc/>
        public virtual IAsyncEnumerable<TAuthorization> FindAsync(
            string subject, string client, string status, string type,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(subject))
            {
                throw new ArgumentException(SR.GetResourceString(SR.ID0198), nameof(subject));
            }

            if (string.IsNullOrEmpty(client))
            {
                throw new ArgumentException(SR.GetResourceString(SR.ID0124), nameof(client));
            }

            if (string.IsNullOrEmpty(status))
            {
                throw new ArgumentException(SR.GetResourceString(SR.ID0199), nameof(status));
            }

            if (string.IsNullOrEmpty(type))
            {
                throw new ArgumentException(SR.GetResourceString(SR.ID0200), nameof(type));
            }

            return ExecuteAsync(cancellationToken);

            async IAsyncEnumerable<TAuthorization> ExecuteAsync([EnumeratorCancellation] CancellationToken cancellationToken)
            {
                var db = GetDatabase();

                foreach (var authorization in await QueryDb(db).Where(authorization =>
                    authorization.Subject == subject &&
                    authorization.ApplicationId == client &&
                    authorization.Status == status &&
                    authorization.Type == type).ToCouchListAsync(cancellationToken))
                {
                    yield return authorization;
                }
            }
        }

        /// <inheritdoc/>
        public virtual IAsyncEnumerable<TAuthorization> FindAsync(
            string subject, string client, string status, string type, ImmutableArray<string> scopes,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(subject))
            {
                throw new ArgumentException(SR.GetResourceString(SR.ID0198), nameof(subject));
            }

            if (string.IsNullOrEmpty(client))
            {
                throw new ArgumentException(SR.GetResourceString(SR.ID0124), nameof(client));
            }

            if (string.IsNullOrEmpty(status))
            {
                throw new ArgumentException(SR.GetResourceString(SR.ID0199), nameof(status));
            }

            if (string.IsNullOrEmpty(type))
            {
                throw new ArgumentException(SR.GetResourceString(SR.ID0200), nameof(type));
            }

            return ExecuteAsync(cancellationToken);

            async IAsyncEnumerable<TAuthorization> ExecuteAsync([EnumeratorCancellation] CancellationToken cancellationToken)
            {
                var db = GetDatabase();

                // Note: Enumerable.All() is deliberately used without the extension method syntax to ensure
                // ImmutableArrayExtensions.All() (which is not supported by MongoDB) is not used instead.
                foreach (var authorization in await QueryDb(db).Where(authorization =>
                    authorization.Subject == subject &&
                    authorization.ApplicationId == client &&
                    authorization.Status == status &&
                    authorization.Type == type &&
                    //authorization.Scopes.All(scope => authorization.Scopes.Contains(scope))).ToAsyncEnumerable(cancellationToken))
                    Enumerable.All(scopes, scope => authorization.Scopes.Contains(scope))).ToCouchListAsync(cancellationToken))
                {
                    yield return authorization;
                }
            }
        }

        /// <inheritdoc/>
        public virtual IAsyncEnumerable<TAuthorization> FindByApplicationIdAsync(
            string identifier, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                throw new ArgumentException(SR.GetResourceString(SR.ID0195), nameof(identifier));
            }

            return ExecuteAsync(cancellationToken);

            async IAsyncEnumerable<TAuthorization> ExecuteAsync([EnumeratorCancellation] CancellationToken cancellationToken)
            {
                var db = GetDatabase();
                var (view, design) = OpenIddictCouchDbViews.Authorization.ApplicationId;
                var result = await db.GetViewAsync<TAuthorization, string, TAuthorization>(view, design, new()
                {
                    Key = identifier
                });

                foreach (var authorization in result.Rows)
                {
                    yield return authorization.Doc;
                }
            }
        }

        /// <inheritdoc/>
        public virtual async ValueTask<TAuthorization?> FindByIdAsync(string identifier, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                throw new ArgumentException(SR.GetResourceString(SR.ID0195), nameof(identifier));
            }

            var db = GetDatabase();
            return await db.FindAsync(identifier, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public virtual IAsyncEnumerable<TAuthorization> FindBySubjectAsync(
            string subject, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(subject))
            {
                throw new ArgumentException(SR.GetResourceString(SR.ID0198), nameof(subject));
            }

            return ExecuteAsync(cancellationToken);

            async IAsyncEnumerable<TAuthorization> ExecuteAsync([EnumeratorCancellation] CancellationToken cancellationToken)
            {
                var db = GetDatabase();
                var (view, design) = OpenIddictCouchDbViews.Authorization.Subject;
                var result = await db.GetViewAsync<TAuthorization, string, TAuthorization>(view, design, new()
                {
                    Key = subject
                });

                foreach (var authorization in result.Rows)
                {
                    yield return authorization.Doc;
                }
            }
        }

        /// <inheritdoc/>
        public virtual ValueTask<string?> GetApplicationIdAsync(TAuthorization authorization, CancellationToken cancellationToken)
        {
            if (authorization is null)
            {
                throw new ArgumentNullException(nameof(authorization));
            }

            if (string.IsNullOrEmpty(authorization.ApplicationId))
            {
                return new ValueTask<string?>(result: null);
            }

            return new ValueTask<string?>(authorization.ApplicationId.ToString());
        }

        /// <inheritdoc/>
        public virtual async ValueTask<TResult> GetAsync<TState, TResult>(
            Func<IQueryable<TAuthorization>, TState, IQueryable<TResult>> query,
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
        public virtual ValueTask<DateTimeOffset?> GetCreationDateAsync(TAuthorization authorization, CancellationToken cancellationToken)
        {
            if (authorization is null)
            {
                throw new ArgumentNullException(nameof(authorization));
            }

            if (authorization.CreationDate is null)
            {
                return new ValueTask<DateTimeOffset?>(result: null);
            }

            return new ValueTask<DateTimeOffset?>(DateTime.SpecifyKind(authorization.CreationDate.Value, DateTimeKind.Utc));
        }

        /// <inheritdoc/>
        public virtual ValueTask<string?> GetIdAsync(TAuthorization authorization, CancellationToken cancellationToken)
        {
            if (authorization is null)
            {
                throw new ArgumentNullException(nameof(authorization));
            }

            return new ValueTask<string?>(authorization.Id.ToString());
        }

        /// <inheritdoc/>
        public virtual ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(TAuthorization authorization, CancellationToken cancellationToken)
        {
            if (authorization is null)
            {
                throw new ArgumentNullException(nameof(authorization));
            }

            if (authorization.Properties is null)
            {
                return new ValueTask<ImmutableDictionary<string, JsonElement>>(ImmutableDictionary.Create<string, JsonElement>());
            }

            using var document = JsonDocument.Parse(authorization.Properties.ToString(Formatting.None));
            var builder = ImmutableDictionary.CreateBuilder<string, JsonElement>();

            foreach (var property in document.RootElement.EnumerateObject())
            {
                builder[property.Name] = property.Value.Clone();
            }

            return new ValueTask<ImmutableDictionary<string, JsonElement>>(builder.ToImmutable());
        }

        /// <inheritdoc/>
        public virtual ValueTask<ImmutableArray<string>> GetScopesAsync(TAuthorization authorization, CancellationToken cancellationToken)
        {
            if (authorization is null)
            {
                throw new ArgumentNullException(nameof(authorization));
            }

            if (authorization.Scopes is null || authorization.Scopes.Count == 0)
            {
                return new ValueTask<ImmutableArray<string>>(ImmutableArray.Create<string>());
            }

            return new ValueTask<ImmutableArray<string>>(authorization.Scopes.ToImmutableArray());
        }

        /// <inheritdoc/>
        public virtual ValueTask<string?> GetStatusAsync(TAuthorization authorization, CancellationToken cancellationToken)
        {
            if (authorization is null)
            {
                throw new ArgumentNullException(nameof(authorization));
            }

            return new ValueTask<string?>(authorization.Status);
        }

        /// <inheritdoc/>
        public virtual ValueTask<string?> GetSubjectAsync(TAuthorization authorization, CancellationToken cancellationToken)
        {
            if (authorization is null)
            {
                throw new ArgumentNullException(nameof(authorization));
            }

            return new ValueTask<string?>(authorization.Subject);
        }

        /// <inheritdoc/>
        public virtual ValueTask<string?> GetTypeAsync(TAuthorization authorization, CancellationToken cancellationToken)
        {
            if (authorization is null)
            {
                throw new ArgumentNullException(nameof(authorization));
            }

            return new ValueTask<string?>(authorization.Type);
        }

        /// <inheritdoc/>
        public virtual ValueTask<TAuthorization> InstantiateAsync(CancellationToken cancellationToken)
        {
            try
            {
                return new ValueTask<TAuthorization>(Activator.CreateInstance<TAuthorization>());
            }
            catch (MemberAccessException exception)
            {
                return new ValueTask<TAuthorization>(Task.FromException<TAuthorization>(
                    new InvalidOperationException(SR.GetResourceString(SR.ID0242), exception)));
            }
        }

        /// <inheritdoc/>
        public virtual async IAsyncEnumerable<TAuthorization> ListAsync(
            int? count, int? offset, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var db = GetDatabase();
            var (design, view) = OpenIddictCouchDbViews.Authorization.All;
            var result = await db.GetViewAsync<TAuthorization, int, TAuthorization>(design, view, cancellationToken: cancellationToken, options: new()
            {
                Limit = count,
                Skip = offset
            });

            foreach (var authorization in result.Rows)
            {
                yield return authorization.Doc;
            }
        }

        /// <inheritdoc/>
        public virtual IAsyncEnumerable<TResult> ListAsync<TState, TResult>(
            Func<IQueryable<TAuthorization>, TState, IQueryable<TResult>> query,
            TState state, CancellationToken cancellationToken)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return ExecuteAsync(cancellationToken);

            async IAsyncEnumerable<TResult> ExecuteAsync([EnumeratorCancellation] CancellationToken cancellationToken)
            {
                // todo:  check
                //foreach (var element in await query(QueryDb(db), state).ToCouchListAsync(cancellationToken))
                foreach (var element in await Task.Run(() => query(QueryDb(), state)))
                {
                    yield return element;
                }
            }
        }

        /// <inheritdoc/>
        public virtual async ValueTask PruneAsync(DateTimeOffset threshold, CancellationToken cancellationToken)
        {
            // todo: prune
            var db = GetDatabase();
            //var database = await Context.GetDatabaseAsync(cancellationToken);
            //var collection = database.GetCollection<TAuthorization>(Options.CurrentValue.AuthorizationsCollectionName);

            //// Note: directly deleting the resulting set of an aggregate query is not supported by MongoDB.
            //// To work around this limitation, the authorization identifiers are stored in an intermediate
            //// list and delete requests are sent to remove the documents corresponding to these identifiers.

            //var identifiers =
            //    await (from authorization in collection.AsQueryable()
            //           join token in database.GetCollection<OpenIddictMongoDbToken>(Options.CurrentValue.TokensCollectionName).AsQueryable()
            //                      on authorization.Id equals token.AuthorizationId into tokens
            //           where authorization.CreationDate < threshold.UtcDateTime
            //           where authorization.Status != Statuses.Valid ||
            //                (authorization.Type == AuthorizationTypes.AdHoc && !tokens.Any())
            //           select authorization.Id).ToListAsync(cancellationToken);

            //// Note: to avoid generating delete requests with very large filters, a buffer is used here and the
            //// maximum number of elements that can be removed by a single call to PruneAsync() is limited to 50000.
            //foreach (var buffer in Buffer(identifiers.Take(50_000), 1_000))
            //{
            //    await collection.DeleteManyAsync(authorization => buffer.Contains(authorization.Id), cancellationToken);
            //}

            //static IEnumerable<List<TSource>> Buffer<TSource>(IEnumerable<TSource> source, int count)
            //{
            //    List<TSource>? buffer = null;

            //    foreach (var element in source)
            //    {
            //        buffer ??= new List<TSource>(capacity: 1);
            //        buffer.Add(element);

            //        if (buffer.Count == count)
            //        {
            //            yield return buffer;

            //            buffer = null;
            //        }
            //    }

            //    if (buffer is not null)
            //    {
            //        yield return buffer;
            //    }
            //}
        }

        /// <inheritdoc/>
        public virtual ValueTask SetApplicationIdAsync(TAuthorization authorization,
            string? identifier, CancellationToken cancellationToken)
        {
            if (authorization is null)
            {
                throw new ArgumentNullException(nameof(authorization));
            }

            if (!string.IsNullOrEmpty(identifier))
            {
                authorization.ApplicationId = identifier;
            }
            else
            {
                authorization.ApplicationId = string.Empty;
            }

            return default;
        }

        /// <inheritdoc/>
        public virtual ValueTask SetCreationDateAsync(TAuthorization authorization,
            DateTimeOffset? date, CancellationToken cancellationToken)
        {
            if (authorization is null)
            {
                throw new ArgumentNullException(nameof(authorization));
            }

            authorization.CreationDate = date?.UtcDateTime;

            return default;
        }

        /// <inheritdoc/>
        public virtual ValueTask SetPropertiesAsync(TAuthorization authorization,
            ImmutableDictionary<string, JsonElement> properties, CancellationToken cancellationToken)
        {
            if (authorization is null)
            {
                throw new ArgumentNullException(nameof(authorization));
            }

            if (properties is null || properties.IsEmpty)
            {
                authorization.Properties = null;

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

            authorization.Properties = JObject.Parse(Encoding.UTF8.GetString(stream.ToArray()));

            return default;
        }

        /// <inheritdoc/>
        public virtual ValueTask SetScopesAsync(TAuthorization authorization,
            ImmutableArray<string> scopes, CancellationToken cancellationToken)
        {
            if (authorization is null)
            {
                throw new ArgumentNullException(nameof(authorization));
            }

            if (scopes.IsDefaultOrEmpty)
            {
                authorization.Scopes = ImmutableList.Create<string>();

                return default;
            }

            authorization.Scopes = scopes.ToImmutableList();

            return default;
        }

        /// <inheritdoc/>
        public virtual ValueTask SetStatusAsync(TAuthorization authorization, string? status, CancellationToken cancellationToken)
        {
            if (authorization is null)
            {
                throw new ArgumentNullException(nameof(authorization));
            }

            authorization.Status = status;

            return default;
        }

        /// <inheritdoc/>
        public virtual ValueTask SetSubjectAsync(TAuthorization authorization, string? subject, CancellationToken cancellationToken)
        {
            if (authorization is null)
            {
                throw new ArgumentNullException(nameof(authorization));
            }

            authorization.Subject = subject;

            return default;
        }

        /// <inheritdoc/>
        public virtual ValueTask SetTypeAsync(TAuthorization authorization, string? type, CancellationToken cancellationToken)
        {
            if (authorization is null)
            {
                throw new ArgumentNullException(nameof(authorization));
            }

            authorization.Type = type;

            return default;
        }

        /// <inheritdoc/>
        public virtual async ValueTask UpdateAsync(TAuthorization authorization, CancellationToken cancellationToken)
        {
            if (authorization is null)
            {
                throw new ArgumentNullException(nameof(authorization));
            }

            var db = GetDatabase();
            try
            {
                await db.AddOrUpdateAsync(authorization, cancellationToken: cancellationToken);
            }
            catch (CouchConflictException ex)
            {
                throw new OpenIddictExceptions.ConcurrencyException(SR.GetResourceString(SR.ID0241), ex);
            }
        }
    }
}