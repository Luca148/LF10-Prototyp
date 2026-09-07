using System.Text.Json;
using LF10.Api.Models;

namespace LF10.Api.Services;

public class User
{
    private const string UserFilePath = "Data/User.json";

    private string Username;
    private string UppercaseUsername;
    private string Password;

    public bool VerifyUsername(string name)
    {
        string json = File.ReadAllText(UserFilePath);
        List<UserEntry>? users = JsonSerializer.Deserialize<List<UserEntry>>(json);

        string uppercaseName = name.Trim().ToUpperInvariant();
        UserEntry? match = users?.FirstOrDefault(u => u.UsernameUpper == uppercaseName);

        if (match is null)
        {
            return false;
        }

        Username = match.DisplayName;
        UppercaseUsername = match.UsernameUpper;
        Password = match.Password;
        return true;
    }
    public bool VerifyPassword(string password)
    {
        return PasswordHasher.VerifyPassword(password, Password);
    }
    public void SetUsername(string Name)
    {
        Username = Name;
        UppercaseUsername = Username.Trim().ToUpperInvariant();
    }

    public bool Register(string name, string password)
    {
        string json = File.ReadAllText(UserFilePath);
        List<UserEntry> users = JsonSerializer.Deserialize<List<UserEntry>>(json) ?? [];

        string uppercaseName = name.Trim().ToUpperInvariant();
        if (users.Any(u => u.UsernameUpper == uppercaseName))
        {
            return false;
        }

        users.Add(new UserEntry
        {
            DisplayName = name.Trim(),
            UsernameUpper = uppercaseName,
            Password = PasswordHasher.HashPassword(password)
        });

        File.WriteAllText(UserFilePath, JsonSerializer.Serialize(users));
        return true;
    }
}