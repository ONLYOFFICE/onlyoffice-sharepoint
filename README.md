# SharePoint ONLYOFFICE integration solution

This app enables users to edit office documents from SharePoint using ONLYOFFICE Docs packaged as Document Server - [Community or Enterprise Edition](#onlyoffice-docs-editions).

## Features

The app allows to:

* Edit text documents, spreadsheets, and presentations.
* Co-edit documents in real-time: use two co-editing modes (Fast and Strict), Track Changes, comments, and built-in chat.

Supported formats: 

* For viewing and editing: DOCX, XLSX, PPTX.
* For viewing only: PDF, DJVU, TXT, CSV, ODT, ODS, ODP, DOC, XLS, PPT, PPS, EPUB, RTF, HTML, HTM, MHT, XPS.

## Installing ONLYOFFICE Docs

You will need an instance of ONLYOFFICE Docs (Document Server) that is resolvable and connectable both from SharePoint and any end clients. ONLYOFFICE Document Server must also be able to POST to SharePoint directly.

You can install free Community version of ONLYOFFICE Docs or scalable Enterprise Edition with pro features.

To install free Community version, use [Docker](https://github.com/onlyoffice/Docker-DocumentServer) (recommended) or follow [these instructions](https://helpcenter.onlyoffice.com/installation/docs-community-install-ubuntu.aspx) for Debian, Ubuntu, or derivatives.  

To install Enterprise Edition, follow instructions [here](https://helpcenter.onlyoffice.com/installation/docs-enterprise-index.aspx).

Community Edition vs Enterprise Edition comparison can be found [here](#onlyoffice-docs-editions).

## Installing SharePoint ONLYOFFICE integration solution

1. Click Start, point to All Programs, point to Administrative Tools, and then click Services, and make sure that SharePoint Administration service is started.
2. Click Start, click SharePoint Management Shell, go to the directory with the .wsp file.
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
5. On the SharePoint Central Administration home page, under Application Management, click Manage web applications.
6. Make sure you select your site and click the Authentication Providers icon.
7. In the Authentication Providers pop-up window click Default zone.
8. Under Edit Authentication, check Enable anonymous access and click Save.
9. Going back to Web Application Management click on the Anonymous Policy icon.
10. Under Anonymous Access Restrictions select your Zone and set the Permissions to None – No policy and click Save.

## Configuring SharePoint ONLYOFFICE integration solution

In SharePoint open the `/_layouts/15/Onlyoffice/Settings.aspx` page with administrative settings.
Enter the following address to connect ONLYOFFICE Document Server:
```
https://<documentserver>/
```
Where the documentserver is the name of the server with the ONLYOFFICE Document Server installed. The address must be accessible for the user browser and from the SharePoint server. The SharePoint server address must also be accessible from ONLYOFFICE Document Server for correct work.

*Please note, that if you have subsites set up with SharePoint, you will need to additionally configure ONLYOFFICE Document Server connection with each of them, in order for it to work properly. Go to each subsite settings and enter the Document Server address to the proper field.*

Starting from version 7.2, JWT is enabled by default and the secret key is generated automatically to restrict the access to ONLYOFFICE Docs and for security reasons and data integrity. 
Specify your own **Secret key** in the SharePoint administrative settings. 
In the ONLYOFFICE Docs [config file](https://api.onlyoffice.com/editors/signature/), specify the same secret key and enable the validation.

If JWT protection is enabled, it is necessary to specify a custom header name since the SharePoint security policy blocks external 'Authorization' Headers. This header should be specified in the ONLYOFFICE Docs signature settings as well (further information can be found [here](https://api.onlyoffice.com/editors/signature/)).

## Compiling SharePoint ONLYOFFICE integration solution

There are two ways to compile ONLYOFFICE SharePoint integration solution:

a. Using MS Visual Studio:
  1. Enter the SharePoint server and open this project in Visual Studio.
  2. In Solution Explorer, open the shortcut menu for the project and then choose Publish.
  3. In the Publish dialog box, choose the Publish to File System option button.
  4. Click the Publish button. When the publishing process is finished, the solution .wsp file will be created.
  5. Copy the resulting file to the folder with the Install.ps1 file (BuildAndInstall folder by default).

b. With the help of the build.bat file provided:
  1. Go to the BuildAndInstall folder.
  2. Run the build.bat file.
  3. The resulting solution .wsp file will be created and placed to the BuildAndInstall folder.

## How it works

* User navigates to a document within SharePoint and selects the Edit in ONLYOFFICE action on context menu or ribbon.
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

* Community Edition (`onlyoffice-documentserver` package)
* Enterprise Edition (`onlyoffice-documentserver-ee` package)

The table below will help you make the right choice.

| Pricing and licensing | Community Edition | Enterprise Edition |
| ------------- | ------------- | ------------- |
| | [Get it now](https://www.onlyoffice.com/download-docs.aspx?utm_source=github&utm_medium=cpc&utm_campaign=GitHubSharePoint#docs-community)  | [Start Free Trial](https://www.onlyoffice.com/download-docs.aspx?utm_source=github&utm_medium=cpc&utm_campaign=GitHubSharePoint#docs-enterprise)  |
| Cost  | FREE  | [Go to the pricing page](https://www.onlyoffice.com/docs-enterprise-prices.aspx?utm_source=github&utm_medium=cpc&utm_campaign=GitHubSharePoint)  |
| Simultaneous connections | up to 20 maximum  | As in chosen pricing plan |
| Number of users | up to 20 recommended | As in chosen pricing plan |
| License | GNU AGPL v.3 | Proprietary |
| **Support** | **Community Edition** | **Enterprise Edition** |
| Documentation | [Help Center](https://helpcenter.onlyoffice.com/installation/docs-community-index.aspx) | [Help Center](https://helpcenter.onlyoffice.com/installation/docs-enterprise-index.aspx) |
| Standard support | [GitHub](https://github.com/ONLYOFFICE/DocumentServer/issues) or paid | One year support included |
| Premium support | [Contact us](mailto:sales@onlyoffice.com) | [Contact us](mailto:sales@onlyoffice.com) |
| **Services** | **Community Edition** | **Enterprise Edition** |
| Conversion Service                | + | + |
| Document Builder Service          | + | + |
| **Interface** | **Community Edition** | **Enterprise Edition** |
| Tabbed interface                       | + | + |
| Dark theme                             | + | + |
| 125%, 150%, 175%, 200% scaling         | + | + |
| White Label                            | - | - |
| Integrated test example (node.js)      | + | + |
| Mobile web editors                     | - | +* |
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
| Data validation           | + | + |
| Conditional formatting          | + | + |
| Sparklines                   | + | + |
| Sheet Views                     | + | + |
| **Presentation Editor features** | **Community Edition** | **Enterprise Edition** |
| Font and paragraph formatting   | + | + |
| Object insertion                | + | + |
| Transitions                     | + | + |
| Presenter mode                  | + | + |
| Notes                           | + | + |
| **Form creator features** | **Community Edition** | **Enterprise Edition** |
| Adding form fields           | + | + |
| Form preview                    | + | + |
| Saving as PDF                   | + | + |
| | [Get it now](https://www.onlyoffice.com/download-docs.aspx?utm_source=github&utm_medium=cpc&utm_campaign=GitHubSharePoint#docs-community)  | [Start Free Trial](https://www.onlyoffice.com/download-docs.aspx?utm_source=github&utm_medium=cpc&utm_campaign=GitHubSharePoint#docs-enterprise)   |

\* If supported by DMS.