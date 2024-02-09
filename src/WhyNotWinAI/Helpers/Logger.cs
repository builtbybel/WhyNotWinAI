using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace WhyNotWinAI
{
    public class Logger
    {
        private WebView2 webView;

        public Logger(WebView2 webView)
        {
            this.webView = webView;
        }

        // Log method for a list of strings
        public void Log(List<string> messages, Color color)
        {
            if (webView.InvokeRequired)
            {
                webView.Invoke(new Action(() => Log(messages, color)));
                return;
            }

            foreach (var message in messages)
            {
                LogToWebView(message, color);
            }
        }

        // Log method for a single string
        public void Log(string message, Color color)
        {
            if (webView.InvokeRequired)
            {
                webView.Invoke(new Action(() => Log(message, color)));
                return;
            }

            LogToWebView(message, color);
        }


        // Log method with custom style 
        public void LogWithCustomStyle(string message, Color foregroundColor, Color backgroundColor, int fontSize)
        {
            if (webView.InvokeRequired)
            {
                webView.Invoke(new Action(() => LogWithCustomStyle(message, foregroundColor, backgroundColor, fontSize)));
                return;
            }

            // Format message with custom style
            string formattedMessage = $"<div style=\"color:{ColorToHex(foregroundColor)}; background-color:{ColorToHex(backgroundColor)}; font-size:{fontSize}px;\">{message}</div>";

            // Log formatted message
            LogToWebView(formattedMessage, Color.Black);
        }

        // Helper method to add log entry to WebView2
        private void LogToWebView(string message, Color color)
        {
            // Replace "\n" with HTML line breaks
            message = message.Replace("\n", "<br>");

            // Wrap each message in a <div> with specified color
            string formattedMessage = $"<div style='color:{ColorToHex(color)}'>{message}</div>";

            // Post the message to WebView2
            webView.CoreWebView2.PostWebMessageAsString(formattedMessage);

            // Scroll down after adding new content
            webView.ExecuteScriptAsync("window.scrollTo(0, document.body.scrollHeight);");
        }

        // Helper method to convert Color to hexadecimal
        private string ColorToHex(Color color)
        {
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }
    }
}
