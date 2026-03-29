using LightRP.Jobs;

namespace LightRP;

/// <summary>
/// Attached to every player. Tracks roleplay state such as
/// current job, display name, and wallet balance.
/// Networked properties are automatically synchronised.
/// </summary>
public sealed class PlayerState : Component
{
	/// <summary>The player's chosen roleplay name.</summary>
	[Property, Sync] public string RpName { get; set; } = "Unnamed";

	/// <summary>Current job name. Use <see cref="SetJob"/> to change.</summary>
	[Property, Sync] public string JobName { get; set; } = "Citizen";

	/// <summary>Current wallet balance.</summary>
	[Property, Sync] public int Money { get; set; } = 500;

	/// <summary>Money given to every new player.</summary>
	[Property] public int StartingMoney { get; set; } = 500;

	/// <summary>Seconds between salary payments.</summary>
	[Property] public float PaydayInterval { get; set; } = 120f;

	private TimeSince _lastPayday;

	/// <summary>Resolved job definition for the current job.</summary>
	public JobDefinition Job => JobManager.GetByName( JobName );

	protected override void OnStart()
	{
		if ( IsProxy ) { return; }
		Money = StartingMoney;
		_lastPayday = 0;
	}

	protected override void OnFixedUpdate()
	{
		if ( IsProxy ) { return; }
		if ( !Networking.IsHost ) { return; }

		if ( _lastPayday >= PaydayInterval )
		{
			GivePayday();
			_lastPayday = 0;
		}
	}

	/// <summary>
	/// Attempt to switch the player to a new job.
	/// Returns true on success.
	/// </summary>
	public bool SetJob( string jobName )
	{
		var job = JobManager.GetByName( jobName );
		if ( job is null ) { return false; }

		JobName = job.Name;
		return true;
	}

	/// <summary>
	/// Add (or subtract) money. Returns false if the result
	/// would be negative.
	/// </summary>
	public bool AddMoney( int amount )
	{
		if ( Money + amount < 0 ) { return false; }
		Money += amount;
		return true;
	}

	/// <summary>
	/// Transfer money from this player to another.
	/// Returns false if funds are insufficient.
	/// </summary>
	public bool TransferMoney( PlayerState recipient, int amount )
	{
		if ( amount <= 0 ) { return false; }
		if ( recipient is null ) { return false; }
		if ( Money < amount ) { return false; }

		Money -= amount;
		recipient.Money += amount;
		return true;
	}

	private void GivePayday()
	{
		var job = Job;
		if ( job is null ) { return; }
		Money += job.Salary;
	}
}
