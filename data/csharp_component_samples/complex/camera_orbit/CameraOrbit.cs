#region Math Variables
#if UNIGINE_DOUBLE
using Scalar = System.Double;
using Vec2 = Unigine.dvec2;
using Vec3 = Unigine.dvec3;
using Vec4 = Unigine.dvec4;
using Mat4 = Unigine.dmat4;
#else
using Scalar = System.Single;
using Vec2 = Unigine.vec2;
using Vec3 = Unigine.vec3;
using Vec4 = Unigine.vec4;
using Mat4 = Unigine.mat4;
using WorldBoundBox = Unigine.BoundBox;
using WorldBoundSphere = Unigine.BoundSphere;
using WorldBoundFrustum = Unigine.BoundFrustum;
#endif
#endregion

using Unigine;

[Component(PropertyGuid = "0e56a38e74ddd5956a5929f0421bb48f43bf18a2")]
public class CameraOrbit : Component
{
	public CameraControls controls = null;
	public float angularSpeed = 90.0f;
	public float zoomSpeed = 3.0f;
	public float minDistance = 5.0f;
	public float maxDistance = 10.0f;
	public float minVerticalAngle = -89.9f;
	public float maxVerticalAngle = 89.9f;
	public Node target = null;

	private PlayerDummy camera = null;

	private float horizontalAngle = 0.0f;
	private float verticalAngle = 0.0f;
	private float distance = 0.0f;

	private Input.MOUSE_HANDLE init_mouse_handle;

	private void Init()
	{
		camera = node as PlayerDummy;
		if (!camera)
			return;

		if (!target)
			return;

		init_mouse_handle = Input.MouseHandle;
		Input.MouseHandle = Input.MOUSE_HANDLE.GRAB;

		// get camera direction
		vec3 direction = new vec3(target.WorldPosition - camera.WorldPosition);
		direction.Normalize();

		// get projection of direction on XY plane
		vec3 horizontalDirection = direction;
		horizontalDirection.z = 0;
		horizontalDirection.Normalize();

		// get current vertical angle of camera
		verticalAngle = MathLib.Angle(direction, horizontalDirection);
		verticalAngle *= -MathLib.Sign(direction.z);
		verticalAngle = MathLib.Clamp(verticalAngle, minVerticalAngle, maxVerticalAngle);

		// get current horizontal angle of camera
		horizontalAngle = MathLib.Angle(horizontalDirection, vec3.FORWARD);
		horizontalAngle *= MathLib.Sign(direction.x);

		// set target camera direction and position
		distance = minDistance + (maxDistance - minDistance) * 0.5f;
		camera.SetWorldDirection(direction, vec3.UP);
		camera.WorldPosition = target.WorldPosition - direction * distance;
	}

	private void Update()
	{
		if (!camera || !target || controls == null)
			return;

		// update vertical and horizontal angles
		verticalAngle -= controls.TurnUp * angularSpeed * Game.IFps;
		verticalAngle += controls.TurnDown * angularSpeed * Game.IFps;
		verticalAngle = MathLib.Clamp(verticalAngle, minVerticalAngle, maxVerticalAngle);

		horizontalAngle += controls.TurnRight * angularSpeed * Game.IFps;
		horizontalAngle -= controls.TurnLeft * angularSpeed * Game.IFps;

		if (horizontalAngle < -180 || 180 < horizontalAngle)
			horizontalAngle = MathLib.Clamp(-horizontalAngle, -180.0f, 180.0f);

		// calculate new camera direction
		vec3 cameraDirection = vec3.FORWARD * MathLib.RotateZ(horizontalAngle);
		cameraDirection = cameraDirection * MathLib.Rotate(MathLib.Cross(cameraDirection, vec3.UP), verticalAngle);

		// update distance
		distance -= controls.ZoomIn * zoomSpeed * Game.IFps;
		distance += controls.ZoomOut * zoomSpeed * Game.IFps;
		distance = MathLib.Clamp(distance, minDistance, maxDistance);

		// set new direction amd position of camera
		camera.SetWorldDirection(cameraDirection, vec3.UP);
		camera.WorldPosition = target.WorldPosition - cameraDirection * distance;
	}

	private void Shutdown()
	{
		Input.MouseHandle = init_mouse_handle;
	}
}
