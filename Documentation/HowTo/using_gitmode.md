# Using GitMode

By default, __StatiC#__ will delete all files in the output folder and then write all files to make up your website. If your website is under source control, this behavior can cause changes in your repository, even if no content has changed. E.q., the files' metadata may have a different value for the date they were created.  

You can use StatiC# in _GitMode_ to ensure that the files in the output directory are only touched if their content has changed. New files are created as needed. At the same time, files that have no corresponding markdown file are deleted. With that behavior, you can delete an article by deleting the markdown file it refers to.  

Activate GitMode while configuring your WebsiteManager:  

```C#
await WebsiteManager.For(myAwesomeWebsite, source: @"/path/to/your/project")
    .WithGitMode()
    .MakeAsync();
```

`WithGitMode()` also takes a boolean, so `WithGitMode(false)` is available when the value
comes from a configuration of your own.
