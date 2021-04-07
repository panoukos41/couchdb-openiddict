# Setup

Using either [setup.ps1](setup.ps1) or [setup.sh](setup.sh) will use [curl](https://curl.se/) to try and create the [ddoc.json](ddoc.json) on the provided url.

This means the contents of this [ddoc.json](ddoc.json) will be added to the design documents of the database.

The database ddoc will have it's name choosen from the `_id` property inside the [ddoc.json](ddoc.json).

#### Important

The url should be fully qualified with password and user. 

eg: User = `admin`, Password = `password`, Host = `localhost:5984`, Database = `my_database`

the complete url then is `https://admin:password@localhost:5984/my_database`

# Views Overview

For each method there is an `if (doc.split_discriminator == 'type')` which if true emit will be executed, type can be `openiddict.application`, `openiddict.authorization`, `openiddict.scope` or `openiddict.token` the if is ommited for brevity.

To call a view you first use the `doc name` then the `view id` below are only the view ids you call and their results.

In code there are some `Internal` namespaces that containt a static `Views` class and a `View` class that is used to call views, you can replace them or use them in case you need functionality that is not provided but do so with care.

## application
Returns the count of all application documents.  
When `reduce` is `false` it returns the document `id`.

    map: emit(doc._id, doc._rev);
    reduce: _count

## authorization
Returns the count of all authorization documents.  
When `reduce` is `false` it returns the document `id`.

    map: emit(doc._id, doc._rev);
    reduce: _count

## authorization.application_id
Returns the `application_id` of an authorization as key and `rev` as value.

    map: emit(doc.application_id, doc._rev);

## authorization.subject
Returns the `subject` of an authorization as key and `rev` as value.

    map: emit(doc.subject, doc._rev);

## authorization.prune
The key is an array of datetime which the first element is `creation_date` and the second is `expiration_date`.  
Returns only the documents that their type is not `inactive` or `valid`.

    map: emit([doc.creation_date, doc.ation_date], doc._rev);

## scope
Returns the `count` of all scope documents.  
When `reduce` is `false` it returns the document `id`.

    map: emit(doc._id, doc._rev);
    reduce: _count

## scope.name
Returns the `name` of a scope as key and `rev` as value.

    map: emit(doc.name, doc._rev);

## token
Returns the `count` of all token documents.  
When `reduce` is `false` it returns the document `id`.

    map: emit(doc._id, doc._rev);
    reduce: _count

## token.application_id
Returns the `application_id` of a token as key and `rev` as value.

    map: emit(doc.application_id, doc._rev);

## token.authorization_id
Returns the `authorization_id` of a token as key and `rev` as value.

    map: emit(doc.authorization_id, doc._rev);

## token.prune
The key is an array of datetime which the first element is `creation_date` and the second is `expiration_date`.  
Returns only the documents that their type is not `inactive` or `valid`.

    map: emit([doc.creation_date, doc.expiration_date], doc._rev);
