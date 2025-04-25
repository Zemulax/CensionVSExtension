using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.IO;
using Microsoft.Web.WebView2.Core;

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
                // 1. Get the output directory where the build files are located
                string distPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WebUI", "dist");

                // Debug confirmation
                //MessageBox.Show($"Will serve from:\n{distPath}");

                // 2. Check if the dist directory and index.html exist
                string indexPath = Path.Combine(distPath, "index.html");
                if (!File.Exists(indexPath))
                {
                    MessageBox.Show($"CRITICAL: index.html not found at:\n{indexPath}\n" +
                                    "Ensure that:\n" +
                                    "1. Vite build output exists\n" +
                                    "2. Files are copied to output dir via .csproj");
                    return;
                }

                // 3. Set up WebView2 environment
                string userDataFolder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "cension",
                    "WebView2Cache"
                );
                var env = await CoreWebView2Environment.CreateAsync(null, userDataFolder);
                await webView.EnsureCoreWebView2Async(env);

                // 4. Set virtual hostname mapping
                string virtualHostName = "ExtensionUI";
                webView.CoreWebView2.SetVirtualHostNameToFolderMapping(
                    virtualHostName,
                    distPath,
                    CoreWebView2HostResourceAccessKind.Allow
                );

                // 5. Load the extensionUI using a fake HTTPS origin
                webView.Source = new Uri("https://" + virtualHostName + "/index.html");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fatal Error: {ex.Message}");
            }

        }
    }
}