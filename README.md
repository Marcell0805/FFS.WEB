# FFS Web — V0 Prototype

In-memory **Blazor WebAssembly** prototype for exploring FFS in the browser (static hosting friendly).

## Run locally

```powershell
cd "D:\repos\FFS Web"
dotnet run --project src/FFS.Web
```

Open `http://localhost:5049`.

No database. No authentication. Demo data is seeded in memory in the browser.

## GitHub Pages

Push to `master`/`main` (or run the **Deploy Blazor App to GitHub Pages** workflow).

Live URL: **https://ffs.foxbytelabs.co.za/**

In the repo **Settings → Pages**:
1. Source: **GitHub Actions**
2. Custom domain: `ffs.foxbytelabs.co.za`
3. After DNS is green, enable **Enforce HTTPS**

Do not open `/FFS.WEB/` on the subdomain — that path is the old project-site URL.
