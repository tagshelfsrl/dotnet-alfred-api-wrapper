using System.Collections.Generic;
using AlfredApiWrapper.Domains.File.Requests;
using TagShelf.Alfred.ApiWrapper.Domains.Account.Results;
using TagShelf.Alfred.ApiWrapper.Domains.DeferredSession.Results;
using TagShelf.Alfred.ApiWrapper.Domains.File;
using TagShelf.Alfred.ApiWrapper.Domains.File.Results;
using TagShelf.Alfred.ApiWrapper.Domains.Job.Requests;
using TagShelf.Alfred.ApiWrapper.Domains.Job.Results;
using TagShelf.Alfred.ApiWrapper.Domains.Tagshelf.Results;
using TagShelf.Alfred.ApiWrapper.Enumerations;

namespace AlfredApiWrapperTests.Dtos;

public class DtoTests
{
    [Fact]
    public void ResultToStrings_IncludeKeyFields()
    {
        var id = Guid.NewGuid();

        Assert.Equal(id.ToString(), new WebhookSetupResult { Id = id }.ToString());
        Assert.Equal(id.ToString(), new CreateJobResult { JobId = id }.ToString());
        Assert.Contains("Job ID", new FileUploadResult { JobId = id }.ToString());
        Assert.Contains("File ID", new UploadFileResult { FileId = id }.ToString());
        Assert.Contains("SessionId", new CreateSessionResult { SessionId = id }.ToString());

        Assert.Contains("invoice.pdf", new JobFileDetail
        {
            FileName = "invoice.pdf", TagName = "invoice", Status = "finished"
        }.ToString());

        Assert.Contains("classification", new JobDetailResult
        {
            Id = id, UserName = "raul", Stage = "classification"
        }.ToString());

        Assert.Contains("doc.pdf", new FileDetailResult
        {
            Id = id, FileName = "doc.pdf", Status = "finished", ContentType = "application/pdf", FileSize = 100
        }.ToString());

        Assert.Contains("open", new SessionDetailResult
        {
            Id = id.ToString(), Status = "open", UserName = "raul", CompanyId = "c1"
        }.ToString());

        Assert.Contains("Pong", new PingResult { Pong = "raul@tagshelf.com" }.ToString());
        Assert.Contains("1.0.0", new TagshelfStatusResult { ApiVersion = "1.0.0", DbStatus = "ok" }.ToString());
    }

    [Fact]
    public void WhoAmIResult_ToString_JoinsRoles()
    {
        var who = new WhoAmIResult
        {
            UserName = "raul",
            Name = "Raul",
            LastName = "Roa",
            Roles = new List<string> { "admin", "user" },
            Status = "active",
            CompanyName = "Tagshelf"
        };

        var text = who.ToString();

        Assert.Contains("raul", text);
        Assert.Contains("admin, user", text);
    }

    [Fact]
    public void FileResult_PlainPoco_HoldsValues()
    {
        var result = new FileResult { FileId = Guid.NewGuid(), FileName = "a.pdf", Size = 1234 };

        Assert.Equal("a.pdf", result.FileName);
        Assert.Equal(1234, result.Size);
    }

    [Fact]
    public void FileDownloadResult_PlainPoco_HoldsValues()
    {
        using var stream = new MemoryStream(new byte[] { 9 });
        var result = new FileDownloadResult
        {
            ContentType = "application/pdf",
            FileName = "a.pdf",
            FileContents = stream
        };

        Assert.Equal("application/pdf", result.ContentType);
        Assert.Equal("a.pdf", result.FileName);
        Assert.Same(stream, result.FileContents);
    }

    [Fact]
    public void CreateJobRequest_HasSensibleDefaults()
    {
        var request = new CreateJobRequest();

        Assert.Equal("library", request.Channel);
        Assert.Equal(JobPriority.Normal, request.JobPriority);
    }

    [Fact]
    public void UploadRequest_HasSensibleDefaults_AndHoldsValues()
    {
        var request = new UploadRequest
        {
            Url = "https://files.test/a.pdf",
            Urls = new List<string> { "https://files.test/b.pdf" },
            Source = "web",
            Container = "docs",
            FileName = "a.pdf",
            FileNames = new List<string> { "a.pdf" },
            Metadata = new { k = "v" },
            ParentFilePrefix = "p",
            PropagateMetadata = true
        };

        Assert.False(request.Merge);
        Assert.Equal(JobPriority.Normal, request.JobPriority);
        Assert.Equal("web", request.Source);
        Assert.True(request.PropagateMetadata);
    }

    [Fact]
    public void UploadFileRequest_HoldsValues()
    {
        var sessionId = Guid.NewGuid();
        using var stream = new MemoryStream();
        var request = new UploadFileRequest
        {
            FileStream = stream,
            FileName = "a.pdf",
            SessionId = sessionId,
            Metadata = "{}",
            ContentType = "application/pdf"
        };

        Assert.Equal("a.pdf", request.FileName);
        Assert.Equal(sessionId, request.SessionId);
        Assert.Equal("application/pdf", request.ContentType);
    }

    [Theory]
    [InlineData(JobPriority.Low)]
    [InlineData(JobPriority.Normal)]
    [InlineData(JobPriority.High)]
    public void JobPriority_AllValuesAreDefined(JobPriority priority)
    {
        Assert.True(Enum.IsDefined(typeof(JobPriority), priority));
    }
}
