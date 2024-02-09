using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace WhyNotWinAI
{
    public partial class checksPageView : UserControl
    {
        private Logger logger;
        private WebView2 webView;
        private bool isChecksRunning = false;

        public checksPageView()
        {
            InitializeComponent();
            InitializeWebView();
            logger = new Logger(webView);
        }

        private async void InitializeWebView()
        {
            webView = new WebView2
            {
                Dock = DockStyle.None,
                Width = this.Width,
                Height = this.Height,
                Parent = this,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            };

            webView.CoreWebView2InitializationCompleted += webView2_CoreWebView2InitializationCompleted;

            await webView.EnsureCoreWebView2Async(null);

            string pathToHtmlFile = Application.StartupPath + "\\frontend.html";
            if (File.Exists(pathToHtmlFile))
            {
                string htmlContent = File.ReadAllText(pathToHtmlFile);
                webView.NavigateToString(htmlContent);
            }

            await webView.EnsureCoreWebView2Async();
            webView.WebMessageReceived += (s, e) =>
            {
                string message = e.TryGetWebMessageAsString();
                if (message == "runChecks")
                {
                    RunChecks();
                }
                else if (message == "toggleChecks")
                {
                    ToggleChecks();
                }
            };
            webView.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
        }

        private async void RunChecks()
        {
            var checks = new List<SystemCheck>
            {
                new InfoPoint(logger),
                new WindowsVersionCheck(),
                new ArchitectureCheck(),
                new MemoryCheck(),
                new StorageTypeCheck(),
                new FirmwareCheck(),
                new TPMCheck(),
                new GraphicsCheck(),
                new DisplayCheck(),
                new InetCheck(),
                new BatteryHealthCheck()
            };

            // Only add ProcessorCheck to the list of checks if the toggle is on
            if (isChecksRunning)
            {
                checks.Add(new ProcessorCheck());
                checks.Add(new CUDACheck());
            }

            int totalChecks = checks.Count;
            int successfulChecks = 0;
            int neutralChecks = 0;
            int failedChecks = 0;
            StringBuilder rockets = new StringBuilder();
            StringBuilder bananas = new StringBuilder();
            StringBuilder tomatos = new StringBuilder();

            foreach (var check in checks)
            {
                (string checkMessage, Color color) = await check.RunCheck(logger);

                if (color == Color.Green)
                {
                    successfulChecks++;
                    rockets.Append("🚀");
                }
                else if (color == Color.Tomato)
                {
                    neutralChecks++;
                    bananas.Append("🍌");
                }
                else if (color == Color.Red)
                {
                    failedChecks++;
                    tomatos.Append("🍅");
                }

                logger.Log(checkMessage, color);
            }

            string finalMessage = "";
            if (successfulChecks == totalChecks)
            {
                finalMessage = "All systems go! You're ready to conquer the digital universe!";
            }
            else if (successfulChecks >= totalChecks / 2)
            {
                finalMessage = "You're almost there! Keep pushing, victory is within reach!";
            }
            else if (failedChecks > successfulChecks)
            {
                finalMessage = "Challenges are just opportunities in disguise. Embrace the journey!";
            }
            else
            {
                finalMessage = "Stay resilient! Every setback is a setup for a comeback!";
            }

            finalMessage += $"<br/><br/><div class='summary'>Checks completed. <br/>{rockets} {successfulChecks} successful, {bananas} {neutralChecks} neutral, {tomatos} {failedChecks} failed</div>";
            webView.CoreWebView2.PostWebMessageAsString(finalMessage);
        }

        private void ToggleChecks()
        {
            isChecksRunning = !isChecksRunning;
        }

        private void CoreWebView2_NewWindowRequested(object sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = e.Uri,
                UseShellExecute = true
            });
            e.Handled = true;
        }

        private void webView2_CoreWebView2InitializationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
            }
            else
            {
                MessageBox.Show("WebView2 initialization failed.");
            }
        }

        private void linkURLIcon_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(DataHelper.Uri.URL_ICONATTRIBUTION);
        }

        private void btnHamburger_Click(object sender, EventArgs e)
        {
            this.contextMenu.Show(Cursor.Position.X, Cursor.Position.Y);
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string websiteUrl = "https://github.com/builtbybel/WhyNotWinAI/releases";
            webView.CoreWebView2.Navigate(websiteUrl);
        }
    }
}