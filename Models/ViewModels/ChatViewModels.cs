using System.Collections.Generic;

namespace PhapClinicX.Models.ViewModels
{
    public class ChatQuery
    {
        public string Message { get; set; } = string.Empty;
        public int? UserId { get; set; }
    }

    public class ChatReply
    {
        public string Answer { get; set; } = string.Empty;
        public List<string> Sources { get; set; } = new();
    }
}
