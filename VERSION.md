# Version Management Guide

## Current Version: 1.0.0.0

This document explains how to manage and increment versions in the PrivacyConfirmed project.

## Version Format

The project uses a **four-part version number** format:

```
Major.Minor.Build.Revision
1.0.0.0
```

### Version Components

1. **Major** (1st digit): Breaking changes, major new features, or significant architectural changes
2. **Minor** (2nd digit): New features that are backward compatible
3. **Build** (3rd digit): Bug fixes, minor improvements, and patches
4. **Revision** (4th digit): Hotfixes, emergency patches, or very minor changes

## How to Update Version

### 1. Update Project Files

Update the version in **both** project files:

#### PrivacyConfirmed/PrivacyConfirmed.csproj
```xml
<PropertyGroup>
  <Version>1.0.0.0</Version>
  <AssemblyVersion>1.0.0.0</AssemblyVersion>
  <FileVersion>1.0.0.0</FileVersion>
  <InformationalVersion>1.0.0.0</InformationalVersion>
</PropertyGroup>
```

#### PrivacyConfirmedAPI/PrivacyConfirmedAPI.csproj
```xml
<PropertyGroup>
  <Version>1.0.0.0</Version>
  <AssemblyVersion>1.0.0.0</AssemblyVersion>
  <FileVersion>1.0.0.0</FileVersion>
  <InformationalVersion>1.0.0.0</InformationalVersion>
</PropertyGroup>
```

### 2. Rebuild the Project

After updating the version numbers:
```bash
dotnet build
```

### 3. Version Display

The version is automatically displayed in:
- **Website Footer**: Visible to all users at the bottom of every page
- **API Swagger UI**: Shown in the API documentation
- **Code**: Accessible via `VersionHelper` class

## Version Increment Examples

### Scenario 1: Bug Fix (Patch)
- Current: `1.0.0.0`
- New: `1.0.1.0` (increment Build)
- When: Fixing bugs, minor improvements

### Scenario 2: New Feature (Minor)
- Current: `1.0.1.0`
- New: `1.1.0.0` (increment Minor, reset Build)
- When: Adding new features that don't break existing functionality

### Scenario 3: Breaking Change (Major)
- Current: `1.1.0.0`
- New: `2.0.0.0` (increment Major, reset Minor and Build)
- When: Major refactoring, breaking API changes, new architecture

### Scenario 4: Hotfix (Revision)
- Current: `1.1.0.0`
- New: `1.1.0.1` (increment Revision)
- When: Emergency patches, critical security fixes

## Using VersionHelper Class

The `VersionHelper` class provides convenient methods to access version information:

```csharp
using PrivacyConfirmed.Helpers;

// Get full version (1.0.0.0)
string version = VersionHelper.GetVersion();

// Get short version (1.0.0)
string shortVersion = VersionHelper.GetShortVersion();

// Get version with prefix (v1.0.0.0)
string versionWithPrefix = VersionHelper.GetVersionWithPrefix();

// Get product name
string product = VersionHelper.GetProductName();

// Get copyright
string copyright = VersionHelper.GetCopyright();
```

## Best Practices

1. **Consistency**: Always update both Web and API project versions together
2. **Documentation**: Update this file when incrementing major or minor versions
3. **Git Tags**: Tag releases with version numbers: `git tag v1.0.0.0`
4. **Changelog**: Maintain a CHANGELOG.md file documenting changes per version
5. **Release Notes**: Create release notes for each version
6. **Semantic Versioning**: Follow semantic versioning principles

## Version History

| Version | Date | Description |
|---------|------|-------------|
| 1.0.0.0 | 2024-XX-XX | Initial release with core features |

## Automated Versioning (Optional Future Enhancement)

Consider implementing automatic version increments using:
- **Git-based versioning**: Use GitVersion or similar tools
- **CI/CD pipeline**: Auto-increment build numbers
- **MSBuild tasks**: Automatic version generation during build

## Notes

- The version is stored in the assembly metadata
- After changing version, do a clean rebuild
- The version is visible to users in the footer
- API version is shown in Swagger documentation
- Version information is part of the assembly and doesn't require configuration changes at runtime
