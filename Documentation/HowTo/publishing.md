# Publishing your website

StatiC# writes your website into the `Output` directory. Getting that directory onto the web
is a separate job, and which way suits you depends on where it should live. This article
covers GitHub Pages first, because that path needs no account anywhere else and no secret.

## The sitemap

Every run writes a `sitemap.xml` into the output, listing the absolute url of every page that
was written. Crawlers look for it at the root of the site, which is where it lands, and the
urls are built on the address you gave `Website.Create`.

It is built from the pages that were actually written rather than from your content, so it
cannot promise a page the generator decided not to write - a tag whose name yields no url, an
article whose filename yields none. It carries no `lastmod`: the dates available would be the
wrong ones, since only an article carries a modification date from its file and in a fresh
checkout even that is the time the file was cloned.

If you want crawlers pointed at it explicitly, put a `robots.txt` holding
`Sitemap: https://yourdomain.com/sitemap.xml` into your `Resources` directory.

## GitHub Pages, built by a workflow

This is the way to do it: GitHub checks out your repository, runs the generator, and deploys
what it produced. The `Output` directory does not even have to be committed.

Copy [`publish.yml`](../ProjectTemplate/.github/workflows/publish.yml) from the project
template to `.github/workflows/publish.yml` in your repository, then set
**Settings → Pages → Build and deployment → Source** to **GitHub Actions**, once.

That is all. Every push to `main` rebuilds and redeploys. There is no token to create: the
deployment is authorized by the workflow's `id-token` permission.

Two details the file already handles, and that are easy to get wrong:

- **The working directory.** The template's `Program.cs` takes the current directory as its
  source, which is right when you run `dotnet run` from the project folder and wrong in a
  workflow, where the current directory is the repository root. The workflow sets
  `working-directory` for that reason. If you move things around, give the generator an
  absolute path instead.
- **`concurrency` without `cancel-in-progress`.** Two deployments at once fail, and
  cancelling one halfway leaves the site partly updated.

### A custom domain

Put a file named `CNAME` holding your domain into your `Resources` directory, and StatiC#
copies it into the output on every run. It is also on the list of files the generator never
deletes, so one you placed in the output directory by hand survives as well - see
[incremental output](incremental_output.md#files-statics-never-deletes).

### Folders starting with an underscore

GitHub Pages runs Jekyll over what you deploy unless a file named `.nojekyll` is there, and
Jekyll drops directories whose name starts with an underscore. StatiC# keeps an underscore in
a url, so a folder called `_drafts` becomes `/_drafts` and would disappear. If you have one,
put an empty `.nojekyll` into your `Resources` directory. The generator never deletes that
file either.

## GitHub Pages from a committed output directory

The older way: the `Output` directory is itself a repository, or a `gh-pages` branch, and you
commit and push what the generator wrote. StatiC# is built for this.

- **Incremental output is the default**, so a file whose content did not change is not
  rewritten. Your commits then contain what you actually changed. See
  [incremental output](incremental_output.md).
- **`.git` is never deleted**, not even by `NoIncrementalOutput()`. The output directory being
  a repository is the case that made that guarantee necessary.
- Files that no longer belong to the website are removed, so deleting an article deletes its
  page. Anything you keep there by hand needs naming:
  `manager.WithPreservedOutput("robots.txt")`.

## Somewhere else

Nothing above is specific to GitHub beyond the workflow file. The generator produces a
directory of static files, so any host that serves one works: Netlify, Cloudflare Pages, an S3
bucket, or a server you `rsync` to. Point the host at the `Output` directory and keep
incremental output on, so that a deploy that compares files has little to transfer.
