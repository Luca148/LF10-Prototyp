public class User
{
    private string Username;
    private string UppercaseUsername;
    private string Password;

    public bool VerifyUsername(string name)
    {
        
    }
    public void SetUsername(string Name)
    {
        Username = Name;
        UppercaseUsername = Username.Trim().ToUpperInvariant();
    }
}