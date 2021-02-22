using CouchDB.Driver.Exceptions;
using CouchDB.Driver.Types;
using Flurl.Http;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace CouchDB.Driver
{
    public static class ICouchDatabaseExtensions
    {
        public static Task<CouchViewResult<TValue>> GetViewAsync<T, TValue>(this ICouchDatabase<T> db, string design, string view, CouchViewOptions? options = null, CancellationToken cancellationToken = default)
            where T : CouchDocument
        {
            IFlurlRequest request = db.NewRequest()
                .AppendPathSegments("_design", design, "_view", view)
                .SetQueryParams(options?.ToQueryParameters());

            return request
                .GetJsonAsync<CouchViewResult<TValue>>(cancellationToken)
                .SendRequestCouchAsync();
        }

        public static Task<CouchViewResult<TValue, TDoc>> GetViewAsync<T, TValue, TDoc>(this ICouchDatabase<T> db, string design, string view, CouchViewOptions? options = null, CancellationToken cancellationToken = default)
            where T : CouchDocument
            where TDoc : CouchDocument
        {
            options ??= new CouchViewOptions();
            options.IncludeDocs = true;

            IFlurlRequest request = db.NewRequest()
                .AppendPathSegments("_design", design, "_view", view)
                .SetQueryParams(options?.ToQueryParameters());

            return request
                .GetJsonAsync<CouchViewResult<TValue, TDoc>>(cancellationToken)
                .SendRequestCouchAsync();
        }
    }

    internal static class RequestsHelper
    {
        public static async Task<TResult> SendRequestCouchAsync<TResult>(this Task<TResult> asyncRequest)
        {
            try
            {
                return await asyncRequest.ConfigureAwait(false);
            }
            catch (FlurlHttpException ex)
            {
                CouchError couchError = await ex.GetResponseJsonAsync<CouchError>().ConfigureAwait(false) ?? new CouchError();

                throw ex.Call.HttpStatus switch
                {
                    HttpStatusCode.Conflict => new CouchConflictException(couchError.ToString(), ex),
                    HttpStatusCode.NotFound => new CouchNotFoundException(couchError.ToString(), ex),
                    HttpStatusCode.BadRequest when couchError.Error == "no_usable_index" => new CouchNoIndexException(
                        couchError.ToString(), ex),
                    _ => new CouchException(couchError.ToString(), ex)
                };
            }
        }

        private class CouchError
        {
            public string Error { get; set; } = string.Empty;

            public string Reason { get; set; } = string.Empty;

            public override string ToString() => $"Error: {Error}, Reason: {Reason}";
        }
    }
}