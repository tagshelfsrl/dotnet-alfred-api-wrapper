# Overview

The `TagShelf.Alfred.ApiWrapper` is a comprehensive .NET library designed to facilitate seamless interactions with the Alfred API. It's tailored to support a wide range of .NET applications, from modern .NET Core and .NET Standard projects to legacy .NET Framework 4.7.2 applications, providing a robust, strongly-typed interface for efficient development.

It provides a robust, strongly-typed interface for efficient development, with specialized domains for handling Deferred Sessions, Files, Jobs and Data Points.

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

Alfred supports three authentication methods: OAuth, HMAC, and App API Key. Each method is suited for different scenarios. To obtain the necessary credentials for the following authentication methods, please refer to the [Alfred API documentation](https://docs.tagshelf.dev/authentication) or contact the Alfred support team.

|OAuth||
|--|--|
| **When to Use**   | Ideal for web portals needing integration with document parsers or API-driven services, especially when users access through browsers. Suitable for short-lived browser sessions and scenarios requiring scoped, secure access.|
| **Benefits**      | Enhanced security via token use, flexible access control, and a user-friendly experience with periodic re-authentication.|
| **Best Practices**| Secure token storage and graceful handling of token expiration.|

To initialize the client for OAuth authentication, specify the credentials explicitly:

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

|App API Key||
|--|--|
| **When to Use**    | Recommended for server-to-server communication, automated processes, and background services where user interaction is not feasible.|
| **Benefits**       | Non-interactive yet robust authentication, simplicity, and easy access control.|
| **Best Practices** | Secure storage of API keys, apply the principle of least privilege, and regularly rotate keys to minimize compromise risks.|

To initialize the client for API key authentication, provide the API key:

```csharp
var alfred = await AlfredFactory.CreateWithApiKeyAsync("api_key", EnvironmentType.Staging);
```

# Getting Started

This library covers 6 different domains: 
- [Tagshelf](#tagshelf-tagshelf-domain)
- [Deferred Sessions](#deferred-sessions-deferred-sessions-domain)
- [Files](#files-files-domain)
- [Jobs](#jobs-jobs-domain)
- [Data Points](#data-points-data-points-domain)
- [Account](#account-account-domain)

## Tagshelf {#tagshelf-domain}

The Tagshelf API namespace serves as a dedicated domain within the API for platform status and identity queries. This namespace is tailored to provide essential functionalities related to the operational status and user context of the platform. It encompasses endpoints that are crucial for system health checks, user identity verification, and basic connectivity tests. 

### Check Platform Operational Status

This endpoint offers a quick way to check the overall operational status of the platform.

```csharp
// Authentication
var alfred = await AlfredFactory.CreateWithOAuthAsync("admin@tagshelf.com", "T@gSh3lf", EnvironmentType.Staging);

// Get the Platform Operational Status
TagshelfStatusResult status = await alfred.Tagshelf.StatusAsync();

// Print the Result
Console.WriteLine(status);
```
**Result**
```cmd
ApiVersion: 1.47.6.0, DbStatus: online
```

### Retrieve User Identity Information

This endpoint provides information about the currently authenticated user.

```csharp
// Authentication
var alfred = await AlfredFactory.CreateWithOAuthAsync("admin@tagshelf.com", "T@gSh3lf", EnvironmentType.Staging);

// Get the Platform Operational Status
WhoAmIResult whoami = await alfred.Tagshelf.WhoAmIAsync();

// Print the Result
Console.WriteLine(whoami);
```
**Result**
```cmd
UserName: admin@tagshelf.com, Name: Admin Tagshelf, Roles: Admin, Developer, Status: active, Company: TAGSHELF, App: , AppId: 00000000-0000-0000-0000-000000000000, RequestOriginIpAddress: 152.0.135.113:20845
```

### Conduct a Basic Connectivity Test

The `ping` endpoint is used to perform a basic connectivity test to the platform.

```csharp
// Authentication
var alfred = await AlfredFactory.CreateWithOAuthAsync("admin@tagshelf.com", "T@gSh3lf", EnvironmentType.Staging);

// Get the Platform Operational Status
PingResult ping = await alfred.Tagshelf.PingAsync();

// Print the Result
Console.WriteLine(ping);
```
**Result**
```cmd
Pong: admin@tagshelf.com
```

## Deferred Sessions {#deferred-sessions-domain}

A Deferred Session in Alfred is a mechanism designed for asynchronous file uploads, integral to the document processing workflow. It serves as a container or grouping for files that are uploaded at different times or from various sources, but are all part of a single processing task or Job.

### Create a new Deferred Session





### Retrieve Details of a Specific Deferred Session






## Files {#files-domain}

In Alfred, a file is an individual document or data unit undergoing specialized operations tailored for document analysis and management. It is processed by the platform for tasks such as classification, optical character recognition (OCR), and data extraction. Each file, as a distinct entity, transitions through various statuses, which mark its progress in the automated workflow. 










## Jobs {#jobs-domain}

## Data Points {#data-points-domain}

## Account {#account-domain}



## File Operations

With the Alfred client initialized, you can perform various API operations, including file uploads.

### Upload File (From URL or URLs)

This method allows you to upload a file from one or multiple URLs to Alfred. During the upload, you can specify metadata for the file. Note that this method triggers job processing automatically.

| Parameter | Type | Description |
| --- | --- | --- |
| Url | string | URL of the file to upload (use url, when you have an URl to single remote file.)|
|Urls | string[] | URLs of the files to upload. (Use urls, when you have URl's for multiple remote files. The current limit for this parameter is **100 elements**.) |
| Source | string | Configured object storage source name. Ideal for referring to files hosted in existing cloud containers. When used, **file_name** and **container** are required. |
|Container | string | Virtual container where the referenced remote file is located. When used, **source** and **file_name** are required.|
| Filename | string | Unique name of the file within an object storage source. When used, **source** and **container** are required.|
| Filenames | string[] | Array of unique names of the files within an object storage source. When used, **source** and **container** are required.|
| Merge | boolean | Boolean value [true/false] - When set to true, will merge all of the remote files into a single PDF file. All of the remote files MUST be images. </br></br>By default this field is set to **false**. |
| Metadata | string | JSON object or JSON array of objects containing metadata fields for a given remote file. </br></br>When merge field is set to **false**:</br></br>When using the urls field this should be a JSON object array that matches the urls field array length.</br></br>When using the url field the metadata field should be a JSON object.</br></br>When the merge field is set to true: The metadata field should be a JSON object.|
| PropagateMetadata | boolean | This parameter enables the specification of a single metadata object to be applied across multiple files from remote URLs or remote sources. When used, `propagate_metadata` ensures that the defined metadata is consistently attached to all the specified files during their upload and processing. This feature is particularly useful for maintaining uniform metadata across a batch of files, streamlining data organization and retrieval. |
| ParentFilePrefix | string | The `parent_file_prefix` parameter is used to specify a virtual folder destination for the uploaded files, diverging from the default 'Inbox' folder. By setting this parameter, users can organize files into specific virtual directories, enhancing file management and accessibility within Alfred's system. |

#### Example: Upload File from URL

```csharp
// Create a request object with the necessary parameters
UploadRequest uploadRequest = new UploadRequest
{
   Urls = new List<string> { "https://example.com/file1.pdf", "https://example.com/file2.pdf" },
   FileNames = new List<string> { "file1.pdf", "file2.pdf" }
};

// Upload remote file
var uploadResult = await alfred.File.UploadAsync(uploadRequest);

// Handle the response
Console.WriteLine(uploadResult.JobId);

// Get job status
var jobStatus = await alfred.Job.GetAsync(uploadResult.JobId);

// Check job completion
while (jobStatus.Stage != "completed")
{
   System.Threading.Thread.Sleep(1000);
   jobStatus = await alfred.Job.GetAsync(uploadResult.JobId);
}

// Retrieve data points for each file
var files = jobStatus.Files;
foreach (var file in files)
{
   var dataResult = await alfred.DataPoint.GetValuesAsync(file.FileId);
   Console.WriteLine(dataResult);
}

```

### Upload File from Stream

This method uploads a file from a stream to Alfred and associates it with a specific session ID. Unlike other methods, it does not trigger job processing, making it ideal for uploading files that are part of a larger job.

| Parameter | Type | Description |
| --- | --- | --- |
| FileStream | Stream | Stream of the file to upload. |
| Filename | string | Unique name of the file within an object storage source. |
| SessionId | Guid | Session ID to associate with the file. |
| Metadata | string | JSON object or JSON array of objects containing metadata fields for a given remote file. |

#### Example: Upload File from Stream

```csharp
// Create a session ID
Guid sessionId = (await alfred.DeferredSession.CreateAsync()).SessionId;

// Create a request object with the necessary parameters
UploadFileRequest uploadFileRequest = new UploadFileRequest
{
   FileStream = new StreamReader("file_path\\sample.pdf").BaseStream,
   FileName = "sample.pdf",
   SessionId = sessionId,
   Metadata = {}
};

// Upload file from stream
var response = await alfred.File.UploadFileAsync(uploadFileRequest);

```

## Job Operations 

### Trigger Job Processing

To process uploaded files, you must trigger job processing using the Job domain.

| Parameter | Type | Description |
| --- | --- | --- |
| SessionId | string | Session ID |
| Metadata | any | Metadata of the job |
| PropagateMetadata | boolean | If `true` ensures that the provided metadata at the Job level is attached to all the specified Files. |
| Merge | boolean | If `true`, when all provided Files are either images or PDFs, the system combines them into a single file for the purpose of processing. |
| Decompose | boolean | If `true`, when the provided File is a PDF, the system will decompose it into individual pages for processing. |
| Channel | string | Channel |
| ParentFilePrefix | string | The `parent_file_prefix` parameter is used to specify a virtual folder destination for the uploaded files, diverging from the default 'Inbox' folder. By setting this parameter, users can organize files into specific virtual directories, enhancing file management and accessibility within Alfred's system. |
| PageRotation | number | Page rotation |
| Container | string | Virtual container where the referenced remote file is located.|
| Filename | string | Unique name of the file within an object storage source.|
| Filenames | string[] | Array of unique names of the files within an object storage source.|

#### Example: Trigger Job Processing

```csharp
// Trigger job processing
Guid jobId = (await alfred.Job.CreateAsync(new CreateJobRequest { SessionId = sessionId })).JobId;

// Get job status
var jobStatus = await alfred.Job.GetAsync(jobId);

// Check job completion
while (jobStatus.Stage != "completed")
{
   System.Threading.Thread.Sleep(1000);
   jobStatus = await alfred.Job.GetAsync(jobId);
}

// Check if the job is completed if not, wait for it to complete
while (jobstatus != "completed")
{
   // Sleep for 1 second to avoid hitting the API rate limit
   System.Threading.Thread.Sleep(1000);
   jobstatus = alfred.Job.GetAsync(jobId).Result.Stage;
}

```

## Data Point Operations

### Retrive Data Points

After job completion, retrieve data points for each file using the DataPoint domain.

#### Example: Retrieve Data Points

```csharp
// Get the files to retrieve the data points
var files = alfred.Job.GetAsync(jobId).Result.Files;

// Get the data points for each file
foreach (var file in files)
{
   var dataResult = alfred.DataPoint.GetValuesAsync(file.FileId).Result;
   Console.WriteLine(dataResult);
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
