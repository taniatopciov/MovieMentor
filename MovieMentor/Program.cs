using Microsoft.EntityFrameworkCore;
using MovieMentor.Controllers;
using MovieMentor.DAL;
using MovieMentor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<MovieContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    var serverVersion = ServerVersion.AutoDetect(connectionString);

    options.UseMySql(connectionString, serverVersion);
});
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IMoviesService, MoviesService>();
builder.Services.AddScoped<IKnowledgeBaseLoader, KnowledgeBaseLoader>();
builder.Services.AddTransient<IInferenceMachineService, InferenceMachineService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// app.UseHttpsRedirection(); // enable cors
app.UseStaticFiles();
app.UseRouting();


app.MapGet("/", () => "Movie Mentor works!");
app.MapGet("/api/movies", MoviesController.GetMovies);
app.MapGet("/api/movies/{id}", MoviesController.GetMovie);
app.MapGet("/api/movies/tags", MoviesController.GetTags);
app.MapPost("/api/inference", InferenceController.Infer);

app.MapFallbackToFile("index.html");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<MovieContext>();
        await DbInitializer.Initialize(context);
    }
    catch (Exception e)
    {
        Console.Error.WriteLine("An error occured creating the DB. {0}", e);
    }
}

app.Run();
