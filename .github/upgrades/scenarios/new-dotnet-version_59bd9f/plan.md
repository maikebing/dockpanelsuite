# .NET 10.0 Upgrade Plan — WinFormsUI.Docking Solution

## Table of Contents

- [Executive Summary](#executive-summary)
- [Migration Strategy](#migration-strategy)
- [Detailed Dependency Analysis](#detailed-dependency-analysis)
- [Project-by-Project Migration Plans](#project-by-project-migration-plans)
  - [Level 0: WinFormsUI.csproj](#level-0-winformsuicsproj)
  - [Level 1: Theme Libraries](#level-1-theme-libraries)
  - [Level 1: Test Libraries](#level-1-test-libraries)
  - [Level 2: Applications](#level-2-applications)
- [Risk Management](#risk-management)
- [Testing & Validation Strategy](#testing--validation-strategy)
- [Complexity & Effort Assessment](#complexity--effort-assessment)
- [Source Control Strategy](#source-control-strategy)
- [Success Criteria](#success-criteria)

---

## Executive Summary

### Scenario Overview

This plan outlines the upgrade of the **WinFormsUI.Docking** solution from multiple .NET Framework versions (.NET Framework 3.5/4.0/4.8, .NET Core 3.1) to **.NET 10.0** (Long Term Support).

**Scope:**
- **11 projects** across 3 dependency levels
- **106 code files** with compatibility issues
- **12,664 total issues** (7,136 mandatory, 5,528 potential)
- Primary technologies: Windows Forms, GDI+, Code Access Security

**Current State:**
- Foundation library `WinFormsUI.csproj`: Multi-targeted (`netcoreapp3.1;net35;net40`)
- Theme libraries (6 projects): Multi-targeted SDK-style projects
- Sample/Test applications (4 projects): Classic .NET Framework 4.8 projects

**Target State:**
- All projects add `net10.0` or `net10.0-windows` target framework
- Classic projects converted to SDK-style format
- All binary/source incompatibilities resolved
- Legacy APIs (CAS, old configuration) removed or replaced

### Complexity Assessment

**Discovered Metrics:**
- **Project count:** 11
- **Dependency depth:** 2 levels (no circular dependencies)
- **High-risk projects:** 2 (WinFormsUI: 1,648 mandatory issues | DockSample: 2,582 mandatory issues)
- **Security vulnerabilities:** None
- **Package compatibility:** All packages (NUnit) are compatible with .NET 10.0
- **Major API surface changes:** 7,121 binary-incompatible API usages, 5,502 source-incompatible

**Classification:** **Medium-to-Complex Solution**

**Rationale:**
- ✓ Project count manageable (≤15)
- ✓ Clear dependency hierarchy (no cycles)
- ⚠️ High issue volume (12k+ issues) due to extensive Windows Forms API surface
- ⚠️ Mixed project formats (SDK-style + classic)
- ⚠️ Multi-targeting complexity (maintaining net35/net40 + adding net10.0)
- ⚠️ Large code surface (106 files, 61 files in foundation library alone)

### Selected Strategy

**Incremental Migration with Phase-Based Batching**

**Rationale:**
- Foundation library (`WinFormsUI`) affects all 10 dependent projects — must migrate first
- Theme libraries share similar patterns — can batch by complexity
- Classic projects need SDK conversion before retargeting — separate phase for safety
- High mandatory issue count requires careful validation at each phase
- Multi-targeting approach allows gradual adoption and backward compatibility

### Iteration Strategy

This plan uses **5 detail iterations** after foundation setup:

1. **Phase 1 (Iteration 2.1):** Foundation library `WinFormsUI.csproj` (highest impact)
2. **Phase 2a (Iteration 2.2):** High-complexity theme libraries (VS2005Multithreading, VS2012, VS2013, VS2015)
3. **Phase 2b (Iteration 2.3):** Medium-complexity theme libraries (VS2003, VS2005) + Simple test libraries (Tests2, Tests3)
4. **Phase 3 (Iteration 2.4):** Applications requiring SDK conversion (DockSample, Tests)
5. **Final (Iteration 2.5):** Testing strategy, Risk management, Success criteria

### Critical Issues

**Mandatory Actions:**
1. **SDK-style conversion:** 4 projects (`DockSample`, `Tests`, `Tests2`, `Tests3`) use classic project format
2. **Target framework updates:** All 11 projects must add `net10.0` or `net10.0-windows` to target frameworks
3. **Binary incompatibility resolution:** 7,121 API usages require recompilation and potential code changes
4. **Source incompatibility fixes:** 5,502 API usages need code modifications (obsolete members, changed signatures)
5. **Technology replacements:**
   - **Code Access Security (CAS):** Remove 4 security attribute usages (not supported in modern .NET)
   - **Legacy Configuration:** Replace 2 usages with modern `Microsoft.Extensions.Configuration`
   - **GDI+ compatibility:** Add `System.Drawing.Common` NuGet package (5,502 usages across solution)

**No Security Vulnerabilities Detected** ✓

### Recommended Approach

**Dependency-Ordered Incremental Migration**

Migrate projects level-by-level (bottom-up) to ensure dependencies are always satisfied:
- **Level 0 (Foundation):** WinFormsUI → Validates multi-targeting approach
- **Level 1 (Dependents):** Theme + Test libraries → Batch similar projects
- **Level 2 (Applications):** DockSample + Tests → Final consumers, most visible changes

Each phase includes:
1. Project file updates (TargetFramework, SDK conversion if needed)
2. NuGet package additions (System.Drawing.Common)
3. Code modifications (API replacements, obsolete API removal)
4. Build validation (all targets build successfully)
5. Test execution (where applicable)

**Timeline:** Complete in 3 phases with clear validation checkpoints between each phase.

---

## Migration Strategy

### Approach Selection

**Selected Strategy: Incremental Migration with Phase-Based Batching**

**Justification:**

1. **Dependency-Driven Sequencing:**
   - WinFormsUI is the foundation library used by all 10 other projects
   - Must validate foundation changes before propagating to dependents
   - Clear 3-level hierarchy enables natural phase boundaries

2. **Risk Management:**
   - 12,664 total issues require careful validation checkpoints
   - High mandatory issue count (7,136) means breaking changes are certain
   - Incremental approach isolates failures to specific phases
   - Each phase leaves solution in buildable state

3. **Multi-Targeting Complexity:**
   - SDK-style projects maintain `net35;net40;netcoreapp3.1` targets while adding `net10.0`
   - Requires validation that new target doesn't break existing targets
   - Incremental testing ensures multi-targeting works correctly

4. **Mixed Project Formats:**
   - 7 SDK-style projects (already modern) vs 4 classic projects
   - Classic projects need SDK conversion before retargeting
   - Separating these into distinct phases reduces cognitive load and risk

5. **Testing Strategy:**
   - Solution includes test projects (Tests, Tests2, Tests3)
   - Each phase can validate changes with available tests
   - Application project (DockSample) provides manual validation opportunity

**Why Not All-At-Once:**
- 11 projects with 12k+ issues create too large a "blast radius"
- Mixed SDK-style + classic format increases SDK conversion risk
- Multi-targeting requires validation that old targets still work
- No single comprehensive test suite to validate entire solution at once

### Dependency-Based Ordering Rationale

**Phase Ordering Principles:**

1. **Bottom-Up Migration (Respect Dependencies):**
   - Migrate projects with zero dependencies first (Level 0)
   - Then migrate projects depending only on completed projects
   - Never migrate a project before its dependencies complete

2. **Foundation-First (Highest Impact):**
   - WinFormsUI affects all 10 downstream projects
   - Breaking changes in WinFormsUI API surface discovered early
   - Pattern established for theme libraries to follow

3. **Complexity-Based Batching (Within Levels):**
   - Group projects with similar issue counts and patterns
   - High-complexity themes (1,400+ issues) in one batch
   - Medium/simple themes + test libraries in another batch
   - Applications requiring SDK conversion in final batch

4. **Validation Checkpoints:**
   - Each phase ends with solution build + test execution
   - Detect issues before they propagate to dependent projects
   - Rollback scope limited to current phase if failures occur

### Parallel vs Sequential Execution Decisions

**Sequential Phases (Required):**
- **Phase 1 → Phase 2:** WinFormsUI must complete before any theme/test library
- **Phase 2 → Phase 3:** All themes must complete before applications
- **Reason:** Dependency constraints — dependents require dependencies to build

**Parallel Within Phases (Possible):**

*Phase 2a (4 theme libraries):*
- All can be migrated in parallel (no inter-dependencies)
- Shared dependency (WinFormsUI) already completed in Phase 1
- Similar patterns reduce context-switching overhead
- **Execution:** Can batch as single coordinated update

*Phase 2b (2 themes + 2 test libraries):*
- All 4 can be migrated in parallel
- Reduces phase duration
- **Execution:** Can batch as single coordinated update

*Phase 3 (2 applications):*
- DockSample and Tests have no inter-dependency
- Both require SDK conversion (similar process)
- Can migrate in parallel
- **Execution:** Can batch as single coordinated update

**Recommended Execution Mode:**
- **Within each phase:** Batch all projects together (parallel conceptually)
- **Across phases:** Strict sequential (validate Phase N before starting Phase N+1)
- **Build validation:** After each phase, build entire solution to detect unexpected interactions

### Phase Definitions

**Phase 1: Foundation Library**
- **Projects:** WinFormsUI.csproj (1 project)
- **Objective:** Add `net10.0` target, resolve 1,648 mandatory API issues
- **Deliverable:** WinFormsUI builds for all targets (`net35`, `net40`, `netcoreapp3.1`, `net10.0`)
- **Validation:** Build succeeds, no new warnings, existing tests pass
- **Duration Category:** High complexity (largest single-project effort)

**Phase 2: Theme & Test Libraries**

*Phase 2a: High-Complexity Themes*
- **Projects:** ThemeVS2005Multithreading, ThemeVS2012, ThemeVS2013, ThemeVS2015 (4 projects)
- **Objective:** Add `net10.0` target, resolve 460-525 mandatory issues per project
- **Deliverable:** All 4 themes build for all targets
- **Validation:** Solution builds, themes load correctly against WinFormsUI net10.0
- **Duration Category:** Medium complexity per project, batchable

*Phase 2b: Medium-Complexity Themes + Simple Tests*
- **Projects:** ThemeVS2003, ThemeVS2005, Tests2, Tests3 (4 projects)
- **Objective:** 
  - Themes: Add `net10.0` target, resolve 373-527 mandatory issues
  - Tests2/3: SDK conversion + retarget to `net10.0-windows`
- **Deliverable:** All 4 projects build successfully
- **Validation:** Solution builds, tests2/3 demonstrate SDK conversion success
- **Duration Category:** Low-to-medium complexity

**Phase 3: Applications**
- **Projects:** DockSample.csproj, Tests.csproj (2 projects)
- **Objective:** SDK conversion + retarget to `net10.0-windows`, resolve all API issues
- **Deliverable:** Applications run successfully on .NET 10.0
- **Validation:** 
  - Build succeeds with zero errors
  - DockSample.exe launches and demonstrates dock panel functionality
  - Tests.csproj unit tests execute and pass
- **Duration Category:** High complexity (DockSample has 2,582 mandatory issues)

### Multi-Targeting Approach

**SDK-Style Projects (WinFormsUI, ThemeVS*):**
- **Current:** `<TargetFrameworks>netcoreapp3.1;net35;net40</TargetFrameworks>`
- **Target:** `<TargetFrameworks>netcoreapp3.1;net35;net40;net10.0</TargetFrameworks>`
- **Rationale:** Maintain backward compatibility for consumers still on older frameworks
- **Validation:** Build all targets to ensure no regressions in net35/net40/netcoreapp3.1

**Classic Projects (DockSample, Tests, Tests2, Tests3):**
- **Current:** `<TargetFramework>net48</TargetFramework>` (classic format)
- **Target:** `<TargetFramework>net10.0-windows</TargetFramework>` (SDK format)
- **Note:** `-windows` suffix required for Windows Forms projects in .NET 5+
- **No Multi-Targeting:** Applications typically target single framework version
- **SDK Conversion:** Use `dotnet upgrade-assistant` or manual conversion to SDK-style

### Breaking Change Strategy

**Expected Breaking Changes (per assessment):**
1. **Binary Incompatibility (7,121 occurrences):**
   - APIs changed between .NET Framework/Core 3.1 and .NET 10.0
   - Recompilation required (automatic during build)
   - May require code changes if API signatures changed

2. **Source Incompatibility (5,502 occurrences):**
   - Obsolete APIs removed
   - Method signatures changed
   - Properties renamed or moved to different types
   - **Action:** Code modifications required to compile

3. **Behavioral Changes (26 occurrences):**
   - APIs behave differently at runtime
   - May not cause build errors but cause runtime issues
   - **Action:** Code review + testing to verify behavior

**Mitigation Approach:**
- Address API changes file-by-file during each phase
- Consult breaking change documentation: https://learn.microsoft.com/dotnet/core/compatibility/
- Use compiler errors/warnings as guide to required changes
- Validate with tests after each batch of fixes

### Technology-Specific Migration Paths

**Windows Forms (7,108 issues):**
- Most issues are binary incompatibility (recompilation fixes)
- Some controls/properties deprecated or moved
- **Action:** Update to modern WinForms APIs, use `net10.0-windows` target
- **Reference:** https://learn.microsoft.com/dotnet/desktop/winforms/migration/

**GDI+ / System.Drawing (5,502 issues):**
- `System.Drawing` namespace split into separate NuGet package in .NET Core
- **Action:** Add `System.Drawing.Common` NuGet package to all projects
- **Note:** Cross-platform support limited; Windows-only in .NET 6+
- **Add to all projects:**
  ```xml
  <PackageReference Include="System.Drawing.Common" Version="9.0.0" />
  ```

**Code Access Security (4 issues):**
- CAS not supported in .NET Core/.NET 5+
- **Action:** Remove `[SecurityPermission]`, `[FileIOPermission]`, and similar attributes
- **Alternative:** Use OS-level permissions or managed security policies
- **Reference:** https://learn.microsoft.com/dotnet/core/compatibility/core-libraries/5.0/code-access-security-apis-obsolete

**Legacy Configuration (2 issues):**
- `System.Configuration.ConfigurationManager` works but deprecated pattern
- **Action:** Migrate to `Microsoft.Extensions.Configuration`
- **Add NuGet:** `Microsoft.Extensions.Configuration.Json` or `...Xml`
- **Code changes:** Replace `ConfigurationManager.AppSettings` with `IConfiguration` pattern

**Windows Forms Legacy Controls (2 issues):**
- Controls like `DataGrid` (old) replaced by `DataGridView`
- **Action:** Replace deprecated controls with modern equivalents
- **Reference:** Check assessment.md for specific control names

---

## Detailed Dependency Analysis

### Dependency Graph Summary

The solution follows a clean 3-level hierarchy with no circular dependencies:

```
Level 0 (Foundation)
└── WinFormsUI.csproj
    │
    ├─── Level 1 (Theme Libraries)
    │    ├── ThemeVS2003.csproj
    │    ├── ThemeVS2005.csproj
    │    ├── ThemeVS2005Multithreading.csproj
    │    ├── ThemeVS2012.csproj
    │    ├── ThemeVS2013.csproj
    │    └── ThemeVS2015.csproj
    │
    ├─── Level 1 (Simple Test Libraries)
    │    ├── Tests2.csproj
    │    └── Tests3.csproj
    │
    └─── Level 2 (Applications)
         ├── DockSample.csproj (depends on: WinFormsUI, ThemeVS2003, VS2005, VS2012, VS2013, VS2015)
         └── Tests.csproj (depends on: WinFormsUI, ThemeVS2012, VS2013, VS2015)
```

### Project Groupings by Migration Phase

**Phase 1: Foundation Library**
- **WinFormsUI.csproj** (Level 0)
  - Current: `netcoreapp3.1;net35;net40`
  - Target: `netcoreapp3.1;net35;net40;net10.0`
  - Issues: 2,290 (1,648 mandatory, 642 potential)
  - **Critical Path:** All 10 projects depend on this — must complete first
  - **Multi-targeting:** Append `net10.0` to maintain backward compatibility

**Phase 2: Theme Libraries & Test Libraries**

*Phase 2a: High-Complexity Themes*
- **ThemeVS2005Multithreading.csproj** (Level 1)
  - Issues: 1,284 (525 mandatory, 759 potential)
  - Current: `netcoreapp3.1;net35;net40`
  - Target: +`net10.0`

- **ThemeVS2012.csproj** (Level 1)
  - Issues: 1,445 (466 mandatory, 979 potential)
  - Current: `netcoreapp3.1;net35;net40`
  - Target: +`net10.0`
  - Used by: DockSample, Tests

- **ThemeVS2013.csproj** (Level 1)
  - Issues: 1,419 (460 mandatory, 959 potential)
  - Current: `netcoreapp3.1;net35;net40`
  - Target: +`net10.0`
  - Used by: DockSample, Tests

- **ThemeVS2015.csproj** (Level 1)
  - Issues: 1,419 (460 mandatory, 959 potential)
  - Current: `netcoreapp3.1;net35;net40`
  - Target: +`net10.0`
  - Used by: DockSample, Tests

*Phase 2b: Medium-Complexity Themes + Simple Tests*
- **ThemeVS2003.csproj** (Level 1)
  - Issues: 1,147 (527 mandatory, 620 potential)
  - Current: `netcoreapp3.1;net35;net40`
  - Target: +`net10.0`
  - Used by: DockSample

- **ThemeVS2005.csproj** (Level 1)
  - Issues: 923 (373 mandatory, 550 potential)
  - Current: `netcoreapp3.1;net35;net40`
  - Target: +`net10.0`
  - Used by: DockSample

- **Tests2.csproj** (Level 1) — Simple
  - Issues: 2 (2 mandatory, 0 potential)
  - Current: `net48`
  - Target: `net10.0-windows`
  - **SDK Conversion Required**

- **Tests3.csproj** (Level 1) — Simple
  - Issues: 2 (2 mandatory, 0 potential)
  - Current: `net48`
  - Target: `net10.0-windows`
  - **SDK Conversion Required**

**Phase 3: Applications**
- **DockSample.csproj** (Level 2)
  - Issues: 2,616 (2,582 mandatory, 34 potential)
  - Current: `net48`
  - Target: `net10.0-windows`
  - **SDK Conversion Required**
  - **Highest Issue Count** — Main demonstration application

- **Tests.csproj** (Level 2)
  - Issues: 117 (91 mandatory, 26 potential)
  - Current: `net48`
  - Target: `net10.0-windows`
  - **SDK Conversion Required**
  - Contains NUnit test projects

### Critical Path Identification

**Blocking Dependency Chain:**
1. **WinFormsUI** must complete before ANY other project
2. Theme libraries can migrate in parallel after WinFormsUI completes
3. Applications (DockSample, Tests) must wait for all themes they reference

**Parallel Execution Opportunities:**
- **Phase 2a themes:** All 4 can be migrated in parallel (no inter-dependencies)
- **Phase 2b themes + tests:** All 4 can be migrated in parallel
- **Phase 3 applications:** DockSample and Tests can be migrated in parallel (no inter-dependency)

**Sequential Requirements:**
- Phase 2 cannot start until Phase 1 completes and builds successfully
- Phase 3 cannot start until all Phase 2 projects complete

### Circular Dependencies

**None detected.** The dependency graph is a clean directed acyclic graph (DAG) with clear levels.

---

## Project-by-Project Migration Plans

### Level 0: WinFormsUI.csproj

**Project:** WinFormsUI  
**Path:** `WinFormsUI\WinFormsUI.csproj`  
**Type:** Class Library (SDK-style)  
**Dependencies:** None (foundation library)  
**Used By:** All 10 other projects in solution

**Current State:**
- Target Frameworks: `netcoreapp3.1;net35;net40`
- SDK-style: Yes
- Files: 61 code files
- Issues: 2,290 (1,648 mandatory, 642 potential)
- NuGet packages: None relevant for upgrade

**Target State:**
- Target Frameworks: `netcoreapp3.1;net35;net40;net10.0` (append net10.0)
- Add NuGet: `System.Drawing.Common` version 9.0.0

#### Migration Steps

**1. Prerequisites**
- Ensure .NET 10.0 SDK installed: `dotnet --list-sdks` (verify 10.0.x present)
- Backup current state: `git commit -m "Before WinFormsUI migration"`
- Create Phase 1 tag: `git tag phase-1-start`

**2. Project File Update**

Update `WinFormsUI\WinFormsUI.csproj`:

```xml
<PropertyGroup>
  <TargetFrameworks>netcoreapp3.1;net35;net40;net10.0</TargetFrameworks>
  <!-- existing properties preserved -->
</PropertyGroup>
```

**3. Add NuGet Package**

Add `System.Drawing.Common` for GDI+ support in .NET 10.0:

```xml
<ItemGroup>
  <PackageReference Include="System.Drawing.Common" Version="9.0.0" />
</ItemGroup>
```

**Note:** Package applies to all target frameworks. If build issues occur with net35/net40, use conditional reference:
```xml
<PackageReference Include="System.Drawing.Common" Version="9.0.0" Condition="'$(TargetFramework)' == 'net10.0'" />
```

**4. Restore Dependencies**

```bash
dotnet restore WinFormsUI\WinFormsUI.csproj
```

**5. Build and Identify Compilation Errors**

Build all target frameworks:

```bash
dotnet build WinFormsUI\WinFormsUI.csproj
```

Expected: Compilation errors for net10.0 target due to:
- Binary incompatible APIs (1,647 occurrences)
- Source incompatible APIs (642 occurrences)
- Code Access Security attributes (4 occurrences)

**6. Expected Breaking Changes**

Based on assessment analysis, the following file groups have compatibility issues:

**High-Impact Files (>100 issues each):**
- `VisualStudioToolStripRenderer.cs` — 413 issues (254 binary, 159 source)
  - GDI+ drawing APIs (`System.Drawing.*`)
  - Color/Brush/Pen API changes

- `IImageService.cs` — 297 issues (297 source incompatible)
  - Image manipulation APIs
  - Likely obsolete methods in Image/Bitmap classes

- `DockWindow.cs` — 139 issues (139 binary)
  - Windows Forms control APIs

- `FloatWindow.cs` — 129 issues (123 binary, 6 source)
  - Form/Control APIs

- `DockPane.cs` — 114 issues (113 binary, 1 source)
  - Control/Component APIs

**Medium-Impact Files (50-100 issues):**
- `InertButtonBase.cs` — 98 issues (45 binary, 53 source)
- `AutoHideStripBase.cs` — 78 issues (48 binary, 30 source)
- `ThemeBase.cs` — 60 issues (60 binary)
- `DockPaneCaptionBase.cs` — 58 issues (57 binary, 1 source)
- `DragForm.cs` — 50 issues (46 binary, 4 source)

**Code Access Security Files:**
- Identify files with `[SecurityPermission]`, `[FileIOPermission]`, etc.
- Remove these attributes (CAS not supported in .NET Core/5+)

**Common API Replacements Expected:**

| Obsolete API | Replacement | Affected Files |
|--------------|-------------|----------------|
| `System.Drawing` APIs | Add `System.Drawing.Common` NuGet | All files using GDI+ |
| `[SecurityPermission(...)]` | Remove attribute | Search for `SecurityPermission` |
| `Control.OnPaint(PaintEventArgs)` signature changes | Review method overrides | Multiple Control-derived classes |
| Color/Brush/Pen constructors | Review for parameter changes | Rendering files |

**7. Code Modifications Approach**

**Iterative File-by-File Strategy:**

1. Build project: `dotnet build WinFormsUI\WinFormsUI.csproj -f net10.0`
2. Address compilation errors in order of impact:
   - Start with files having most errors (VisualStudioToolStripRenderer.cs first)
   - Use compiler error messages to identify specific API issues
3. Common fix patterns:
   - **Missing namespace:** Add `using System.Drawing;` if needed
   - **Obsolete method:** Check Microsoft docs for replacement API
   - **Changed signature:** Update method signature to match new definition
   - **Removed property:** Search for alternative property or workaround
4. Rebuild after each file fix to track progress
5. Reference breaking change docs: https://learn.microsoft.com/dotnet/core/compatibility/

**Remove Code Access Security Attributes:**

Search entire project for:
```csharp
[SecurityPermission(SecurityAction.*)]
[FileIOPermission(*)]
```

Remove these attributes — they are no-ops in .NET Core/5+ and may cause warnings.

**GDI+ Image API Updates:**

If `IImageService.cs` has obsolete `Image` or `Bitmap` methods:
- Check https://learn.microsoft.com/dotnet/api/system.drawing.image
- Common changes: `Image.FromHbitmap` signature, `Bitmap.LockBits` behavior
- May need to update image handling code

**Windows Forms Control API Updates:**

For Control-derived classes (`DockWindow`, `FloatWindow`, `DockPane`):
- Override methods may have signature changes
- Event handling patterns may require updates
- Check for removed/renamed properties

**8. Multi-Target Validation**

After net10.0 builds successfully, validate all targets:

```bash
dotnet build WinFormsUI\WinFormsUI.csproj -f net35
dotnet build WinFormsUI\WinFormsUI.csproj -f net40
dotnet build WinFormsUI\WinFormsUI.csproj -f netcoreapp3.1
dotnet build WinFormsUI\WinFormsUI.csproj -f net10.0
```

**Critical:** Ensure changes for net10.0 don't break existing net35/net40/netcoreapp3.1 builds.

If conditional compilation needed:
```csharp
#if NET10_0_OR_GREATER
    // .NET 10.0-specific code
#else
    // Legacy framework code
#endif
```

**9. Testing Strategy**

**Build Validation:**
- All 4 target frameworks build with 0 errors
- Warnings reviewed (acceptable warnings documented)

**Unit Tests (if available):**
- Run existing tests against net10.0 build
- If test project exists, target it at WinFormsUI net10.0 assembly
- Verify no regression in test results

**Smoke Test (manual):**
- Build dependent project (e.g., DockSample targeting net10.0 temporarily)
- Verify WinFormsUI.dll loads correctly
- Check for runtime errors related to API changes

**10. Validation Checklist**

- [ ] .NET 10.0 SDK installed and verified
- [ ] Project file updated with `net10.0` in TargetFrameworks
- [ ] `System.Drawing.Common` NuGet package added (version 9.0.0)
- [ ] Dependencies restored successfully
- [ ] All compilation errors in net10.0 target resolved
- [ ] All Code Access Security attributes removed
- [ ] Multi-target build succeeds for all 4 frameworks
- [ ] No new warnings introduced (or warnings documented/suppressed)
- [ ] Existing tests pass (if applicable)
- [ ] Changes committed: `git commit -m "Phase 1: WinFormsUI migrated to net10.0"`
- [ ] Phase 1 completion tag: `git tag phase-1-complete`

**Success Criteria:**
- WinFormsUI builds for `net35`, `net40`, `netcoreapp3.1`, and `net10.0` with zero errors
- No regressions in existing target frameworks
- Foundation ready for Phase 2 (theme libraries)

---

### Level 1: Theme Libraries

#### ThemeVS2005Multithreading.csproj

**Project:** ThemeVS2005Multithreading  
**Path:** `WinFormsUI\ThemeVS2005Multithreading.csproj`  
**Type:** Class Library (SDK-style)  
**Dependencies:** WinFormsUI  
**Used By:** None (top-level library)

**Current State:**
- Target Frameworks: `netcoreapp3.1;net35;net40`
- SDK-style: Yes
- Issues: 1,284 (525 mandatory, 759 potential)

**Target State:**
- Target Frameworks: `netcoreapp3.1;net35;net40;net10.0`
- Add NuGet: `System.Drawing.Common` version 9.0.0

#### Migration Steps

**1. Prerequisites**
- Phase 1 complete: WinFormsUI builds successfully for net10.0 ✓
- Commit Phase 1: `git tag phase-1-complete` ✓

**2. Project File Update**

```xml
<PropertyGroup>
  <TargetFrameworks>netcoreapp3.1;net35;net40;net10.0</TargetFrameworks>
</PropertyGroup>
```

**3. Add NuGet Package**

```xml
<ItemGroup>
  <PackageReference Include="System.Drawing.Common" Version="9.0.0" />
</ItemGroup>
```

**4. Build and Fix Compilation Errors**

```bash
dotnet build WinFormsUI\ThemeVS2005Multithreading.csproj -f net10.0
```

**Expected Issues:**
- **Binary incompatibility:** 525 occurrences (Windows Forms controls/properties)
- **Source incompatibility:** 759 occurrences (GDI+ APIs, obsolete methods)
- **Multithreading patterns:** Review for behavioral changes in thread-related APIs

**Common Fix Patterns:**
- GDI+ API updates (Color, Brush, Pen usage)
- Windows Forms control property changes
- Theme rendering code using System.Drawing

**5. Validation Checklist**

- [ ] All 4 target frameworks build successfully
- [ ] No threading-related behavioral issues introduced
- [ ] Theme renders correctly (visual validation if possible)

---

#### ThemeVS2012.csproj

**Project:** ThemeVS2012  
**Path:** `WinFormsUI\ThemeVS2012.csproj`  
**Type:** Class Library (SDK-style)  
**Dependencies:** WinFormsUI  
**Used By:** DockSample, Tests

**Current State:**
- Target Frameworks: `netcoreapp3.1;net35;net40`
- SDK-style: Yes
- Issues: 1,445 (466 mandatory, 979 potential)

**Target State:**
- Target Frameworks: `netcoreapp3.1;net35;net40;net10.0`
- Add NuGet: `System.Drawing.Common` version 9.0.0

#### Migration Steps

**1-3. Project File + NuGet** (same as ThemeVS2005Multithreading)

**4. Build and Fix Compilation Errors**

```bash
dotnet build WinFormsUI\ThemeVS2012.csproj -f net10.0
```

**Expected Issues:**
- **Binary incompatibility:** 466 occurrences
- **Source incompatibility:** 979 occurrences (highest potential issue count in theme group)

**Key Areas:**
- VS2012 theme-specific rendering (light/dark color schemes)
- ToolStrip rendering customizations
- DockPanel theme integration

**5. Validation Checklist**

- [ ] All 4 target frameworks build successfully
- [ ] Theme loads in DockSample (manual test after Phase 3)
- [ ] Used by 2 consumers (DockSample, Tests) — critical for validation

---

#### ThemeVS2013.csproj

**Project:** ThemeVS2013  
**Path:** `WinFormsUI\ThemeVS2013.csproj`  
**Type:** Class Library (SDK-style)  
**Dependencies:** WinFormsUI  
**Used By:** DockSample, Tests

**Current State:**
- Target Frameworks: `netcoreapp3.1;net35;net40`
- SDK-style: Yes
- Issues: 1,419 (460 mandatory, 959 potential)

**Target State:**
- Target Frameworks: `netcoreapp3.1;net35;net40;net10.0`
- Add NuGet: `System.Drawing.Common` version 9.0.0

#### Migration Steps

**1-3. Project File + NuGet** (same as ThemeVS2005Multithreading)

**4. Build and Fix Compilation Errors**

```bash
dotnet build WinFormsUI\ThemeVS2013.csproj -f net10.0
```

**Expected Issues:**
- **Binary incompatibility:** 460 occurrences
- **Source incompatibility:** 959 occurrences

**Key Areas:**
- VS2013 theme styling (similar to VS2012 but different color palettes)
- Custom control rendering
- Theme switcher integration

**5. Validation Checklist**

- [ ] All 4 target frameworks build successfully
- [ ] Theme loads in DockSample (manual test after Phase 3)
- [ ] Used by 2 consumers — validate integration

---

#### ThemeVS2015.csproj

**Project:** ThemeVS2015  
**Path:** `WinFormsUI\ThemeVS2015.csproj`  
**Type:** Class Library (SDK-style)  
**Dependencies:** WinFormsUI  
**Used By:** DockSample, Tests

**Current State:**
- Target Frameworks: `netcoreapp3.1;net35;net40`
- SDK-style: Yes
- Issues: 1,419 (460 mandatory, 959 potential)

**Target State:**
- Target Frameworks: `netcoreapp3.1;net35;net40;net10.0`
- Add NuGet: `System.Drawing.Common` version 9.0.0

#### Migration Steps

**1-3. Project File + NuGet** (same as ThemeVS2005Multithreading)

**4. Build and Fix Compilation Errors**

```bash
dotnet build WinFormsUI\ThemeVS2015.csproj -f net10.0
```

**Expected Issues:**
- **Binary incompatibility:** 460 occurrences
- **Source incompatibility:** 959 occurrences
- **Note:** Identical issue count to ThemeVS2013 (likely similar implementation)

**Key Areas:**
- VS2015 theme styling
- Flat design elements
- Modern color schemes

**5. Validation Checklist**

- [ ] All 4 target frameworks build successfully
- [ ] Theme loads in DockSample (manual test after Phase 3)

---

### Phase 2a Summary

**Batch Execution Approach:**

All 4 high-complexity themes can be updated together:

1. Update all 4 project files simultaneously (add net10.0 target)
2. Add System.Drawing.Common to all 4
3. Restore dependencies for entire solution: `dotnet restore`
4. Build each project individually, fix compilation errors
5. Build entire solution to verify integration: `dotnet build WinFormsUI.Docking.sln`

**Shared Patterns:**
- All use GDI+ extensively (System.Drawing.Common required)
- All implement custom theme rendering
- All depend on WinFormsUI (already migrated)
- Similar API surface means similar fixes

**Phase 2a Completion Criteria:**
- All 4 themes build successfully for all target frameworks
- Solution builds without errors
- Ready to proceed to Phase 2b
- Commit: `git commit -m "Phase 2a: High-complexity themes migrated"`
- Tag: `git tag phase-2a-complete`

---

#### ThemeVS2003.csproj

**Project:** ThemeVS2003  
**Path:** `WinFormsUI\ThemeVS2003.csproj`  
**Type:** Class Library (SDK-style)  
**Dependencies:** WinFormsUI  
**Used By:** DockSample

**Current State:**
- Target Frameworks: `netcoreapp3.1;net35;net40`
- SDK-style: Yes
- Issues: 1,147 (527 mandatory, 620 potential)

**Target State:**
- Target Frameworks: `netcoreapp3.1;net35;net40;net10.0`
- Add NuGet: `System.Drawing.Common` version 9.0.0

#### Migration Steps

**1-3. Project File + NuGet** (same pattern as Phase 2a themes)

**4. Build and Fix Compilation Errors**

```bash
dotnet build WinFormsUI\ThemeVS2003.csproj -f net10.0
```

**Expected Issues:**
- **Binary incompatibility:** 527 occurrences
- **Source incompatibility:** 620 occurrences

**Key Areas:**
- VS2003 classic theme styling (older Visual Studio look)
- May have different rendering approach than modern themes
- GDI+ drawing for classic look-and-feel

**5. Validation Checklist**

- [ ] All 4 target frameworks build successfully
- [ ] Classic theme appearance validated (if visual testing available)

---

#### ThemeVS2005.csproj

**Project:** ThemeVS2005  
**Path:** `WinFormsUI\ThemeVS2005.csproj`  
**Type:** Class Library (SDK-style)  
**Dependencies:** WinFormsUI  
**Used By:** DockSample

**Current State:**
- Target Frameworks: `netcoreapp3.1;net35;net40`
- SDK-style: Yes
- Issues: 923 (373 mandatory, 550 potential)

**Target State:**
- Target Frameworks: `netcoreapp3.1;net35;net40;net10.0`
- Add NuGet: `System.Drawing.Common` version 9.0.0

#### Migration Steps

**1-3. Project File + NuGet** (same pattern)

**4. Build and Fix Compilation Errors**

```bash
dotnet build WinFormsUI\ThemeVS2005.csproj -f net10.0
```

**Expected Issues:**
- **Binary incompatibility:** 373 occurrences (lowest mandatory count in themes)
- **Source incompatibility:** 550 occurrences

**Key Areas:**
- VS2005 theme styling
- Simpler implementation compared to other themes
- Lower issue count suggests less API surface

**5. Validation Checklist**

- [ ] All 4 target frameworks build successfully
- [ ] Theme integrates with DockSample

---

### Level 1: Test Libraries

#### Tests2.csproj

**Project:** Tests2  
**Path:** `Tests2\Tests2.csproj`  
**Type:** Class Library (Classic .NET Framework)  
**Dependencies:** WinFormsUI  
**Used By:** None

**Current State:**
- Target Framework: `net48`
- SDK-style: **No** (classic project format)
- Issues: 2 (2 mandatory, 0 potential)
- **Requires SDK Conversion**

**Target State:**
- Target Framework: `net10.0-windows`
- SDK-style: Yes (after conversion)

#### Migration Steps

**1. Convert to SDK-Style Project**

**Option A: Using upgrade-assistant tool**

```bash
dotnet tool install -g upgrade-assistant
upgrade-assistant upgrade Tests2\Tests2.csproj --non-interactive
```

**Option B: Manual conversion**

1. Backup original: `copy Tests2\Tests2.csproj Tests2\Tests2.csproj.old`
2. Create new SDK-style project file:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0-windows</TargetFramework>
    <UseWindowsForms>true</UseWindowsForms>
    <!-- Copy other essential properties from old .csproj -->
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\WinFormsUI\WinFormsUI.csproj" />
    <!-- Manually add any NuGet package references from old format -->
  </ItemGroup>
</Project>
```

3. Remove `packages.config` (if exists) — SDK-style uses PackageReference
4. Update references to use WinFormsUI net10.0 target

**2. Build and Validate**

```bash
dotnet build Tests2\Tests2.csproj
```

**Expected Issues:**
- Only 2 mandatory issues (likely project format + target framework)
- No significant API changes expected (simple test library)

**3. Validation Checklist**

- [ ] SDK conversion successful
- [ ] Builds with net10.0-windows target
- [ ] References WinFormsUI net10.0 correctly
- [ ] No compilation errors

---

#### Tests3.csproj

**Project:** Tests3  
**Path:** `Tests3\Tests3.csproj`  
**Type:** Class Library (Classic .NET Framework)  
**Dependencies:** WinFormsUI  
**Used By:** None

**Current State:**
- Target Framework: `net48`
- SDK-style: **No** (classic project format)
- Issues: 2 (2 mandatory, 0 potential)
- **Requires SDK Conversion**

**Target State:**
- Target Framework: `net10.0-windows`
- SDK-style: Yes (after conversion)

#### Migration Steps

**1-3. SDK Conversion + Build** (identical to Tests2 approach)

**Validation Checklist:**

- [ ] SDK conversion successful
- [ ] Builds with net10.0-windows target
- [ ] References WinFormsUI net10.0 correctly
- [ ] No compilation errors

---

### Phase 2b Summary

**Batch Execution Approach:**

All 4 projects (2 themes + 2 test libraries) can be updated together:

1. **Themes (ThemeVS2003, ThemeVS2005):**
   - Update project files (add net10.0 target)
   - Add System.Drawing.Common NuGet
   - Fix compilation errors (similar patterns to Phase 2a)

2. **Test Libraries (Tests2, Tests3):**
   - Convert to SDK-style format (upgrade-assistant or manual)
   - Retarget to net10.0-windows
   - Validate WinFormsUI reference works

3. **Solution Build:**
   ```bash
   dotnet build WinFormsUI.Docking.sln
   ```

**Phase 2b Completion Criteria:**
- ThemeVS2003 and ThemeVS2005 build for all target frameworks
- Tests2 and Tests3 converted to SDK-style and build successfully
- Entire solution (9 projects) builds without errors
- Ready to proceed to Phase 3 (applications)
- Commit: `git commit -m "Phase 2b: Medium themes and test libraries migrated"`
- Tag: `git tag phase-2b-complete`

**Combined Phase 2 (2a + 2b) Completion:**
- All 6 theme libraries migrated ✓
- All simple test libraries (Tests2, Tests3) migrated ✓
- WinFormsUI + 8 dependent libraries ready ✓
- Only applications remaining (DockSample, Tests)

---

### Level 2: Applications

#### DockSample.csproj

**Project:** DockSample  
**Path:** `DockSample\DockSample.csproj`  
**Type:** Windows Forms Application (Classic .NET Framework)  
**Dependencies:** WinFormsUI, ThemeVS2003, ThemeVS2005, ThemeVS2012, ThemeVS2013, ThemeVS2015  
**Used By:** None (top-level application)

**Current State:**
- Target Framework: `net48`
- SDK-style: **No** (classic project format)
- Files: 40 code files
- Issues: 2,616 (2,582 mandatory, 34 potential)
- **Highest issue count in solution**
- **Requires SDK Conversion**

**Target State:**
- Target Framework: `net10.0-windows`
- SDK-style: Yes (after conversion)
- Add NuGet: `System.Drawing.Common` version 9.0.0

#### Migration Steps

**1. Prerequisites**
- All library projects (Phase 1 + Phase 2) complete and building successfully ✓
- WinFormsUI, all 6 themes, Tests2, Tests3 all target net10.0 ✓
- Commit Phase 2: `git tag phase-2-complete` ✓

**2. Convert to SDK-Style Project**

**Critical:** This is the most complex SDK conversion in the solution.

**Option A: Using upgrade-assistant (Recommended)**

```bash
dotnet tool install -g upgrade-assistant
upgrade-assistant upgrade DockSample\DockSample.csproj --non-interactive --target-tfm net10.0-windows
```

The tool will:
- Convert project file to SDK-style format
- Update target framework to net10.0-windows
- Convert package references
- Preserve project references to theme libraries
- Update app.config handling

**Option B: Manual conversion**

1. Backup original project:
   ```bash
   copy DockSample\DockSample.csproj DockSample\DockSample.csproj.old
   copy DockSample\app.config DockSample\app.config.old
   ```

2. Create new SDK-style project file:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net10.0-windows</TargetFramework>
    <UseWindowsForms>true</UseWindowsForms>
    <ApplicationIcon>DockSample.ico</ApplicationIcon>
    <StartupObject>DockSample.Program</StartupObject>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\WinFormsUI\WinFormsUI.csproj" />
    <ProjectReference Include="..\WinFormsUI\ThemeVS2003.csproj" />
    <ProjectReference Include="..\WinFormsUI\ThemeVS2005.csproj" />
    <ProjectReference Include="..\WinFormsUI\ThemeVS2012.csproj" />
    <ProjectReference Include="..\WinFormsUI\ThemeVS2013.csproj" />
    <ProjectReference Include="..\WinFormsUI\ThemeVS2015.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="System.Drawing.Common" Version="9.0.0" />
    <PackageReference Include="System.Configuration.ConfigurationManager" Version="9.0.0" />
  </ItemGroup>
</Project>
```

3. **Configuration Migration:**
   - Classic app.config is partially supported in .NET 5+
   - Assessment shows 2 legacy configuration issues
   - Add `System.Configuration.ConfigurationManager` NuGet for compatibility
   - Alternative: Migrate to `Microsoft.Extensions.Configuration` for modern approach

**3. Restore Dependencies**

```bash
dotnet restore DockSample\DockSample.csproj
```

**4. Build and Identify Compilation Errors**

```bash
dotnet build DockSample\DockSample.csproj
```

**Expected Issues (2,582 mandatory):**

**High-Impact Areas:**

1. **Designer-Generated Files (Designer.cs):**
   - `DummyDoc.Designer.cs`, `MainForm.Designer.cs`, etc.
   - Windows Forms controls initialization code
   - ToolStrip, MenuStrip, ContextMenuStrip API changes
   - **Strategy:** Most designer issues auto-fix with recompilation
   - **If errors persist:** May need to regenerate designer files using Visual Studio

2. **Resources (Properties\Resources.Designer.cs):**
   - 2 source incompatibility issues with `System.Drawing.Bitmap`
   - Image resource handling changes
   - **Fix:** Add proper using statements, verify resource manager configuration

3. **Settings (Properties\Settings.Designer.cs):**
   - `ApplicationSettingsBase` API changes (legacy configuration system)
   - **Option 1:** Add `System.Configuration.ConfigurationManager` NuGet (backward compatible)
   - **Option 2:** Migrate to `IConfiguration` pattern (modern, recommended for new code)

4. **Main Application Code:**
   - Windows Forms control instantiation
   - Event handling patterns
   - Dock panel integration with themes

**5. Code Modifications**

**Configuration System Migration:**

**Current (Properties\Settings.Designer.cs):**
```csharp
internal sealed partial class Settings : ApplicationSettingsBase
{
    // Legacy configuration
}
```

**Option 1 - Add NuGet for Compatibility:**
```xml
<PackageReference Include="System.Configuration.ConfigurationManager" Version="9.0.0" />
```
This maintains existing code pattern.

**Option 2 - Migrate to Modern Configuration (if refactoring):**
```csharp
// Add NuGet: Microsoft.Extensions.Configuration.Json
// Update app startup to use IConfiguration
// Migrate settings to appsettings.json
```

**Resource Files:**
- Verify `Resources.resx` compiles correctly
- Ensure embedded resources are included in SDK-style project:
```xml
<ItemGroup>
  <EmbeddedResource Include="Properties\Resources.resx" />
  <!-- Add other .resx files if not auto-detected -->
</ItemGroup>
```

**Windows Forms API Updates:**
- Most issues are binary incompatibility (auto-fix with rebuild)
- Some potential source changes in event signatures
- Use compiler errors to guide specific fixes

**6. Multi-Theme Integration Validation**

DockSample references all 6 theme libraries. Verify:

```csharp
// Ensure theme switching works with net10.0 builds
// Test each theme loads correctly:
// - ThemeVS2003, ThemeVS2005, ThemeVS2012, ThemeVS2013, ThemeVS2015
```

**7. Application Testing**

**Build Validation:**
```bash
dotnet build DockSample\DockSample.csproj
```
Expected: 0 errors

**Run Application:**
```bash
dotnet run --project DockSample\DockSample.csproj
```

**Manual Testing Checklist:**
- [ ] Application launches without crashes
- [ ] Main form displays correctly
- [ ] Dock panels render properly
- [ ] Theme switching works (test all 6 themes)
- [ ] Document windows can be created/closed
- [ ] Docking operations work (drag/drop panels)
- [ ] Tool windows function correctly
- [ ] Application settings load/save (if using configuration)
- [ ] Resources load correctly (images, icons)
- [ ] No runtime exceptions in common workflows

**8. Validation Checklist**

- [ ] SDK conversion completed successfully
- [ ] Project builds with net10.0-windows target
- [ ] All 6 theme project references work correctly
- [ ] System.Drawing.Common NuGet added
- [ ] Configuration system working (app.config or migrated)
- [ ] Application runs and launches UI
- [ ] All 6 themes loadable and functional
- [ ] Dock panel features work (docking, floating, hiding)
- [ ] No runtime crashes in basic usage
- [ ] Commit: `git commit -m "Phase 3: DockSample migrated to net10.0"`

---

#### Tests.csproj

**Project:** Tests  
**Path:** `Tests\Tests.csproj`  
**Type:** Class Library with NUnit Tests (Classic .NET Framework)  
**Dependencies:** WinFormsUI, ThemeVS2012, ThemeVS2013, ThemeVS2015  
**Used By:** None (test project)

**Current State:**
- Target Framework: `net48`
- SDK-style: **No** (classic project format)
- Issues: 117 (91 mandatory, 26 potential)
- NuGet: NUnit 3.9.0, NUnit3TestAdapter 3.9.0 (compatible with .NET 10.0)
- **Requires SDK Conversion**

**Target State:**
- Target Framework: `net10.0-windows`
- SDK-style: Yes (after conversion)
- NuGet packages: Retain NUnit 3.9.0 (compatible)

#### Migration Steps

**1. Prerequisites**
- Phase 1 + Phase 2 complete ✓
- WinFormsUI, ThemeVS2012, ThemeVS2013, ThemeVS2015 all support net10.0 ✓

**2. Convert to SDK-Style Project**

**Option A: Using upgrade-assistant**

```bash
upgrade-assistant upgrade Tests\Tests.csproj --non-interactive --target-tfm net10.0-windows
```

**Option B: Manual conversion**

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0-windows</TargetFramework>
    <UseWindowsForms>true</UseWindowsForms>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\WinFormsUI\WinFormsUI.csproj" />
    <ProjectReference Include="..\WinFormsUI\ThemeVS2012.csproj" />
    <ProjectReference Include="..\WinFormsUI\ThemeVS2013.csproj" />
    <ProjectReference Include="..\WinFormsUI\ThemeVS2015.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="NUnit" Version="3.9.0" />
    <PackageReference Include="NUnit3TestAdapter" Version="3.9.0" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.12.0" />
  </ItemGroup>
</Project>
```

**Note:** Add `Microsoft.NET.Test.Sdk` for .NET 10.0 test execution.

**3. Build and Fix Compilation Errors**

```bash
dotnet build Tests\Tests.csproj
```

**Expected Issues:**
- **Binary incompatibility:** 91 occurrences (Windows Forms test utilities)
- **Behavioral changes:** 26 occurrences (may affect test outcomes)
  - Review behavioral change details in assessment
  - Update test assertions if API behavior changed

**4. Run Tests**

```bash
dotnet test Tests\Tests.csproj
```

**Validation:**
- All tests discovered and executed
- Test results comparable to net48 results (if baseline exists)
- Investigate any new failures:
  - API behavior changes (.NET 10.0 vs .NET Framework 4.8)
  - Test assumptions about Windows Forms behavior
  - Theme rendering differences

**5. Address Test Failures**

If tests fail:
1. Review the 26 behavioral change issues in assessment
2. Update test expectations to match .NET 10.0 behavior
3. Verify theme libraries work correctly with WinFormsUI net10.0
4. Check for timing issues (async/threading changes in .NET 10)

**6. Validation Checklist**

- [ ] SDK conversion completed
- [ ] Project builds with net10.0-windows target
- [ ] NUnit packages work with .NET 10.0
- [ ] All tests discovered by test runner
- [ ] Tests execute successfully
- [ ] No unexpected test failures
- [ ] Behavioral changes reviewed and addressed
- [ ] Commit: `git commit -m "Phase 3: Tests project migrated to net10.0"`

---

### Phase 3 Summary

**Batch Execution Approach:**

DockSample and Tests can be migrated in parallel (no inter-dependency):

1. **SDK Conversion:** Both projects simultaneously
2. **Build Fixes:** Address compilation errors independently
3. **Validation:** 
   - DockSample: Manual application testing
   - Tests: Automated test execution

**Phase 3 Completion Criteria:**
- DockSample builds and runs successfully on .NET 10.0
- All 6 themes load and work in DockSample
- Tests project builds and test suite passes
- Entire solution builds without errors
- Final commit: `git commit -m "Phase 3 complete: All applications migrated to net10.0"`
- Final tag: `git tag phase-3-complete`

**Migration Complete:**
- All 11 projects successfully targeting .NET 10.0 ✓
- Solution builds end-to-end ✓
- Application functional ✓
- Tests passing ✓

---

## Risk Management

### High-Risk Changes

| Project | Risk Level | Description | Mitigation |
|---------|-----------|-------------|------------|
| **WinFormsUI** | 🔴 **High** | 1,648 mandatory issues in foundation library affecting all 10 dependents. API surface changes propagate to entire solution. | Validate thoroughly before proceeding to Phase 2. Run all available tests. Build all multi-targets to ensure no regressions in net35/net40/netcoreapp3.1. |
| **DockSample** | 🔴 **High** | 2,582 mandatory issues (highest in solution). SDK conversion + retargeting + extensive API changes. Main demonstration application. | SDK conversion as separate step. Test manual execution after migration. Validate all dock panel features work correctly. |
| **ThemeVS2005Multithreading** | 🟡 **Medium** | 1,284 issues including multithreading patterns. Behavioral changes (26 occurrences) could affect thread safety. | Review behavioral change details in assessment. Test threading scenarios if available. |
| **ThemeVS2012** | 🟡 **Medium** | 1,445 issues. Referenced by both DockSample and Tests — failures impact multiple consumers. | Validate with both consumer projects after migration. |
| **ThemeVS2013** | 🟡 **Medium** | 1,419 issues. Referenced by both DockSample and Tests. | Same as ThemeVS2012. |
| **ThemeVS2015** | 🟡 **Medium** | 1,419 issues. Referenced by both DockSample and Tests. | Same as ThemeVS2012. |
| **ThemeVS2003** | 🟢 **Low** | 1,147 issues but well-isolated (only DockSample depends). | Standard validation. |
| **ThemeVS2005** | 🟢 **Low** | 923 issues, well-isolated. | Standard validation. |
| **Tests** | 🟡 **Medium** | SDK conversion + test project migration. NUnit tests must continue working. | Verify NUnit 3.9.0 compatibility with .NET 10.0. Run tests after migration. |
| **Tests2** | 🟢 **Low** | Only 2 issues. SDK conversion straightforward. | Minimal risk. |
| **Tests3** | 🟢 **Low** | Only 2 issues. SDK conversion straightforward. | Minimal risk. |

### Security Vulnerabilities

**None detected.** ✅

The assessment found no security vulnerabilities in current NuGet packages:
- NUnit 3.9.0 — Compatible with .NET 10.0, no known CVEs
- NUnit3TestAdapter 3.9.0 — Compatible with .NET 10.0, no known CVEs

**Post-Migration Security Considerations:**
- Verify `System.Drawing.Common` version 9.0.0 has no vulnerabilities before adding
- Remove Code Access Security attributes (4 occurrences) — CAS is obsolete and not enforced in .NET 10.0

### Contingency Plans

**Blocking Issues & Alternatives:**

1. **Issue:** WinFormsUI fails to build with net10.0 due to API changes
   - **Alternative:** Investigate conditional compilation (`#if NET10_0`) to isolate .NET 10-specific code
   - **Fallback:** Use `net9.0` or `net8.0` as interim target, then upgrade to net10.0 later

2. **Issue:** Multi-targeting breaks existing net35/net40 builds
   - **Alternative:** Conditionally exclude net10.0-specific NuGet packages from old targets
   - **Example:** 
     ```xml
     <PackageReference Include="System.Drawing.Common" Version="9.0.0" Condition="'$(TargetFramework)' == 'net10.0'" />
     ```

3. **Issue:** SDK conversion tool fails for classic projects
   - **Alternative:** Manual SDK conversion (copy-paste project content to new SDK-style project)
   - **Reference:** https://learn.microsoft.com/dotnet/core/porting/upgrade-assistant-overview

4. **Issue:** Behavioral changes cause runtime failures in DockSample
   - **Alternative:** Review .NET breaking changes documentation for specific API behavioral changes
   - **Action:** Code review + runtime testing to identify and fix behavioral differences
   - **Reference:** https://learn.microsoft.com/dotnet/core/compatibility/

5. **Issue:** GDI+ (`System.Drawing.Common`) produces cross-platform warnings
   - **Alternative:** Suppress warnings if Windows-only usage is intentional
   - **Code:** `<SuppressWarnings>CA1416</SuppressWarnings>` (platform compatibility warning)
   - **Long-term:** Consider migration to SkiaSharp or ImageSharp for cross-platform scenarios

6. **Issue:** Tests fail after migration
   - **Alternative:** Update NUnit to latest 4.x version if 3.9.0 has compatibility issues
   - **Alternative:** Investigate test-specific API changes in .NET 10.0
   - **Action:** Run tests individually to isolate failing tests

### Rollback Strategy

**Per-Phase Rollback:**

Each phase works on upgrade branch (`upgrade-to-NET10`), enabling clean rollback:

1. **Before starting each phase:**
   - Commit all changes from previous phase
   - Tag commit: `git tag phase-N-complete`

2. **If phase fails:**
   - Revert to last successful phase: `git reset --hard phase-N-complete`
   - Analyze failures in assessment.md
   - Adjust approach and retry

3. **Phase-specific rollback points:**
   - **Phase 1 rollback:** Revert WinFormsUI.csproj changes, other projects unaffected
   - **Phase 2 rollback:** Revert theme/test library changes, WinFormsUI changes preserved
   - **Phase 3 rollback:** Revert application changes, all libraries already working

**Full Solution Rollback:**
- Switch back to source branch: `git checkout master`
- All files return to pre-upgrade state
- Upgrade branch preserved for investigation: `upgrade-to-NET10`

**Partial Success Strategy:**
- If Phase 1 + Phase 2 succeed but Phase 3 fails:
  - Can ship libraries (WinFormsUI, themes) with net10.0 support
  - Applications (DockSample, Tests) remain on net48 temporarily
  - Continue work on Phase 3 in separate effort

### Breaking Change Documentation

**Expected Breaking Change Categories:**

1. **API Removals (Binary Incompatibility):**
   - **Impact:** 7,121 occurrences
   - **Example:** Windows Forms controls/properties removed between .NET Framework and .NET 10.0
   - **Fix:** Replace with modern equivalents or conditional compilation
   - **Reference:** https://learn.microsoft.com/dotnet/core/compatibility/windowsforms

2. **Method Signature Changes (Source Incompatibility):**
   - **Impact:** 5,502 occurrences (primarily GDI+)
   - **Example:** `System.Drawing` namespace APIs moved to `System.Drawing.Common` package
   - **Fix:** Add NuGet package + update usings if namespace changed
   - **Reference:** https://learn.microsoft.com/dotnet/core/compatibility/core-libraries/6.0/system-drawing-common-windows-only

3. **Behavioral Changes:**
   - **Impact:** 26 occurrences
   - **Example:** Default behavior changes in controls or event handling
   - **Fix:** Code review + testing to identify + adjust to new behavior
   - **Reference:** https://learn.microsoft.com/dotnet/core/compatibility/

4. **Code Access Security (CAS) Removal:**
   - **Impact:** 4 occurrences
   - **Example:** `[SecurityPermission(SecurityAction.Demand)]` attributes
   - **Fix:** Remove attributes (not enforced in .NET Core/5+)
   - **Reference:** https://learn.microsoft.com/dotnet/core/compatibility/core-libraries/5.0/code-access-security-apis-obsolete

5. **Configuration System Changes:**
   - **Impact:** 2 occurrences
   - **Example:** `ConfigurationManager.AppSettings["key"]`
   - **Fix:** Migrate to `Microsoft.Extensions.Configuration` pattern
   - **Reference:** https://learn.microsoft.com/aspnet/core/fundamentals/configuration/

**Per-Project Breaking Change Expectations:**

| Project | Binary Incompat | Source Incompat | Behavioral | CAS | Config |
|---------|----------------|-----------------|------------|-----|--------|
| WinFormsUI | 1,647 | 642 | 0 | 4 | 0 |
| ThemeVS2005Multithreading | 525 | 759 | 0 | 0 | 0 |
| ThemeVS2012 | 466 | 979 | 0 | 0 | 0 |
| ThemeVS2013 | 460 | 959 | 0 | 0 | 0 |
| ThemeVS2015 | 460 | 959 | 0 | 0 | 0 |
| ThemeVS2003 | 527 | 620 | 0 | 0 | 0 |
| ThemeVS2005 | 373 | 550 | 0 | 0 | 0 |
| DockSample | 2,580 | 34 | 0 | 0 | 2 |
| Tests | 91 | 26 | 26 | 0 | 0 |
| Tests2 | 0 | 0 | 0 | 0 | 0 |
| Tests3 | 0 | 0 | 0 | 0 | 0 |

**Action:** Review `assessment.md` file-by-file issue list for specific API names and locations.

---

## Testing & Validation Strategy

### Phase-by-Phase Testing Requirements

#### Phase 1 Testing (WinFormsUI Foundation)

**Build Validation:**
```bash
# Validate all target frameworks build
dotnet build WinFormsUI\WinFormsUI.csproj -f net35
dotnet build WinFormsUI\WinFormsUI.csproj -f net40
dotnet build WinFormsUI\WinFormsUI.csproj -f netcoreapp3.1
dotnet build WinFormsUI\WinFormsUI.csproj -f net10.0
```

**Success Criteria:**
- Zero compilation errors for all 4 target frameworks
- Warnings reviewed and documented (acceptable warnings listed)
- No breaking changes in net35/net40/netcoreapp3.1 targets

**Regression Testing:**
- If existing tests target WinFormsUI, run them against net10.0 build
- Compare test results: net48/netcoreapp3.1 baseline vs net10.0
- Document any behavioral differences

**Phase 1 Gate:**
- ✅ All builds succeed
- ✅ No unexpected warnings
- ✅ Tests pass (if available)
- **DO NOT proceed to Phase 2 until Phase 1 validates completely**

---

#### Phase 2 Testing (Theme & Test Libraries)

**Phase 2a (High-Complexity Themes):**

```bash
# Build each theme for all targets
dotnet build WinFormsUI\ThemeVS2005Multithreading.csproj
dotnet build WinFormsUI\ThemeVS2012.csproj
dotnet build WinFormsUI\ThemeVS2013.csproj
dotnet build WinFormsUI\ThemeVS2015.csproj
```

**Validation:**
- All 4 themes build successfully for all target frameworks
- No dependency resolution errors with WinFormsUI net10.0
- Warnings reviewed

**Phase 2b (Medium Themes + Simple Tests):**

```bash
# Build themes
dotnet build WinFormsUI\ThemeVS2003.csproj
dotnet build WinFormsUI\ThemeVS2005.csproj

# Build test libraries
dotnet build Tests2\Tests2.csproj
dotnet build Tests3\Tests3.csproj
```

**Validation:**
- All 4 projects build successfully
- SDK conversion successful for Tests2/Tests3
- References to WinFormsUI net10.0 work correctly

**Phase 2 Combined Validation:**

```bash
# Build entire solution
dotnet build WinFormsUI.Docking.sln
```

**Success Criteria:**
- Solution builds with 0 errors
- All 9 projects (WinFormsUI + 6 themes + Tests2 + Tests3) complete
- No inter-project dependency conflicts
- Multi-targeting works across all theme libraries

**Phase 2 Gate:**
- ✅ All library projects build
- ✅ Solution-level build succeeds
- ✅ No new dependency conflicts introduced
- **DO NOT proceed to Phase 3 until Phase 2 validates completely**

---

#### Phase 3 Testing (Applications)

**DockSample Application Testing:**

**Build Validation:**
```bash
dotnet build DockSample\DockSample.csproj
```

**Runtime Validation:**
```bash
dotnet run --project DockSample\DockSample.csproj
```

**Manual Test Plan:**

| Test Area | Test Steps | Expected Result |
|-----------|-----------|-----------------|
| **Application Launch** | Run DockSample.exe | Application opens without crashes |
| **Main Form** | Verify main window displays | UI renders correctly, no missing controls |
| **Theme Loading** | Switch between all 6 themes | Each theme loads without errors |
| **Theme Rendering** | Visual check each theme | Theme styling displays correctly |
| **Document Windows** | Create new document windows | Windows appear in dock area |
| **Dock Operations** | Drag/drop panels | Panels dock correctly (left, right, top, bottom) |
| **Float Windows** | Drag panel to float | Panel becomes floating window |
| **Tabbed Documents** | Create multiple documents | Tabs display and switch correctly |
| **Tool Windows** | Show/hide tool windows | Windows respond to menu commands |
| **Auto-Hide** | Use auto-hide feature | Panels slide in/out correctly |
| **Configuration** | Save/load layout | Application settings persist (if implemented) |
| **Resources** | Check icons, images | All resources load correctly |
| **Menus** | Test all menu items | Menu commands execute without errors |

**Smoke Test Scenarios:**
1. Launch app → Load VS2013 theme → Create 3 documents → Dock panels → Close app
2. Launch app → Load VS2015 theme → Float window → Dock window → Switch themes → Close app
3. Launch app → Cycle through all 6 themes → Verify each renders → Close app

**Expected Issues:**
- Most binary incompatibility auto-resolved by recompilation
- Potential visual differences in theme rendering (.NET 10.0 vs .NET Framework)
- Configuration system behavior differences (if using app.config)

**Tests Project Validation:**

**Build:**
```bash
dotnet build Tests\Tests.csproj
```

**Run Tests:**
```bash
dotnet test Tests\Tests.csproj --logger "console;verbosity=detailed"
```

**Test Results Analysis:**

| Metric | Baseline (net48) | Target (net10.0) | Variance |
|--------|------------------|------------------|----------|
| Total Tests | [TBD] | [TBD] | Should be equal |
| Passed | [TBD] | [TBD] | Review differences |
| Failed | [TBD] | [TBD] | Investigate failures |
| Skipped | [TBD] | [TBD] | Document reasons |

**Failure Investigation:**
- Review 26 behavioral change issues from assessment
- Check if test expectations need updating for .NET 10.0 behavior
- Verify theme libraries work correctly in test context
- Test timing/threading differences

**Phase 3 Success Criteria:**
- DockSample builds and runs without crashes
- All 6 themes load and render correctly
- Dock panel operations functional
- Tests project builds
- Test suite executes (investigate failures, update tests as needed)
- No regressions in core functionality

---

### Comprehensive Validation (Post-Phase 3)

**Full Solution Build:**
```bash
dotnet clean WinFormsUI.Docking.sln
dotnet restore WinFormsUI.Docking.sln
dotnet build WinFormsUI.Docking.sln --configuration Release
```

**Expected Output:**
- 11 projects build successfully
- 0 errors
- Warnings reviewed and acceptable
- All multi-target projects produce outputs for all frameworks

**Multi-Target Validation:**

For each SDK-style library project:
```bash
# Verify all targets built
ls WinFormsUI\bin\Release\net35\
ls WinFormsUI\bin\Release\net40\
ls WinFormsUI\bin\Release\netcoreapp3.1\
ls WinFormsUI\bin\Release\net10.0\
```

All 4 output directories should contain compiled assemblies.

**Dependency Graph Verification:**

```bash
# Use dotnet list to verify project references resolved correctly
dotnet list WinFormsUI.Docking.sln reference
```

Ensure all ProjectReference elements point to correct paths.

**Performance Spot Check:**

If baseline performance data available:
- Compare DockSample startup time (net48 vs net10.0)
- Compare theme rendering performance
- Document significant differences (±20%)

**No Security Vulnerabilities:**

```bash
dotnet list package --vulnerable
```

Expected: No vulnerable packages detected.

---

### Test Environment Requirements

**Development Machine:**
- .NET 10.0 SDK installed (verify: `dotnet --list-sdks`)
- Visual Studio 2022 (17.12 or later) OR Visual Studio Code with C# extension
- Windows OS (required for Windows Forms)

**Test Data:**
- Existing DockSample.exe from net48 build (for comparison)
- Baseline test results from Tests project on net48 (if available)

**Tools:**
- `dotnet CLI` for build/test automation
- `upgrade-assistant` for SDK conversion
- NUnit test runner (integrated in dotnet test)

---

### Test Execution Checklist

**Phase 1:**
- [ ] WinFormsUI builds for net35, net40, netcoreapp3.1, net10.0
- [ ] No new warnings introduced
- [ ] Existing tests pass (if available)

**Phase 2a:**
- [ ] All 4 high-complexity themes build successfully
- [ ] No dependency errors with WinFormsUI

**Phase 2b:**
- [ ] ThemeVS2003 and ThemeVS2005 build successfully
- [ ] Tests2 and Tests3 SDK conversion successful
- [ ] Tests2 and Tests3 build with net10.0-windows

**Phase 2 Overall:**
- [ ] Full solution build succeeds

**Phase 3:**
- [ ] DockSample builds with net10.0-windows
- [ ] DockSample runs and displays UI
- [ ] All 6 themes load in DockSample
- [ ] Dock panel operations work (manual testing)
- [ ] Tests project builds
- [ ] Tests execute and results reviewed

**Final Validation:**
- [ ] Clean + rebuild entire solution succeeds
- [ ] All multi-target outputs generated
- [ ] No vulnerable packages
- [ ] Manual smoke test of DockSample passes
- [ ] Test results acceptable (failures investigated and resolved)

---

## Complexity & Effort Assessment

### Per-Project Complexity Ratings

| Project | Complexity | Dependencies | Issues (M/P) | Risk | Rationale |
|---------|-----------|--------------|--------------|------|-----------|
| **WinFormsUI** | 🔴 **High** | 0 | 1,648 / 642 | High | Foundation library, 61 files, affects all 10 projects, multi-targeting complexity |
| **DockSample** | 🔴 **High** | 6 | 2,582 / 34 | High | Highest issue count, SDK conversion required, main demo app, 40 files |
| **ThemeVS2005Multithreading** | 🟡 **Medium** | 1 | 525 / 759 | Medium | 1,284 total issues, threading patterns, multi-targeting |
| **ThemeVS2012** | 🟡 **Medium** | 1 | 466 / 979 | Medium | 1,445 total issues, used by 2 projects, multi-targeting |
| **ThemeVS2013** | 🟡 **Medium** | 1 | 460 / 959 | Medium | 1,419 total issues, used by 2 projects, multi-targeting |
| **ThemeVS2015** | 🟡 **Medium** | 1 | 460 / 959 | Medium | 1,419 total issues, used by 2 projects, multi-targeting |
| **ThemeVS2003** | 🟡 **Medium** | 1 | 527 / 620 | Low | 1,147 total issues, isolated consumer, multi-targeting |
| **ThemeVS2005** | 🟢 **Low** | 1 | 373 / 550 | Low | 923 total issues, isolated consumer, multi-targeting |
| **Tests** | 🟡 **Medium** | 4 | 91 / 26 | Medium | SDK conversion, NUnit tests must work, behavioral changes (26) |
| **Tests2** | 🟢 **Low** | 1 | 2 / 0 | Low | Only 2 issues, simple SDK conversion |
| **Tests3** | 🟢 **Low** | 1 | 2 / 0 | Low | Only 2 issues, simple SDK conversion |

### Phase-Level Complexity Assessment

**Phase 1: Foundation (WinFormsUI)**
- **Complexity:** 🔴 **High**
- **Rationale:** 
  - Largest single-project effort (1,648 mandatory issues)
  - 61 code files requiring review
  - Multi-targeting: Must maintain net35/net40/netcoreapp3.1 while adding net10.0
  - Foundation for all other projects — errors propagate widely
  - Windows Forms + GDI+ API surface changes
- **Dependencies:** None (can start immediately)
- **Estimated Effort:** Highest individual project effort in plan
- **Validation Required:** 
  - Build all 4 target frameworks
  - Run any existing tests against net10.0 build
  - Verify no regressions in net35/net40/netcoreapp3.1 builds

**Phase 2a: High-Complexity Themes**
- **Complexity:** 🟡 **Medium** (per project)
- **Projects:** ThemeVS2005Multithreading, ThemeVS2012, ThemeVS2013, ThemeVS2015
- **Rationale:**
  - Each project: 460-525 mandatory issues, 759-979 potential issues
  - Similar patterns across all 4 (theme implementation, GDI+ usage)
  - Multi-targeting complexity
  - Can batch together due to no inter-dependencies
- **Dependencies:** WinFormsUI must be complete
- **Estimated Effort:** Medium per project, but batchable in parallel
- **Validation Required:**
  - Build all 4 target frameworks for each project
  - Verify themes load correctly in net10.0 context

**Phase 2b: Medium Themes + Simple Tests**
- **Complexity:** 🟢 **Low-to-Medium**
- **Projects:** ThemeVS2003, ThemeVS2005 (medium), Tests2, Tests3 (low)
- **Rationale:**
  - Themes: 373-527 mandatory issues (lower than Phase 2a)
  - Tests2/3: Only 2 issues each, simple SDK conversion
  - Can batch all 4 together
  - Least complex phase
- **Dependencies:** WinFormsUI must be complete
- **Estimated Effort:** Low per project, fast batch
- **Validation Required:**
  - Build all targets for themes
  - Verify Tests2/3 SDK conversion successful

**Phase 3: Applications**
- **Complexity:** 🔴 **High** (DockSample) + 🟡 **Medium** (Tests)
- **Projects:** DockSample, Tests
- **Rationale:**
  - **DockSample:** 2,582 mandatory issues (highest in solution), SDK conversion, 40 files
  - **Tests:** 91 mandatory + 26 behavioral changes, NUnit must work
  - Both require SDK conversion (new risk)
  - Both are top-level projects (most visible failures)
- **Dependencies:** All libraries (Phase 1 + Phase 2) must be complete
- **Estimated Effort:** High for DockSample, medium for Tests
- **Validation Required:**
  - Build succeeds
  - DockSample.exe runs and demonstrates functionality
  - Tests execute and pass with NUnit on .NET 10.0

### Resource Requirements

**Skills Required:**

| Skill | Phase 1 | Phase 2 | Phase 3 | Importance |
|-------|---------|---------|---------|------------|
| **Windows Forms expertise** | ✅✅✅ | ✅✅ | ✅✅✅ | Critical |
| **.NET API migration knowledge** | ✅✅✅ | ✅✅ | ✅✅ | Critical |
| **Multi-targeting experience** | ✅✅✅ | ✅✅ | ✅ | High |
| **SDK-style project format** | ✅ | ✅ | ✅✅✅ | High |
| **GDI+ / System.Drawing** | ✅✅✅ | ✅✅ | ✅ | High |
| **NUnit testing** | ✅ | — | ✅✅ | Medium |
| **Breaking change analysis** | ✅✅✅ | ✅✅ | ✅✅ | High |

**Parallelization Capacity:**

- **Phase 1:** 1 developer (single project, sequential)
- **Phase 2a:** Up to 4 developers (4 independent theme projects) OR 1 developer batching
- **Phase 2b:** Up to 4 developers (4 independent projects) OR 1 developer batching
- **Phase 3:** Up to 2 developers (DockSample + Tests independent) OR 1 developer sequential

**Recommended Team Size:** 1-2 experienced .NET developers

**Concurrent Work:**
- Not recommended to split WinFormsUI work (Phase 1) — too many inter-file dependencies
- Phase 2/3: Can parallelize if multiple developers available, but single developer batching is viable

### Effort Distribution Summary

**By Phase:**
- **Phase 1 (Foundation):** ~45% of total effort (WinFormsUI: 1,648 issues, 61 files, critical path)
- **Phase 2 (Themes + Tests):** ~35% of total effort (8 projects, similar patterns, batchable)
- **Phase 3 (Applications):** ~20% of total effort (SDK conversion + DockSample high issue count)

**By Activity Type:**
- **Project file updates:** ~5% (straightforward: add target framework, add NuGet)
- **SDK conversion:** ~10% (4 projects need conversion)
- **API compatibility fixes:** ~70% (bulk of work: 7,121 binary + 5,502 source incompatibilities)
- **Testing & validation:** ~10% (build verification, test execution, manual validation)
- **Troubleshooting:** ~5% (addressing unexpected issues, behavioral changes)

**Critical Path Activities:**
1. WinFormsUI API fixes (blocks everything)
2. Theme library API fixes (blocks applications)
3. DockSample SDK conversion + API fixes (highest visible impact)

### Effort Estimation Notes

**Complexity Factors Considered:**
- ✅ Issue count (mandatory vs potential)
- ✅ File count (more files = more review surface)
- ✅ Dependency count (more dependencies = more integration risk)
- ✅ Project format (classic vs SDK-style)
- ✅ Multi-targeting (maintaining old targets while adding new)
- ✅ Consumer count (how many projects depend on this one)
- ✅ Technology scope (Windows Forms, GDI+, CAS, Configuration)

**Relative Complexity Only:**
- Estimates are **relative** (High/Medium/Low), not absolute time
- Actual duration depends on:
  - Developer experience with .NET migration
  - Familiarity with WinForms codebase
  - Availability of original developers
  - Tooling (Visual Studio, dotnet CLI, upgrade-assistant)
  - Unforeseen breaking changes not detected in assessment

**Use Phase Ordering for Planning:**
- Focus on completing one phase at a time
- Don't start Phase 2 until Phase 1 validates successfully
- Each phase is a logical checkpoint for progress tracking

---

## Source Control Strategy

### Branching Strategy

**Main Branch:** `master`  
**Source Branch:** `master` (starting point)  
**Upgrade Branch:** `upgrade-to-NET10` (created and switched during Assessment stage ✓)

**Branch Workflow:**

```
master (production)
  │
  └─── upgrade-to-NET10 (feature branch for .NET 10.0 migration)
         │
         ├─── Phase 1 work (WinFormsUI)
         ├─── Phase 2a work (high-complexity themes)
         ├─── Phase 2b work (medium themes + test libraries)
         └─── Phase 3 work (applications)
```

**Branch Protection:**
- Work exclusively on `upgrade-to-NET10` branch
- Do NOT merge to `master` until entire migration validated
- Keep `master` branch stable for production/fallback

---

### Commit Strategy

**Commit Frequency:**

Commit at logical checkpoints, not per-file edits:

- After completing each project's migration
- After completing each phase
- Before attempting risky changes (e.g., SDK conversion)
- After fixing critical compilation errors

**Recommended Commits:**

1. **Phase 1:**
   - Before: `git commit -m "chore: save pending changes before .NET 10.0 upgrade"` ✅ (already done)
   - Before: `git commit -m "Phase 1 start: Begin WinFormsUI migration"`
   - After: `git commit -m "Phase 1 complete: WinFormsUI migrated to net10.0"`

2. **Phase 2a:**
   - Before: `git commit -m "Phase 2a start: Begin high-complexity theme migration"`
   - Per project (optional): `git commit -m "Migrate ThemeVS2012 to net10.0"`
   - After: `git commit -m "Phase 2a complete: High-complexity themes migrated"`

3. **Phase 2b:**
   - Before: `git commit -m "Phase 2b start: Begin medium themes and test libraries"`
   - After: `git commit -m "Phase 2b complete: All libraries migrated to net10.0"`
   - Combined: `git commit -m "Phase 2 complete: All theme and test libraries migrated"`

4. **Phase 3:**
   - Before: `git commit -m "Phase 3 start: Begin application migration"`
   - Per app: `git commit -m "SDK conversion: DockSample to SDK-style"`
   - Per app: `git commit -m "Migrate DockSample to net10.0-windows"`
   - After: `git commit -m "Phase 3 complete: All applications migrated to net10.0"`

5. **Final:**
   - After validation: `git commit -m "Migration complete: All projects targeting .NET 10.0"`

**Commit Message Format:**

Use conventional commit format:
- `feat:` - New .NET 10.0 feature/capability
- `fix:` - Bug fixes from API changes
- `chore:` - Project file updates, SDK conversions
- `refactor:` - Code changes for API compatibility
- `test:` - Test updates for .NET 10.0

**Examples:**
- `chore: add net10.0 target to WinFormsUI project`
- `fix: update obsolete System.Drawing APIs in VisualStudioToolStripRenderer`
- `refactor: replace ApplicationSettingsBase with IConfiguration in DockSample`
- `chore: convert DockSample to SDK-style project format`

---

### Tagging Strategy

**Create tags at phase completion for rollback points:**

```bash
# After Phase 1 validates
git tag -a phase-1-complete -m "Phase 1: WinFormsUI net10.0 migration complete"

# After Phase 2a validates
git tag -a phase-2a-complete -m "Phase 2a: High-complexity themes complete"

# After Phase 2b validates
git tag -a phase-2b-complete -m "Phase 2b: All libraries complete"

# After Phase 3 validates
git tag -a phase-3-complete -m "Phase 3: Applications migrated, upgrade complete"

# Final milestone
git tag -a v10.0-migration-complete -m "Solution fully migrated to .NET 10.0"
```

**Tag Usage:**
- Rollback to any phase: `git reset --hard phase-N-complete`
- Reference specific milestone: `git checkout phase-2a-complete`
- Compare changes: `git diff phase-1-complete phase-2a-complete`

---

### Review and Merge Process

**Before Merging to Master:**

1. **Full Validation Pass:**
   - [ ] All 11 projects build successfully
   - [ ] All tests pass (or failures documented and accepted)
   - [ ] DockSample application runs and functions correctly
   - [ ] All 6 themes load without errors
   - [ ] No vulnerable packages: `dotnet list package --vulnerable`
   - [ ] Multi-targeting validated for all SDK-style projects

2. **Code Review Checklist:**
   - [ ] No hardcoded version numbers left in comments
   - [ ] Conditional compilation used correctly (`#if NET10_0_OR_GREATER`)
   - [ ] Code Access Security attributes fully removed
   - [ ] Configuration system migrated or compatibility packages added
   - [ ] Breaking changes documented in commit messages
   - [ ] TODO/HACK comments reviewed and resolved

3. **Documentation Updates:**
   - [ ] Update README.md with .NET 10.0 support
   - [ ] Update build instructions (.NET 10.0 SDK requirement)
   - [ ] Document any breaking changes for library consumers
   - [ ] Update CHANGELOG with migration notes

4. **Pull Request (if using PR workflow):**
   ```
   Title: Migrate solution to .NET 10.0

   Description:
   - All 11 projects now target .NET 10.0 (or multi-target including net10.0)
   - SDK-style conversion completed for 4 classic projects
   - System.Drawing.Common package added for GDI+ support
   - Code Access Security attributes removed
   - [Additional notable changes]

   Validation:
   - Solution builds: ✅
   - Tests pass: ✅
   - DockSample runs: ✅
   - All themes functional: ✅
   ```

5. **Merge Command:**
   ```bash
   # Ensure upgrade branch is current
   git checkout upgrade-to-NET10
   git pull origin upgrade-to-NET10

   # Switch to master
   git checkout master
   git pull origin master

   # Merge (use --no-ff to preserve branch history)
   git merge --no-ff upgrade-to-NET10 -m "Merge: Complete .NET 10.0 migration"

   # Push to remote
   git push origin master

   # Push tags
   git push origin --tags
   ```

---

### Rollback Procedures

**Phase-Level Rollback:**

If Phase N fails validation:
```bash
# Revert to last successful phase
git reset --hard phase-(N-1)-complete

# OR: Revert specific commits
git revert <commit-hash>

# Continue work from stable point
```

**Full Rollback to Master:**

If migration needs to be abandoned:
```bash
# Switch back to master (pre-upgrade state)
git checkout master

# Preserve upgrade branch for future retry
# DO NOT DELETE upgrade-to-NET10 branch

# Optionally: stash changes from upgrade branch
git checkout upgrade-to-NET10
git stash save "WIP: .NET 10.0 upgrade paused"
git checkout master
```

**Partial Rollback (Selective):**

If only certain projects need rollback:
```bash
# Rollback specific project file
git checkout phase-N-complete -- WinFormsUI/ThemeVS2012.csproj

# Rollback specific source files
git checkout phase-N-complete -- DockSample/MainForm.cs
```

---

### Commit Hygiene Best Practices

**DO:**
- ✅ Commit working code (builds successfully)
- ✅ Use descriptive commit messages
- ✅ Commit related changes together
- ✅ Create tags at validation milestones
- ✅ Keep commits focused (one logical change per commit)

**DON'T:**
- ❌ Commit broken code (unless explicitly WIP marked)
- ❌ Commit large unrelated changes together
- ❌ Force push to shared branches
- ❌ Delete phase tags (needed for rollback)
- ❌ Commit sensitive data (API keys, credentials)

---

### Branch Lifecycle

**Timeline:**

1. **Assessment Stage:** Create and switch to `upgrade-to-NET10` ✅ (complete)
2. **Planning Stage:** Work on `upgrade-to-NET10` (current stage)
3. **Execution Stage:** All development on `upgrade-to-NET10`
4. **Validation Stage:** Final testing on `upgrade-to-NET10`
5. **Merge Stage:** Merge `upgrade-to-NET10` → `master`
6. **Post-Merge:** Keep `upgrade-to-NET10` for 1-2 weeks, then optionally delete

**Branch Retention:**
- Keep `upgrade-to-NET10` branch even after merge (at least temporarily)
- Useful for comparing changes, investigating issues
- Delete only after confirming master branch is stable post-merge

---

## Success Criteria

### Technical Criteria

**All Projects Migrated:**
- [ ] **WinFormsUI.csproj:** Multi-targets `net35;net40;netcoreapp3.1;net10.0`
- [ ] **ThemeVS2003.csproj:** Multi-targets `net35;net40;netcoreapp3.1;net10.0`
- [ ] **ThemeVS2005.csproj:** Multi-targets `net35;net40;netcoreapp3.1;net10.0`
- [ ] **ThemeVS2005Multithreading.csproj:** Multi-targets `net35;net40;netcoreapp3.1;net10.0`
- [ ] **ThemeVS2012.csproj:** Multi-targets `net35;net40;netcoreapp3.1;net10.0`
- [ ] **ThemeVS2013.csproj:** Multi-targets `net35;net40;netcoreapp3.1;net10.0`
- [ ] **ThemeVS2015.csproj:** Multi-targets `net35;net40;netcoreapp3.1;net10.0`
- [ ] **Tests2.csproj:** Targets `net10.0-windows` (SDK-style)
- [ ] **Tests3.csproj:** Targets `net10.0-windows` (SDK-style)
- [ ] **DockSample.csproj:** Targets `net10.0-windows` (SDK-style)
- [ ] **Tests.csproj:** Targets `net10.0-windows` (SDK-style)

**All Builds Succeed:**
- [ ] Solution-level build: `dotnet build WinFormsUI.Docking.sln` → 0 errors
- [ ] Release configuration build succeeds
- [ ] All multi-target projects produce outputs for all frameworks
- [ ] No dependency resolution conflicts

**All Tests Pass:**
- [ ] Tests.csproj test suite executes: `dotnet test Tests\Tests.csproj`
- [ ] Test results reviewed (failures investigated, accepted, or fixed)
- [ ] No regressions compared to net48 baseline (if available)

**No Package Vulnerabilities:**
- [ ] Vulnerability check passes: `dotnet list package --vulnerable` → No vulnerabilities
- [ ] All NuGet packages updated to compatible versions
- [ ] `System.Drawing.Common` version 9.0.0 added to all applicable projects

**Breaking Changes Resolved:**
- [ ] All 7,121 binary incompatibilities addressed (via recompilation or code fixes)
- [ ] All 5,502 source incompatibilities fixed
- [ ] All 26 behavioral changes reviewed and tested
- [ ] 4 Code Access Security attributes removed
- [ ] 2 legacy configuration usages migrated or compatibility package added

**SDK Conversion Complete:**
- [ ] DockSample.csproj converted to SDK-style
- [ ] Tests.csproj converted to SDK-style
- [ ] Tests2.csproj converted to SDK-style
- [ ] Tests3.csproj converted to SDK-style
- [ ] All 4 conversions validated (build succeeds, references work)

---

### Quality Criteria

**Code Quality Maintained:**
- [ ] No new compiler warnings introduced (or warnings documented/suppressed)
- [ ] Code follows existing project conventions
- [ ] No hardcoded workarounds (or documented as technical debt)
- [ ] Conditional compilation used appropriately for multi-targeting
- [ ] No `#pragma warning disable` without justification comments

**Test Coverage Maintained:**
- [ ] Existing test count unchanged (or increases documented)
- [ ] Test pass rate comparable to baseline
- [ ] New tests added for .NET 10.0-specific behavior (if needed)

**Documentation Updated:**
- [ ] README.md updated with .NET 10.0 support
- [ ] Build requirements document .NET 10.0 SDK
- [ ] CHANGELOG entry for migration
- [ ] Breaking changes documented for library consumers
- [ ] Multi-targeting approach documented (for contributors)

---

### Process Criteria

**Incremental Migration Strategy Followed:**
- [ ] Phase 1 (WinFormsUI) completed and validated before Phase 2
- [ ] Phase 2 (themes + test libraries) completed and validated before Phase 3
- [ ] Phase 3 (applications) completed last
- [ ] Each phase validated independently (builds + tests)

**Source Control Strategy Followed:**
- [ ] All work on `upgrade-to-NET10` branch
- [ ] Commits at logical checkpoints (per project/per phase)
- [ ] Phase completion tags created (`phase-1-complete`, `phase-2a-complete`, etc.)
- [ ] Conventional commit messages used
- [ ] No commits directly to `master` branch

**All-At-Once Strategy Principles Applied:**

*Note: This plan uses **Incremental Migration**, not All-At-Once. The All-At-Once principles below would apply if the strategy were different.*

**N/A** - This migration uses Incremental Migration with phase-based batching. All-At-Once would require:
- All 11 projects updated simultaneously
- Single atomic commit
- Full solution validation in one pass

**Actual Strategy Applied: Incremental with Batching**
- [ ] Within each phase, projects batched together (parallel conceptually)
- [ ] Across phases, strict sequential validation (Phase N validates before Phase N+1 starts)
- [ ] Dependency order respected (foundation → dependents → applications)

---

### Functional Criteria

**DockSample Application Functional:**
- [ ] Application launches without crashes
- [ ] Main form renders correctly
- [ ] All 6 themes loadable (VS2003, VS2005, VS2005Multithreading, VS2012, VS2013, VS2015)
- [ ] Theme switching works without errors
- [ ] Dock panel operations functional:
  - [ ] Drag/drop panels to dock
  - [ ] Float windows
  - [ ] Tabbed documents
  - [ ] Auto-hide panels
  - [ ] Tool windows show/hide
- [ ] Document windows create/close correctly
- [ ] No runtime exceptions in core workflows
- [ ] Application settings load/save (if configured)
- [ ] Resources display correctly (icons, images)

**Theme Libraries Functional:**
- [ ] ThemeVS2003 renders classic Visual Studio 2003 appearance
- [ ] ThemeVS2005 renders Visual Studio 2005 appearance
- [ ] ThemeVS2005Multithreading renders correctly (no threading issues)
- [ ] ThemeVS2012 renders Visual Studio 2012 appearance (light/dark)
- [ ] ThemeVS2013 renders Visual Studio 2013 appearance
- [ ] ThemeVS2015 renders Visual Studio 2015 appearance
- [ ] No visual regressions compared to net48 builds (if baseline available)

**Multi-Targeting Works:**
- [ ] All SDK-style libraries build for `net35`, `net40`, `netcoreapp3.1`, `net10.0`
- [ ] No regressions in older target frameworks (net35/net40/netcoreapp3.1)
- [ ] Consumers can choose appropriate target framework
- [ ] Conditional compilation works correctly where used

---

### Validation Evidence

**Build Logs:**
- [ ] Capture successful build output: `dotnet build > build-success.log`
- [ ] Document any warnings and justification
- [ ] Verify 0 errors in build log

**Test Results:**
- [ ] Capture test execution results: `dotnet test --logger "trx" --results-directory TestResults`
- [ ] Compare to baseline (if available)
- [ ] Document any test changes or skips

**Application Screenshots:**
- [ ] Screenshot of DockSample running on .NET 10.0
- [ ] Screenshot of each theme (6 total)
- [ ] Evidence of functional dock panel operations

**Package Vulnerability Report:**
- [ ] Capture output: `dotnet list package --vulnerable > package-report.txt`
- [ ] Confirm: "No vulnerable packages found"

---

### Definition of Done

**The .NET 10.0 migration is complete when:**

1. ✅ **All 11 projects** target .NET 10.0 (multi-target or single-target as appropriate)
2. ✅ **Solution builds** end-to-end with 0 errors in Release configuration
3. ✅ **All tests pass** (or failures documented and accepted)
4. ✅ **DockSample runs** and demonstrates functional dock panel features
5. ✅ **All 6 themes load** and render correctly in DockSample
6. ✅ **No security vulnerabilities** in NuGet packages
7. ✅ **Breaking changes resolved:** All 12,664 issues addressed
8. ✅ **SDK conversions complete:** All 4 classic projects converted
9. ✅ **Documentation updated:** README, CHANGELOG, build requirements
10. ✅ **Source control clean:** All changes committed, tagged, on `upgrade-to-NET10` branch
11. ✅ **Validation evidence captured:** Build logs, test results, screenshots
12. ✅ **Ready for merge:** `upgrade-to-NET10` → `master`

**Post-Merge Success:**
- Merge to `master` branch successful
- CI/CD pipeline passes (if configured)
- No production issues reported in first week
- Rollback plan tested and validated

---

### Acceptance Sign-Off

**Technical Lead Approval:**
- [ ] Code changes reviewed
- [ ] Build artifacts validated
- [ ] Test coverage acceptable
- [ ] Documentation complete

**Product Owner Approval:**
- [ ] Application functionality validated
- [ ] User-facing features work correctly
- [ ] Performance acceptable
- [ ] Ready for production deployment

**Final Approval:**
- [ ] Merge `upgrade-to-NET10` → `master` approved
- [ ] Release notes prepared
- [ ] Deployment plan ready (if applicable)

---

## Migration Complete ✅

When all success criteria above are met, the .NET 10.0 upgrade is **COMPLETE** and ready for production use.
