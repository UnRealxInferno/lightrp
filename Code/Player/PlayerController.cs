namespace LightRP;

/// <summary>
/// First-person citizen controller.
/// Handles player movement, jumping, and eye-angle synchronisation.
/// Attach this to the same GameObject as a <see cref="CharacterController"/>.
/// A child GameObject named "Camera" is expected to hold the
/// <see cref="CameraComponent"/> that this controller drives.
/// </summary>
public sealed class PlayerController : Component
{
	/// <summary>Walk speed in units per second.</summary>
	[Property] public float Speed { get; set; } = 180f;

	/// <summary>Height of the player's eyes above their origin.</summary>
	[Property] public float EyeHeight { get; set; } = 64f;

	/// <summary>Networked eye angles so other clients can read look direction.</summary>
	[Sync] public Angles EyeAngles { get; set; }

	private CharacterController _cc;
	private CameraComponent _camera;

	protected override void OnStart()
	{
		_cc = Components.Get<CharacterController>();
		_camera = Components.GetInDescendantsOrSelf<CameraComponent>();

		if ( _camera is not null )
		{
			// Only the owning client renders through their own camera.
			_camera.Enabled = !IsProxy;
			_camera.Priority = 100;
		}
	}

	protected override void OnUpdate()
	{
		if ( IsProxy ) { return; }

		var angles = EyeAngles;
		angles.pitch = System.Math.Clamp( angles.pitch + Input.MouseDelta.y * 0.1f, -89f, 89f );
		angles.yaw -= Input.MouseDelta.x * 0.1f;
		EyeAngles = angles;

		// Rotate the body to face the yaw direction.
		Transform.Rotation = Rotation.FromYaw( angles.yaw );

		// Keep the camera at eye level, pitched to the look angle.
		if ( _camera is not null )
		{
			_camera.Transform.LocalPosition = Vector3.Up * EyeHeight;
			_camera.Transform.Rotation = angles.ToRotation();
		}
	}

	protected override void OnFixedUpdate()
	{
		if ( IsProxy ) { return; }

		var cc = _cc;
		if ( cc is null ) { return; }

		var moveInput = Input.AnalogMove;
		var wishDir = ( Transform.Rotation * new Vector3( moveInput.x, moveInput.y, 0 ) )
			.WithZ( 0 )
			.Normal;

		if ( cc.IsOnGround )
		{
			cc.Acceleration = 10f;
			cc.Accelerate( wishDir * Speed );
			cc.ApplyFriction( 4f );

			if ( Input.Pressed( "Jump" ) )
			{
				cc.Punch( Vector3.Up * 300f );
			}
		}
		else
		{
			cc.Acceleration = 5f;
			cc.Accelerate( wishDir * ( Speed * 0.5f ) );
			cc.Velocity += Scene.PhysicsWorld.Gravity * Time.Delta;
		}

		cc.Move();
	}
}
