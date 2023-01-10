using System.Text.Json;
using Microsoft.Extensions.Options;
using OnlineShop.Configuration;

namespace OnlineShop.Services;

public class FileStorageService
{
    private readonly IOptions<DatabaseConfiguration> _databaseConfigOptions;

    public FileStorageService(IOptions<DatabaseConfiguration> databaseConfigOptions)
    {
        _databaseConfigOptions = databaseConfigOptions;
        FileStorage = JsonSerializer.Deserialize<FileStorage>(_databaseConfigOptions.Value.FileStorageLocation) ??
                      throw new Exception();
    }

    public async Task SaveChangesAsync()
    {
        var text = JsonSerializer.Serialize(FileStorage);
        await File.WriteAllTextAsync(_databaseConfigOptions.Value.FileStorageLocation, text);
    }

    public FileStorage FileStorage { get; private set; }
}

public class FileStorage
{
    public required string ActiveDb { get; set; }
}