# Overview

The `TagShelf.Alfred.ApiWrapper` is a comprehensive .NET library designed to facilitate seamless interactions with the Alfred API. It's tailored to support a wide range of .NET applications, from modern .NET Core and .NET Standard projects to legacy .NET Framework 4.7.2 applications, providing a robust, strongly-typed interface for efficient development. It includes specialized domains for handling [Deferred Sessions](#deferred-sessions), [Files](#files), [Jobs](#jobs) and [Data Points](#data-points).

This library serves as a wrapper for our HTTP API, which you can find the documentation for in this link https://docs.tagshelf.dev/

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

To initialize the client for HMAC authentication, provide the secret key and public key:

```csharp
var alfred = await AlfredFactory.CreateWithHmacAsync("secret_key", "public_key", EnvironmentType.Production);
```

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

This library covers 6 different domains: 
- [Tagshelf](#tagshelf)
- [Deferred Sessions](#deferred-sessions)
- [Files](#files)
- [Jobs](#jobs)
- [Data Points](#data-points)
- [Account](#account)

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

# Notes

For more information on job stages, data points and other parameters to customize your calls to Alfreds' different endpoints, refer to the [Alfred Official Documentation](https://docs.tagshelf.dev/). Ensure to handle any exceptions or errors that may occur during API calls for robust implementation.

# Contributing

Contributions to improve `TagShelf.Alfred.ApiWrapper` are welcome. Please feel free to fork the repository, make your changes, and submit a pull request for review.

# License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

# Acknowledgments

Thanks to all the contributors who invest their time into making `TagShelf.Alfred.ApiWrapper` better.
