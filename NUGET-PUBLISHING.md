# Quick Reference: NuGet Publishing

## 📦 Publish New Release

```bash
# 1. Create and push tag (format: v<major>.<minor>.<patch>.<build>)
git tag v3.1.2.0
git push origin v3.1.2.0

# 2. Workflow runs automatically
# 3. Check: https://github.com/YOUR_ORG/dockpanelsuite/actions
```

## 🔑 Required Secret

**Name:** `NUGET_API_KEY`  
**Location:** Repository Settings → Secrets and variables → Actions  
**Get key from:** https://www.nuget.org/account/apikeys

## 📋 Published Packages (7 total)

- DockPanelSuite
- DockPanelSuite.ThemeVS2003
- DockPanelSuite.ThemeVS2005
- DockPanelSuite.ThemeVS2005Multithreading
- DockPanelSuite.ThemeVS2012
- DockPanelSuite.ThemeVS2013
- DockPanelSuite.ThemeVS2015

## 🔢 Version Mapping

| Tag      | FileVersion | PackageVersion |
|----------|-------------|----------------|
| v3.1.1.9 | 3.1.1.9     | 3.1.1          |
| v3.1.2.0 | 3.1.2.0     | 3.1.2          |
| v4.0.0   | 4.0.0       | 4.0.0          |

## ✅ Test Locally

```powershell
# Test version extraction
.\test-version-extraction.ps1 v3.1.2.0

# Test packaging (single project)
dotnet build WinFormsUI\WinFormsUI.csproj -c Release -f net10.0-windows -p:FileVersion=3.1.2.0
dotnet pack WinFormsUI\WinFormsUI.csproj -c Release --no-build -p:PackageVersion=3.1.2 -o ./artifacts
```

## 🐛 Troubleshooting

| Issue | Solution |
|-------|----------|
| Workflow doesn't trigger | Ensure tag format is `v*.*.*.*` |
| 401 error during publish | Check `NUGET_API_KEY` secret is valid |
| 409 duplicate package | Version already exists, increment and retry |
| net40 build errors | Expected — workflow skips failed targets |

## 📚 More Info

See: `.github/workflows/README-NUGET-PUBLISH.md`
