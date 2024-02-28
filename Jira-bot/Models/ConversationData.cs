namespace Models
{
    public class ConversationData
    {
        public bool PromptedForEmail { get; set; } = false;
        public bool PromptedForURL { get; set; } = false;
        public bool PromptedForToken { get; set; } = false;
    }
}
