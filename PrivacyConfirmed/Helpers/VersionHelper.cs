using System.Reflection;

namespace PrivacyConfirmed.Helpers
{
    /// <summary>
    /// Helper class to retrieve application version information
    /// </summary>
    public static class VersionHelper
    {
        /// <summary>
        /// Gets the full version string (Major.Minor.Build.Revision)
        /// </summary>
        public static string GetVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0.0";
        }

        /// <summary>
        /// Gets the short version string (Major.Minor.Build)
        /// </summary>
        public static string GetShortVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            if (version != null)
            {
                return $"{version.Major}.{version.Minor}.{version.Build}";
            }
            return "1.0.0";
        }

        /// <summary>
        /// Gets the version with 'v' prefix (e.g., v1.0.0.0)
        /// </summary>
        public static string GetVersionWithPrefix()
        {
            return $"v{GetVersion()}";
        }

        /// <summary>
        /// Gets the informational version (same as Version in this case)
        /// </summary>
        public static string GetInformationalVersion()
        {
            return Assembly.GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion ?? GetVersion();
        }

        /// <summary>
        /// Gets the product name
        /// </summary>
        public static string GetProductName()
        {
            return Assembly.GetExecutingAssembly()
                .GetCustomAttribute<AssemblyProductAttribute>()?
                .Product ?? "PrivacyConfirmed";
        }

        /// <summary>
        /// Gets the copyright information
        /// </summary>
        public static string GetCopyright()
        {
            return Assembly.GetExecutingAssembly()
                .GetCustomAttribute<AssemblyCopyrightAttribute>()?
                .Copyright ?? $"Copyright © {DateTime.Now.Year} PrivacyConfirmed";
        }
    }
}
