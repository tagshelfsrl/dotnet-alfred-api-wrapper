# TagShelf.Alfred.ApiWrapper

The `TagShelf.Alfred.ApiWrapper` is a comprehensive .NET library designed to facilitate seamless interactions with the Alfred API. It's tailored to support a wide range of .NET applications, from modern .NET Core and .NET Standard projects to legacy .NET Framework 4.7.2 applications, providing a robust, strongly-typed interface for efficient development.

It provides a robust, strongly-typed interface for efficient development, with specialized domains for handling Tags, Jobs, Files, Deferred Sessions, and Data Points.

## Features

- **Comprehensive Authentication Support**: Seamlessly handles OAuth, HMAC, and API key authentication methods, simplifying the process of connecting to the Alfred API.
- **Domain-Specific Operations**: Offers specialized support for File and Job operations, enabling developers to intuitively manage and interact with API resources.
- **Cross-Platform Compatibility**: Designed to be fully compatible across .NET Core, .NET Standard, and .NET Framework 4.7.2, ensuring broad usability in diverse development environments.
- **Minimal Dependencies**: Crafted to minimize external dependencies, facilitating an easier integration and deployment process with reduced conflict risk.

## Getting Started

### Prerequisites

- .NET Core 2.1+ or .NET Framework 4.7.2+ installed on your development machine.
- An active Alfred API key for authentication.

### Installation

To integrate the Alfred API wrapper into your .NET project, install the package via NuGet:

```bash
dotnet add package TagShelf.Alfred.ApiWrapper
```

### Usage

1. **Initialize the Client**

   Begin by creating an instance of the Alfred client using your preferred authentication method:

   ```csharp
   var alfred = await AlfredFactory.CreateWithApiKeyFromEnvironmentAsync(EnvironmentType.Staging);
   ```

   For OAuth authentication, specify the method and credentials explicitly:

   ```csharp
   var alfred = await AlfredFactory.CreateWithOAuthAsync("username", "password", EnvironmentType.Production);
   ```

2. **Executing Operations**

   With the client initialized, you're ready to perform API operations. Access the File and Job domains as follows:

   ```csharp
   // Upload remote file
   var uploadResult = await alfred.File.UploadAsync(new FileUploadRequest
   {
       Urls = new List<string> { "http://example.com/file.pdf" },
       Metadata = "{'DocumentType':'Invoice'}",       
   });   
   ```

## Contributing

Contributions to improve `TagShelf.Alfred.ApiWrapper` are welcome. Please feel free to fork the repository, make your changes, and submit a pull request for review.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Thanks to all the contributors who invest their time into making `TagShelf.Alfred.ApiWrapper` better.
