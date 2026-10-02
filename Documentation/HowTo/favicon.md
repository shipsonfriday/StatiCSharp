# Customize your favicon

If your website template does not provide a favicon or you don't like the one proposed, you can use your very own.  
Place your favicon into the Resources directory:

```bash
Resources/favicon.png
```

The name and the extension both matter: StatiC# writes
`<link rel="icon" type="image/png" href="/favicon.png">` into the head of every page, so the
file has to be a png called `favicon.png`.  
Check out [favicon.io](https://favicon.io/) to generate a favicon.