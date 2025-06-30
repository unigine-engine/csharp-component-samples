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

[Component(PropertyGuid = "14e937489e55720774e387ba153af3d19cfb9afc")]
public class CameraSpectator : Component
{
	public float speed = 3.0f;
	public float angularSpeed = 90.0f;

	public CameraControls controls = null;

	private PlayerDummy camera = null;

	private float horizontalAngle = 0.0f;
	private float verticalAngle = 0.0f;

	private void Init()
	{
		camera = node as PlayerDummy;
		if (!camera)
			return;

		Input.MouseHandle = Input.MOUSE_HANDLE.GRAB;

		vec3 direction = camera.GetWorldDirection();

		// get projection of direction on XY plane
		vec3 horizontalDirection = direction;
		horizontalDirection.z = 0;
		horizontalDirection.Normalize();

		// get current vertical angle of camera
		verticalAngle = MathLib.Angle(direction, horizontalDirection);
		verticalAngle *= -MathLib.Sign(direction.z);

		// get current horizontal angle of camera
		horizontalAngle = MathLib.Angle(horizontalDirection, vec3.FORWARD);
		horizontalAngle *= MathLib.Sign(direction.x);
	}

	private void Update()
	{
		if (!camera || controls == null)
			return;

		vec3 forward = camera.GetWorldDirection();
		vec3 right = camera.GetWorldDirection(MathLib.AXIS.X);
		vec3 up = camera.GetWorldDirection(MathLib.AXIS.Y);

		// get moving direction of camera
		vec3 targetVelocityDirection = vec3.ZERO;

		targetVelocityDirection += forward * controls.Forward;
		targetVelocityDirection -= forward * controls.Backward;

		targetVelocityDirection += right * controls.Right;
		targetVelocityDirection -= right * controls.Left;

		targetVelocityDirection += up * controls.Up;
		targetVelocityDirection -= up * controls.Down;

		// move camera in target direction
		float currentSpeed = speed * controls.Acceleration;

		if (targetVelocityDirection.Length2 > 0)
			targetVelocityDirection.Normalize();

		camera.WorldTranslate(new Vec3(targetVelocityDirection) * currentSpeed * Game.IFps);

		// update vertical and horizontal angles
		verticalAngle -= controls.TurnUp * angularSpeed * Game.IFps;
		verticalAngle += controls.TurnDown * angularSpeed * Game.IFps;
		verticalAngle = MathLib.Clamp(verticalAngle, -89.9f, 89.9f);

		horizontalAngle += controls.TurnRight * angularSpeed * Game.IFps;
		horizontalAngle -= controls.TurnLeft * angularSpeed * Game.IFps;

		if (horizontalAngle < -180 || 180 < horizontalAngle)
			horizontalAngle = MathLib.Clamp(-horizontalAngle, -180.0f, 180.0f);

		// calculate new camera direction
		vec3 cameraDirection = vec3.FORWARD * MathLib.RotateZ(horizontalAngle);
		cameraDirection = cameraDirection * MathLib.Rotate(MathLib.Cross(cameraDirection, vec3.UP), verticalAngle);

		// set new direction of camera
		camera.SetWorldDirection(cameraDirection, vec3.UP);
	}
}
