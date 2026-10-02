# Controlling source directories
 
 After starting a new WebsiteManager, the default location for your output, content, and static files is in the given source directory. E.g.
 ```C#
 ...
 source: @"C:\Users\Roland\myWebsite"
 ...
 ```
 and StatiC# will assume this folder structure:
 
 ```bash
 ├── myWebsite
 │   ├── ...
 │   ├── Content
 │   ├── Output
 │   ├── Resources
 │   ├── ...
 ```
 
If you want to change this behavior, override the defaults while configuring the manager:

```C#
await WebsiteManager.For(myAwesomeWebsite, source: @"C:\Users\Roland\myWebsite")
    .WithContentDirectory(@"another\path\to\Content")
    .WithOutputDirectory(@"another\path\to\Output")
    .WithResourcesDirectory(@"another\path\to\Resources")
    .MakeAsync();
```

Each of them is independent: override only the ones you want to move and the rest stay
inside the source directory. The source directory itself is fixed once the manager is
created.
