using System.Reflection;
using Discord.Commands;
using Discord.WebSocket;

public class CommandHandler
{
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;
    private IServiceProvider _services;

    // Retrieve client and CommandService instance via ctor
    public CommandHandler(DiscordSocketClient client, CommandService commands, IServiceProvider services)
    {
        _commands = commands;
        _client = client;
        _services = services;
    }
    
    public async Task InstallCommandsAsync()
    {
        Console.WriteLine("I was here");
        // Hook the MessageReceived event into our command handler
        _client.MessageReceived += HandleCommandAsync;

        // Here we discover all of the command modules in the entry assembly and load them.
        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
    }

    private async Task HandleCommandAsync(SocketMessage messageParam)
    {
        // Don't process the command if it was a system message
        var message = messageParam as SocketUserMessage;
        if (message == null) return;

        // Create a number to track where the prefix ends and the command begins
        int argPos = 0;

        // Determine if the message is a command based on the prefix and make sure no bots trigger commands
        if (!(message.HasCharPrefix('!', ref argPos) || 
            message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
            message.Author.IsBot)
            return;

        // Create a WebSocket-based command context based on the message
        var context = new SocketCommandContext(_client, message);

        // Execute the command with the command context we just
        // created, along with the service provider for precondition checks.
        var result = await _commands.ExecuteAsync(context, argPos, _services);
        if (!result.IsSuccess) Console.WriteLine(result.ErrorReason);
        if (result.Error.Equals(CommandError.UnmetPrecondition)) await message.Channel.SendMessageAsync(result.ErrorReason);
    }
}