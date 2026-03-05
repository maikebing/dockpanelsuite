# NuGet Package Publishing Guide

## Overview

This workflow automatically builds and publishes all DockPanelSuite NuGet packages when a version tag is pushed.

## Packages Published

The workflow publishes **7 NuGet packages**:

1. **DockPanelSuite** — Core docking library
2. **DockPanelSuite.ThemeVS2003** — Visual Studio 2003 theme
3. **DockPanelSuite.ThemeVS2005** — Visual Studio 2005 theme
4. **DockPanelSuite.ThemeVS2005Multithreading** — VS2005 multithreading theme
5. **DockPanelSuite.ThemeVS2012** — Visual Studio 2012 themes (Light/Blue/Dark)
6. **DockPanelSuite.ThemeVS2013** — Visual Studio 2013 themes (Light/Blue/Dark)
7. **DockPanelSuite.ThemeVS2015** — Visual Studio 2015 themes (Light/Blue/Dark)

## Version Format

**Tag Format:** `v<major>.<minor>.<patch>.<build>`

**Example:** `v3.1.1.9`

### Version Mapping

- **FileVersion:** Full version from tag → `3.1.1.9`
- **AssemblyVersion:** Full version from tag → `3.1.1.9`
- **PackageVersion:** First 3 segments only → `3.1.1`

**Rationale:** NuGet semantic versioning typically uses 3 segments (major.minor.patch). The 4th segment (build number) is kept in file/assembly versions for traceability but excluded from package version.

## Usage

### 1. Create and Push a Tag

```bash
# Create a tag locally
git tag v3.1.2.0

# Push the tag to GitHub
git push origin v3.1.2.0
```

### 2. Automatic Workflow Execution

The workflow will automatically:
- ✅ Trigger on tag push
- ✅ Extract version numbers from tag
- ✅ Restore dependencies
- ✅ Build all projects for `net40` and `net10.0-windows`
- ✅ Pack all 7 NuGet packages
- ✅ Publish to NuGet.org
- ✅ Upload artifacts to GitHub (retained for 90 days)

### 3. Monitor Progress

Go to: **Actions** tab → **Publish NuGet Packages** → Select the run

## Prerequisites

### Required Secret: `NUGET_API_KEY`

Add your NuGet.org API key to repository secrets:

1. Go to repository **Settings** → **Secrets and variables** → **Actions**
2. Click **New repository secret**
3. Name: `NUGET_API_KEY`
4. Value: Your NuGet.org API key (get from https://www.nuget.org/account/apikeys)
5. Click **Add secret**

### Target Frameworks

Packages are multi-targeted:
- **.NET Framework 4.0** (`net40`)
- **.NET 10 Windows** (`net10.0-windows`)

## Example Workflow Run

```
Tag: v3.1.2.0

Extracted Versions:
  FileVersion: 3.1.2.0
  PackageVersion: 3.1.2

Building projects...
  ✅ WinFormsUI.csproj → DockPanelSuite.3.1.2.nupkg
  ✅ ThemeVS2003.csproj → DockPanelSuite.ThemeVS2003.3.1.2.nupkg
  ✅ ThemeVS2005.csproj → DockPanelSuite.ThemeVS2005.3.1.2.nupkg
  ✅ ThemeVS2005Multithreading.csproj → DockPanelSuite.ThemeVS2005Multithreading.3.1.2.nupkg
  ✅ ThemeVS2012.csproj → DockPanelSuite.ThemeVS2012.3.1.2.nupkg
  ✅ ThemeVS2013.csproj → DockPanelSuite.ThemeVS2013.3.1.2.nupkg
  ✅ ThemeVS2015.csproj → DockPanelSuite.ThemeVS2015.3.1.2.nupkg

Publishing to NuGet.org...
  ✅ All 7 packages published successfully
```

## Troubleshooting

### Workflow doesn't trigger
- Ensure tag matches pattern `v*.*.*.*` (must start with 'v')
- Check if workflow file is on the branch the tag points to

### Build fails
- Verify .NET 10.0 SDK is available in runner
- Check project files for syntax errors
- Ensure all dependencies are restorable

### Publish fails with 401/403
- Verify `NUGET_API_KEY` secret is set correctly
- Check API key has not expired
- Ensure API key has "Push new packages and package versions" permission

### Duplicate package error (409)
- Package with same version already exists on NuGet.org
- Workflow uses `--skip-duplicate` to continue with other packages
- Increment version and create new tag

## Manual Publishing

To publish manually (fallback):

```bash
# Build all packages
dotnet pack WinFormsUI.Docking.sln -c Release -p:PackageVersion=3.1.2 -p:FileVersion=3.1.2.0 -o ./artifacts

# Publish each package
dotnet nuget push ./artifacts/DockPanelSuite.3.1.2.nupkg --api-key YOUR_KEY --source https://api.nuget.org/v3/index.json
dotnet nuget push ./artifacts/DockPanelSuite.ThemeVS2003.3.1.2.nupkg --api-key YOUR_KEY --source https://api.nuget.org/v3/index.json
# ... repeat for all packages
```

## Version History Example

| Git Tag | FileVersion | PackageVersion | Published Date |
|---------|-------------|----------------|----------------|
| v3.1.1.9 | 3.1.1.9 | 3.1.1 | Current |
| v3.1.2.0 | 3.1.2.0 | 3.1.2 | Next release |
| v3.2.0.0 | 3.2.0.0 | 3.2.0 | Future |

## Notes

- Workflow includes **symbol packages** (`.snupkg`) for debugging support
- Artifacts are retained for **90 days** on GitHub
- `--skip-duplicate` prevents failures if package version already exists
- All 7 projects must build successfully; if any fails, no packages are published
