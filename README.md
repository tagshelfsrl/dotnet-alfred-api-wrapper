# Overview

The `TagShelf.Alfred.ApiWrapper` is a comprehensive .NET library designed to facilitate seamless interactions with the Alfred API. It's tailored to support a wide range of .NET applications, from modern .NET Core and .NET Standard projects to legacy .NET Framework 4.7.2 applications, providing a robust, strongly-typed interface for efficient development.

It provides a robust, strongly-typed interface for efficient development, with specialized domains for handling Tags, Jobs, Files, Deferred Sessions, and Data Points.

## Features

- **Comprehensive Authentication Support**: Seamlessly handles OAuth, HMAC, and API key authentication methods, simplifying the process of connecting to the Alfred API.
- **Domain-Specific Operations**: Offers specialized support for File and Job operations, enabling developers to intuitively manage and interact with API resources.
- **Cross-Platform Compatibility**: Designed to be fully compatible across .NET Core, .NET Standard, and .NET Framework 4.7.2, ensuring broad usability in diverse development environments.
- **Minimal Dependencies**: Crafted to minimize external dependencies, facilitating an easier integration and deployment process with reduced conflict risk.

## Prerequisites

- .NET Core 2.1+ or .NET Framework 4.7.2+ installed on your development machine.
- An active Alfred API key, HMAC credentials, or OAuth credentials for authenticating API requests.

## Installation

To integrate the Alfred API wrapper into your .NET project, install the package via NuGet:

```bash
dotnet add package TagShelf.Alfred.ApiWrapper
```

Alternatively, you can install the package using the NuGet Package Manager in Visual Studio. Search for `TagShelf.Alfred.ApiWrapper` and click the Install button.

For .NET Core CLI, use the following command:

```bash
Install-Package TagShelf.Alfred.ApiWrapper
```

## Usage

### Initialize the Client

   Begin by creating an instance of the Alfred client using your preferred authentication method along with the desired environment type (Production or Staging). The following examples demonstrate how to initialize the client with different authentication methods:

#### Environments description

- **Production**: The production environment are used for live applications and real-world scenarios. It is recommended to use this environment for production-ready applications. The frontend URL for the production environment is [https://app.tagshelf.com](https://app.tagshelf.com)</br></br>

- **Staging**: The staging environment is used for testing and development purposes. It is recommended to use this environment for testing and development scenarios. The frontend URL for the staging environment is [https://staging.tagshelf.com](https://staging.tagshelf.com)</br></br>

#### Authentication Methods

The following examples demonstrate how to initialize the Alfred client with different authentication methods:

   For OAuth authentication, specify the method and credentials explicitly and provide the environment type (Production or Staging):

   ```csharp
   var alfred = await AlfredFactory.CreateWithOAuthAsync("username", "password", EnvironmentType.Production);
   ```

   For HMAC authentication, provide the secret key, public key, and environment type (Production or Staging):

   ```csharp
   var alfred = await AlfredFactory.CreateWithHmacAsync("secret_key", "public_key", EnvironmentType.Staging);
   ```

   For API key authentication, use the following method with the API key and environment type (Production or Staging):

   ```csharp
   var alfred = await AlfredFactory.CreateWithApiKeyAsync("api_key", EnvironmentType.Staging);
   ```

### File

With the client initialized, you're ready to perform API operations

These steps demonstrate how to upload a file to Alfred using the `File` domain:

1. Initialize the Alfred client.
2. Choose the method to upload the file (from URL or stream).
3. Create a request object with the necessary parameters.
4. Call the appropriate method to upload the file.
5. Handle the response accordingly.(The response will contain the file ID or the job ID, depending on the method used.)

#### Upload File (From URL or URLs)

This method allows you to upload a file from a URL or multiple URLs to Alfred. You can also specify metadata for the file during the upload process you have to take into account that this method will automatically trigger the job processing.

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

**Example:**

   ```csharp
   // Upload remote file
   var uploadResult = await alfred.File.UploadAsync(new FileUploadRequest
   {
       Urls = new List<string> { "https://pdfobject.com/pdf/sample.pdf" },
       Metadata = "{'DocumentType':'Invoice'}",       
   });   
   ```

#### Upload File from Stream

This method allows you to upload a file from a stream to Alfred and associate it with a session ID this method will not trigger the job processing this method is ideal for uploading files that are part of a larger job.

   | Parameter | Type | Description |
| --- | --- | --- |
| FileStream | Stream | Stream of the file to upload. |
| Filename | string | Unique name of the file within an object storage source. |
| SessionId | Guid | Session ID to associate with the file. |
| Metadata | string | JSON object or JSON array of objects containing metadata fields for a given remote file. |

**Example:**

   ```csharp
   // Upload file from stream
   Stream stream = new StreamReader("file_path\\sample.pdf").BaseStream;
Guid sessionId= (await alfred.DeferredSession.CreateAsync()).SessionId;

var response = await alfred.File.UploadFileAsync(new UploadFileRequest { FileStream = stream, FileName = "sample.pdf", SessionId = sessionId, Metadata = { } });
Console.WriteLine(response);
   ```

## Alfred

Alfred is a powerful document processing platform that enables you to extract, index, and search through large document collections with ease. It offers a wide range of features, including:

- **Indexing**: Powerful indexing engine that can index and search through millions of documents in seconds.

- **Extraction**: Can extract specific data from PDFs, images, and other documents with ease using its powerful extraction engine.

- **Tagging**: Tag documents based on their content, making it easy to organize and search through large document collections.

- **Job Management**: Provides a robust job management system that allows you to schedule and monitor document processing jobs.

- **Integration**: Alfred can be easily integrated into your existing applications using its powerful API and SDKs.

- **Scalability**: Alfred is designed to scale with your needs, whether you're processing thousands of documents a day or millions.

## Contributing

Contributions to improve `TagShelf.Alfred.ApiWrapper` are welcome. Please feel free to fork the repository, make your changes, and submit a pull request for review.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Thanks to all the contributors who invest their time into making `TagShelf.Alfred.ApiWrapper` better.
