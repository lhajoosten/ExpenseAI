using Microsoft.Extensions.Options;
using ExpenseAI.Application.Interfaces;

namespace ExpenseAI.Infrastructure.Storage;

public class AzureBlobStorageService : IFileStorageService
{
    private readonly BlobStorageSettings _settings;

    public AzureBlobStorageService(IOptions<BlobStorageSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task<string> UploadFileAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // For now, this is a mock implementation
            // In production, you would use Azure.Storage.Blobs package
            await Task.Delay(100, cancellationToken);

            var mockUrl = $"https://mockstorageaccount.blob.core.windows.net/files/{GenerateUniqueBlobName(fileName)}";
            return mockUrl;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to upload file {fileName}: {ex.Message}", ex);
        }
    }

    public async Task<Stream> DownloadFileAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        try
        {
            // Mock implementation
            await Task.Delay(100, cancellationToken);
            return new MemoryStream();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to download file from {fileUrl}: {ex.Message}", ex);
        }
    }

    public async Task DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        try
        {
            // Mock implementation
            await Task.Delay(100, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to delete file {fileUrl}: {ex.Message}", ex);
        }
    }

    private static string GenerateUniqueBlobName(string originalFileName)
    {
        var extension = Path.GetExtension(originalFileName);
        var nameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        var timestamp = DateTimeOffset.UtcNow.ToString("yyyyMMdd");

        return $"{timestamp}/{nameWithoutExtension}_{uniqueId}{extension}";
    }
}

public class BlobStorageSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string ContainerName { get; set; } = "expenseai-files";
}
