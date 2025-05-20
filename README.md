# RaidLootAnnotatorApi

RaidLootAnnotatorApi is a Google Cloud Function API for storing and retrieving raid static and member information for a loot annotator plugin.

## Features

- Create a new static (POST `/static`):  
  Accepts a name, generates a GUID, stores both in Google Cloud Datastore, and returns the GUID.
- Add or update a static member (POST `/staticmember`):  
  Stores or updates member data for a static, identified by name and static GUID.
- Retrieve all members for a static (GET `/static?guid=...`):  
  Returns all members associated with a given static GUID.

## Deployment

This API is deployed as a Google Cloud Function using .NET.  
It uses Google Cloud Datastore as its backend database.

## Build & Deploy

1. **Build:**  
   ```
   dotnet build
   ```

2. **Deploy to Google Cloud Functions:**  
   ```
   gcloud functions deploy RaidLootAnnotatorApi \
     --runtime dotnet8 \
     --trigger-http \
     --entry-point HelloHttp.Function \
     --allow-unauthenticated
   ```

3. **Index Configuration:**  
   Make sure your Datastore indexes are configured (see `index.yaml`).

## Requirements

- .NET 8 SDK
- Google Cloud SDK
- Google Cloud project with Datastore and Cloud Functions enabled
