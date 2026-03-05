# WinFormsUI.Docking .NET 10.0 Upgrade Tasks

## Overview

This document tracks the incremental migration of the WinFormsUI.Docking solution from multiple .NET Framework versions (.NET Framework 3.5/4.0/4.8, .NET Core 3.1) to .NET 10.0. The upgrade follows a dependency-ordered approach with validation checkpoints between phases.

**Progress**: 3/6 tasks complete (50%) ![0%](https://progress-bar.xyz/50)

---

## Tasks

### [✓] TASK-001: Verify prerequisites *(Completed: 2026-03-05 12:27)*
**References**: Plan §Phase 1 Migration Steps §1. Prerequisites

- [✓] (1) Verify .NET 10.0 SDK installed: `dotnet --list-sdks`
- [✓] (2) .NET 10.0 SDK present in list (**Verify**)

---

### [✓] TASK-002: Phase 1 - Migrate WinFormsUI foundation library *(Completed: 2026-03-05 12:37)*
**References**: Plan §Phase 1: WinFormsUI.csproj, Plan §Project-by-Project Migration Plans §Level 0

- [✓] (1) Update `WinFormsUI\WinFormsUI.csproj` TargetFrameworks to `netcoreapp3.1;net35;net40;net10.0`
- [✓] (2) TargetFrameworks property updated (**Verify**)
- [✓] (3) Add `System.Drawing.Common` NuGet package version 9.0.0 to WinFormsUI.csproj
- [✓] (4) PackageReference added (**Verify**)
- [✓] (5) Restore dependencies: `dotnet restore WinFormsUI\WinFormsUI.csproj`
- [✓] (6) Dependencies restored successfully (**Verify**)
- [✓] (7) Build WinFormsUI for net10.0 target and fix all compilation errors per Plan §Breaking Changes Catalog (focus: GDI+ APIs in VisualStudioToolStripRenderer.cs, IImageService.cs; Code Access Security attributes removal; Windows Forms control APIs)
- [✓] (8) WinFormsUI builds for net10.0 with 0 errors (**Verify**)
- [✓] (9) Build WinFormsUI for all other targets (net35, net40, netcoreapp3.1) to verify no regressions
- [✓] (10) All 4 target frameworks build successfully (**Verify**)
- [✓] (11) Commit changes with message: "Phase 1 complete: WinFormsUI migrated to net10.0"
- [✓] (12) Tag commit: `git tag phase-1-complete`

---

### [✓] TASK-003: Phase 2a - Migrate high-complexity themes *(Completed: 2026-03-05 12:45)*
**References**: Plan §Phase 2a: High-Complexity Themes, Plan §Project-by-Project Migration Plans §Level 1

- [✓] (1) Update TargetFrameworks to `netcoreapp3.1;net35;net40;net10.0` in: ThemeVS2005Multithreading.csproj, ThemeVS2012.csproj, ThemeVS2013.csproj, ThemeVS2015.csproj
- [✓] (2) All 4 project files updated (**Verify**)
- [✓] (3) Add `System.Drawing.Common` version 9.0.0 to all 4 theme projects
- [✓] (4) PackageReferences added (**Verify**)
- [✓] (5) Restore dependencies for all 4 projects
- [✓] (6) Dependencies restored successfully (**Verify**)
- [✓] (7) Build all 4 theme projects for net10.0 and fix compilation errors per Plan §Breaking Changes Catalog (focus: GDI+ rendering APIs, theme-specific drawing code, Windows Forms controls)
- [✓] (8) All 4 themes build for net10.0 with 0 errors (**Verify**)
- [✓] (9) Build all 4 themes for other targets (net35, net40, netcoreapp3.1) to verify no regressions
- [✓] (10) All themes build successfully for all targets (**Verify**)
- [✓] (11) Commit changes with message: "Phase 2a complete: High-complexity themes migrated"
- [✓] (12) Tag commit: `git tag phase-2a-complete`

---

### [▶] TASK-004: Phase 2b - Migrate medium themes and test libraries
**References**: Plan §Phase 2b: Medium-Complexity Themes + Simple Tests, Plan §Project-by-Project Migration Plans §Level 1

- [▶] (1) Update TargetFrameworks to `netcoreapp3.1;net35;net40;net10.0` in ThemeVS2003.csproj and ThemeVS2005.csproj
- [ ] (2) Theme project files updated (**Verify**)
- [ ] (3) Add `System.Drawing.Common` version 9.0.0 to ThemeVS2003 and ThemeVS2005
- [ ] (4) PackageReferences added to themes (**Verify**)
- [ ] (5) Convert Tests2.csproj and Tests3.csproj to SDK-style format and set TargetFramework to `net10.0-windows` (reference Plan §Tests2.csproj and §Tests3.csproj for conversion details)
- [ ] (6) Tests2 and Tests3 converted to SDK-style (**Verify**)
- [ ] (7) Restore dependencies for all 4 projects
- [ ] (8) Dependencies restored successfully (**Verify**)
- [ ] (9) Build all 4 projects and fix any compilation errors
- [ ] (10) All 4 projects build with 0 errors (**Verify**)
- [ ] (11) Build themes for all targets to verify no regressions
- [ ] (12) All targets build successfully (**Verify**)
- [ ] (13) Commit changes with message: "Phase 2b complete: All libraries migrated to net10.0"
- [ ] (14) Tag commit: `git tag phase-2b-complete`

---

### [ ] TASK-005: Phase 3 - Migrate applications
**References**: Plan §Phase 3: Applications, Plan §DockSample.csproj, Plan §Tests.csproj

- [ ] (1) Convert DockSample.csproj to SDK-style format and set TargetFramework to `net10.0-windows` per Plan §DockSample.csproj §SDK Conversion
- [ ] (2) DockSample converted to SDK-style (**Verify**)
- [ ] (3) Add `System.Drawing.Common` version 9.0.0 and `System.Configuration.ConfigurationManager` version 9.0.0 to DockSample
- [ ] (4) PackageReferences added to DockSample (**Verify**)
- [ ] (5) Convert Tests.csproj to SDK-style format and set TargetFramework to `net10.0-windows` per Plan §Tests.csproj §SDK Conversion
- [ ] (6) Tests converted to SDK-style (**Verify**)
- [ ] (7) Update NUnit packages in Tests.csproj: add `Microsoft.NET.Test.Sdk` version 17.12.0, retain NUnit 3.9.0 and NUnit3TestAdapter 3.9.0
- [ ] (8) NUnit packages updated (**Verify**)
- [ ] (9) Restore dependencies for both applications
- [ ] (10) Dependencies restored successfully (**Verify**)
- [ ] (11) Build DockSample and fix all compilation errors per Plan §DockSample Migration Steps (focus: designer files, configuration system, Windows Forms APIs, theme integration)
- [ ] (12) DockSample builds with 0 errors (**Verify**)
- [ ] (13) Build Tests.csproj and fix compilation errors
- [ ] (14) Tests project builds with 0 errors (**Verify**)
- [ ] (15) Build entire solution to verify integration: `dotnet build WinFormsUI.Docking.sln`
- [ ] (16) Solution builds with 0 errors (**Verify**)
- [ ] (17) Commit changes with message: "Phase 3 complete: All applications migrated to net10.0"
- [ ] (18) Tag commit: `git tag phase-3-complete`

---

### [ ] TASK-006: Run full test suite and validate upgrade
**References**: Plan §Testing & Validation Strategy §Phase 3 Testing

- [ ] (1) Run tests in Tests.csproj: `dotnet test Tests\Tests.csproj`
- [ ] (2) Fix any test failures (reference Plan §Breaking Changes for behavioral changes)
- [ ] (3) Re-run tests after fixes
- [ ] (4) All tests pass with 0 failures (**Verify**)
- [ ] (5) Commit test fixes and completion with message: "Migration complete: All projects targeting .NET 10.0"
- [ ] (6) Tag commit: `git tag v10.0-migration-complete`

---






