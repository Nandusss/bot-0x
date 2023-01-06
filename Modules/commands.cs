using Discord.Commands;

public class commands : ModuleBase<SocketCommandContext>
{
    [Command("ping")]
	public async Task pong()
    {
        await ReplyAsync("pong");
    }
}