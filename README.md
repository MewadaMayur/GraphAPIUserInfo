# GraphAPIUserInfo

A simple **ASP.NET Core Web API** project that retrieves user information from Microsoft Entra ID using the **Microsoft Graph API** with app-only authentication (Client Credentials Flow).

## Overview

This project demonstrates how to:

- Register an application in Microsoft Entra ID.
- Configure Microsoft Graph API permissions.
- Generate an app-only access token.
- Authenticate using Client ID, Tenant ID, and Client Secret.
- Call Microsoft Graph API from C#.
- Retrieve users from an Entra ID tenant.
- Return user information as JSON through an ASP.NET Core API.

## Technology Stack

- C#
- ASP.NET Core Web API
- Microsoft Entra ID
- Microsoft Graph API
- OAuth 2.0 Client Credentials Flow
- HttpClient
- .NET User Secrets

## Architecture

```text
Client / Browser / Postman
          |
          v
ASP.NET Core Web API
          |
          v
GraphController
          |
          | 1. Request Access Token
          v
Microsoft Entra ID
          |
          | 2. Return Access Token
          v
GraphController
          |
          | 3. Call Microsoft Graph API
          v
Microsoft Graph API
          |
          | 4. Return User Information
          v
ASP.NET Core API Response
```

## Prerequisites

Before running this project, ensure you have:

- .NET SDK installed.
- Visual Studio or VS Code.
- An active Microsoft Azure subscription or Entra ID tenant.
- Access to Microsoft Entra admin center.
- A registered application in Microsoft Entra ID.

## Step 1: Create App Registration

1. Open [Microsoft Entra Admin Center](https://entra.microsoft.com/).
2. Navigate to **Identity → Applications → App registrations**.
3. Click **New registration**.
4. Enter an application name, for example:

   `GraphAPIUserInfo`

5. Click **Register**.

After registration, collect:

- Directory (Tenant) ID
- Application (Client) ID

## Step 2: Create Client Secret

1. Open your registered application.
2. Navigate to **Certificates & secrets**.
3. Select **Client secrets**.
4. Click **New client secret**.
5. Add a description and select an expiry.
6. Click **Add**.
7. Copy the secret value immediately.

**Important:** Never commit client secrets or access tokens to GitHub.

## Step 3: Configure Microsoft Graph Permissions

1. Open your application registration.
2. Navigate to **API permissions**.
3. Click **Add a permission**.
4. Select **Microsoft Graph**.
5. Choose **Application permissions**.
6. Search for and select:

   `User.Read.All`

7. Click **Add permissions**.
8. Select **Grant admin consent**.

This permission allows the application to read user profiles across the tenant, subject to Microsoft Graph's permission and consent rules.

## Step 4: Configure Application Settings

Add the following configuration to `appsettings.json`:

```json
{
  "AzureAd": {
    "TenantId": "YOUR_TENANT_ID",
    "ClientId": "YOUR_CLIENT_ID"
  }
}
```

Store the client secret separately using .NET User Secrets.

Run these commands from the folder containing your `.csproj` file:

```bash
dotnet user-secrets init
```

```bash
dotnet user-secrets set "AzureAd:ClientSecret" "YOUR_NEW_CLIENT_SECRET"
```

To verify the configuration:

```bash
dotnet user-secrets list
```

Do not share the output publicly because it displays the secret.

## Step 5: API Endpoint

The project exposes the following endpoint:

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/graph/users` | Retrieves users from Microsoft Entra ID |

Example:

```http
GET https://localhost:7000/api/graph/users
```

Use the actual HTTPS port configured by your application.

## Step 6: Authentication Flow

The application uses the OAuth 2.0 Client Credentials Flow.

**Token endpoint:**

```http
POST https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token
```

Request parameters:

```text
client_id
client_secret
scope=https://graph.microsoft.com/.default
grant_type=client_credentials
```

After obtaining the access token, the application calls:

```http
GET https://graph.microsoft.com/v1.0/users
Authorization: Bearer {access_token}
```

## Sample API Response

```json
{
  "@odata.context": "https://graph.microsoft.com/v1.0/$metadata#users",
  "value": [
    {
      "displayName": "User Name",
      "userPrincipalName": "user@example.com",
      "id": "user-object-id"
    }
  ]
}
```

The actual response contains additional properties depending on the Graph API response and requested fields.

## Step 7: Run the Project

Open the terminal in the project directory.

Restore dependencies:

```bash
dotnet restore
```

Build the project:

```bash
dotnet build
```

Run the application:

```bash
dotnet run
```

Test the endpoint using Postman or a browser:

```text
https://localhost:{port}/api/graph/users
```

## Important Security Notes

- Never upload client secrets to GitHub.
- Never commit access tokens to source control.
- Use .NET User Secrets for local development.
- Use Azure Key Vault or another secure secret store in production.
- Prefer certificates or managed identities over client secrets for production workloads.
- Apply the principle of least privilege when assigning Microsoft Graph permissions.
- Protect the API endpoint with appropriate authorization before exposing it to other users.

## Learning Outcomes

After completing this project, you will understand:

1. Microsoft Entra ID App Registration.
2. Tenant ID and Client ID.
3. Client Secret authentication.
4. OAuth 2.0 Client Credentials Flow.
5. Microsoft Graph API.
6. Application permissions and admin consent.
7. Access token generation.
8. Calling external REST APIs from C#.
9. Secure configuration using .NET User Secrets.

## References

- [Microsoft Entra ID Documentation](https://learn.microsoft.com/en-us/entra/identity/)
- [Microsoft Graph API Documentation](https://learn.microsoft.com/en-us/graph/overview)
- [Microsoft Graph - List Users](https://learn.microsoft.com/en-us/graph/api/user-list?view=graph-rest-1.0)
- [OAuth 2.0 Client Credentials Flow](https://learn.microsoft.com/en-us/entra/identity-platform/v2-oauth2-client-creds-grant-flow)
- [.NET User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets)

---

**Project:** GraphAPIUserInfo  
**Category:** Azure Security / Microsoft Graph API / ASP.NET Core  
**Purpose:** Learning and practical implementation
