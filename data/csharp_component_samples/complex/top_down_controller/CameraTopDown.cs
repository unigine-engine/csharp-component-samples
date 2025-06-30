using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unigine;
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


[Component(PropertyGuid = "a965eb3d6b4b3378ac7a60772cb4436ae9cc9bc1")]
public class CameraTopDown : Component
{
	public float phi = 180.0f;
	public float theta = 40.0f;
	public vec2 thetaMinMax = new vec2(10.0f, 70.0f);

	public float distance = 25.0f;
	public vec2 distanceMinMax = new vec2(5.0f, 45.0f);

	public float zoomSpeed = 5.0f;

	private Player camera;
	private WorldIntersection intersection = new WorldIntersection();
	private vec3 previousMouseToIntersectionPointVector;
	private vec3 currentMouseToIntersectionPointVector;
	private bool isPreviousHooked;

	private float currentPhi = 180.0f;
	private float targetPhi = 180.0f;

	private float currentTheta = 40.0f;
	private float targetTheta = 40.0f;
	private float maxTheta = 70.0f;
	private float minTheta = 10.0f;

	private Scalar currentDistance = 25.0f;
	private Scalar targetDistance = 25.0f;
	private Scalar maxDistance = 45.0f;
	private Scalar minDistance = 5.0f;

	private float interpolationFactor = 1.0f;

	private Vec3 currentCameraPivotPosition;
	private Vec3 targetCameraPivotPosition;

	private float degreesPerUnit = 1.0f;

	private Input.MOUSE_HANDLE init_mouse_handle;

	CameraSelection selection;

	private void Init()
	{
		targetPhi = phi;
		currentPhi = targetPhi;
		targetTheta = theta;
		currentTheta = targetTheta;
		minTheta = thetaMinMax.x;
		maxTheta = thetaMinMax.y;

		targetDistance = distance;
		currentDistance = targetDistance;
		minDistance = distanceMinMax.x;
		maxDistance = distanceMinMax.y;

		interpolationFactor = zoomSpeed;

		if (maxDistance != minDistance)
			degreesPerUnit =  (maxTheta - minTheta) / (float)(maxDistance - minDistance);
		else
			degreesPerUnit = 0.0f;

		camera = node as Player;
		if(camera == null)
		{
			Log.Error("CameraTopDown::init(): camera is not valid\n");
			return;
		}

		targetCameraPivotPosition = node.WorldPosition;
		targetCameraPivotPosition.z = 2.0f;
		currentCameraPivotPosition = targetCameraPivotPosition;

		init_mouse_handle = Input.MouseHandle;
		Input.MouseHandle = Input.MOUSE_HANDLE.USER;

		vec3 cameraViewDirection = new quat(vec3.UP, currentPhi) * vec3.FORWARD;
		cameraViewDirection = new quat(MathLib.Cross(vec3.UP, cameraViewDirection), -currentTheta) * cameraViewDirection * -1;
		cameraViewDirection.Normalize();

		camera.ViewDirection = cameraViewDirection;
		camera.WorldPosition = currentCameraPivotPosition - cameraViewDirection * currentDistance;

		NodeDummy logic = new NodeDummy();
		selection = ComponentSystem.AddComponent<CameraSelection>(logic);
	}
	
	private void Update()
	{
		if (!camera)
			return;

		if(!Unigine.Console.Active)
		{
			if(Input.IsMouseButtonDown(Input.MOUSE_BUTTON.MIDDLE))
			{
				ivec2 mouse = Input.MousePosition;
				vec3 rayDir = camera.GetDirectionFromMainWindow(mouse.x, mouse.y);
				Unigine.Object obj = World.GetIntersection(camera.WorldPosition, camera.WorldPosition + rayDir * 10000, ~0, intersection);
				if (obj != null)
				{
					isPreviousHooked = true;
					previousMouseToIntersectionPointVector = new vec3(intersection.Point - camera.WorldPosition);
				}
				else
				{
					isPreviousHooked = false;
				}
			}

			int mouseAxis = Input.MouseWheel;
			if(mouseAxis != 0)
			{
				targetDistance = MathLib.Clamp(targetDistance - mouseAxis, minDistance, maxDistance);
				targetTheta = MathLib.Clamp(targetTheta - mouseAxis, minTheta, maxTheta);
			}

			if(Input.IsKeyPressed(Input.KEY.Q))
			{
				targetPhi -= 50.0f * Game.IFps;
			}
			if (Input.IsKeyPressed(Input.KEY.E))
			{
				targetPhi += 50.0f * Game.IFps;
			}

			if(Input.IsKeyPressed(Input.KEY.F) && selection.Selection)
			{
				targetDistance = selection.BoundRadius;
				targetCameraPivotPosition = selection.Center;
				targetCameraPivotPosition.z = 2.0f;

				targetDistance = MathLib.Clamp(targetDistance, minDistance, maxDistance);
				targetTheta = MathLib.Clamp(minTheta + (float)(targetDistance - minDistance) * degreesPerUnit, minTheta, maxTheta);
			}

			if(Input.IsMouseButtonPressed(Input.MOUSE_BUTTON.MIDDLE) && isPreviousHooked)
			{
				ivec2 mouse = Input.MousePosition;
				currentMouseToIntersectionPointVector = camera.GetDirectionFromMainWindow(mouse.x, mouse.y);
				currentMouseToIntersectionPointVector *= previousMouseToIntersectionPointVector.z / currentMouseToIntersectionPointVector.z;

				vec3 displacement = currentMouseToIntersectionPointVector - previousMouseToIntersectionPointVector;

				targetCameraPivotPosition -= displacement;
				currentCameraPivotPosition -= displacement;
				previousMouseToIntersectionPointVector = currentMouseToIntersectionPointVector;
			}
			else
			{
				vec3 forward = new quat(vec3.UP, targetPhi) * vec3.FORWARD * -1;
				forward.Normalize();
				vec3 right = new quat(vec3.UP, targetPhi) * vec3.RIGHT * -1;
				right.Normalize();
				ivec2 mouse = Input.MousePosition;
				ivec2 windowPos = WindowManager.MainWindow.Position;
				ivec2 windowSize = WindowManager.MainWindow.RenderSize;

				if (mouse.x < windowPos.x + 10)
					targetCameraPivotPosition -= right * 10.0f * Game.IFps;

				if (mouse.y < windowPos.y + 10)
					targetCameraPivotPosition += forward * 10.0f * Game.IFps;

				if (mouse.x > windowPos.x + windowSize.x - 10)
					targetCameraPivotPosition += right * 10.0f * Game.IFps;

				if (mouse.y > windowPos.y + windowSize.y - 10)
					targetCameraPivotPosition -= forward * 10.0f * Game.IFps;

			}
		}

		currentPhi = MathLib.Lerp(currentPhi, targetPhi, interpolationFactor * Game.IFps);
		currentTheta = MathLib.Lerp(currentTheta, targetTheta, interpolationFactor * Game.IFps);
		currentDistance = MathLib.Lerp(currentDistance, targetDistance, interpolationFactor * Game.IFps);
		currentCameraPivotPosition = MathLib.Lerp(currentCameraPivotPosition, targetCameraPivotPosition, interpolationFactor * Game.IFps);
	}

	private void PostUpdate()
	{
		vec3 cameraViewDirection = new quat(vec3.UP, currentPhi) * vec3.FORWARD;
		cameraViewDirection = new quat(MathLib.Cross(vec3.UP, cameraViewDirection), -currentTheta) * cameraViewDirection * -1;
		cameraViewDirection.Normalize();

		camera.ViewDirection = cameraViewDirection;
		camera.WorldPosition = currentCameraPivotPosition - cameraViewDirection * currentDistance;
	}

	private void Shutdown()
	{
		Input.MouseHandle = init_mouse_handle;
	}

	public void SetPosition(vec3 pos)
	{
		targetCameraPivotPosition = pos;
		targetCameraPivotPosition.z = 2.0f;
		currentCameraPivotPosition = targetCameraPivotPosition;
	}

	public void SetTargetPosition(vec3 pos)
	{
		targetCameraPivotPosition = pos;
		targetCameraPivotPosition.z = 2.0f;
	}

	public void SetDistance(float dist)
	{
		targetDistance = dist;

		targetDistance = MathLib.Clamp(targetDistance, minDistance, maxDistance);
		targetTheta = MathLib.Clamp(minTheta + (float)(targetDistance - minDistance) * degreesPerUnit, minTheta, maxTheta);

		currentDistance = targetDistance;
		currentTheta = targetTheta;
	}

	public void SetTargetDistance(float dist)
	{
		targetDistance = dist;

		targetDistance = MathLib.Clamp(targetDistance, minDistance, maxDistance);
		targetTheta = MathLib.Clamp(minTheta + (float)(targetDistance - minDistance) * degreesPerUnit, minTheta, maxTheta);
	}


}
