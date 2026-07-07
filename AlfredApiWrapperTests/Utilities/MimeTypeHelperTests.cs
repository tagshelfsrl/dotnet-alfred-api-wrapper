using TagShelf.Alfred.ApiWrapper.Utilities;
using Xunit;

namespace AlfredApiWrapperTests.Utilities;

public class MimeTypeHelperTests
{
    [Theory]
    [InlineData("document.pdf", "application/pdf")]
    [InlineData("scan.tif", "image/tiff")]
    [InlineData("scan.tiff", "image/tiff")]
    [InlineData("archive.zip", "application/zip")]
    [InlineData("photo.jpg", "image/jpeg")]
    [InlineData("photo.jpeg", "image/jpeg")]
    [InlineData("screenshot.png", "image/png")]
    [InlineData("animation.gif", "image/gif")]
    [InlineData("bitmap.bmp", "image/bmp")]
    public void GetMimeType_ReturnsExpectedType_ForKnownExtensions(string fileName, string expected)
    {
        Assert.Equal(expected, MimeTypeHelper.GetMimeType(fileName));
    }

    [Theory]
    [InlineData("PHOTO.JPG", "image/jpeg")]
    [InlineData("Document.PDF", "application/pdf")]
    [InlineData("SCAN.TIFF", "image/tiff")]
    public void GetMimeType_IsCaseInsensitive(string fileName, string expected)
    {
        Assert.Equal(expected, MimeTypeHelper.GetMimeType(fileName));
    }

    [Theory]
    [InlineData("unknown.xyz")]
    [InlineData("noextension")]
    [InlineData("")]
    [InlineData(null)]
    public void GetMimeType_ReturnsOctetStream_ForUnknownOrMissingExtension(string fileName)
    {
        Assert.Equal("application/octet-stream", MimeTypeHelper.GetMimeType(fileName));
    }

    [Fact]
    public void GetMimeType_HandlesFullPathsWithDirectories()
    {
        Assert.Equal("application/pdf", MimeTypeHelper.GetMimeType("/data/files/report.pdf"));
    }
}
