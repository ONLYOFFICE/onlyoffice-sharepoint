---
name: release
description: Release a new version of the ONLYOFFICE SharePoint solution — bump version, build the .wsp on a SharePoint host, merge to master, let CI tag and create the GitHub Release, then attach the .wsp manually. Use when asked to prepare, cut or publish a release.
---

# Release ONLYOFFICE solution for SharePoint

Distribution is **only GitHub Releases** (users download the `.wsp` there). No marketplace submission.
CI creates the tag and the release text but **does not build or attach the `.wsp`** — that is manual.

Ask the user for the version `X.Y.Z` if not given. Steps marked 👤 need a human (SharePoint host / access you don't have) — list them for the user, don't pretend they are done.

## 1. Prepare on `develop`

1. `git checkout develop && git pull`
2. Optionally update the formats submodule: `git submodule update --init --remote ONLYOFFICE/Layouts/Onlyoffice/formats`; if the list of formats changed, update README "Supported formats" and mention it in CHANGELOG.
3. `CHANGELOG.md` — add the new section **at the top**, keeping the exact format (CI parses it):
   ```
   ## X.Y.Z
   ## Added
   - ...
   ## Changed
   - ...
   ```
   The first version number in the file becomes the tag; the release body is the text between the first two `## <digit>` headings.
4. `ONLYOFFICE/Properties/AssemblyInfo.cs` — `AssemblyVersion` and `AssemblyFileVersion` → `X.Y.Z.0`.
5. If a new calendar year: update copyright year in file headers, `AssemblyCopyright`.
6. Commit (`X.Y.Z`), push `develop`.

## 2. Build the `.wsp` 👤

On a Windows host with SharePoint (SE preferred) and Visual Studio with SharePoint tools, from the `develop` commit:
```powershell
git submodule update --init
cd ONLYOFFICE\.nuget; .\NuGet.exe restore; cd ..\..
```
Then VS → project → **Publish** → File System, or `BuildAndInstall\build.bat` → `BuildAndInstall\ONLYOFFICE.wsp`.
Check that `Build.log` has no errors and the `.wsp` contains `JWT.dll`, `Newtonsoft.Json.dll`, `System.Text.Json.dll` (it is a CAB: `expand -F:* ONLYOFFICE.wsp out\`).

## 3. Verify 👤

Get a test farm from DevOps (no permanent one). Install with `BuildAndInstall\Install.ps1` (it looks for `onlyoffice.wsp` next to itself; `ONLYOFFICE.wsp` matches since Windows paths are case-insensitive) and run the manual checklist in `HANDOVER.md` §8. Ideally on both SharePoint SE and 2019.

## 4. Merge to `master`

Open PR `develop` → `master` titled `Release/X.Y.Z` on the internal Gitea (`git.onlyoffice.com`), get it reviewed and merged.
Make sure the change reaches `github.com/ONLYOFFICE/onlyoffice-sharepoint` `master` — how Gitea → GitHub sync works is not documented (see HANDOVER §3); confirm with the team if the commit doesn't appear on GitHub.

## 5. CI (automatic)

- `create-tag.yml` creates tag `vX.Y.Z` on push to `master`.
- `release.yml` creates GitHub Release `vX.Y.Z` with the CHANGELOG section.
Check GitHub → Actions that both succeeded and the release text is correct (edit it on GitHub if needed).
If `create-tag` failed because the tag already exists, the version in CHANGELOG was not bumped.

## 6. Attach the `.wsp` 👤

Upload the `.wsp` built in step 2 to release `vX.Y.Z` (GitHub UI → Edit release → attach, or `gh release upload vX.Y.Z ONLYOFFICE.wsp --repo ONLYOFFICE/onlyoffice-sharepoint`).
Use the same file name as in previous releases (check the assets of the previous release).

## 7. After release

- Download the asset from the release page once and verify it installs.
- Bugs fixed in the release: update their status in Bugzilla.
