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

Push to `main` (or run the **Deploy Blazor App to GitHub Pages** workflow). The site is published as a project page:

`https://<you>.github.io/FFS.WEB/`

In the repo **Settings → Pages**, set Source to **GitHub Actions**.
