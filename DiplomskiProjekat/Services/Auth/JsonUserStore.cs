using DiplomskiProjekat.Models.Auth;
using System.Text.Json;

namespace DiplomskiProjekat.Services.Auth
{
    public class JsonUserStore : IUserStore
    {
        private readonly string _filePath;
        private static readonly SemaphoreSlim _lock = new(1, 1);

        public JsonUserStore(IWebHostEnvironment env)
        {
            var dataDir = Path.Combine(env.ContentRootPath, "Data");
            Directory.CreateDirectory(dataDir);
            _filePath = Path.Combine(dataDir, "users.json");

            if (!File.Exists(_filePath))
                File.WriteAllText(_filePath, "[]");
        }

        public async Task<User?> FindByEmailAsync(string email)
        {
            var normalized = (email ?? "").Trim().ToLowerInvariant();

            var users = await ReadAllAsync();
            return users.FirstOrDefault(u =>
                (u.Email ?? "").Trim().ToLowerInvariant() == normalized
            );
        }


        public async Task<User> CreateAsync(User user)
        {
            await _lock.WaitAsync();
            try
            {
                var users = await ReadAllInternalAsync();
                users.Add(user);
                await File.WriteAllTextAsync(_filePath, JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true }));
                return user;
            }
            finally
            {
                _lock.Release();
            }
        }

        private async Task<List<User>> ReadAllAsync()
        {
            await _lock.WaitAsync();
            try { return await ReadAllInternalAsync(); }
            finally { _lock.Release(); }
        }

        private async Task<List<User>> ReadAllInternalAsync()
        {
            var json = await File.ReadAllTextAsync(_filePath);
            return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
        }
    }
}
