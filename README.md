# dotnet-project / PersonalCloudDrive

Personal Cloud Drive: A .NET Core web application with Identity authentication, allowing users to manage, upload, and organize files and folders securely in a personal cloud storage environment.

This repository contains the full project and instructions to run it locally.

## Prerequisites

- .NET 9.0 SDK (download from https://dotnet.microsoft.com)
- SQL Server LocalDB (usually installed with Visual Studio) or SQL Server accessible and connection string updated in `appsettings.json`
  - If you don't have LocalDB, you can install it via the SQL Server Express installer or install the `SqlLocalDB` feature bundled with Visual Studio. On Windows you can also use the standalone LocalDB installer from Microsoft.
- (Optional) Visual Studio 2022/2023 or VS Code for development

## Setup and run

Open a terminal in the project root (for example `E:\Projects\PersonalCloudDrive`).

1. Restore and build

```powershell
# restore and build
dotnet restore
dotnet build
```

2. Update configuration

- The default connection string is set in `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=PersonalCloudDrive;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

Change it if you need to use a different SQL Server instance.

3. Apply EF Core migrations (create database and schema)

```powershell
# create and apply migrations (if migrations already present this will ensure database is up to date)
dotnet ef database update
```

If `dotnet ef` is not available, install the tools:

```powershell
dotnet tool install --global dotnet-ef
```

If you need to create the initial migration locally (project already contains migrations in the `Migrations/` folder in this repo, so this is normally not required):

```powershell
# create a new migration (only if you modify the model and need a new migration)
dotnet ef migrations add InitialCreate
dotnet ef database update
```

4. Run the app

```powershell
dotnet run
```

Open a browser at https://localhost:5001 (or as printed in output) and register/login.

**Note:** You must register a new user account and log in before you can access the Dashboard and upload files.

## Uploads

Files uploaded are saved under `wwwroot/uploads/{userId}` by default. Upload path and limits are configured in `appsettings.json` under `StorageSettings`.

## Git instructions — push this project to your GitHub repo

If you want to push this project to `https://github.com/Meet1394/dotnet-project.git` do the following:

```powershell
# initialize git if not already initialized
git init
git add .
git commit -m "Initial commit - PersonalCloudDrive"

# add remote (replace URL with your repo URL)
git remote add origin https://github.com/Meet1394/dotnet-project.git

# push to GitHub
git branch -M main
git push -u origin main
```

If the remote already exists, you can instead set the URL:

```powershell
git remote set-url origin https://github.com/Meet1394/dotnet-project.git
git push -u origin main
```

## Notes and troubleshooting

- If `dotnet run` fails because the output file is locked, stop any running instance of the app (close the terminal or stop the process), then run again.
- If you change `appsettings.json`, remember to re-run the app so the new configuration is picked up.
- For production deployment, configure a proper SQL Server, storage location, and secure secrets (do not keep production connection strings in plaintext in `appsettings.json`).

- If you see errors about missing `dotnet-ef`, install it with `dotnet tool install --global dotnet-ef`.
- If you get port conflicts, change the port in `Properties/launchSettings.json` or use the `--urls` option: `dotnet run --urls "https://localhost:5002"`
- If migrations fail, check your connection string and SQL Server installation.
- If you see "green files" in `git status`, commit and push them to update GitHub.

---

If you want, I can:
- Commit and push these changes (README) to the remote repo for you (I need your permission and Git credentials or you can run the git commands locally).
- Add a LICENSE file.
- Add a Dockerfile to containerize the app.
