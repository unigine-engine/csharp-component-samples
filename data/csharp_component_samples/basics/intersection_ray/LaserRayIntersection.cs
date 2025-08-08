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

[Component(PropertyGuid = "d86ba149dc4a3a14ab4bde63e18fb0662af98c8d")]
public class LaserRayIntersection : Component
{
	public Node laserRay = null;
	public Node laserHit = null;
	public float laserDistance = 25.0f;

	// use mask to separate objects for intersection
	[ParameterMask(MaskType = ParameterMaskAttribute.TYPE.INTERSECTION)]
	public int mask = 1;

	private WorldIntersectionNormal intersection = null;
	private vec3 laserRayScale = vec3.ONE;
	private SampleDescriptionWindow sampleDescriptionWindow;

	private void Init()
	{
		sampleDescriptionWindow = new SampleDescriptionWindow();
		sampleDescriptionWindow.createWindow();
		// check parts of laser
		if (!laserRay || !laserHit)
			return;

		// create an intersection object to obtain the necessary information
		// about the intersection result
		intersection = new WorldIntersectionNormal();

		// save the source laser ray scale for changing length after intersection
		laserRayScale = laserRay.WorldScale;
	}
	
	private void Update()
	{
		// check parts of laser
		if (!laserRay || !laserHit)
			return;

		// get points to detect intersection based on the direction of the laser ray
		Vec3 firstPoint = laserRay.WorldPosition;
		Vec3 secondPoint = firstPoint + laserRay.GetWorldDirection(MathLib.AXIS.Y) * laserDistance;

		var status = "Hit Object:";
		// try to get intersection object
		Unigine.Object hitObject = World.GetIntersection(firstPoint, secondPoint, mask, intersection);
		if (hitObject)
		{
			// show object name
			status += $" {hitObject.Name}";

			// set current laser ray length
			float length = (float)(intersection.Point - laserRay.WorldPosition).Length;
			laserRayScale.y = length;
			laserRay.WorldScale = laserRayScale;

			// activate laserHit if it was hidden earlier
			if (!laserHit.Enabled)
				laserHit.Enabled = true;

			// update laser hit transform based on intersection information
			laserHit.WorldPosition = intersection.Point;
			laserHit.SetWorldDirection(intersection.Normal, vec3.UP, MathLib.AXIS.Y);
		}
		else
		{
			status += " none";
			// set default ray length
			laserRayScale.y = laserDistance;
			laserRay.WorldScale = laserRayScale;

			// hide hit point
			laserHit.Enabled = false;
		}
		sampleDescriptionWindow.setStatus(status);
	}

	private void Shutdown()
	{
		sampleDescriptionWindow.shutdown();
	}
}
