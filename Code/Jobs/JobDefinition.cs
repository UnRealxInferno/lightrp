namespace LightRP.Jobs;

/// <summary>
/// Defines a roleplay job that players can select.
/// To add custom jobs, use <see cref="JobManager.Register"/>.
/// </summary>
public sealed class JobDefinition
{
	/// <summary>Unique display name of the job.</summary>
	public string Name { get; set; } = "Unnamed";

	/// <summary>Short description shown in the job menu.</summary>
	public string Description { get; set; } = "";

	/// <summary>Colour used for HUD and chat display.</summary>
	public Color Color { get; set; } = Color.White;

	/// <summary>Amount of money earned each payday.</summary>
	public int Salary { get; set; } = 45;

	/// <summary>Maximum players allowed in this job. 0 = unlimited.</summary>
	public int MaxSlots { get; set; } = 0;

	/// <summary>Whether a vote is required to become this job.</summary>
	public bool VoteRequired { get; set; } = false;

	/// <summary>Category shown in the job selection menu.</summary>
	public string Category { get; set; } = "Civilian";
}
