using Unigine;
#region Math Variables
#if UNIGINE_DOUBLE
using Vec3 = Unigine.dvec3;
using Mat4 = Unigine.dmat4;
#else
using Vec3 = Unigine.vec3;
using Mat4 = Unigine.mat4;
#endif
#endregion

[Component(PropertyGuid = "d303a5eb1cf092c71065f658f6fe6e9724e6cd81")]
public class ZoomController : Component
{
	private float defaultFOV = 60.0f;
	private float defaultDistanceScale = 1.0f;
	private float defaultSensivity = 1.0f;
	private float defaultPlayerTurning = 90.0f;

	private Player player;

	private void Init()
	{
		player = node as Player;
		if (!player)
		{
			Log.Error("ZoomSample::init cannot cast node to player!\n");
		}

		defaultFOV = player.Fov;
		defaultDistanceScale = Render.DistanceScale;
		defaultSensivity = ControlsApp.MouseSensitivity;

		if (node.Type == Node.TYPE.PLAYER_SPECTATOR)
		{
			PlayerSpectator playerSpectator = player as PlayerSpectator;
			defaultPlayerTurning = playerSpectator.Turning;
		}
		if (node.Type == Node.TYPE.PLAYER_ACTOR)
		{
			PlayerActor playerActor = player as PlayerActor;
			defaultPlayerTurning = playerActor.Turning;
		}
	}

	private void Shutdown()
	{
		//so settings won't be affected between sessions
		Render.DistanceScale = defaultDistanceScale;
		ControlsApp.MouseSensitivity = defaultSensivity;
	}
	public void FocusOnTarget(Node target)
	{
		Vec3 dir = target.WorldPosition - node.WorldPosition;
		dir.Normalize();
		player.ViewDirection = (vec3)dir;
	}
	public void UpdateZoomFactor(float zoomFactor)
	{
		player.Fov = defaultFOV / zoomFactor;
		Render.DistanceScale = defaultDistanceScale * zoomFactor;
		ControlsApp.MouseSensitivity = defaultSensivity / zoomFactor;

		if (node.Type == Node.TYPE.PLAYER_SPECTATOR || node.Type == Node.TYPE.PLAYER_ACTOR)
		{
			UpdateTurning(zoomFactor);
		}
	}
	private void UpdateTurning(float zoomFactor)
	{
		// Turning determines speed at which the Player is rotated. It should be lowered and heightened depending on zoom factor
		// ZoomController has been made for the base Player class so it could work with any Player derived class But not every Player has a Turning property
		// Because of that we should regulate Turning depeping on player node type.
		// There is no work around this situation and since changing behavior depending on class type is bad practice this functionality has been hidden from public interface

		if (node.Type == Node.TYPE.PLAYER_SPECTATOR)
		{
			PlayerSpectator playerSpectator = player as PlayerSpectator;
			playerSpectator.Turning = defaultPlayerTurning / zoomFactor;
		}
		if (node.Type == Node.TYPE.PLAYER_ACTOR)
		{
			PlayerActor player_actor = player as PlayerActor;
			player_actor.Turning = defaultPlayerTurning / zoomFactor;
		}
	}
	public void Reset()
	{
		UpdateZoomFactor(1);
	}
}
