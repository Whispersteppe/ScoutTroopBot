using System.Text;
using Xunit.Abstractions;

namespace ScoutBot_Test;

public class GenerateDiscordSocketClientEvents
{
    ITestOutputHelper _output;
    public GenerateDiscordSocketClientEvents(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void GenerateEvents()
    {
        foreach (var eventInfo in typeof(Discord.WebSocket.DiscordSocketClient).GetEvents())
        {
            _output.WriteLine($"// {eventInfo.Name}");
            _output.WriteLine($"void On{eventInfo.Name}({string.Join(", ", eventInfo.EventHandlerType.GetMethod("Invoke")!.GetParameters().Select(p => $"{p.ParameterType.FullName} {p.Name}"))})");
            _output.WriteLine("{");
            _output.WriteLine("    throw new NotImplementedException();");
            _output.WriteLine("}");
            _output.WriteLine("");
        }
    }

    [Fact]
    public void GenerateEventEnum()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("public enum DiscordSocketClientEvent");
        sb.AppendLine("{");
        foreach (var eventInfo in typeof(Discord.WebSocket.DiscordSocketClient).GetEvents())
        {
            sb.AppendLine($"    {eventInfo.Name},");
        }
        sb.AppendLine("}");

        _output.WriteLine(sb.ToString());
    }
}
