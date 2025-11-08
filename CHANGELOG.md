# Changelog

All notable changes to the PrivacyConfirmed project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0.0] - 2024-XX-XX

### Added
- Initial release of PrivacyConfirmed platform
- Resource Center with file upload, download, and delete functionality
- Contact Us form with database integration
- PostgreSQL database support
- RESTful API for resource management
- Responsive Bootstrap 5 UI
- Admin and Developer Console features
- India-first data residency compliance
- Version display in footer
- VersionHelper utility class
- Comprehensive API documentation via Swagger

### Features
- Enterprise-grade identity and access management
- File upload with validation (.zip, .doc, .docx, .xlsx, .xls)
- File size limit: 10 MB
- Soft delete functionality for files
- File statistics dashboard
- Modern gradient-based UI design
- Responsive mobile-friendly layout
- Footer overlap prevention
- Auto-dismissing alerts

### Technical
- .NET 8.0 framework
- Entity Framework Core with PostgreSQL
- Layered architecture (DAL, BAL, Model, API, Web)
- Bootstrap 5 with Bootstrap Icons
- jQuery for client-side interactions
- ASP.NET Core Razor Pages
- RESTful API with Swagger documentation

### Security
- Request size limits
- File type validation
- Antiforgery token protection
- Secure file handling

---

## [Unreleased]

### Planned
- User authentication and authorization
- Role-based access control
- File preview functionality
- Advanced search and filtering
- Bulk operations
- Activity logging and audit trails
- Email notifications
- Advanced analytics dashboard

---

## Version Format

We use a four-part version number: `Major.Minor.Build.Revision`

- **Major**: Breaking changes or significant new features
- **Minor**: New features that are backward compatible  
- **Build**: Bug fixes and minor improvements
- **Revision**: Hotfixes and emergency patches

---

## How to Update This File

When releasing a new version:

1. Move items from `[Unreleased]` to a new version section
2. Add the version number and date: `## [1.1.0.0] - 2024-XX-XX`
3. Categorize changes under:
   - **Added** - New features
   - **Changed** - Changes to existing functionality
   - **Deprecated** - Soon-to-be removed features
   - **Removed** - Removed features
   - **Fixed** - Bug fixes
   - **Security** - Security improvements

---

## Links

- [GitHub Repository](https://github.com/rahulj230125/PrivacyConfirmed)
- [Version Management Guide](VERSION.md)
