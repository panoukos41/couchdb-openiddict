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
    /// Provides methods allowing to manage the tokens stored in a database.
    /// </summary>
    /// <typeparam name="TToken">The type of the Token entity.</typeparam>
    public class OpenIddictCouchDbTokenStore<TToken> : BaseOpenIddictCouchDbStore<TToken>, IOpenIddictTokenStore<TToken>
        where TToken : OpenIddictCouchDbToken
    {
        public OpenIddictCouchDbTokenStore(
            IServiceProvider provider,
            IOptionsMonitor<OpenIddictCouchDbOptions> options)
            : base(provider, options)
        {
        }

        protected override string Discriminator => Options.CurrentValue.TokenDiscriminator;

        /// <inheritdoc/>
        public virtual async ValueTask<long> CountAsync(CancellationToken cancellationToken)
        {
            var db = GetDatabase();
            var (design, view) = OpenIddictCouchDbViews.Token.Count;
            return (await db.GetViewAsync<TToken, int>(design, view, cancellationToken: cancellationToken))
                .Rows.FirstOrDefault()?.Value ?? 0;
        }

        /// <inheritdoc/>
        public ValueTask<long> CountAsync<TResult>(
            Func<IQueryable<TToken>, IQueryable<TResult>> query, CancellationToken cancellationToken)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var db = GetDatabase();

            return new(query(QueryDb(db)).LongCount());
        }

        /// <inheritdoc/>
        public virtual async ValueTask CreateAsync(TToken token, CancellationToken cancellationToken)
        {
            if (token is null)
            {
                throw new ArgumentNullException(nameof(token));
            }
            token.Discriminator = Discriminator;

            var db = GetDatabase();

            await db.AddAsync(token, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async ValueTask DeleteAsync(TToken token, CancellationToken cancellationToken)
        {
            if (token is null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            var db = GetDatabase();

            try
            {
                await db.RemoveAsync(token, cancellationToken: cancellationToken);
            }
            catch (CouchConflictException ex)
            {
                throw new OpenIddictExceptions.ConcurrencyException(SR.GetResourceString(SR.ID0247), ex);
            }
        }

        /// <inheritdoc/>
        public virtual IAsyncEnumerable<TToken> FindAsync(string subject,
            string client, CancellationToken cancellationToken)
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

            async IAsyncEnumerable<TToken> ExecuteAsync([EnumeratorCancellation] CancellationToken cancellationToken)
            {
                var db = GetDatabase();

                foreach (var token in await QueryDb(db).Where(token =>
                    token.ApplicationId == client &&
                    token.Subject == subject).ToCouchListAsync(cancellationToken))
                {
                    yield return token;
                }
            }
        }

        /// <inheritdoc/>
        public virtual IAsyncEnumerable<TToken> FindAsync(
            string subject, string client,
            string status, CancellationToken cancellationToken)
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

            async IAsyncEnumerable<TToken> ExecuteAsync([EnumeratorCancellation] CancellationToken cancellationToken)
            {
                var db = GetDatabase();

                foreach (var token in await QueryDb(db).Where(token =>
                    token.ApplicationId == client &&
                    token.Subject == subject &&
                    token.Status == status).ToCouchListAsync(cancellationToken))
                {
                    yield return token;
                }
            }
        }

        /// <inheritdoc/>
        public virtual IAsyncEnumerable<TToken> FindAsync(
            string subject, string client,
            string status, string type, CancellationToken cancellationToken)
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

            async IAsyncEnumerable<TToken> ExecuteAsync([EnumeratorCancellation] CancellationToken cancellationToken)
            {
                var db = GetDatabase();

                foreach (var token in await QueryDb(db).Where(token =>
                    token.ApplicationId == client &&
                    token.Subject == subject &&
                    token.Status == status &&
                    token.Type == type).ToCouchListAsync(cancellationToken))
                {
                    yield return token;
                }
            }
        }

        /// <inheritdoc/>
        public virtual IAsyncEnumerable<TToken> FindByApplicationIdAsync(string identifier, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                throw new ArgumentException(SR.GetResourceString(SR.ID0195), nameof(identifier));
            }

            return ExecuteAsync(cancellationToken);

            async IAsyncEnumerable<TToken> ExecuteAsync([EnumeratorCancellation] CancellationToken cancellationToken)
            {
                var db = GetDatabase();

                foreach (var token in await QueryDb(db).Where(token =>
                    token.ApplicationId == identifier).ToCouchListAsync(cancellationToken))
                {
                    yield return token;
                }
            }
        }

        /// <inheritdoc/>
        public virtual IAsyncEnumerable<TToken> FindByAuthorizationIdAsync(string identifier, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                throw new ArgumentException(SR.GetResourceString(SR.ID0195), nameof(identifier));
            }

            return ExecuteAsync(cancellationToken);

            async IAsyncEnumerable<TToken> ExecuteAsync([EnumeratorCancellation] CancellationToken cancellationToken)
            {
                var db = GetDatabase();

                foreach (var token in await QueryDb(db).Where(token =>
                    token.AuthorizationId == identifier).ToCouchListAsync(cancellationToken))
                {
                    yield return token;
                }
            }
        }

        /// <inheritdoc/>
        public virtual async ValueTask<TToken?> FindByIdAsync(string identifier, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                throw new ArgumentException(SR.GetResourceString(SR.ID0195), nameof(identifier));
            }

            var db = GetDatabase();

            return await db.FindAsync(identifier, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async ValueTask<TToken?> FindByReferenceIdAsync(string identifier, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                throw new ArgumentException(SR.GetResourceString(SR.ID0195), nameof(identifier));
            }

            var db = GetDatabase();

            return (await QueryDb(db)
                .Where(x => x.ReferenceId == identifier)
                .Take(1)
                .ToCouchListAsync(cancellationToken))
                .FirstOrDefault();
        }

        /// <inheritdoc/>
        public virtual IAsyncEnumerable<TToken> FindBySubjectAsync(string subject, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(subject))
            {
                throw new ArgumentException(SR.GetResourceString(SR.ID0198), nameof(subject));
            }

            return ExecuteAsync(cancellationToken);

            async IAsyncEnumerable<TToken> ExecuteAsync([EnumeratorCancellation] CancellationToken cancellationToken)
            {
                var db = GetDatabase();

                foreach (var token in await QueryDb(db).Where(token =>
                    token.Subject == subject).ToCouchListAsync(cancellationToken))
                {
                    yield return token;
                }
            }
        }

        /// <inheritdoc/>
        public virtual ValueTask<string?> GetApplicationIdAsync(TToken token, CancellationToken cancellationToken)
        {
            if (token is null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (string.IsNullOrEmpty(token.ApplicationId))
            {
                return new ValueTask<string?>(result: null);
            }

            return new ValueTask<string?>(token.ApplicationId.ToString());
        }

        /// <inheritdoc/>
        public virtual async ValueTask<TResult> GetAsync<TState, TResult>(
            Func<IQueryable<TToken>, TState, IQueryable<TResult>> query,
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
        public virtual ValueTask<string?> GetAuthorizationIdAsync(TToken token, CancellationToken cancellationToken)
        {
            if (token is null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (token.AuthorizationId == string.Empty)
            {
                return new ValueTask<string?>(result: null);
            }

            return new ValueTask<string?>(token.AuthorizationId.ToString());
        }

        /// <inheritdoc/>
        public virtual ValueTask<DateTimeOffset?> GetCreationDateAsync(TToken token, CancellationToken cancellationToken)
        {
            if (token is null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (token.CreationDate is null)
            {
                return new ValueTask<DateTimeOffset?>(result: null);
            }

            return new ValueTask<DateTimeOffset?>(DateTime.SpecifyKind(token.CreationDate.Value, DateTimeKind.Utc));
        }

        /// <inheritdoc/>
        public virtual ValueTask<DateTimeOffset?> GetExpirationDateAsync(TToken token, CancellationToken cancellationToken)
        {
            if (token is null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (token.ExpirationDate is null)
            {
                return new ValueTask<DateTimeOffset?>(result: null);
            }

            return new ValueTask<DateTimeOffset?>(DateTime.SpecifyKind(token.ExpirationDate.Value, DateTimeKind.Utc));
        }

        /// <inheritdoc/>
        public virtual ValueTask<string?> GetIdAsync(TToken token, CancellationToken cancellationToken)
        {
            if (token is null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            return new ValueTask<string?>(token.Id.ToString());
        }

        /// <inheritdoc/>
        public virtual ValueTask<string?> GetPayloadAsync(TToken token, CancellationToken cancellationToken)
        {
            if (token is null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            return new ValueTask<string?>(token.Payload);
        }

        /// <inheritdoc/>
        public virtual ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(TToken token, CancellationToken cancellationToken)
        {
            if (token is null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (token.Properties is null)
            {
                return new ValueTask<ImmutableDictionary<string, JsonElement>>(ImmutableDictionary.Create<string, JsonElement>());
            }

            using var document = JsonDocument.Parse(token.Properties.ToString(Formatting.None));
            var builder = ImmutableDictionary.CreateBuilder<string, JsonElement>();

            foreach (var property in document.RootElement.EnumerateObject())
            {
                builder[property.Name] = property.Value.Clone();
            }

            return new ValueTask<ImmutableDictionary<string, JsonElement>>(builder.ToImmutable());
        }

        /// <inheritdoc/>
        public virtual ValueTask<DateTimeOffset?> GetRedemptionDateAsync(TToken token, CancellationToken cancellationToken)
        {
            if (token is null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (token.RedemptionDate is null)
            {
                return new ValueTask<DateTimeOffset?>(result: null);
            }

            return new ValueTask<DateTimeOffset?>(DateTime.SpecifyKind(token.RedemptionDate.Value, DateTimeKind.Utc));
        }

        /// <inheritdoc/>
        public virtual ValueTask<string?> GetReferenceIdAsync(TToken token, CancellationToken cancellationToken)
        {
            if (token is null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            return new ValueTask<string?>(token.ReferenceId);
        }

        /// <inheritdoc/>
        public virtual ValueTask<string?> GetStatusAsync(TToken token, CancellationToken cancellationToken)
        {
            if (token is null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            return new ValueTask<string?>(token.Status);
        }

        /// <inheritdoc/>
        public virtual ValueTask<string?> GetSubjectAsync(TToken token, CancellationToken cancellationToken)
        {
            if (token is null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            return new ValueTask<string?>(token.Subject);
        }

        /// <inheritdoc/>
        public virtual ValueTask<string?> GetTypeAsync(TToken token, CancellationToken cancellationToken)
        {
            if (token is null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            return new ValueTask<string?>(token.Type);
        }

        /// <inheritdoc/>
        public virtual ValueTask<TToken> InstantiateAsync(CancellationToken cancellationToken)
        {
            try
            {
                return new ValueTask<TToken>(Activator.CreateInstance<TToken>());
            }
            catch (MemberAccessException exception)
            {
                return new ValueTask<TToken>(Task.FromException<TToken>(
                    new InvalidOperationException(SR.GetResourceString(SR.ID0248), exception)));
            }
        }

        /// <inheritdoc/>
        public virtual async IAsyncEnumerable<TToken> ListAsync(
            int? count, int? offset, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var db = GetDatabase();
            var (design, view) = OpenIddictCouchDbViews.Token.All;
            var result = await db.GetViewAsync<TToken, int, TToken>(design, view, cancellationToken: cancellationToken, options: new()
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
            Func<IQueryable<TToken>, TState, IQueryable<TResult>> query,
            TState state, CancellationToken cancellationToken)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return ExecuteAsync(cancellationToken);

            async IAsyncEnumerable<TResult> ExecuteAsync([EnumeratorCancellation] CancellationToken cancellationToken)
            {
                foreach (var element in await Task.Run(() => query(QueryDb(), state)))
                {
                    yield return element;
                }
            }
        }

        /// <inheritdoc/>
        public virtual async ValueTask PruneAsync(DateTimeOffset threshold, CancellationToken cancellationToken)
        {
            // todo: Prune
            var db = GetDatabase<CouchDocumentDelete>();
            var (design, view) = OpenIddictCouchDbViews.Token.Prune;
            var result = await db.GetViewAsync<CouchDocumentDelete, string, CouchDocumentEmpty>(design, view, cancellationToken: cancellationToken);

            await db.AddOrUpdateRangeAsync(result.Rows.Select(x => new CouchDocumentDelete(x.Id, x.Doc.Rev)).ToArray(), cancellationToken);

            //var database = await Context.GetDatabaseAsync(cancellationToken);
            //var collection = database.GetCollection<TToken>(Options.CurrentValue.TokensCollectionName);

            //// Note: directly deleting the resulting set of an aggregate query is not supported by MongoDB.
            //// To work around this limitation, the token identifiers are stored in an intermediate list
            //// and delete requests are sent to remove the documents corresponding to these identifiers.

            //var identifiers =
            //    await (from token in collection.AsQueryable()
            //           join authorization in database.GetCollection<OpenIddictMongoDbAuthorization>(Options.CurrentValue.AuthorizationsCollectionName).AsQueryable()
            //                              on token.AuthorizationId equals authorization.Id into authorizations
            //           where token.CreationDate < threshold.UtcDateTime
            //           where (token.Status != Statuses.Inactive && token.Status != Statuses.Valid) ||
            //                  token.ExpirationDate < DateTime.UtcNow ||
            //                  authorizations.Any(authorization => authorization.Status != Statuses.Valid)
            //           select token.Id).ToListAsync(cancellationToken);

            //// Note: to avoid generating delete requests with very large filters, a buffer is used here and the
            //// maximum number of elements that can be removed by a single call to PruneAsync() is limited to 50000.
            //foreach (var buffer in Buffer(identifiers.Take(50_000), 1_000))
            //{
            //    await collection.DeleteManyAsync(token => buffer.Contains(token.Id), cancellationToken);
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
        public virtual ValueTask SetApplicationIdAsync(TToken token, string? identifier, CancellationToken cancellationToken)
        {
            if (token is null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (!string.IsNullOrEmpty(identifier))
            {
                token.ApplicationId = identifier;
            }
            else
            {
                token.ApplicationId = string.Empty;
            }

            return default;
        }

        /// <inheritdoc/>
        public virtual ValueTask SetAuthorizationIdAsync(TToken token, string? identifier, CancellationToken cancellationToken)
        {
            if (token is null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (!string.IsNullOrEmpty(identifier))
            {
                token.AuthorizationId = identifier;
            }
            else
            {
                token.AuthorizationId = string.Empty;
            }

            return default;
        }

        /// <inheritdoc/>
        public virtual ValueTask SetCreationDateAsync(TToken token, DateTimeOffset? date, CancellationToken cancellationToken)
        {
            if (token is null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            token.CreationDate = date?.UtcDateTime;

            return default;
        }

        /// <inheritdoc/>
        public virtual ValueTask SetExpirationDateAsync(TToken token, DateTimeOffset? date, CancellationToken cancellationToken)
        {
            if (token is null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            token.ExpirationDate = date?.UtcDateTime;

            return default;
        }

        /// <inheritdoc/>
        public virtual ValueTask SetPayloadAsync(TToken token, string? payload, CancellationToken cancellationToken)
        {
            if (token is null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            token.Payload = payload;

            return default;
        }

        /// <inheritdoc/>
        public virtual ValueTask SetPropertiesAsync(TToken token,
            ImmutableDictionary<string, JsonElement> properties, CancellationToken cancellationToken)
        {
            if (token is null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (properties is null || properties.IsEmpty)
            {
                token.Properties = null;

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

            token.Properties = JObject.Parse(Encoding.UTF8.GetString(stream.ToArray()));

            return default;
        }

        /// <inheritdoc/>
        public virtual ValueTask SetRedemptionDateAsync(TToken token, DateTimeOffset? date, CancellationToken cancellationToken)
        {
            if (token is null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            token.RedemptionDate = date?.UtcDateTime;

            return default;
        }

        /// <inheritdoc/>
        public virtual ValueTask SetReferenceIdAsync(TToken token, string? identifier, CancellationToken cancellationToken)
        {
            if (token is null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            token.ReferenceId = identifier;

            return default;
        }

        /// <inheritdoc/>
        public virtual ValueTask SetStatusAsync(TToken token, string? status, CancellationToken cancellationToken)
        {
            if (token is null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            token.Status = status;

            return default;
        }

        /// <inheritdoc/>
        public virtual ValueTask SetSubjectAsync(TToken token, string? subject, CancellationToken cancellationToken)
        {
            if (token is null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            token.Subject = subject;

            return default;
        }

        /// <inheritdoc/>
        public virtual ValueTask SetTypeAsync(TToken token, string? type, CancellationToken cancellationToken)
        {
            if (token is null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            token.Type = type;

            return default;
        }

        /// <inheritdoc/>
        public virtual async ValueTask UpdateAsync(TToken token, CancellationToken cancellationToken)
        {
            if (token is null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            var db = GetDatabase();
            try
            {
                await db.AddOrUpdateAsync(token, cancellationToken: cancellationToken);
            }
            catch (CouchConflictException ex)
            {
                throw new OpenIddictExceptions.ConcurrencyException(SR.GetResourceString(SR.ID0247), ex);
            }
        }
    }
}