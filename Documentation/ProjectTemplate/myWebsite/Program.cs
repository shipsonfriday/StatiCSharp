using StatiCSharp;

// The source directory holds Content, Resources and Output. Running this with "dotnet run"
// from the project folder makes the current directory the right one; replace it with an
// absolute path if you start the program from somewhere else.
string source = Directory.GetCurrentDirectory();

var myAwesomeWebsite = Website.Create(
        url: "https://yourdomain.com",
        name: "My Awesome Website")
    .WithDescription("Description of your website")
    .WithLanguage("en-US")
    .WithSections("posts");          // Folders that should be treated as sections.

await WebsiteManager
    .For(myAwesomeWebsite, source)
    .MakeAsync();
