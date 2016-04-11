namespace Tomataboard.Services.Greetings
{
    public interface IGreetingsService
    {
        string GetGreeting(long milliseconds);
    }
}
