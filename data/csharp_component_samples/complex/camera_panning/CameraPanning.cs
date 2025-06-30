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

[Component(PropertyGuid = "f54dff80b8b1ecdfa39fd4b2ffadac6941338ab4")]
public class CameraPanning : Component
{
	public float defaultLinearSpeed = 0.02f;
	public float mouseSensitivity = 0.08f;
	public float mouseWheelSensitivity = 0.5f;

	private PlayerDummy camera = null;

	private float horizontalAngle = 0.0f;
	private float verticalAngle = 0.0f;

	private Vec3? grabbingPoint = Vec3.ZERO;
	private Vec3 grabbingCameraPos = Vec3.ZERO;
	private vec3 planeNormal = vec3.UP;
	private WorldIntersection intersection = null;

	private ivec2 savedMousePos = ivec2.ZERO;
	private Input.MOUSE_HANDLE init_mouse_handle;

	private void Init()
	{
		camera = node as PlayerDummy;
		if (!camera)
			return;

		init_mouse_handle = Input.MouseHandle;
		Input.MouseHandle = Input.MOUSE_HANDLE.USER;

		intersection = new WorldIntersection();

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
		if (!camera)
			return;

		if (Input.IsMouseButtonDown(Input.MOUSE_BUTTON.LEFT))
		{
			// try to get target object
			ivec2 mouse_coord = Input.MousePosition;
			camera.GetDirectionFromMainWindow(out var p0, out var p1, mouse_coord.x, mouse_coord.y);
			Node obj = World.GetIntersection(p0, p1, 0xFFFFFF, intersection);

			grabbingPoint = null;
			if (obj != null)
			{
				// save for camera movement
				grabbingPoint = intersection.Point;
				grabbingCameraPos = camera.WorldPosition;
				planeNormal = camera.ViewDirection;
			}
		}

		if (Input.IsMouseButtonDown(Input.MOUSE_BUTTON.RIGHT))
		{
			// save mouse position and start rotate mode
			savedMousePos = Input.MousePosition;
			Input.MouseHandle = Input.MOUSE_HANDLE.GRAB;
			Input.MouseGrab = true;
		}

		if (Input.IsMouseButtonPressed(Input.MOUSE_BUTTON.LEFT))
		{
			if (grabbingPoint != null)
			{
				// move camera based on target object plane
				ivec2 mouse_coord = Input.MousePosition;
				camera.GetDirectionFromMainWindow(out var p0, out var p1, mouse_coord.x, mouse_coord.y);
				MathLib.LinePlaneIntersection(new vec3(p0), new vec3(p1), new vec3(grabbingPoint.Value), new vec3(planeNormal), out vec3 currentPoint);
				camera.WorldTranslate(grabbingPoint.Value - currentPoint);
			}
			else
				camera.Translate(new Vec3(-Input.MouseDeltaPosition.x * defaultLinearSpeed, Input.MouseDeltaPosition.y * defaultLinearSpeed, 0));
		}
		else if (Input.IsMouseButtonPressed(Input.MOUSE_BUTTON.RIGHT))
		{
			// update vertical and horizontal angles
			verticalAngle += Input.MouseDeltaPosition.y * mouseSensitivity;
			verticalAngle = MathLib.Clamp(verticalAngle, -89.9f, 89.9f);

			horizontalAngle += Input.MouseDeltaPosition.x * mouseSensitivity;
			if (horizontalAngle < -180 || 180 < horizontalAngle)
				horizontalAngle = MathLib.Clamp(-horizontalAngle, -180.0f, 180.0f);

			// calculate new camera direction
			vec3 cameraDirection = vec3.FORWARD * MathLib.RotateZ(horizontalAngle);
			cameraDirection = cameraDirection * MathLib.Rotate(MathLib.Cross(cameraDirection, vec3.UP), verticalAngle);

			// set new direction of camera
			camera.SetWorldDirection(cameraDirection, vec3.UP);
		}

		if (Input.IsMouseButtonUp(Input.MOUSE_BUTTON.RIGHT))
		{
			// stop rotate mode
			Input.MouseHandle = Input.MOUSE_HANDLE.USER;
			Input.MouseGrab = false;
			Input.MousePosition = savedMousePos;
		}

		// zooming
		if (Input.MouseWheel != 0)
			camera.Translate(new Vec3(0, 0, -Input.MouseWheel * mouseWheelSensitivity));
	}

	private void Shutdown()
	{
		Input.MouseHandle = init_mouse_handle;
	}
}
