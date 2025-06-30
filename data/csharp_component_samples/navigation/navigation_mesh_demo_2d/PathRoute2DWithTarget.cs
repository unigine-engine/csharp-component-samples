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

[Component(PropertyGuid = "3fee417fcf07e0d2d7eb0a09d43f0bd44a0d0523")]
public class PathRoute2DWithTarget : Component
{
	public float speed = 4.0f;
	public NavigationMesh navigationMesh = null;

	[ParameterFile(Filter = ".node")]
	public string targetReferencePath = "";

	public bool visualizeRoute = false;

	[ParameterColor]
	public vec4 routeColor = vec4.ZERO;

	private PathRoute route = null;
	private Node target = null;

	private void Init()
	{
		if (!navigationMesh)
			return;

		// create target
		target = World.LoadNode(targetReferencePath);
		if (target)
		{
			// set random position in navigation mesh
			target.WorldPosition = new Vec3(Game.GetRandomFloat(-55.0f, 55.0f), Game.GetRandomFloat(-55.0f, 55.0f), 0.5f);

			// save target position in navigation mesh
			while (!navigationMesh.Inside2D(target.WorldPosition, 0.5f))
				target.WorldPosition = new Vec3(Game.GetRandomFloat(-55.0f, 55.0f), Game.GetRandomFloat(-55.0f, 55.0f), 0.5f);

			// create route to path calculation
			route = new PathRoute();

			// set point radius inside navigation mesh
			route.Radius = 0.5f;

			// does not use negative speed
			speed = MathLib.Max(0.0f, speed);

			// enabled for visualization
			if (visualizeRoute)
				Visualizer.Enabled = true;
		}
	}
	
	private void Update()
	{
		if (!navigationMesh || !target)
			return;

		// change position of target if it is near to current node
		if ((target.WorldPosition - node.WorldPosition).Length < 1.0f)
		{
			target.WorldPosition = new Vec3(Game.GetRandomFloat(-55.0f, 55.0f), Game.GetRandomFloat(-55.0f, 55.0f), 0.5f);

			// save target position in navigation mesh
			while (!navigationMesh.Inside2D(target.WorldPosition, 0.5f))
				target.WorldPosition = new Vec3(Game.GetRandomFloat(-55.0f, 55.0f), Game.GetRandomFloat(-55.0f, 55.0f), 0.5f);
		}

		// if current path is ready, try to move node
		if (route.IsReady)
		{
			// in successful case change direction and position
			if (route.IsReached)
			{
				// enable target from inside obstacle
				if (!target.Enabled)
					target.Enabled = true;

				// get new direction for node
				vec3 direction = new vec3(route.GetPoint(1) - route.GetPoint(0));
				if (direction.Length2 > MathLib.EPSILON)
				{
					// get rotation target based on new direction
					quat targetRotation = new quat(MathLib.SetTo(vec3.ZERO, direction.Normalized, vec3.UP, MathLib.AXIS.Y));

					// smoothly change rotation
					quat currentRotation = MathLib.Slerp(node.GetWorldRotation(), targetRotation, Game.IFps * 8.0f);
					node.SetWorldRotation(currentRotation);

					// translate in forward direction and try to save node position in navigation mesh
					Vec3 lastValidPosition = node.WorldPosition;
					node.Translate(Vec3.FORWARD * Game.IFps * speed);

					// restore last position if node is outside navigation mesh
					if (!navigationMesh.Inside2D(node.WorldPosition, route.Radius))
						node.WorldPosition = lastValidPosition;
				}

				// render current path
				if (visualizeRoute)
					route.RenderVisualizer(routeColor);

				// try to create new path
				route.Create2D(node.WorldPosition + vec3.UP * 0.5f, target.WorldPosition, 1);
			}
			else
			{
				// hide target and change position, because it can be in obstacle
				target.Enabled = false;
				target.WorldPosition = new Vec3(Game.GetRandomFloat(-55.0f, 55.0f), Game.GetRandomFloat(-55.0f, 55.0f), 0.5f);

				// save target position in navigation mesh
				while (!navigationMesh.Inside2D(target.WorldPosition, 0.5f))
					target.WorldPosition = new Vec3(Game.GetRandomFloat(-55.0f, 55.0f), Game.GetRandomFloat(-55.0f, 55.0f), 0.5f);

				// try to create new path
				route.Create2D(node.WorldPosition + vec3.UP * 0.5f, target.WorldPosition, 1);
			}
		}
		// try to create new path
		else if (!route.IsQueued)
			route.Create2D(node.WorldPosition + vec3.UP * 0.5f, target.WorldPosition, 1);
	}

	private void Shutdown()
	{
		Visualizer.Enabled = false;
	}
}
