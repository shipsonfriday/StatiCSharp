# Using custom parsers

You can add your custom parsers to the generating process pipeline. StatiC# is distributed with a default markdown parser, using [Markdig](https://github.com/xoofx/markdig). This markdown parser runs at the end of every pipeline by default. If you add no additional parsers, [Markdig](https://github.com/xoofx/markdig) is the only one used.

## Add a parser

You can add every parser that implements `IPipelineParser` by calling the `AddParser()` method of the website manager. Configure your parser before adding it.

```C#
var myParser = new CSharpToColouredHtmlParser();

await WebsiteManager.For(myAwesomeWebsite, source: @"/path/to/your/project")
    .AddParser(myParser)
    .MakeAsync();
```

You can add as many parsers as you want, as the `AddParser()` method is chainable. The parsers run in the same order as they were added.

```C#
var myParser1 = new CSharpToColouredHtmlParser();
var myParser2 = new ReplacementParser();

await WebsiteManager.For(myAwesomeWebsite, source: @"/path/to/your/project")
    .AddParser(myParser1)
    .AddParser(myParser2)
    .MakeAsync();
```

## Deactivate the default parser

You can stop the default markdown parser by [Markdig](https://github.com/xoofx/markdig) from running:

```C#
await WebsiteManager.For(myAwesomeWebsite, source: @"/path/to/your/project")
    .AddParser(myParser)
    .NoDefaultMarkdownParser()
    .MakeAsync();
```

## Create your own parser

You can code up your parser easily by implementing [`IPipelineParser`](https://github.com/RolandBraunDev/StatiCSharp/blob/master/src/StatiCSharp/Interfaces/IPipelineParser.cs).  
The optional content of the `HeaderContent` property will be added to the &lt;head&gt;&lt;/head&gt; of every site. This may be some CSS or JavaScript that the output of your parser needs.  
`Parse()` will be called each time StatiC# parses the users' input files. Its parameter is the parsed string. Your method should then return the result of your parsing process.
