# Overview

The `TagShelf.Alfred.ApiWrapper` is a comprehensive .NET library designed to facilitate seamless interactions with the Alfred API. It's tailored to support a wide range of .NET applications, from modern .NET Core and .NET Standard projects to legacy .NET Framework 4.7.2 applications, providing a robust, strongly-typed interface for efficient development. It includes specialized domains for handling [Deferred Sessions](#deferred-sessions), [Files](#files), [Jobs](#jobs) and [Data Points](#data-points).

This library serves as a wrapper for our REST API, which you can find the documentation for in this link https://docs.tagshelf.dev/

## Alfred

Alfred is a powerful document processing platform that enables you to extract, index, and search through large document collections with ease. It offers a wide range of features, including:

- **Job Management**: Provides a robust job management system that allows you to schedule and monitor document processing jobs.

- **Tagging**: Tag documents based on their content, making it easy to organize and search through large document collections.

- **Extraction**: Can extract specific data from PDFs, images, and other documents with ease using its powerful extraction engine.

- **Indexing**: Powerful indexing engine that can index and search through millions of documents in seconds.

- **Integration**: Alfred can be easily integrated into your existing applications using its powerful API and SDKs.

- **Scalability**: Alfred is designed to expand its capacity seamlessly according to client needs, efficiently processing anywhere from thousands to millions of documents each day.

- **Comprehensive Authentication Support**: Seamlessly handles OAuth, HMAC, and API key authentication methods, simplifying the process of connecting to the Alfred API.

- **Domain-Specific Operations**: Offers specialized support for File and Job operations, enabling developers to intuitively manage and interact with API resources.

- **Real-Time Events**: Streams job and file events as they happen over Socket.IO through a standalone real-time client, removing the need for polling.

- **Tag Management**: List, create, inspect, and update tags (document types), including their configured data points and visual-extraction setup.

- **Cross-Platform Compatibility**: Designed to be fully compatible across .NET Core, .NET Standard, and .NET Framework 4.7.2, ensuring broad usability in diverse development environments.

- **Minimal Dependencies**: Crafted to minimize external dependencies, facilitating an easier integration and deployment process with reduced conflict risk.

## Prerequisites

- .NET Core 2.1+ or .NET Framework 4.7.2+ installed on your development machine.
- An active Alfred API key, HMAC credentials, or OAuth credentials for authenticating API requests.

# Installation

To integrate the Alfred API wrapper into your .NET project, install the package via NuGet in the `src` folder of your project:

```bash
dotnet add package TagShelf.Alfred.ApiWrapper
```

Alternatively, you can install the package using the NuGet Package Manager in Visual Studio. Search for `TagShelf.Alfred.ApiWrapper` and click the Install button.

For .NET Core CLI, use the following command:

```bash
Install-Package TagShelf.Alfred.ApiWrapper
```

# Environment

You will always use the production environment for all your integrations using this library. The production environment is used for live applications and real-world scenarios. It is recommended to use this environment for production-ready applications. The frontend URL for the production environment is [https://app.tagshelf.com](https://app.tagshelf.com)</br></br>

# Authentication

Alfred supports three authentication methods: OAuth, HMAC, and API Key. Each method is suited for different scenarios. To obtain the necessary credentials for the following authentication methods, please refer to the [Alfred API documentation](https://docs.tagshelf.dev/authentication) or contact the Alfred support team at support@tagshelf.com.

|OAuth||
|--|--|
| **When to Use**   | Ideal for web portals needing integration with document parsers or API-driven services, especially when users access through browsers. Suitable for short-lived browser sessions and scenarios requiring scoped, secure access.|
| **Benefits**      | Enhanced security via token use, flexible access control, and a user-friendly experience with periodic re-authentication.|
| **Best Practices**| Secure token storage and graceful handling of token expiration.|

To initialize the client for OAuth authentication, specify your Tagshelf credentials explicitly:

```csharp
var alfred = await AlfredFactory.CreateWithOAuthAsync("username", "password", EnvironmentType.Production);
```

|HMAC||
|--|--|
| **When to Use**    | Best for applications needing individual account access without token management, ensuring secure transactions.|
| **Benefits**       | Secure request authentication with a unique signature per request.|
| **Best Practices** | Secure key management, include timestamps to prevent replay attacks, and use a dynamic nonce for each request.|

To initialize the client for HMAC authentication, provide your secret key and API key:

```csharp
var alfred = await AlfredFactory.CreateWithHmacAsync("secret_key", "api_key", EnvironmentType.Production);
```

Every request is signed on its own (the `Authorization: amx ...` header), with a fresh nonce and the current time. Alfred rejects a signature more than five minutes old, so keep the machine's clock in sync.

|API Key||
|--|--|
| **When to Use**    | Recommended for server-to-server communication, automated processes, and background services where user interaction is not feasible.|
| **Benefits**       | Non-interactive yet robust authentication, simplicity, and easy access control.|
| **Best Practices** | Secure storage of API keys, apply the principle of least privilege, and regularly rotate keys to minimize compromise risks.|

To initialize the client for API key authentication, provide the API key:

```csharp
var alfred = await AlfredFactory.CreateWithApiKeyAsync("api_key", EnvironmentType.Production);
```

# Getting Started

This library covers these domains:
- [Tagshelf](#tagshelf)
- [Deferred Sessions](#deferred-sessions)
- [Files](#files)
- [Jobs](#jobs)
- [Data Points](#data-points)
- [Account](#account)
- [Tags](#tags)
- [Search](#search)
- [Remote Sources](#remote-sources)
- [Real-Time Events](#real-time-events)

When a call fails, the library throws an `ApiException` carrying the HTTP `StatusCode`, the server's `ErrorMessage` (when it sent one) and the raw `ResponseContent`. See [Handling Errors](#handling-errors).

## Tagshelf

The Tagshelf API namespace serves as a dedicated domain within the API for platform status and identity queries. This namespace is tailored to provide essential functionalities related to the operational status and user context of the platform. It encompasses endpoints that are crucial for system health checks, user identity verification, and basic connectivity tests. 

### Check Platform Operational Status

This endpoint offers a quick way to check the overall operational status of the platform.

```csharp
// Imports
using TagShelf.Alfred.ApiWrapper.Core;
using TagShelf.Alfred.ApiWrapper.Domains.Tagshelf.Results;
using TagShelf.Alfred.ApiWrapper.Enumerations;

// Authentication
var alfred = await AlfredFactory.CreateWithOAuthAsync("example@mail.com", "password", EnvironmentType.Production);

// Get the Platform Operational Status
TagshelfStatusResult status = await alfred.Tagshelf.StatusAsync();
```

### Retrieve User Identity Information

This endpoint provides information about the currently authenticated user.

```csharp
// Imports
using TagShelf.Alfred.ApiWrapper.Core;
using TagShelf.Alfred.ApiWrapper.Domains.Tagshelf.Results;
using TagShelf.Alfred.ApiWrapper.Enumerations;

// Authentication
var alfred = await AlfredFactory.CreateWithOAuthAsync("example@mail.com", "password", EnvironmentType.Production);

// Get the Platform Operational Status
WhoAmIResult whoami = await alfred.Tagshelf.WhoAmIAsync();
```

### Conduct a Basic Connectivity Test

The `ping` endpoint is used to perform a basic connectivity test to the platform.

```csharp
// Imports
using TagShelf.Alfred.ApiWrapper.Core;
using TagShelf.Alfred.ApiWrapper.Domains.Tagshelf.Results;
using TagShelf.Alfred.ApiWrapper.Enumerations;

// Authentication
var alfred = await AlfredFactory.CreateWithOAuthAsync("example@mail.com", "password", EnvironmentType.Production);

// Get the Platform Operational Status
PingResult ping = await alfred.Tagshelf.PingAsync();
```

## Deferred Sessions

A Deferred Session in Alfred is a mechanism designed for asynchronous file uploads, integral to the document processing workflow. It serves as a container or grouping for files that are uploaded at different times or from various sources, but are all part of a single processing task or Job.

### Create a new Deferred Session

This endpoint initiates a new Deferred Session in Alfred for asynchronous, staggered file uploads for a future Job.

```csharp
// Imports
using TagShelf.Alfred.ApiWrapper.Core;
using TagShelf.Alfred.ApiWrapper.Domains.DeferredSession.Results;
using TagShelf.Alfred.ApiWrapper.Enumerations;

// Authentication
var alfred = await AlfredFactory.CreateWithOAuthAsync("example@mail.com", "password", EnvironmentType.Production);

// Create a Session
CreateSessionResult sessionId = await alfred.DeferredSession.CreateAsync();
```

### Retrieve Details of a Specific Deferred Session

This endpoint provides detailed information about a Deferred Session, identified by its unique `id`.

```csharp
// Imports
using TagShelf.Alfred.ApiWrapper.Core;
using TagShelf.Alfred.ApiWrapper.Domains.DeferredSession.Results;
using TagShelf.Alfred.ApiWrapper.Enumerations;

// Authentication
var alfred = await AlfredFactory.CreateWithOAuthAsync("example@mail.com", "password", EnvironmentType.Production);

// Create a Session
CreateSessionResult sessionId = await alfred.DeferredSession.CreateAsync();

// Get Session Details
SessionDetailResult sessiondetails = await alfred.DeferredSession.GetAsync(sessionId.SessionId);
```

## Files

In Alfred, a file is an individual document or data unit undergoing specialized operations tailored for document analysis and management. It is processed by the platform for tasks such as classification, optical character recognition (OCR), and data extraction. Each file, as a distinct entity, transitions through various statuses, which mark its progress in the automated workflow.

You can find the full list of job statuses and their description in the [Alfred Official Documentation](https://docs.tagshelf.dev/enpoints/file/file-status).

### Upload Files from Remote Sources

This endpoint facilitates the uploading of files from remote sources, accommodating various types of external storage like URLs, or Blob storage from cloud providers including AWS, GCP, or DigitalOcean. 

In Alfred, uploading a file via this endpoint contributes to the initiation of a Job, which encompasses a series of processes over several files.

You can find the full list of parameters you can use in the [Alfred Official Documentation](https://docs.tagshelf.dev/enpoints/file#upload-files-from-remote-sources).

```csharp
// Imports
using TagShelf.Alfred.ApiWrapper.Core;
using AlfredApiWrapper.Domains.File.Requests;
using TagShelf.Alfred.ApiWrapper.Domains.File.Results;
using TagShelf.Alfred.ApiWrapper.Enumerations;

// Authentication
var alfred = await AlfredFactory.CreateWithOAuthAsync("example@mail.com", "password", EnvironmentType.Production);

// Create a Request Object with the Necessary Parameters
UploadRequest uploadRequest = new UploadRequest
{
    Urls = new List<string> { "https://example.com/file1.pdf", "https://example.com/file2.pdf" },
    FileNames = new List<string> { "file1.pdf", "file2.pdf" }
};

// Upload Remote File
FileUploadResult uploadResult = await alfred.File.UploadAsync(uploadRequest);
```

Useful options on `UploadRequest` (the same apply to `CreateJobRequest`):

| Property | Description |
| --- | --- |
| `CorrelationId` | Your own ID for the operation that submitted the job (an ERP transaction, an email Message-ID), up to 512 characters. It is returned on the job and can be searched with `Job.GetAllAsync`. Alfred does not deduplicate on it. |
| `JobPriority` | `Low`, `Normal`, `High` or `Urgent`. Leave it unset to use your account's default priority. |
| `Decompose` | Split each input into separate units of work (for example each page of a PDF). `ParentFilePrefix` and `PageRotation` only apply when this is `true`. |
| `Channel` | The channel the job is recorded under (for example `"api"`). |

`FileUploadResult` returns the `JobId`, the `CorrelationId`, and any `Warnings` about the submission (for example that your account has no enabled tags). The job is still created when there are warnings.

### Upload File by Stream

Uploads a single remote file to the system's pipeline. Before using this endpoint, you must have the `id` of a deferred session already created.

This endpoint uploads a file from a stream to Alfred and associates it with a specific session `id`. It does not trigger job processing, making it ideal for uploading files that are part of a larger job.

| Parameter | Type | Description |
| --- | --- | --- |
| FileStream | Stream | Stream of the file to upload. |
| Filename | string | Unique name of the file within an object storage source. |
| SessionId | Guid | Session ID to associate with the file. |
| Metadata | string | JSON object or JSON array of objects containing metadata fields for a given remote file. |

```csharp
// Imports
using TagShelf.Alfred.ApiWrapper.Core;
using AlfredApiWrapper.Domains.File.Requests;
using TagShelf.Alfred.ApiWrapper.Domains.File.Results;
using TagShelf.Alfred.ApiWrapper.Enumerations;

// Authentication
var alfred = await AlfredFactory.CreateWithOAuthAsync("example@mail.com", "password", EnvironmentType.Production);

// Create a Deferred Session
Guid sessionId = (await alfred.DeferredSession.CreateAsync()).SessionId;

// Create a Request Object with the Necessary Parameters
UploadFileRequest uploadFileRequest = new UploadFileRequest
{
    FileStream = new StreamReader("file_path\\sample.pdf").BaseStream,
    FileName = "sample.pdf",
    SessionId = sessionId,
    Metadata = { }
};

// Upload File From Stream
UploadFileResult uploadFileResult = await alfred.File.UploadFileAsync(uploadFileRequest);
```

### Get File Details by ID

Get all file details by file `id`.

```csharp
// Imports
using TagShelf.Alfred.ApiWrapper.Core;
using TagShelf.Alfred.ApiWrapper.Domains.File.Results;
using TagShelf.Alfred.ApiWrapper.Enumerations;

// Authentication
var alfred = await AlfredFactory.CreateWithOAuthAsync("example@mail.com", "password", EnvironmentType.Production);

// File ID
Guid fileid = Guid.Parse("6783e660-2e97-4534-b559-5c90b4d30d41");

// Upload File From Stream
FileDetailResult filedetail = await alfred.File.GetAsync(fileid);
```

### Download File by ID

Returns a byte stream representing the data for a given file.

```csharp
// Imports
using TagShelf.Alfred.ApiWrapper.Core;
using TagShelf.Alfred.ApiWrapper.Domains.File.Results;
using TagShelf.Alfred.ApiWrapper.Enumerations;

// Authentication
var alfred = await AlfredFactory.CreateWithOAuthAsync("example@mail.com", "password", EnvironmentType.Production);

// File ID
Guid fileid = Guid.Parse("6783e660-2e97-4534-b559-5c90b4d30d41");

// Upload File From Stream
FileDownloadResult download = await alfred.File.DownloadAsync(fileid);
```

### List Files

Lists your account's files one page at a time (at most 200 per page).

```csharp
PagedResult<FileDetailResult> page = await alfred.File.GetAllAsync(page: 1, pageSize: 50);
Console.WriteLine($"{page.Total} files");
```

### Get File Events and Versions

`GetEventsAsync` lists the events Alfred raised for a file (for example `alfred_event_file_done`) and whether each reached the event broker and your webhook. `GetVersionsAsync` lists the versions of the file's extracted text.

```csharp
List<FileEventResult> events = await alfred.File.GetEventsAsync(fileId);
List<FileVersionResult> versions = await alfred.File.GetVersionsAsync(fileId);
```

### Share a File

Creates a link anyone can use to download the file without credentials. The file must have finished processing. Set how long the link works (at least 60 seconds), or make it permanent.

```csharp
using TagShelf.Alfred.ApiWrapper.Domains.File.Requests;

ShareFileResult share = await alfred.File.ShareAsync(fileId, new ShareFileRequest
{
    ExpiresInSeconds = 24 * 60 * 60, // or DoesNotExpire = true
    ShortUrl = true,                 // the default
    AlwaysUseLastVersion = true      // the default; false pins the current version
});
Console.WriteLine(share.ShareUrl);
```

### Reprocess or Re-extract a File

Start with the lightest fix. `ReextractAsync` runs extraction again with the file's current tag, for when the extracted data is wrong or the tag's data points changed; the file must already be classified. `ReprocessAsync` runs the whole pipeline again (transformation, classification and extraction), for when the file got the wrong tag or its transformation failed.

```csharp
FileOperationResult reextract = await alfred.File.ReextractAsync(fileId, requestReview: true);
FileOperationResult reprocess = await alfred.File.ReprocessAsync(fileId, skipCache: true);
```

`requestReview: true` holds the result for human review whatever the tag's validation settings say. A file that a run in progress already holds is refused with an `ApiException` with status 409.

### Change a File's Tag

Gives a file a different tag (document type) and sends it to extraction under the new one, as **Change type** does in the console. A file someone else is reviewing is refused with status 409. Requires Alfred 1.128.4 or later.

```csharp
ChangeFileTagResult changed = await alfred.File.ChangeTagAsync(fileId, newTagId);
```

## Jobs

A Job in Alfred represents a single unit of work that performs a sequence of operations on one or more files for the purpose of document classification, extraction, and indexing. It is an asynchronous entity, orchestrated by a state machine that manages its progress through various stages.

You can find the full list of job stages and their description in the [Alfred Official Documentation](https://docs.tagshelf.dev/enpoints/job/job-stages).

### Create a New Job and Close the Deferred Upload Session

This endpoint is used for creating a new Job in Alfred. It serves a dual purpose: it finalizes the deferred upload session, ensuring that all files needed for the Job are uploaded, and initiates the Job itself.

The only parameter that is **required** for this endpoint is `session_id`. You can find the full list of parameters you can use in the [Alfred Official Documentation](https://docs.tagshelf.dev/enpoints/job#create-a-new-job-and-close-the-deferred-upload-session).

```csharp
// Imports
using TagShelf.Alfred.ApiWrapper.Core; 
using TagShelf.Alfred.ApiWrapper.Domains.Job.Requests;
using TagShelf.Alfred.ApiWrapper.Domains.Job.Results;
using TagShelf.Alfred.ApiWrapper.Enumerations;

// Authentication
var alfred = await AlfredFactory.CreateWithOAuthAsync("example@mail.com", "password", EnvironmentType.Production);

// Create a Deferred Session
Guid sessionId = (await alfred.DeferredSession.CreateAsync()).SessionId;

// Create a Request Object with the Necessary Parameters 
CreateJobRequest request = new CreateJobRequest
{
    SessionId = sessionId
};

// Create a New Job
CreateJobResult job = await alfred.Job.CreateAsync(request);
```

`CreateJobRequest` also accepts `CorrelationId` and `JobPriority`; see the options table under [Upload Files from Remote Sources](#upload-files-from-remote-sources). `CreateJobResult` returns the `JobId` and the `CorrelationId`.

### Retrieve Detailed Information About a Specific Job

This endpoint provides comprehensive details about a Job identified by its unique `id`. 

```csharp
// Imports
using TagShelf.Alfred.ApiWrapper.Core; 
using TagShelf.Alfred.ApiWrapper.Domains.Job.Results;
using TagShelf.Alfred.ApiWrapper.Enumerations;

// Authentication
var alfred = await AlfredFactory.CreateWithOAuthAsync("example@mail.com", "password", EnvironmentType.Production);

// Job ID
Guid jobid = Guid.Parse("ffad4bcb-0d38-47a6-9886-22e98818ee84");

// Get Job Details
JobDetailResult jobdetail = await alfred.Job.GetAsync(jobid);
```

For a large job, leave out the sections you don't need:

```csharp
JobDetailResult slim = await alfred.Job.GetAsync(jobid, JobDetailExclude.Files | JobDetailExclude.Metadata);
```

### List Jobs

Lists your account's jobs, newest first, 10 to 100 per page. You can filter by a date window (UTC days), by the correlation ID the job was submitted with (an exact, case-sensitive match), and by channel.

```csharp
using TagShelf.Alfred.ApiWrapper.Domains.Job.Requests;

PagedResult<JobSummaryResult> jobs = await alfred.Job.GetAllAsync(new JobListRequest
{
    PageSize = 25,
    StartDate = new DateTime(2026, 10, 1),
    EndDate = new DateTime(2026, 10, 31),
    CorrelationId = "erp:invoice:INV-1042"
});
```

### Restart, Cancel or Check a Job

```csharp
JobRestartResult restarted = await alfred.Job.RestartAsync(jobid);
JobCancelResult canceled = await alfred.Job.CancelAsync(jobid);

// Before restarting with fresh downloads, check the remote inputs are still there.
JobSourcesCheckResult sources = await alfred.Job.CheckSourcesAsync(jobid);
if (sources.Available)
{
    await alfred.Job.RestartAsync(jobid, redownloadRemoteFiles: true);
}
```

A restart with `redownloadRemoteFiles: true` is refused with status 409 when a source has gone missing; the `ApiException`'s `ResponseContent` lists them.

### Reprocess or Re-extract Several Files of a Job

Runs one operation on a chosen set of a job's files. Each file is checked on its own: files that are not in the job, or not eligible, are skipped and reported instead of failing the whole call.

```csharp
BulkFileOperationResult bulk = await alfred.Job.BulkFileOperationAsync(jobid, new BulkFileOperationRequest
{
    FileIds = { fileId1, fileId2 },
    Operation = FileOperation.Reextract,
    RequestReview = false
});

foreach (var r in bulk.Results)
{
    Console.WriteLine($"{r.Id}: {r.Outcome} {r.Reason}"); // Outcome is "done", "skipped" or "failed"
}
```

## Data Points

> **Important:** Data Points where previously known as Metadata.

In Alfred's Intelligent Document Processing (IDP) platform, the concept of data points is intricately linked with tags and plays a vital role in extracting specific data points from content, particularly files. Data points in Alfred are unique in that they are associated with a specific tag and encompass information intended to be extracted based on the file's classified content. 

Data point values are critical components that represent the extracted information based on the data points defined for each tag.

### Get Data Points Values from File ID

Get a list of metadata values from File ID.

```csharp
// Imports
using TagShelf.Alfred.ApiWrapper.Core;
using TagShelf.Alfred.ApiWrapper.Domains.DataPoint.Results;
using TagShelf.Alfred.ApiWrapper.Enumerations;

// Authentication
var alfred = await AlfredFactory.CreateWithOAuthAsync("example@mail.com", "password", EnvironmentType.Production);

// File ID
Guid fileid = Guid.Parse("010065f8-c515-465b-9bd4-13a1fd36e908");

// Get the Data Point Values
List<DataPointResult> datapointvalues = await alfred.DataPoint.GetValuesAsync(fileid);
```

Each `DataPointResult` includes its `FieldKey`, the `ExtractedValue` when formatting rules changed the extractor's original output, and, when the platform produced one, a `NormalizedValue`. When a data point was validated, its `Validation` property is populated (otherwise `null`); it reports the overall `Status`, a per-validator `Results` list, and a roll-up `Summary`.

```csharp
foreach (var dp in datapointvalues)
{
    Console.WriteLine($"{dp.FieldKey} = {dp.Value} (normalized: {dp.NormalizedValue})");

    if (dp.Validation != null)
    {
        var v = dp.Validation;
        Console.WriteLine($"  Validation: {v.Status} — {v.Summary}"); // e.g. "passed — 3/3 passed, 0 failed, 0 skipped"

        foreach (var check in v.Results)
        {
            Console.WriteLine($"    {check.Type}: {(check.Passed ? "passed" : "failed")}");
            // check.Message, check.Runtime and check.Metadata (raw JSON) are available for custom validators.
        }
    }
}
```

### Get Data Point Values for a Tag

Returns the confirmed values of every file of a tag, one flat row per file. Each row has fixed keys (`file_id`, `tag_name`, `blob_url`, `creation_date` and others) plus one key per data point: its name in snake_case (`InvoiceNumber` becomes `invoice_number`). The response is not paged, so it can be large for a busy tag.

```csharp
List<Dictionary<string, JToken>> rows = await alfred.DataPoint.GetValuesByTagAsync(tagId);
foreach (var row in rows)
{
    Console.WriteLine($"{row["file_id"]}: {row["invoice_number"]}");
}
```

### Get Data Point Definitions

Lists what Alfred extracts for a tag: each data point's name, stable `FieldKey`, `FieldType`, whether it holds personal information (`IsPii`, `PiiMask`), and its formatting and validation rules (as raw JSON). Data points are configured in Alfred; the API reads them.

```csharp
List<DataPointDefinitionResult> definitions = await alfred.DataPoint.GetDefinitionsAsync(tagId);
DataPointDefinitionResult one = await alfred.DataPoint.GetDefinitionAsync(definitions[0].Id);
```

## Account

This namespace is designed to facilitate crucial account-related activities and insights. It encompasses a range of functionalities essential for enhancing user interaction and management within the platform.

### Set a Webhook for a Given Account

Sets a web hook for a given account. An HTTP end-point will get notified every time an event is triggered within the system.

```csharp
// Imports
using TagShelf.Alfred.ApiWrapper.Core;
using TagShelf.Alfred.ApiWrapper.Domains.Account.Requests;
using TagShelf.Alfred.ApiWrapper.Domains.Account.Results;
using TagShelf.Alfred.ApiWrapper.Enumerations;

// Authentication
var alfred = await AlfredFactory.CreateWithOAuthAsync("example@mail.com", "password", EnvironmentType.Production);

// Create a Request Object with the Necessary Parameters 
WebhookSetupRequest request = new WebhookSetupRequest
{
    Url = "https://my.webhook.co/path"
};

// Set a Webhook for a Given Account
WebhookSetupResult webhook = await alfred.Account.SetupWebhookAsync(request);
```

### List Accounts and Their Tags

`GetCompaniesAsync` lists the accounts you have access to. An API key belongs to one account; a user signed in with OAuth or HMAC may have several. `GetTagsAsync` lists the tags you can see in an account (your own when you pass no ID).

```csharp
List<CompanyResult> companies = await alfred.Account.GetCompaniesAsync();
List<TagResult> tags = await alfred.Account.GetTagsAsync(companies[0].Id);
```

## Tags

This namespace provides operations for managing tags (document types): listing them, creating new ones, retrieving details (including configured data points and visual-extraction setup), and updating them.

### List Tags

Listing is unpaged by default and returns the full array. You can filter by a case-insensitive substring (`query`) and optionally include disabled tags.

```csharp
// Imports
using TagShelf.Alfred.ApiWrapper.Core;
using TagShelf.Alfred.ApiWrapper.Domains.Tag.Results;
using TagShelf.Alfred.ApiWrapper.Enumerations;

// Authentication
var alfred = await AlfredFactory.CreateWithApiKeyAsync("your-api-key", EnvironmentType.Production);

// Retrieve every tag accessible to the caller (optionally filtered)
List<TagResult> tags = await alfred.Tag.GetAllAsync(query: "invoice", includeDisabled: false);
foreach (var tag in tags)
{
    Console.WriteLine($"{tag.Name} ({tag.Slug})");
}
```

For large tenants, page the results instead. Paging returns an envelope with the page rows plus the total count:

```csharp
PagedResult<TagResult> page = await alfred.Tag.GetPagedAsync(page: 1, pageSize: 50, query: "invoice");
Console.WriteLine($"Showing {page.Result.Count} of {page.Total} tag(s)");
```

### Create a Tag

```csharp
using TagShelf.Alfred.ApiWrapper.Domains.Tag.Requests;
using TagShelf.Alfred.ApiWrapper.Domains.Tag.Results;

CreateTagResult created = await alfred.Tag.CreateAsync(new CreateTagRequest
{
    Name = "Invoice",
    Description = "Vendor invoices" // optional, but tags without one classify poorly
});

Console.WriteLine($"Created tag {created.Id}");
foreach (var warning in created.Warnings ?? Enumerable.Empty<string>())
{
    Console.WriteLine($"Warning: {warning}");
}
```

### Retrieve Tag Details

The detail endpoint is **slim by default**: identity fields plus each data point's `Id`, `Name`, and `FieldKey` (hidden data points excluded).

```csharp
using TagShelf.Alfred.ApiWrapper.Domains.Tag.Results;

TagDetailResult detail = await alfred.Tag.GetAsync(tagId);
Console.WriteLine($"{detail.Name} — {detail.DataPoints.Count} data point(s)");

foreach (var dataPoint in detail.DataPoints)
{
    Console.WriteLine($"  {dataPoint.FieldKey} ({dataPoint.Name})");
}
```

Opt in to the full data point configuration and/or the tag's visual-extraction setup with `TagExpand` (a `[Flags]` enum). Expanded data points include configuration fields (`DataType`, `ValueSource`, `Required`, hidden ones, …); without the expansion those fields are left at their defaults.

```csharp
using TagShelf.Alfred.ApiWrapper.Enumerations;

TagDetailResult full = await alfred.Tag.GetAsync(tagId, TagExpand.DataPoints | TagExpand.VisualExtraction);
// or: TagExpand.All

foreach (var dataPoint in full.DataPoints)
{
    Console.WriteLine($"  {dataPoint.FieldKey}: {dataPoint.DataType} (source: {dataPoint.ValueSource})");
}

if (full.VisualExtraction != null)
{
    Console.WriteLine($"Visual extraction: {full.VisualExtraction}");
}
```

### Update a Tag

Updates are partial — only the fields you set are changed.

```csharp
using TagShelf.Alfred.ApiWrapper.Domains.Tag.Requests;

UpdateTagResult updated = await alfred.Tag.UpdateAsync(tagId, new UpdateTagRequest
{
    Description = "Updated description",
    Disabled = false
});
```

## Search

Searches document content within the tags you can see. `BasicAsync` can be narrowed to one tag (by ID or name); `UniversalAsync` searches everything you can see. Each result carries the file's tag and its extracted data points, with each value in its typed JSON form and as text.

```csharp
using TagShelf.Alfred.ApiWrapper.Domains.Search.Requests;
using TagShelf.Alfred.ApiWrapper.Domains.Search.Results;

PagedResult<SearchResult> hits = await alfred.Search.BasicAsync(new SearchRequest
{
    Query = "RMP002-26",
    TagName = "Invoice",
    Fuzzy = true
});

PagedResult<SearchResult> everywhere = await alfred.Search.UniversalAsync("RMP002-26", page: 1, pageSize: 50);
```

## Remote Sources

A remote source is a cloud storage location (Amazon S3, DigitalOcean Spaces or Azure Blob Storage) that Alfred can read files from. Once created, refer to it by name in an upload with `UploadRequest.Source`, `Container` and `FileName`/`FileNames`. The credentials are kept in your account's secret vault and are never returned.

```csharp
using TagShelf.Alfred.ApiWrapper.Domains.RemoteSource.Requests;

CreateRemoteSourceResult created = await alfred.RemoteSource.CreateAsync(new CreateRemoteSourceRequest
{
    SourceName = "invoices-bucket",
    Type = RemoteSourceType.Aws,
    AccessKey = "AKIA...",
    SecretKey = "...",
    AccountName = "my-bucket",
    Region = "us-east-1"
});

List<RemoteSourceResult> sources = await alfred.RemoteSource.GetAllAsync();

await alfred.RemoteSource.RemoveAsync("invoices-bucket"); // by name
```

## Real-Time Events

The `AlfredRealTimeClient` is a standalone client that streams events from Alfred in real time over [Socket.IO](https://socket.io/). It authenticates with an API key and lets you react to job and file activity as it happens — no polling required.

The server forwards every event on one of two aggregate channels — `job_event` and `file_event`. The specific event type is carried in the payload's inner `event_name` field (for example `alfred_event_job_start`), exposed as `RealTimeEvent.EventName`.

### Connect and Listen to All Job and File Events

```csharp
// Imports
using TagShelf.Alfred.ApiWrapper.Core;
using TagShelf.Alfred.ApiWrapper.Enumerations;
using TagShelf.Alfred.ApiWrapper.RealTime;

// Create a real-time client (no I/O happens yet)
var realtime = AlfredFactory.CreateRealTimeClient("your-api-key", EnvironmentType.Production);

// Optional: observe the connection life-cycle
realtime.Connected    += (s, e)      => Console.WriteLine("Connected!");
realtime.Disconnected += (s, reason) => Console.WriteLine($"Disconnected: {reason}");
realtime.Error        += (s, error)  => Console.WriteLine($"Error: {error}");

// Register handlers for every job and file event
realtime.OnJobEvent(evt =>
    Console.WriteLine($"Job event: {evt.EventName} (job {evt.JobId})"));

realtime.OnFileEvent(evt =>
    Console.WriteLine($"File event: {evt.EventName}"));

// Establish the connection (throws TimeoutException if it cannot connect in time)
await realtime.ConnectAsync();

// ... keep your application alive to receive events ...

await realtime.DisconnectAsync();
realtime.Dispose();
```

### Listen to a Specific Event Type

Register a handler that fires only when the inner `event_name` matches. Well-known values are provided as constants in `JobEventNames` and `FileEventNames`.

```csharp
using TagShelf.Alfred.ApiWrapper.RealTime;

// Only fire when a job starts
realtime.On(RealTimeChannels.JobEvent, JobEventNames.JobStart, evt =>
    Console.WriteLine($"Job {evt.JobId} started"));

// Only fire when a file finishes processing
realtime.On(RealTimeChannels.FileEvent, FileEventNames.FileDone, evt =>
    Console.WriteLine($"File finished: {evt.Payload["file_name"]}"));
```

### Accessing the Payload

Each `RealTimeEvent` exposes the common discriminator fields (`EventName`, `CompanyId`, `JobId`), the full raw payload as a Newtonsoft `JObject` (`Payload`), and a `GetData<T>()` helper to deserialize into your own model. Decorate the model with `[JsonProperty("snake_case")]` attributes to map fields.

```csharp
using Newtonsoft.Json;
using TagShelf.Alfred.ApiWrapper.RealTime;

public class FileStatusPayload
{
    [JsonProperty("event_name")]        public string EventName { get; set; }
    [JsonProperty("file_status")]       public string FileStatus { get; set; }
    [JsonProperty("classification_score")] public double ClassificationScore { get; set; }
}

realtime.OnFileEvent(evt =>
{
    var data = evt.GetData<FileStatusPayload>();
    Console.WriteLine($"{data.FileStatus} ({data.ClassificationScore:P0})");
});
```

### Configuration

Pass a `RealTimeOptions` instance to tune the client. All values are optional:

| Option | Default | Description |
| --- | --- | --- |
| `RealTimeUrl` | environment default | Overrides the server URL (useful for testing). |
| `Reconnection` | `true` | Automatically reconnect when the connection drops. |
| `ReconnectionAttempts` | `int.MaxValue` | Maximum number of reconnection attempts. |
| `ReconnectionDelay` | `1000` | Delay between reconnection attempts, in milliseconds. |
| `ConnectionTimeout` | `20000` | Handshake timeout for `ConnectAsync`, in milliseconds. |
| `Verbose` | `false` | Emit diagnostic messages to the console. |

```csharp
var options = new RealTimeOptions { Verbose = true, ConnectionTimeout = 10000 };
var realtime = AlfredFactory.CreateRealTimeClient("your-api-key", EnvironmentType.Production, options);
```

## Handling Errors

Every failed call throws an `ApiException`:

| Property | Description |
| --- | --- |
| `StatusCode` | The HTTP status (`null` for a network error). For example 404 when the item is not in your account, 409 when the item is busy, 403 when your account's permissions refuse the action. |
| `ErrorMessage` | The server's explanation, when it sent one. |
| `ResponseContent` | The raw response body, for details such as the missing sources of a refused restart. |

```csharp
try
{
    await alfred.File.ReprocessAsync(fileId);
}
catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
{
    Console.WriteLine($"Busy: {ex.ErrorMessage}");
}
```

# Notes

For more information on job stages, data points and other parameters to customize your calls to Alfreds' different endpoints, refer to the [Alfred Official Documentation](https://docs.tagshelf.dev/). Ensure to handle any exceptions or errors that may occur during API calls for robust implementation.

# Contributing

Contributions to improve `TagShelf.Alfred.ApiWrapper` are welcome. Please feel free to fork the repository, make your changes, and submit a pull request for review.

# License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

# Acknowledgments

Thanks to all the contributors who invest their time into making `TagShelf.Alfred.ApiWrapper` better.
