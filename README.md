[![downloads][downloads-shield]][downloads-url] [![Contributors][contributors-shield]][contributors-url] [![Forks][forks-shield]][forks-url] [![Stargazers][stars-shield]][stars-url] [![Issues][issues-shield]][issues-url] [![License][license-shield]][license-url] [![LinkedIn][linkedin-shield]][linkedin-url]

# ![Logo][Logo] Portfolie
A high‑performance portfolio application built with C# and WebAssembly, running entirely in the browser.

The project is primarily aimed at software engineers who want to showcase their work to potential employers, but it is flexible enough to be adapted for other professions as well (designers, data specialists, etc.).
The application supports certificate-based login out of the box and is designed so that additional authentication methods can be added in the future if requested.

## ✨ Features

> See Milestones section for details what is planning.

## 🚀 Getting Started

Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- A valid client certificate (for login)
- (Optional) A server or reverse proxy configured to request/require client certificates

### Clone the Repository

```bash
git clone https://github.com/TirsvadWeb/DotNet.Portfolio.git
cd DotNet.Portfolio
```

### Configuration

1. App Settings
Update your configuration file (e.g., appsettings.json, appsettings.Development.json, or similar) with:

	- Certificate validation settings
	- Any environment-specific URLs (API base URL, callback URLs, etc.)

1. Certificates

	- Configure your development environment (e.g., Kestrel, IIS, Nginx, Apache, or a dev certificate manager) to:
		
		- Accept / require client certificates
		- Forward certificate information to the application if needed

	- Import your client certificate into your browser or OS certificate store.

Exact steps will depend on your hosting and environment. Add more specific instructions here based on how your project is set up.

### Build & Run
```bash
dotnet restore
dotnet build
dotnet run
```

Then open your browser and navigate to the URL shown in the console (typically https://localhost:5001 or similar).

## 🧑‍💼 Using the Portfolio
Once running, you can:
- Log in with your certificate
- Customize your profile: name, title, summary
- Add projects: title, description, tech stack, links, screenshots
- Optionally upload or link your CV (PDF or other supported format)
- Share the URL with potential employers as your personal portfolio

## 🗺️ Roadmap / Future Ideas
- [ ] v0.1 Basic profile with certificate login
	- [ ] User profile
	- [ ] Profile details (name, title, summary)
	- [ ] Certificate login
- [ ] v0.2 Portfolio basics
	- [ ] Portfolio project management (CRUD operations)
	- [ ] Tags / tech stack
	- [ ] Project details (description, links, screenshots)
- [ ] v0.3 Localization
	- [ ] Localization / multi-language support
	- [ ] English
	- [ ] Danish
- [ ] v0.4 Blog and articles
	- [ ] Blog post management (CRUD operations)
	- [ ] Blog post details (title, content, author, date)
	- [ ] Blog post comments
- [ ] v0.5 Add multiple login options and Role-based access control
	- [ ] Add support for OAuth2 / OpenID Connect providers
	- [ ] Add roles (Admin, Editor and Guest)
- [ ] v0.6 Add export and import functionality
	- [ ] Export portfolio data as JSON
	- [ ] Import portfolio data from JSON
- [ ] v1.0 Stable release
	- [ ] Polish UI/UX
	- [ ] Comprehensive testing (unit, integration, e2e)
	- [ ] Documentation and user guides
	- [ ] Custom themes (light/dark)

Feel free to open an issue or submit a feature request.
For detailed tasks and progress, see the [GitHub Project Tasks][githubProjectTasks-url].

<!-- LINK REFERENCES -->
[contributors-shield]: https://img.shields.io/github/contributors/TirsvadWeb/DotNet.Portfolio?style=for-the-badge
[contributors-url]: https://github.com/TirsvadWeb/DotNet.Portfolio/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/TirsvadWeb/DotNet.Portfolio?style=for-the-badge
[forks-url]: https://github.com/TirsvadWeb/DotNet.Portfolio/network/members
[stars-shield]: https://img.shields.io/github/stars/TirsvadWeb/DotNet.Portfolio?style=for-the-badge
[stars-url]: https://github.com/TirsvadWeb/DotNet.Portfolio/stargazers
[issues-shield]: https://img.shields.io/github/issues/TirsvadWeb/DotNet.Portfolio?style=for-the-badge
[issues-url]: https://github.com/TirsvadWeb/DotNet.Portfolio/issues
[license-shield]: https://img.shields.io/github/license/TirsvadWeb/DotNet.Portfolio?style=for-the-badge
[license-url]: https://github.com/TirsvadWeb/DotNet.Portfolio/blob/master/LICENSE.txt
[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=for-the-badge&logo=linkedin&colorB=555
[linkedin-url]: https://www.linkedin.com/in/jens-tirsvad-nielsen-13b795b9/
[logo]: https://raw.githubusercontent.com/TirsvadWeb/DotNet.Portfolio/main/images/logo/32x32/logo.png

[downloads-shield]: https://img.shields.io/github/downloads/TirsvadWeb/DotNet.Portfolio/total?style=for-the-badge
[downloads-url]: https://github.com/TirsvadWeb/DotNet.Portfolio/releases

<!-- Github Links -->
[githubIssue-url]: https://github.com/TirsvadWeb/DotNet.Portfolio/issues/
[githubProjectTasks-url]: https://github.com/orgs/TirsvadWeb/projects/7