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
	/// Builds a full citizen pawn and assigns default RP state.
	/// </summary>
	public void OnActive( Connection connection )
	{
		// Pick a random spawn point, or fall back to just above the world origin.
		var spawn = Scene.GetAllComponents<SpawnPoint>().FirstOrDefault();
		var spawnPos = spawn?.Transform.Position ?? Vector3.Up * 50f;

		// Root player object.
		var playerObj = new GameObject( true, $"Player - {connection.DisplayName}" );
		playerObj.Transform.Position = spawnPos;

		// Citizen model and animations.
		var renderer = playerObj.Components.Create<SkinnedModelRenderer>();
		renderer.Model = Model.Load( "models/citizen/citizen.vmdl" );
		var animHelper = playerObj.Components.Create<CitizenAnimationHelper>();
		animHelper.Target = renderer;

		// Physics-driven character movement.
		var cc = playerObj.Components.Create<CharacterController>();
		cc.Height = 72f;
		cc.Radius = 16f;

		// First-person camera lives in a child object so it can be
		// enabled/disabled independently per client.
		var camObj = new GameObject( true, "Camera" );
		camObj.Parent = playerObj;
		var cam = camObj.Components.Create<CameraComponent>();
		cam.ZNear = 1f;
		cam.ZFar = 10000f;
		cam.FieldOfView = 90f;

		// Input / locomotion controller.
		playerObj.Components.Create<PlayerController>();

		// RP state.
		var state = playerObj.Components.Create<PlayerState>();
		state.RpName = connection.DisplayName;
		state.JobName = DefaultJobName;
		state.StartingMoney = DefaultStartingMoney;
		state.PaydayInterval = DefaultPaydayInterval;
		state.Money = DefaultStartingMoney;

		playerObj.NetworkSpawn( connection );
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
