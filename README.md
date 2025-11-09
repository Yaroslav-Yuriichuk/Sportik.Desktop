## Overview

Sportik Desktop is a desktop application to create reminders for exercises and track the progress.

## Release installation

To install the application, download the latest certificate, utility executable and installer from the [Releases](https://github.com/Yaroslav-Yuriichuk/Sportik.Desktop/releases) page.

Install certificate:
- Right-click on the certificate file and select "Install Certificate".
- Select "Local Machine" and click "Next".
- Select "Place all certificates in the following store" and click "Browse".
- Select "Trusted Root Certification Authorities" and click "OK".
- Click "Next" and "Finish".
- Click "Yes" to confirm the installation.
- Click "OK" to close the window.

Install the application:
- Run the utility executable as Administrator.
- Double-click on the installer file.
- Select "Launch when ready" (optional)
- Click "Install" to install the application.

## Build

To build the application, you need to have the following tools installed:
- Windows SDK
- Visual Studio 2022 with following workloads:
  - .NET desktop development      
  - Universal Windows Platform development

To build the application, follow these steps:
- Clone the repository:
  ```bash
  https://github.com/Yaroslav-Yuriichuk/Sportik.Desktop.git
- Open the solution file in IDE.
- Select the configuration and platform you want to build.
- Select package signing certificate in `Package.appxmanifest`.
- Click "Build" -> "Build Solution" to build the application or run directly in IDE.
