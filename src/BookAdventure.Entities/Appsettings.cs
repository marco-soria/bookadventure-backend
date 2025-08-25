namespace BookAdventure.Entities;

public class AppSettings
{
    public Jwt Jwt { get; set; } = new();
    public SmtpConfiguration SmtpConfiguration { get; set; } = new();
}

public class Jwt
{
    public string JwtKey { get; set; } = string.Empty;
    public int LifetimeInSeconds { get; set; } = 86400;
}

public class SmtpConfiguration
{
    public string UserName { get; set; } = string.Empty;
    public string Server { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int PortNumber { get; set; } = 587;
    public string FromName { get; set; } = string.Empty;
    public bool EnableSsl { get; set; } = true;
}