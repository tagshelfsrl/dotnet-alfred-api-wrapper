using TagShelf.Alfred.ApiWrapper.Core;
using TagShelf.Alfred.ApiWrapper.Enumerations;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Starting Alfred API Wrapper Test...");

        try
        {
            // Initialize Alfred with API key (assuming API key authentication is implemented)
            var alfred = new Alfred("your_api_key", EnvironmentType.Production);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}
