using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.ComponentModel.Design;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Task = System.Threading.Tasks.Task;

namespace CensionExtension
{
    internal class CensionContextAwareness
    {
        private readonly AsyncPackage package;
        private static readonly string endpoint = "https://cubanzemulax--cension-chatbot-censionchatbot.modal.run";

        private CensionContextAwareness(AsyncPackage package, OleMenuCommandService commandService)
        {

            this.package = package ?? throw new ArgumentNullException(nameof(package));

            var cmdID = new CommandID(CommandIds.CommandSetGuid, CommandIds.CensionContextAwarenessCommandId);
            var menuCommand = new MenuCommand(Execute, cmdID);

            commandService?.AddCommand(menuCommand);
        }

        public static CensionContextAwareness Instance { get; private set; }

        private IAsyncServiceProvider ServiceProvider => package;

        public static async Task InitializeAsync(AsyncPackage package)
        {

            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new CensionContextAwareness(package, commandService);
        }

        /// <summary>
        /// this method is called when the command is executed.
        /// this method is responsible for getting the current context of the class.
        private async void Execute(object sender, EventArgs e)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            // Get the current solution and project context
            var dte = await ServiceProvider?.GetServiceAsync(typeof(DTE)) as DTE2;
            if (dte == null || dte.ActiveDocument == null)
            {
                MessageBox.Show("Unable to get the DTE object.");
                return;
            }
            var activeClass = dte?.ActiveDocument;

            if (activeClass?.Object("TextDocument") is TextDocument textDocument)
            {
                var selection = textDocument.Selection;

                int currentLineNumber = selection.ActivePoint.Line;

                //get full line and extract it
                selection.StartOfLine();
                selection.SelectLine();
                string currentLine = selection.Text.Trim();

                //if its not a comment line, show an error message
                if (!currentLine.StartsWith("//", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Please place the cursor on a comment line starting with //.",
                        "Comment Not Found",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                        );
                    return;
                }

                //
                string prompt = currentLine.TrimStart('/').Trim();
                string generatedCode = await GetCodeFromModel(prompt);

                if (!string.IsNullOrWhiteSpace(generatedCode))
                {
                    generatedCode = generatedCode.TrimStart('\r', '\n');

                    //Move to line *after* the comment
                    selection.MoveToLineAndOffset(currentLineNumber + 1, 1);

                    //Collapse selection to avoid partial replacements
                    selection.Collapse();

                    selection.Insert(generatedCode);

                    //format the whole class
                    dte.ExecuteCommand("Edit.FormatDocument");
                }
            }


        }

        /// </summary>
        /// this method is responsible for getting the code from the model.
        private async Task<string> GetCodeFromModel(string prompt)
        {
            try
            {
                // Create an HTTP client and set the request headers
                var client = new HttpClient();
                var json = $"{{\"prompt\": \"{EscapeForJson(prompt)}\"}}";
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                //get the response from the model
                var response = await client.PostAsync(endpoint, content);
                response.EnsureSuccessStatusCode();
                // Check if the response is successful
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    MessageBox.Show($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    return "// Error: Unable to generate code. please try again";
                }

                var result = await response.Content.ReadAsStringAsync();

                if (result != null)
                {
                    var index = result.IndexOf(":") + 1;
                    return result.Substring(index).Trim('{', '}', '\"', ' ').Replace("```csharp", "").Replace("```", "");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            }
            return "// Error: Unable to generate code.";
        }

        private string EscapeForJson(string str)
        {
            return str.Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r");
        }
    }
}
