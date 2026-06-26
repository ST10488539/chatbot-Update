using System;

namespace WpfApp1
{
    public class ChatMessage
    {
        public string Sender { get; set; }
        public string Text { get; set; }
        public string Timestamp { get; set; }

        public ChatMessage(string sender, string text)
        {
            Sender = sender;
            Text = text;
            // Automatically records the current time (e.g., 13:24)
            Timestamp = DateTime.Now.ToString("HH:mm");
        }
    }
}