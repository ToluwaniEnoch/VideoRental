namespace Api.Services.Infrastructure
{
    public interface IOtpGenerator
    {
        string Generate(string key, int expireTimeMinutes = 2, int digitsCount = 6);

        bool Verify(string key, string token, int expireTimeMinutes = 2, int digitsCount = 6);
    }
}