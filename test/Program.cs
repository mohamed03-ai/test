using DotNetEnv;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using System;
using System.IO;


namespace DataverseConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get project root path
            string projectRoot = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;

            // Load the .env file
            Env.Load(Path.Combine(projectRoot, ".env"));

            string environmentUrl = Env.GetString("ENVIRONMENT_URL");
            string clientId = Env.GetString("CLIENT_ID");
            string clientSecret = Env.GetString("CLIENT_SECRET");
            string tenantId = Env.GetString("TENANT_ID");
            Console.WriteLine("haja"+environmentUrl);

            var connectionString = $@"
                AuthType=ClientSecret;
                Url={environmentUrl};
                ClientId={clientId};
                ClientSecret={clientSecret};
                TenantId={tenantId};
                RequireNewInstance=true;
            ";

            Console.WriteLine("Attempting to connect to Dataverse...");

            try
            {
                using (ServiceClient serviceClient = new ServiceClient(connectionString))
                {
                    if (!serviceClient.IsReady)
                    {
                        Console.WriteLine("Connection failed!");

                        // Check for inner exceptions
                        var innerEx = serviceClient.LastException?.InnerException;
                        while (innerEx != null)
                        {
                            Console.WriteLine($"Inner Exception: {innerEx.Message}");
                            innerEx = innerEx.InnerException;
                        }
                        return;
                    }

                    Console.WriteLine("Connected successfully to Dataverse!");

                    // Test creating a contact
                    Entity test = new Entity("crc51_test");
                    test["crc51_invisible"] = "Mohamed";
                    test["crc51_date"] = DateTime.UtcNow;

                    Guid testId = serviceClient.Create(test);
                    Console.WriteLine($"Contact created with ID: {testId}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                Console.WriteLine($"Full exception: {ex.ToString()}");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}