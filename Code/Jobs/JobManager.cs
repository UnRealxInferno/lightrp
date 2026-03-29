namespace LightRP.Jobs;

/// <summary>
/// Central registry of available jobs.
/// Edit <see cref="RegisterDefaultJobs"/> to add, remove, or modify
/// the jobs available on your server.
/// </summary>
public static class JobManager
{
	private static readonly List<JobDefinition> AllJobs = new();

	/// <summary>Read-only view of every registered job.</summary>
	public static IReadOnlyList<JobDefinition> Jobs => AllJobs;

	static JobManager()
	{
		RegisterDefaultJobs();
	}

	/// <summary>
	/// Populates the default set of jobs.
	/// Server owners: add, remove, or tweak jobs here.
	/// </summary>
	public static void RegisterDefaultJobs()
	{
		AllJobs.Clear();

		Register( new JobDefinition
		{
			Name = "Citizen",
			Description = "A regular citizen of the city.",
			Color = new Color( 0.30f, 0.69f, 0.31f ),
			Salary = 45,
			MaxSlots = 0,
			Category = "Civilian"
		} );

		Register( new JobDefinition
		{
			Name = "Police Officer",
			Description = "Protect and serve the citizens.",
			Color = new Color( 0.13f, 0.59f, 0.95f ),
			Salary = 65,
			MaxSlots = 4,
			Category = "Government"
		} );

		Register( new JobDefinition
		{
			Name = "Mayor",
			Description = "Lead the city and create laws.",
			Color = new Color( 1.00f, 0.76f, 0.03f ),
			Salary = 85,
			MaxSlots = 1,
			VoteRequired = true,
			Category = "Government"
		} );

		Register( new JobDefinition
		{
			Name = "Medic",
			Description = "Heal injured citizens for a fee.",
			Color = new Color( 0.91f, 0.12f, 0.39f ),
			Salary = 55,
			MaxSlots = 3,
			Category = "Civilian"
		} );

		Register( new JobDefinition
		{
			Name = "Gun Dealer",
			Description = "Sell weapons to those who can afford them.",
			Color = new Color( 0.61f, 0.15f, 0.69f ),
			Salary = 50,
			MaxSlots = 2,
			Category = "Civilian"
		} );

		Register( new JobDefinition
		{
			Name = "Gangster",
			Description = "Live outside the law and cause trouble.",
			Color = new Color( 0.62f, 0.62f, 0.62f ),
			Salary = 40,
			MaxSlots = 4,
			Category = "Criminal"
		} );

		Register( new JobDefinition
		{
			Name = "Mob Boss",
			Description = "Organise the criminal underworld.",
			Color = new Color( 0.38f, 0.38f, 0.38f ),
			Salary = 60,
			MaxSlots = 1,
			Category = "Criminal"
		} );
	}

	/// <summary>Register a new job definition.</summary>
	public static void Register( JobDefinition job )
	{
		if ( job is null ) { return; }

		// Prevent duplicate names
		AllJobs.RemoveAll( j => j.Name == job.Name );
		AllJobs.Add( job );
	}

	/// <summary>Remove a job by name.</summary>
	public static bool Unregister( string name )
	{
		return AllJobs.RemoveAll( j => j.Name == name ) > 0;
	}

	/// <summary>Look up a job by its exact name.</summary>
	public static JobDefinition GetByName( string name )
	{
		return AllJobs.FirstOrDefault( j => j.Name == name );
	}

	/// <summary>Return every job that belongs to a category.</summary>
	public static IEnumerable<JobDefinition> GetByCategory( string category )
	{
		return AllJobs.Where( j => j.Category == category );
	}

	/// <summary>Return the distinct list of categories.</summary>
	public static IEnumerable<string> GetCategories()
	{
		return AllJobs.Select( j => j.Category ).Distinct();
	}

	/// <summary>Count how many of a given PlayerState list hold a specific job.</summary>
	public static int CountPlayersInJob( string jobName, IEnumerable<PlayerState> allPlayers )
	{
		return allPlayers.Count( p => p.JobName == jobName );
	}

	/// <summary>
	/// Check whether a player can switch to the given job,
	/// respecting MaxSlots.
	/// </summary>
	public static bool CanJoinJob( string jobName, IEnumerable<PlayerState> allPlayers )
	{
		var job = GetByName( jobName );
		if ( job is null ) { return false; }
		if ( job.MaxSlots <= 0 ) { return true; }

		return CountPlayersInJob( jobName, allPlayers ) < job.MaxSlots;
	}
}
