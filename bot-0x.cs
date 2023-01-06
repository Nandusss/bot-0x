using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    private DiscordSocketClient _client;
    private LoggingService _loggingService;
    private CommandService _commands;
    private IServiceProvider _services;
    private CommandHandler _commandHandler;

    public static Task Main(string[] args) => new Program().MainAsync();

    public async Task MainAsync()
    {
    _client = new DiscordSocketClient();
    _loggingService = new LoggingService(_client);
    _commands = new CommandService();
    _commandHandler = new CommandHandler(_client, _commands, _services);
    

    _services = new ServiceCollection()
        .AddSingleton(_client)
        .AddSingleton(_commands)
        .BuildServiceProvider();
    
    // Create a bot.env file with BOT_TOKEN having the token id from discord dev portal
    DotNetEnv.Env.Load("./bot.env");
    var token = System.Environment.GetEnvironmentVariable("BOT_TOKEN");

    // _client.MessageReceived += ClientOnMessageReceived;

    await _client.LoginAsync(TokenType.Bot, token);
    await _client.StartAsync();

    _commandHandler.InstallCommandsAsync();
    

    // Block this task until the program is closed.
    await Task.Delay(-1);
    }

    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }

    // private async Task ClientOnMessageReceived(SocketMessage socketMessage)
    // {
    //     await Task.Run(() =>
    //     {
    //         //Activity is not from a Bot.
    //         if (!socketMessage.Author.IsBot)
    //         {

    //             var authorId = socketMessage.Author.Id;
    //             var channelId  = socketMessage.Channel.Id.ToString();
    //             var messageId = socketMessage.Id.ToString();
    //             var message = socketMessage.Content;

    //             var channel = _client.GetChannel(Convert.ToUInt64(channelId));
    //             var socketChannel = (ISocketMessageChannel)channel;

    //             //Do Something and send a response here.
    //             socketChannel.SendMessageAsync("YOUR RESPONSE");
    //         }
    //     });
    // }

}