namespace LzqNet.AI.Domain.Expands;

public class AIChatContent
{
    public string ChatClient { get; set; }

    public string AIAgentName { get; set; }

    public string Instructions { get; set; }

    public string Prompt { get; set; }

    public string Content { get; set; }

    public DateTime CreationTime { get; set; }

    public int PromptTokens { get; set; }

    public int CompletionTokens { get; set; }
}
