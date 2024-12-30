using System.Text.Json;

namespace Datasync.Core
{
    public static class Deserializer
    {
        public static object Deserialize(string json)
        {
            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                JsonElement root = doc.RootElement;

                if (root.TryGetProperty("SupplierCompanyId", out _))
                {
                    return JsonSerializer.Deserialize<SupplierCompanyContext>(json)!;
                }
                else if (root.TryGetProperty("DepartmentId", out _))
                {
                    return JsonSerializer.Deserialize<Department>(json)!;
                }
                else if (root.TryGetProperty("PolicyId", out _))
                {
                    return JsonSerializer.Deserialize<Policy>(json)!;
                }
                else if (root.TryGetProperty("TowDriverId", out _))
                {
                    return JsonSerializer.Deserialize<TowDriver>(json)!;
                }
                else
                {
                    throw new InvalidOperationException("No se pudo determinar el tipo del objeto a partir del JSON.");
                }
            }
        }
    }
}
