using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MovieMentorDatabase;
using MovieMentorDatabase.DAL;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
    .AddJsonFile("appsettings.json", false)
    .Build();


var connectionString = configuration.GetConnectionString("DefaultConnection");
var serverVersion = ServerVersion.AutoDetect(connectionString);

await using var movieContext = new MovieContext(connectionString, serverVersion);

try
{
    // populate DB

    // await movieContext.Database.EnsureDeletedAsync();
    // await DbInitializer.Initialize(movieContext);
    await DbInitializerFromCsv.Initialize(movieContext);

    Console.WriteLine("Done.");
}
catch (Exception e)
{
    Console.Error.WriteLine("An error occured creating the DB. {0}", e);
}
