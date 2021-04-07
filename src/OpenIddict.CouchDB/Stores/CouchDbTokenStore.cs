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
    /// Provides methods allowing to manage the tokens stored in a database.
    /// </summary>
    /// <typeparam name="TToken">The type of the Token entity.</typeparam>
    public class CouchDbTokenStore<TToken> : StoreBase<TToken>, IOpenIddictTokenStore<TToken>
        where TToken : CouchDbToken
    {
        public CouchDbTokenStore(
            IOptionsMonitor<CouchDbOpenIddictOptions> options,
            IServiceProvider provider)
            : base(options, provider)
        {
            Discriminator = Options.CurrentValue.TokenDiscriminator;
        }

        /// <inheritdoc/>
        protected override string Discriminator { get; }

        /// <inheritdoc/>
        public virtual async ValueTask<long> CountAsync(CancellationToken cancellationToken)
        {
            var value = (await GetDatabase()
                .GetViewAsync(Views.Token<TToken>.Tokens, cancellationToken: cancellationToken))
                .FirstOrDefault()?.Value;

            if (long.TryParse(value, out var count))
            {
                return count;
            }
            return 0;
        }

        /// <inheritdoc/>
        public ValueTask<long> CountAsync<TResult>(
            Func<IQueryable<TToken>, IQueryable<TResult>> query, CancellationToken cancellationToken)
        {
            Check.NotNull(query, nameof(query));

            return new(query(QueryDb()).LongCount());
        }

        /// <inheritdoc/>
        public virtual async ValueTask CreateAsync(TToken token, CancellationToken cancellationToken)
        {
            Check.NotNull(token, nameof(token));

            await GetDatabase().AddAsync(token, cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async ValueTask DeleteAsync(TToken token, CancellationToken cancellationToken)
        {
            Check.NotNull(token, nameof(token));

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
                foreach (var token in await QueryDb().Where(token =>
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
                foreach (var token in await QueryDb().Where(token =>
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
                foreach (var token in await QueryDb().Where(token =>
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
                foreach (var token in await QueryDb().Where(token =>
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
                foreach (var token in await QueryDb().Where(token =>
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

            return (await QueryDb()
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
                foreach (var token in await QueryDb().Where(token =>
                    token.Subject == subject).ToCouchListAsync(cancellationToken))
                {
                    yield return token;
                }
            }
        }

        /// <inheritdoc/>
        public virtual ValueTask<string?> GetApplicationIdAsync(TToken token, CancellationToken cancellationToken)
        {
            Check.NotNull(token, nameof(token));

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
            Check.NotNull(query, nameof(query));

            return await query(QueryDb(), state).FirstOrDefaultAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public virtual ValueTask<string?> GetAuthorizationIdAsync(TToken token, CancellationToken cancellationToken)
        {
            Check.NotNull(token, nameof(token));

            if (string.IsNullOrEmpty(token.AuthorizationId))
            {
                return new ValueTask<string?>(result: null);
            }

            return new ValueTask<string?>(token.AuthorizationId.ToString());
        }

        /// <inheritdoc/>
        public virtual ValueTask<DateTimeOffset?> GetCreationDateAsync(TToken token, CancellationToken cancellationToken)
        {
            Check.NotNull(token, nameof(token));

            if (token.CreationDate is null)
            {
                return new ValueTask<DateTimeOffset?>(result: null);
            }

            return new ValueTask<DateTimeOffset?>(DateTime.SpecifyKind(token.CreationDate.Value, DateTimeKind.Utc));
        }

        /// <inheritdoc/>
        public virtual ValueTask<DateTimeOffset?> GetExpirationDateAsync(TToken token, CancellationToken cancellationToken)
        {
            Check.NotNull(token, nameof(token));

            if (token.ExpirationDate is null)
            {
                return new ValueTask<DateTimeOffset?>(result: null);
            }

            return new ValueTask<DateTimeOffset?>(DateTime.SpecifyKind(token.ExpirationDate.Value, DateTimeKind.Utc));
        }

        /// <inheritdoc/>
        public virtual ValueTask<string?> GetIdAsync(TToken token, CancellationToken cancellationToken)
        {
            Check.NotNull(token, nameof(token));

            return new ValueTask<string?>(token.Id.ToString());
        }

        /// <inheritdoc/>
        public virtual ValueTask<string?> GetPayloadAsync(TToken token, CancellationToken cancellationToken)
        {
            Check.NotNull(token, nameof(token));

            return new ValueTask<string?>(token.Payload);
        }

        /// <inheritdoc/>
        public virtual ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(TToken token, CancellationToken cancellationToken)
        {
            Check.NotNull(token, nameof(token));

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
            Check.NotNull(token, nameof(token));

            if (token.RedemptionDate is null)
            {
                return new ValueTask<DateTimeOffset?>(result: null);
            }

            return new ValueTask<DateTimeOffset?>(DateTime.SpecifyKind(token.RedemptionDate.Value, DateTimeKind.Utc));
        }

        /// <inheritdoc/>
        public virtual ValueTask<string?> GetReferenceIdAsync(TToken token, CancellationToken cancellationToken)
        {
            Check.NotNull(token, nameof(token));

            return new ValueTask<string?>(token.ReferenceId);
        }

        /// <inheritdoc/>
        public virtual ValueTask<string?> GetStatusAsync(TToken token, CancellationToken cancellationToken)
        {
            Check.NotNull(token, nameof(token));

            return new ValueTask<string?>(token.Status);
        }

        /// <inheritdoc/>
        public virtual ValueTask<string?> GetSubjectAsync(TToken token, CancellationToken cancellationToken)
        {
            Check.NotNull(token, nameof(token));

            return new ValueTask<string?>(token.Subject);
        }

        /// <inheritdoc/>
        public virtual ValueTask<string?> GetTypeAsync(TToken token, CancellationToken cancellationToken)
        {
            Check.NotNull(token, nameof(token));

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
            var options = new CouchViewOptions<string>
            {
                Reduce = false,
                IncludeDocs = true,
                Limit = count,
                Skip = offset ?? 0
            };

            foreach (var row in await GetDatabase()
                .GetViewAsync(Views.Token<TToken>.Tokens, options, cancellationToken)
                .ConfigureAwait(false))
            {
                yield return row.Document;
            }
        }

        /// <inheritdoc/>
        public virtual IAsyncEnumerable<TResult> ListAsync<TState, TResult>(
            Func<IQueryable<TToken>, TState, IQueryable<TResult>> query,
            TState state, CancellationToken cancellationToken)
        {
            Check.NotNull(query, nameof(query));

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
            // todo: Prune Token
            const int take = 10_000;

            var options = new CouchViewOptions<DateTime[]>
            {
                Descending = true,
                StartKey = new DateTime[] { threshold.DateTime, DateTime.UtcNow }
            };
            var tokens = await GetDatabase().GetViewAsync(Views.Token<TToken>.Prune, options, cancellationToken);

            var delDb = GetDatabase<DeleteDocument>();
            while (tokens.Count != 0)
            {
                var toDelete = tokens.Take(take)
                    .Select(x => new DeleteDocument(x.Id, x.Value))
                    .ToArray();

                await delDb.AddOrUpdateRangeAsync(toDelete, cancellationToken);

                tokens.RemoveRange(0, take);
            }
        }

        /// <inheritdoc/>
        public virtual ValueTask SetApplicationIdAsync(TToken token, string? identifier, CancellationToken cancellationToken)
        {
            Check.NotNull(token, nameof(token));

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
            Check.NotNull(token, nameof(token));

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
            Check.NotNull(token, nameof(token));

            token.CreationDate = date?.UtcDateTime;

            return default;
        }

        /// <inheritdoc/>
        public virtual ValueTask SetExpirationDateAsync(TToken token, DateTimeOffset? date, CancellationToken cancellationToken)
        {
            Check.NotNull(token, nameof(token));

            token.ExpirationDate = date?.UtcDateTime;

            return default;
        }

        /// <inheritdoc/>
        public virtual ValueTask SetPayloadAsync(TToken token, string? payload, CancellationToken cancellationToken)
        {
            Check.NotNull(token, nameof(token));

            token.Payload = payload;

            return default;
        }

        /// <inheritdoc/>
        public virtual ValueTask SetPropertiesAsync(TToken token,
            ImmutableDictionary<string, JsonElement> properties, CancellationToken cancellationToken)
        {
            Check.NotNull(token, nameof(token));

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
            Check.NotNull(token, nameof(token));

            token.RedemptionDate = date?.UtcDateTime;

            return default;
        }

        /// <inheritdoc/>
        public virtual ValueTask SetReferenceIdAsync(TToken token, string? identifier, CancellationToken cancellationToken)
        {
            Check.NotNull(token, nameof(token));

            token.ReferenceId = identifier;

            return default;
        }

        /// <inheritdoc/>
        public virtual ValueTask SetStatusAsync(TToken token, string? status, CancellationToken cancellationToken)
        {
            Check.NotNull(token, nameof(token));

            token.Status = status;

            return default;
        }

        /// <inheritdoc/>
        public virtual ValueTask SetSubjectAsync(TToken token, string? subject, CancellationToken cancellationToken)
        {
            Check.NotNull(token, nameof(token));

            token.Subject = subject;

            return default;
        }

        /// <inheritdoc/>
        public virtual ValueTask SetTypeAsync(TToken token, string? type, CancellationToken cancellationToken)
        {
            Check.NotNull(token, nameof(token));

            token.Type = type;

            return default;
        }

        /// <inheritdoc/>
        public virtual async ValueTask UpdateAsync(TToken token, CancellationToken cancellationToken)
        {
            Check.NotNull(token, nameof(token));

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