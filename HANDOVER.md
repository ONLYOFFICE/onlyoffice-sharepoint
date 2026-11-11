# HANDOVER — ONLYOFFICE solution for SharePoint

State as of 2026-11-11, latest version **3.2.0** (tag `v3.2.0`, 2026-05-13).
Markers: ⚠️ — not confirmed, ❓ — unknown at handover time.

## 1. Integration type

- **Embedded into the host**: a SharePoint *farm solution* (`.wsp`). `ONLYOFFICE.dll` and its dependencies (JWT, Newtonsoft.Json, System.Text.Json) are deployed to the GAC; pages and handlers go to `/_layouts/15/Onlyoffice/`. Feature `bb60a219-eb5d-47ff-b023-353a3be7b8ee`, scope Site.
- **Host**: SharePoint Subscription Edition — primary platform; SharePoint 2019 — supported. SharePoint 2016 and older are not supported since 2.0 (a frozen branch exists for 2016, see §6). SharePoint Online is not supported (farm solutions are impossible there).
- **ONLYOFFICE product**: ONLYOFFICE Docs (Document Server), self-hosted or Docs Cloud; plus a built-in 30-day demo connection to a cloud Docs server.
  Minimum Docs version: ⚠️ 6.0+ (CHANGELOG 3.0.0: "v6.0 and earlier is no longer supported"); minimum for version history — ❓.
- **How it talks to Docs** (Docs API):
  1. The browser opens `editorPage.aspx?SPListItemId=…&SPListURLDir=…`; the page builds the editor config, signs it with JWT and loads `<Docs>/web-apps/apps/api/documents/api.js?shardkey=<key>`.
  2. Docs downloads the file: `CallbackHandler.ashx?data=<hash>` (action `download`).
  3. Docs sends status updates: `CallbackHandler.ashx?data=<hash>` (action `track`); on status 2 the file is downloaded from Docs and overwritten in the library.
  4. The browser calls `EditorHandler.ashx?action=saveas|history|version`.
- JWT is sent in a custom header (default `AuthorizationJWT`) because SharePoint blocks an external `Authorization` header. The same header must be configured in Docs.
- Callbacks **require anonymous access** on the web application (README, steps 5–10): Docs calls `CallbackHandler.ashx` without SharePoint authentication. This is a deliberate trade-off — the scheme does not work otherwise.

## 2. Key features (user scenarios)

1. "Edit in ONLYOFFICE" in the file context menu (ECB, document libraries) and on the ribbon (Files tab).
2. Open for editing or viewing: mode depends on the user's rights (`EditListItems`) and the format's `actions` from `onlyoffice-docs-formats.json`; unsupported formats redirect to the site.
3. Co-editing and autosave: on callback status 2 the new version is written to the library on behalf of the first user in `users`.
4. Save As from the editor into the same folder (checks `AddListItems`).
5. Version history (3.2.0): list of SharePoint versions, view a version, compare with the previous one.
6. Admin settings `/_layouts/15/Onlyoffice/Settings.aspx` (site admins only; link in Site Actions): Docs address, JWT secret, JWT header, demo mode; a warning popup when the secret is empty. **Settings are stored per SPWeb** — each subsite is configured separately.
7. Docs Cloud demo for 30 days from first activation; after expiry the checkbox is locked. The editor shows a "using demo" message.
8. Localization: en, ru-RU.

## 3. Build

- Windows host with SharePoint (SE or 2019) + Visual Studio with SharePoint Developer Tools only. Build directly on the host: VS → Build / Publish (File System) → `.wsp`. Alternative: `BuildAndInstall\build.bat` (MSBuild .NET 4.0.30319, `build.proj`).
- Toolchain: .NET Framework 4.8, C# (SharePoint project type), ToolsVersion 15.0. Exact VS version — ❓ (any with SharePoint tools compatible with the host).
- Non-standard steps:
  1. `git submodule update --init` — submodule `ONLYOFFICE/Layouts/Onlyoffice/formats` (public `github.com/ONLYOFFICE/document-formats`). `onlyoffice-docs-formats.json` is an EmbeddedResource: without it the build fails, and if the resource is missing at runtime all formats are "unknown".
  2. `cd ONLYOFFICE\.nuget; .\NuGet.exe restore` — packages go to `ONLYOFFICE\.nuget\packages` (not in git; csproj HintPaths and `Package.package` `sourcePath` point there).
- Assembly signing: `ONLYOFFICE/key.snk` is in the repository. Whether a separate private release key exists — ❓.
- No private dependencies. Main remote — `git.onlyoffice.com` (Gitea, SSH access required); public — `github.com/ONLYOFFICE/onlyoffice-sharepoint`. How they are synced (auto-mirror or manual) — ❓.
- No tests. No permanent test farm — DevOps deploy SharePoint + Docs on request, different each time.

## 4. Publishing

- The solution is **not published to any marketplace** (farm solutions are not accepted by AppSource). Users download the `.wsp` from **GitHub Releases**.
- CI (GitHub Actions):
  - `create-tag.yml` — on push to `master` (except changes only in `.github/`, README, LICENSE, AUTHORS) takes the first version from `CHANGELOG.md` and creates tag `vX.Y.Z`. If the tag already exists the job fails (expected for non-release commits to master, but clutters Actions).
  - `release.yml` — on tag `v*` creates a GitHub Release with the top CHANGELOG section as the body.
  - **CI does not attach the `.wsp`** — it is built on a SharePoint host and uploaded to the release manually.
- Step by step — skill `.claude/skills/release/SKILL.md`.

## 5. Implicit knowledge

Derived from code, not confirmed by the previous maintainer. Do not change without a reason:

- **Document key format** — `FileUtility.GenerateRevisionId`. Changing it breaks open co-editing sessions and version-history keys.
- **Docs URL signature format** — `Encryption.GetUrlHash` / `Payload.cs`. Changing it makes already issued download/callback URLs fail validation.
- **`CHANGELOG.md` format** — CI takes the tag and release notes from the first `## X.Y.Z` section.

## 6. Known issues, tech debt, branches

Bugs are tracked in ONLYOFFICE **Bugzilla** (numbers like "Bug 80822" in commits). User reports also come via GitHub Issues (templates in `.github/ISSUE_TEMPLATE`).

**Branches**:
- `master` — releases; `develop` — fully merged into `master`, no pending changes.
- `feature/sharepoint-2016` — frozen branch (v1.2, 2021) for SharePoint 2016. **Do not delete.**

## 7. Plans and commitments

- Plans for the next version — ❓ (unknown at handover time).
- Commitments to customers/partners — ❓ (unknown).

## 8. Manual verification checklist

No automated tests and no permanent farm. Before a release, on a DevOps-provided farm (SE and, if possible, 2019) + Docs with JWT enabled:

**Installation**
- [ ] `Install.ps1` installs the `.wsp` and activates the feature on the site; `Uninstall.ps1` removes it cleanly.
- [ ] Anonymous access is enabled on the web application (README, steps 5–10).

**Settings**
- [ ] Site Actions → ONLYOFFICE item opens `Settings.aspx`; non-admins are redirected.
- [ ] Saving Docs address / secret / header works; an empty secret shows the warning popup.
- [ ] Demo: enabling shows the end date; the editor opens with the demo message.
- [ ] Subsite settings are independent of the root site.

**Editor**
- [ ] DOCX, XLSX, PPTX, PDF open from the context menu and the ribbon (root site and subsite, file in a folder).
- [ ] A read-only user gets view mode; a view-only format (e.g. ODT/DOC) opens in view mode.
- [ ] Two users edit simultaneously; after closing (~10 s) a new version with the correct author appears in the library.
- [ ] Save As creates a copy in the same folder; without add permission it is refused.
- [ ] Version history: list, open an old version, compare with previous, close history.
- [ ] Wrong JWT secret on the SharePoint side → Docs cannot download the file (error in the editor), ULS log "JWT validation failed".
- [ ] File names with spaces / Cyrillic / special characters open and save.
- [ ] Logs: ULS, area "SharePoint ONLYOFFICE".
