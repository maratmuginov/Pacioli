using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pacioli.Lib.Shared.Helpers
{
    public static class JsonHelper
    {
        public static async Task<T> DeserializeFileAsync<T>(string filePath)
        {
            await using var stream = File.OpenRead(filePath);
            return await JsonSerializer.DeserializeAsync<T>(stream);
        }
    }
}
