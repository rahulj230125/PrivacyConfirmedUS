using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PrivacyConfirmed.Models;

public class CoreFeaturesModel : PageModel
{
    public record FeatureCard(string Title, string Description, string IconPath, string IconAlt);

    public IReadOnlyList<FeatureCard> FeatureCards { get; private set; } = Array.Empty<FeatureCard>();

    public void OnGet()
    {
        FeatureCards = new List<FeatureCard>
        {
            new(
                "Authentication & Single Sign-On",
                "Secure user access across applications using OAuth 2.0 and OpenID Connect. Authenticate once and access everything you’re authorized for.",
                "/images/icons/auth-sso.svg",
                "Authentication and SSO icon"
            ),
            new(
                "Adaptive Multi-Factor Authentication",
                "Apply risk-based MFA using context like device, location, and behavior. Increase security only when risk rises—without adding friction.",
                "/images/icons/adaptive-mfa.svg",
                "Adaptive MFA icon"
            ),
            new(
                "Authorization & Token Management",
                "Issue and manage access/ID tokens with scopes, lifetimes, and rotation policies for secure, scalable authorization across APIs and apps.",
                "/images/icons/token-mgmt.svg",
                "Token management icon"
            ),
            new(
                "Role-Based Access Control (RBAC)",
                "Define roles and permissions once and enforce them consistently. Enable least-privilege access with centralized policy-driven control.",
                "/images/icons/rbac.svg",
                "RBAC icon"
            ),
            new(
                "External & Social Identity Providers",
                "Support external identity providers while keeping policies centralized. Reduce onboarding friction without losing security governance.",
                "/images/icons/external-idp.svg",
                "External identity providers icon"
            ),
            new(
                "Developer-First Integration",
                "Integrate identity flows in hours using clean APIs, SDKs, clear documentation, and ready-to-use integrations across any tech stack.",
                "/images/icons/dev-first.svg",
                "Developer tools icon"
            ),
            new(
                "Centralized Admin Console",
                "Manage users, applications, roles, and policies from one control plane. Apply updates instantly across connected systems.",
                "/images/icons/admin-console.svg",
                "Admin console icon"
            ),
            new(
                "Observability & Audit Logs",
                "Track authentication and access decisions in real time. Keep detailed audit trails and integrate with logging/SIEM tools.",
                "/images/icons/observability.svg",
                "Observability icon"
            ),
            new(
                "Built for Scale & Compliance",
                "Designed for cloud-native and multi-tenant environments with evolving compliance needs. Scale securely from startup to enterprise.",
                "/images/icons/scale-compliance.svg",
                "Scale and compliance icon"
            ),
        };
    }
}
