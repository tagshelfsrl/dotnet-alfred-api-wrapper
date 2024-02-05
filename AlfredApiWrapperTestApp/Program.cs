using AlfredApiWrapper.Domains.File.Requests;
using Newtonsoft.Json;
using TagShelf.Alfred.ApiWrapper.Core;
using TagShelf.Alfred.ApiWrapper.Domains.Job.Requests;
using TagShelf.Alfred.ApiWrapper.Enumerations;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Starting Alfred API Wrapper Test...");

        try
        {
            // Create an Alfred instance with an API key from the environment
            var alfred = await AlfredFactory.CreateWithApiKeyFromEnvironmentAsync(EnvironmentType.Staging);

            // Get the current user information
            var whoAmI = await alfred.Tagshelf.WhoAmIAsync();            
            Console.WriteLine($"Alfred who am I: {whoAmI}");
            
            // Create a deferred session
            var createSessionResult = await alfred.DeferredSession.CreateAsync();
            Console.WriteLine($"Deferred session created with ID: {createSessionResult.SessionId}");
            
            // Get the details of the deferred session
            var SessionDetailResult = await alfred.DeferredSession.GetAsync(createSessionResult.SessionId);
            Console.WriteLine($"Deferred session details: {SessionDetailResult}");

            // Load filestream and file name from local file in project directory
            var fileName = "test.png";
            var fileStream = File.OpenRead(fileName);
            
            // Upload a file
            var uploadFileRequest = new UploadFileRequest
            {
                FileStream=fileStream,
                FileName=fileName,
                SessionId = createSessionResult.SessionId,
                Metadata = JsonConvert.SerializeObject(new
                {
                    name = "Test file",
                    description = "This is a test file"
                })
            };

            // Create a job request
            var createJobRequest = new CreateJobRequest
            {
                SessionId = createSessionResult.SessionId
            };

            // Create a job
            var createJobResult = await alfred.Job.CreateAsync(createJobRequest);
            Console.WriteLine($"Job created with ID: {createJobResult.JobId}");

            // Get the details of the job
            var jobDetailResult = await alfred.Job.GetAsync(createJobResult.JobId);
            Console.WriteLine($"Job details: {jobDetailResult}");
        }
        catch(ApiException ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            Console.WriteLine($"API error: {ex.ResponseContent}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}
