using Microsoft.Extensions.AI;

public record AIAgentModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public ChatOptions ChatOptions { get; set; } = new ChatOptions();
}