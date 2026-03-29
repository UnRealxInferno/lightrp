using LightRP.Jobs;

namespace LightRP;

/// <summary>
/// Core game-mode manager for LightRP.
/// Place this component on a single GameObject in the scene.
/// It handles player connections and acts as the central
/// orchestrator for the RP framework.
/// </summary>
public sealed class GameManager : Component, Component.INetworkListener
{
	/// <summary>Default job assigned to players on connect.</summary>
	[Property] public string DefaultJobName { get; set; } = "Citizen";

	/// <summary>Starting money for new players.</summary>
	[Property] public int DefaultStartingMoney { get; set; } = 500;

	/// <summary>Seconds between salary payments.</summary>
	[Property] public float DefaultPaydayInterval { get; set; } = 120f;

	/// <summary>Quick access to the singleton instance.</summary>
	public static GameManager Instance { get; private set; }

	protected override void OnStart()
	{
		Instance = this;
	}

	/// <summary>
	/// Called by the network system when a player connects.
	/// Creates their player object and assigns default RP state.
	/// </summary>
	public void OnActive( Connection connection )
	{
		// Create a new player object from the scene's network prefab
		var playerObj = new GameObject( true, $"Player - {connection.DisplayName}" );
		playerObj.NetworkSpawn( connection );

		// Attach player state
		var state = playerObj.Components.Create<PlayerState>();
		state.RpName = connection.DisplayName;
		state.JobName = DefaultJobName;
		state.StartingMoney = DefaultStartingMoney;
		state.PaydayInterval = DefaultPaydayInterval;
		state.Money = DefaultStartingMoney;
	}

	/// <summary>Retrieve every PlayerState in the scene.</summary>
	public static IEnumerable<PlayerState> GetAllPlayers()
	{
		return Game.ActiveScene?
			.GetAllComponents<PlayerState>() ?? Enumerable.Empty<PlayerState>();
	}

	/// <summary>Find a player state by their display name.</summary>
	public static PlayerState FindPlayer( string name )
	{
		return GetAllPlayers()
			.FirstOrDefault( p =>
				p.RpName.Equals( name, System.StringComparison.OrdinalIgnoreCase ) );
	}
}
