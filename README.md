# ONLYOFFICE solution for SharePoint

Imagine opening any SharePoint document — docx, xlsx, pptx — and having a full-fledged modern editor appear instantly, with real-time co-editing, comments, versioning, and everything else you'd expect from a modern office suite. That's exactly what the ONLYOFFICE integration does.

You connect [ONLYOFFICE Docs](https://www.onlyoffice.com/docs), deploy a lightweight SharePoint solution package, and your team gets a seamless editing experience right inside the SharePoint interface.

<p align="center">
  <a href="https://www.onlyoffice.com/office-for-sharepoint">
    <img width="800" src="https://static-site.onlyoffice.com/public/images/templates/office-for-sharepoint/hero/screen5@2x.png" alt="ONLYOFFICE for SharePoint">
  </a>
</p>

## Features

### Full editing, right inside SharePoint ✍️

- DOCX, XLSX, PPTX, PDF editing
- Autosave, revision control
- Smooth performance (even with large files)

### Real-time co-authoring 👥

- Fast + Strict co-editing modes
- Comments & Track Changes
- Conflict-free collaboration
- Built-in chat and version history

### Secure by design 🔐

- JWT protection
- Custom JWT header for SharePoint environments
- HTTPS recommended everywhere

### Supported formats

**For viewing:**

- **WORD**: DOC, DOCM, DOCX, DOT, DOTM, DOTX, EPUB, FB2, FODT, HML, HTM, HTML, HWP, HWPX, MD, MHT, MHTML, ODT, OTT, PAGES, RTF, STW, SXW, TXT, WPS, WPT, XML
- **CELL**: CSV, ET, ETT, FODS, NUMBERS, ODS, OTS, SXC, XLS, XLSB, XLSM, XLSX, XLT, XLTM, XLTX
- **SLIDE**: DPS, DPT, FODP, KEY, ODG, ODP, OTP, POT, POTM, POTX, PPS, PPSM, PPSX, PPT, PPTM, PPTX, SXI
- **PDF**: DJVU, OXPS, PDF, XPS
- **DIAGRAM**: VSDM, VSDX, VSSM, VSSX, VSTM, VSTX

**For editing:**

- **WORD**: DOCM, DOCX, DOTM, DOTX
- **CELL**: XLSB, XLSM, XLSX, XLTM, XLTX
- **SLIDE**: POTM, POTX, PPSM, PPSX, PPTM, PPTX
- **PDF**: PDF

## Installing ONLYOFFICE Docs

You will need an instance of ONLYOFFICE Docs (Document Server) that is resolvable and connectable both from SharePoint and any end clients. ONLYOFFICE Document Server must also be able to POST to SharePoint directly.

### ☁️ Option 1: ONLYOFFICE Docs Cloud

No installation needed — just [register here](https://www.onlyoffice.com/docs-registration) and get instant access. Your registration email includes all required connection details.

### 🏠 Option 2: Self-hosted ONLYOFFICE Docs

Install ONLYOFFICE Docs on your own infrastructure for full control. You have two main choices:

* **Community Edition (Free)**: Ideal for small teams and personal use.
  * The **recommended** installation method is [Docker](https://github.com/onlyoffice/Docker-DocumentServer).
  * To install it on Debian, Ubuntu, or other derivatives, click [here](https://helpcenter.onlyoffice.com/docs/installation/docs-community-install-ubuntu.aspx).
* **Enterprise Edition**: Provides scalability for larger organizations. To install, click [here](https://helpcenter.onlyoffice.com/docs/installation/enterprise).

Community Edition vs Enterprise Edition comparison can be found [here](#onlyoffice-docs-editions).

## Installing ONLYOFFICE solution for SharePoint

1. Click Start, point to All Programs, point to Administrative Tools, and then click Services, and make sure that **SharePoint Administration service** is started.
2. Click Start, click **SharePoint Management Shell**, go to the directory with the .wsp file.
3. Run the Install.ps1 script:
    ```
    PS> .\Install.ps1
    ```
4. Enter your SharePoint site:
    ```
    https://<yoursharepointsite>
    ```
    > Alternatively to steps 3 and 4 you can type the following command:
    ```
    Add-SPSolution -LiteralPath <SolutionPath>/onlyoffice.wsp
    ```
    > On the SharePoint Central Administration Home page, click System Settings > Farm Management > Manage farm solutions.
    > On the Solution Management page, click the "onlyoffice.wsp", then click "Deploy Solution".
5. On the SharePoint Central Administration home page, under Application Management, click **Manage web applications**.
6. Make sure you select your site and click the **Authentication Providers** icon.
7. In the Authentication Providers pop-up window, click **Default zone**.
8. Under Edit Authentication, check **Enable anonymous access** and click Save.
9. Going back to Web Application Management click on the **Anonymous Policy** icon.
10. Under Anonymous Access Restrictions select your Zone and set the Permissions to **None - No policy** and click Save.

## Configuring ONLYOFFICE solution for SharePoint ⚙️

In SharePoint, open the `/_layouts/15/Onlyoffice/Settings.aspx` page with administrative settings.
Enter the following address to connect ONLYOFFICE Docs (Document Server):

```
https://<documentserver>/
```
Where the *documentserver* is the name of the server with ONLYOFFICE Docs installed. The address must be accessible for the user browser and from the SharePoint server. The SharePoint server address must also be accessible from ONLYOFFICE Docs for correct work.

*Please note, that if you have subsites set up with SharePoint, you will need to additionally configure ONLYOFFICE Document Server connection with each of them, in order for it to work properly. Go to each subsite settings and enter the Document Server address to the proper field.*

Configuration settings include JWT, enabled by default to protect the editors from unauthorized access. If setting a custom secret key, ensure it matches the one in the ONLYOFFICE Docs [config file](https://api.onlyoffice.com/docs/docs-api/additional-api/signature/) for proper validation.

If JWT protection is enabled, it is necessary to specify a custom header name since the SharePoint security policy blocks external 'Authorization' Headers. This header should be specified in the ONLYOFFICE Docs signature settings as well (further information can be found [here](https://api.onlyoffice.com/docs/docs-api/additional-api/signature/)).

## Compiling ONLYOFFICE solution for SharePoint

There are two ways to compile ONLYOFFICE solution for SharePoint:

**a. Using MS Visual Studio:**
  1. Enter the SharePoint server and open this project in Visual Studio.
  2. In Solution Explorer, open the shortcut menu for the project and then choose Publish.
  3. In the Publish dialog box, choose the Publish to File System option button.
  4. Click the Publish button. When the publishing process is finished, the solution .wsp file will be created.
  5. Copy the resulting file to the folder with the Install.ps1 file (BuildAndInstall folder by default).

**b. With the help of the build.bat file provided:**
  1. Go to the BuildAndInstall folder.
  2. Run the build.bat file.
  3. The resulting solution .wsp file will be created and placed to the BuildAndInstall folder.

## How it works

* User navigates to a document within SharePoint and selects the **Edit in ONLYOFFICE action** on context menu or ribbon.
* SharePoint ONLYOFFICE solution makes a request to the editor page (URL of the form: `/_layouts/15/Onlyoffice/editorPage.aspx?SPListItemId={ItemId}&SPListURLDir={ListUrlDir}&action=track`).
* SharePoint ONLYOFFICE solution prepares a JSON object with the following properties:
  * **url** - the URL that ONLYOFFICE Document Server uses to download the document;
  * **callbackUrl** - the URL that ONLYOFFICE Document Server informs about status of the document editing;
  * **DocumentSeverHost** - the URL that the client needs to reply to ONLYOFFICE Document Server (can be set at the settings page);
  * **Key** - the file identificator from SharePoint;
  * **FileName** - the document Title (name);
  * **CurrentUserId** - the identification of the user;
  * **CurrentUserName** - the name of the user.

* SharePoint ONLYOFFICE solution constructs a page, filling in all of those values so that the client browser can load up the editor.
* The client browser makes a request for the javascript library from ONLYOFFICE Document Server and sends ONLYOFFICE Document Server the DocEditor configuration with the above properties.
* Then ONLYOFFICE Document Server downloads the document from SharePoint and the user begins editing.
* When all users and client browsers are done with editing, they close the editing window.
* After 10 seconds of inactivity, ONLYOFFICE Document Server sends a POST to the callback URL letting SharePoint ONLYOFFICE solution know that the clients have finished editing the document and closed it.
* SharePoint ONLYOFFICE solution downloads the new version of the document, replacing the old one.

## ONLYOFFICE Docs editions

ONLYOFFICE offers different versions of its online document editors that can be deployed on your own servers.

* Community Edition 🆓 (`onlyoffice-documentserver` package)
* Enterprise Edition 🏢 (`onlyoffice-documentserver-ee` package)

The table below will help you to make the right choice.

| Pricing and licensing | Community Edition | Enterprise Edition |
| ------------- | ------------- | ------------- |
| | [Get it now](https://www.onlyoffice.com/download-community?utm_source=github&utm_medium=cpc&utm_campaign=GitHubSharePoint#docs-community)  | [Start Free Trial](https://www.onlyoffice.com/download?utm_source=github&utm_medium=cpc&utm_campaign=GitHubSharePoint#docs-enterprise)  |
| Cost  | FREE  | [Go to the pricing page](https://www.onlyoffice.com/docs-enterprise-prices?utm_source=github&utm_medium=cpc&utm_campaign=GitHubSharePoint)  |
| Simultaneous connections | up to 20 maximum  | As in chosen pricing plan |
| Number of users | up to 20 recommended | As in chosen pricing plan |
| License | GNU AGPL v.3 | Proprietary |
| **Support** | **Community Edition** | **Enterprise Edition** |
| Documentation | [Help Center](https://helpcenter.onlyoffice.com/docs/installation/community) | [Help Center](https://helpcenter.onlyoffice.com/docs/installation/enterprise) |
| Standard support | [GitHub](https://github.com/ONLYOFFICE/DocumentServer/issues) or paid | 1 or 3 years support included |
| Premium support | [Contact us](mailto:sales@onlyoffice.com) | [Contact us](mailto:sales@onlyoffice.com) |
| **Services** | **Community Edition** | **Enterprise Edition** |
| Conversion Service                | + | + |
| Document Builder Service          | + | + |
| **Interface** | **Community Edition** | **Enterprise Edition** |
| Tabbed interface                  | + | + |
| Dark theme                        | + | + |
| 125%, 150%, 175%, 200% scaling    | + | + |
| White Label                       | - | - |
| Integrated test example (node.js) | + | + |
| Mobile web editors                | - | +* |
| **Plugins & Macros** | **Community Edition** | **Enterprise Edition** |
| Plugins                           | + | + |
| Macros                            | + | + |
| **Collaborative capabilities** | **Community Edition** | **Enterprise Edition** |
| Two co-editing modes              | + | + |
| Comments                          | + | + |
| Built-in chat                     | + | + |
| Review and tracking changes       | + | + |
| Display modes of tracking changes | + | + |
| Version history                   | + | + |
| **Document Editor features** | **Community Edition** | **Enterprise Edition** |
| Font and paragraph formatting   | + | + |
| Object insertion                | + | + |
| Adding Content control          | + | + |
| Editing Content control         | + | + |
| Layout tools                    | + | + |
| Table of contents               | + | + |
| Navigation panel                | + | + |
| Mail Merge                      | + | + |
| Comparing Documents             | + | + |
| **Spreadsheet Editor features** | **Community Edition** | **Enterprise Edition** |
| Font and paragraph formatting   | + | + |
| Object insertion                | + | + |
| Functions, formulas, equations  | + | + |
| Table templates                 | + | + |
| Pivot tables                    | + | + |
| Data validation                 | + | + |
| Conditional formatting          | + | + |
| Sparklines                      | + | + |
| Sheet Views                     | + | + |
| **Presentation Editor features** | **Community Edition** | **Enterprise Edition** |
| Font and paragraph formatting   | + | + |
| Object insertion                | + | + |
| Transitions                     | + | + |
| Animations                      | + | + |
| Presenter mode                  | + | + |
| Notes                           | + | + |
| **Form creator features** | **Community Edition** | **Enterprise Edition** |
| Adding form fields              | + | + |
| Form preview                    | + | + |
| Saving as PDF                   | + | + |
| **PDF Editor features**      | **Community Edition** | **Enterprise Edition** |
| Text editing and co-editing                                | + | + |
| Work with pages (adding, deleting, rotating)               | + | + |
| Inserting objects (shapes, images, hyperlinks, etc.)       | + | + |
| Text annotations (highlight, underline, cross out, stamps) | + | + |
| Comments                        | + | + |
| Freehand drawings               | + | + |
| Form filling                    | + | + |
| | [Get it now](https://www.onlyoffice.com/download-community?utm_source=github&utm_medium=cpc&utm_campaign=GitHubSharePoint#docs-community)  | [Start Free Trial](https://www.onlyoffice.com/download?utm_source=github&utm_medium=cpc&utm_campaign=GitHubSharePoint#docs-enterprise)  |

\* If supported by DMS.

## Need help? User Feedback and Support 💡

* **🐞 Found a bug?** Please report it by creating an [issue](https://github.com/ONLYOFFICE/onlyoffice-sharepoint/issues).
* **❓ Have a question?** Ask our community and developers on the [ONLYOFFICE Forum](https://community.onlyoffice.com).
* **👨‍💻 Need help for developers?** Check our [API documentation](https://api.onlyoffice.com).
* **💡 Want to suggest a feature?** Share your ideas on our [feedback platform](https://feedback.onlyoffice.com/forums/966080-your-voice-matters).