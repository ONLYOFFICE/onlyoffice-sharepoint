# ONLYOFFICE solution for SharePoint

SharePoint **farm solution** (`.wsp`, GAC, full trust) that opens SharePoint library files in ONLYOFFICE Docs.
Hosts: SharePoint **Subscription Edition** (primary), SharePoint 2019 (supported). Not SharePoint Online.
Details, history and known issues: [HANDOVER.md](HANDOVER.md). Release: `/release` skill.

## Build

Only on a Windows host with SharePoint installed (references `Microsoft.SharePoint*` from GAC) and Visual Studio with SharePoint Developer Tools.

```powershell
git submodule update --init                 # formats/onlyoffice-docs-formats.json is an EmbeddedResource; build fails without it
cd ONLYOFFICE\.nuget; .\NuGet.exe restore    # restores into ONLYOFFICE\.nuget\packages (HintPaths point there)
```
Then either Visual Studio → project → **Publish** → File System (→ `.wsp`), or `BuildAndInstall\build.bat` (→ `BuildAndInstall\ONLYOFFICE.wsp`, log in `Build.log`).

Install/remove on a farm (SharePoint Management Shell, `.wsp` next to script): `BuildAndInstall\Install.ps1` / `Uninstall.ps1`. Feature id `bb60a219-eb5d-47ff-b023-353a3be7b8ee`.

No automated tests and no permanent test farm (DevOps deploys one on request). Verify with the manual checklist in HANDOVER.md §8.
This dev machine may not have SharePoint — then code can be edited but not compiled; say so instead of claiming a successful build.

## Layout

- `ONLYOFFICE/Layouts/Onlyoffice/` — deployed to `/_layouts/15/Onlyoffice/`
  - `editorPage.aspx(.cs)` — builds Docs editor config + JWT, loads `api.js`; JS handlers for Save As / history
  - `CallbackHandler.ashx.cs` — called **by Docs** (anonymous): `download` (serve file/version) and `track` (save on status 2)
  - `EditorHandler.ashx.cs` — called by the browser: `saveas`, `history`, `version`
  - `Settings.aspx(.cs)` — per-site admin settings (Docs URL, JWT secret/header, demo)
  - `classes/` — `AppConfig` (settings in `SPWeb.Properties`, `DocsDemo`), `Encryption` (URL hash + JWT), `FileUtility` (formats, document key), `Configuration` (editor config DTO)
  - `formats/` — git submodule `ONLYOFFICE/document-formats`
- `MenuAction`, `RibbonAction`, `SettingAction` `/Elements.xml` — UI entry points
- `Resources/Resource*.resx` — en + ru-RU strings (add every new key to both)
- `Package/Package.package` — `.wsp` content; third-party DLLs deployed to GAC are listed here **and** in the csproj

## Rules

- Never commit secrets (JWT secrets, test-farm credentials). Settings live in `SPWeb.Properties` per (sub)site.
- Do not change `FileUtility.GenerateRevisionId` or the `Encryption.GetUrlHash` payload format without a reason: they break open co-editing sessions, history keys and already-issued callback URLs.
- Changing a NuGet dependency version: update `packages.config`, csproj `HintPath` **and** `Package.package` `sourcePath`.
- `CHANGELOG.md` top entry must stay `## X.Y.Z` — CI derives the tag/release from the first version number in that file.
- Branching: `feature/*` → `develop` → `master` (release). Remote `origin` is internal Gitea; GitHub is the public repo.
- Keep the MIT header in new `.cs` files.
