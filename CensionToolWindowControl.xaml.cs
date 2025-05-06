using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.IO;
using Microsoft.Web.WebView2.Core;
using System.Reflection;

namespace CensionExtension
{
    /// <summary>
    /// Interaction logic for CensionToolWindowControl.
    /// </summary>
    public partial class CensionToolWindowControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CensionToolWindowControl"/> class.
        /// </summary>
        public CensionToolWindowControl()
        {
            this.InitializeComponent();
            _ = InitializeWebViewAsync(); // Start the async initialization of WebView2
        }

        private async Task InitializeWebViewAsync()
        {
            try
            {
                // 1. Get the extension's installation directory (where VSIX deploys files)
                string extensionPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string distPath = Path.Combine(extensionPath, "WebUI", "dist");

                // 2. Check if index.html exists
                string indexPath = Path.Combine(distPath, "index.html");
                if (!File.Exists(indexPath))
                {
                    MessageBox.Show($"ERROR: WebUI files missing!\nExpected at: {indexPath}\n" +
                                  "Ensure 'WebUI/dist' is included in the VSIX as 'Content'.");
                    return;
                }

                // 3. Set up WebView2 environment (cache in LocalAppData)
                string userDataFolder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "cension",
                    "WebView2Cache"
                );
                var env = await CoreWebView2Environment.CreateAsync(null, userDataFolder);
                await webView.EnsureCoreWebView2Async(env);

                // 4. Map virtual hostname to the folder
                string virtualHostName = "ExtensionUI";
                webView.CoreWebView2.SetVirtualHostNameToFolderMapping(
                    virtualHostName,
                    distPath,
                    CoreWebView2HostResourceAccessKind.Allow
                );

                // 5. Load the UI
                webView.Source = new Uri($"https://{virtualHostName}/index.html");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fatal Error: {ex.Message}");
            }
        }
    }
}