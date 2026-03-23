using System;

namespace MediCom.Komunikacia.Models
{
    public class ChatMessage
    {
        public string Sender { get; set; }

        public string Text { get; set; }

        public DateTime Timestamp { get; set; }

        public bool IsUser { get; set; }
    }
}
