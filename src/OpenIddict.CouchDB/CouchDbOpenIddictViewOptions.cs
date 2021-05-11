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
            ApplicationClientId = "application.client_id";
            ApplicationRedirectUri = "application.redirect_uris";
            ApplicationPostLogoutRedirectUri = "application.post_logout_redirect_uris";
            Authorization = "authorization";
            AuthorizationApplicationId = "authorization.application_id";
            AuthorizationSubject = "authorization.subject";
            AuthorizationPrune = "authorization.prune";
            Scope = "scope";
            ScopeName = "scope.name";
            ScopeResources = "scope.resources";
            Token = "token";
            TokenApplicationId = "token.application_id";
            TokenAuthorizationId = "token.authorization_id";
            TokenReferenceId = "token.reference_id";
            TokenSubject = "token.subject";
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
        /// Key = ClientId, Value = Rev
        /// </summary>
        public string ApplicationClientId { get; set; }

        /// <summary>
        /// Key = RedirectUri, Value = Rev
        /// </summary>
        /// <remarks>Each URI for each application is returned as a single row.</remarks>
        public string ApplicationRedirectUri { get; set; }

        /// <summary>
        /// Key = PostLogoutRedirectUri, Value = Rev
        /// </summary>
        /// <remarks>Each URI for each application is returned as a single row.</remarks>
        public string ApplicationPostLogoutRedirectUri { get; set; }

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
        /// Key = ScopeResource, Value = Rev
        /// </summary>
        public string ScopeResources { get; set; }

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
        /// Key = ReferenceId, Value = Rev
        /// </summary>
        public string TokenReferenceId { get; set; }
        
        /// <summary>
        /// Key = Subject, Value = Rev
        /// </summary>
        public string TokenSubject { get; set; }

        /// <summary>
        /// Key = [CreationDate, ExpirationDate] , Value = Rev
        /// </summary>
        /// <remarks>
        public string TokenPrune { get; set; }
    }
}