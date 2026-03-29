using LightRP.Jobs;

namespace LightRP.Chat;

/// <summary>
/// Provides simple chat-based commands for the RP framework.
/// Commands are prefixed with '/' and processed on the server.
/// Add new commands by creating additional methods and
/// registering them in <see cref="TryHandle"/>.
/// </summary>
public static class ChatCommands
{
	/// <summary>
	/// Attempt to handle a chat message as a command.
	/// Returns true when the message was consumed as a command.
	/// </summary>
	public static bool TryHandle( string message, PlayerState caller )
	{
		if ( string.IsNullOrWhiteSpace( message ) ) { return false; }
		if ( !message.StartsWith( "/" ) ) { return false; }
		if ( caller is null ) { return false; }

		var parts = message.Split( ' ', System.StringSplitOptions.RemoveEmptyEntries );
		var command = parts[0].ToLowerInvariant();

		return command switch
		{
			"/job" => HandleJob( parts, caller ),
			"/money" => HandleMoney( caller ),
			"/name" => HandleName( parts, caller ),
			"/give" => HandleGive( parts, caller ),
			"/jobs" => HandleJobsList( caller ),
			"/help" => HandleHelp( caller ),
			_ => false
		};
	}

	private static bool HandleJob( string[] parts, PlayerState caller )
	{
		if ( parts.Length < 2 )
		{
			Log.Info( $"[LightRP] {caller.RpName} is currently a {caller.JobName}." );
			return true;
		}

		var jobName = string.Join( " ", parts.Skip( 1 ) );
		var job = JobManager.GetByName( jobName );

		if ( job is null )
		{
			Log.Info( $"[LightRP] Job '{jobName}' does not exist. Use /jobs to see available jobs." );
			return true;
		}

		if ( !JobManager.CanJoinJob( job.Name, GameManager.GetAllPlayers() ) )
		{
			Log.Info( $"[LightRP] Job '{job.Name}' is full." );
			return true;
		}

		if ( job.VoteRequired )
		{
			Log.Info( $"[LightRP] Job '{job.Name}' requires a vote." );
			return true;
		}

		caller.SetJob( job.Name );
		Log.Info( $"[LightRP] {caller.RpName} became a {job.Name}." );
		return true;
	}

	private static bool HandleMoney( PlayerState caller )
	{
		Log.Info( $"[LightRP] {caller.RpName} has ${caller.Money}." );
		return true;
	}

	private static bool HandleName( string[] parts, PlayerState caller )
	{
		if ( parts.Length < 2 )
		{
			Log.Info( "[LightRP] Usage: /name <new name>" );
			return true;
		}

		var newName = string.Join( " ", parts.Skip( 1 ) );
		var oldName = caller.RpName;
		caller.RpName = newName;
		Log.Info( $"[LightRP] {oldName} changed their name to {newName}." );
		return true;
	}

	private static bool HandleGive( string[] parts, PlayerState caller )
	{
		if ( parts.Length < 3 )
		{
			Log.Info( "[LightRP] Usage: /give <player> <amount>" );
			return true;
		}

		if ( !int.TryParse( parts[^1], out var amount ) || amount <= 0 )
		{
			Log.Info( "[LightRP] Amount must be a positive number." );
			return true;
		}

		var targetName = string.Join( " ", parts.Skip( 1 ).SkipLast( 1 ) );
		var target = GameManager.FindPlayer( targetName );

		if ( target is null )
		{
			Log.Info( $"[LightRP] Player '{targetName}' not found." );
			return true;
		}

		if ( caller.TransferMoney( target, amount ) )
		{
			Log.Info( $"[LightRP] {caller.RpName} gave ${amount} to {target.RpName}." );
		}
		else
		{
			Log.Info( $"[LightRP] {caller.RpName} cannot afford to give ${amount}." );
		}

		return true;
	}

	private static bool HandleJobsList( PlayerState caller )
	{
		Log.Info( "[LightRP] Available jobs:" );

		foreach ( var category in JobManager.GetCategories() )
		{
			Log.Info( $"  -- {category} --" );

			foreach ( var job in JobManager.GetByCategory( category ) )
			{
				var slots = job.MaxSlots > 0
					? $" [{JobManager.CountPlayersInJob( job.Name, GameManager.GetAllPlayers() )}/{job.MaxSlots}]"
					: "";
				Log.Info( $"  {job.Name}{slots} - ${job.Salary}/payday" );
			}
		}

		return true;
	}

	private static bool HandleHelp( PlayerState caller )
	{
		Log.Info( "[LightRP] Commands:" );
		Log.Info( "  /job <name>        - Switch to a job" );
		Log.Info( "  /jobs              - List available jobs" );
		Log.Info( "  /money             - Check your balance" );
		Log.Info( "  /name <name>       - Change your RP name" );
		Log.Info( "  /give <player> <$> - Give money to a player" );
		Log.Info( "  /help              - Show this list" );
		return true;
	}
}
