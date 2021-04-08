namespace OpenIddict.CouchDB
{
    public record CouchDbOpenIddictViewOptions
    {
        /// <summary>
        /// Initialize a new <see cref="CouchDbOpenIddictViewOptions"/> with default values.
        /// </summary>
        public CouchDbOpenIddictViewOptions()
        {
            Document = "openiddict";
            Application = "application";
            Authorization = "authorization";
            AuthorizationApplicationId = "authorization.application_id";
            AuthorizationSubject = "authorization.subject";
            AuthorizationPrune = "authorization.prune";
            Scope = "scope";
            ScopeName = "scope.name";
            Token = "token";
            TokenApplicationId = "token.application_id";
            TokenAuthorizationId = "token.authorization_id";
            TokenPrune = "token.prune";
        }

        /// <summary>
        /// The name of the design document that will contain the views.
        /// </summary>
        public string Document { get; set; }

        /// <summary>
        /// With reduce = true (default) then Key = null, Value = 'count'<br/>
        /// When reduce = false (manual) then Key = Id, Value = Rev
        /// </summary>
        public string Application { get; set; }

        /// <summary>
        /// With reduce = true (default) then Key = null, Value = 'count'<br/>
        /// When reduce = false (manual) then Key = Id, Value = Rev
        /// </summary>
        public string Authorization { get; set; }

        /// <summary>
        /// Key = ApplicationId, Value = Rev
        /// </summary>
        public string AuthorizationApplicationId { get; set; }

        /// <summary>
        /// Key = Subject, Value = Rev
        /// </summary>
        public string AuthorizationSubject { get; set; }

        /// <summary>
        /// Key = [ CreationDate, ExpirationDate ] , Value = Rev
        /// </summary>
        public string AuthorizationPrune { get; set; }

        /// <summary>
        /// With reduce = true (default) then Key = null, Value = 'count'<br/>
        /// When reduce = false (manual) then Key = Id, Value = Rev
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// Key = Name, Value = Rev
        /// </summary>
        public string ScopeName { get; set; }

        /// <summary>
        /// With reduce = true (default) then Key = null, Value = 'count'<br/>
        /// When reduce = false (manual) then Key = Id, Value = Rev
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Key = ApplicationId, Value = Rev
        /// </summary>
        public string TokenApplicationId { get; set; }

        /// <summary>
        /// Key = AuthorizationId, Value = Rev
        /// </summary>
        public string TokenAuthorizationId { get; set; }

        /// <summary>
        /// Key = [CreationDate, ExpirationDate] , Value = Rev
        /// </summary>
        /// <remarks>
        public string TokenPrune { get; set; }
    }
}