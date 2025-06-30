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

using System.Collections.Generic;
using Unigine;
using static Unigine.VREyeTracking;

[Component(PropertyGuid = "b1b8facb749391ae2df6ad35c525dd10ebff8d2f")]
public class BoundBoxIntersection : Component
{
	public dvec3 minPoint = dvec3.ZERO;
	public dvec3 maxPoint = dvec3.ZERO;

	private WorldBoundBox boundBox;
	private List<Node> nodes = null;
	private SampleDescriptionWindow sampleDescriptionWindow = new SampleDescriptionWindow();

	private void Init()
	{
		// create bound box based on minimum and maximum world coordinates
		boundBox = new WorldBoundBox(new Vec3(minPoint), new Vec3(maxPoint));

		// create collection for intersecting nodes
		nodes = new List<Node>();

		Visualizer.Enabled = true;
		sampleDescriptionWindow.createWindow();
	}
	
	private void Update()
	{
		// show bound box
		Visualizer.RenderBoundBox(new BoundBox(new vec3(boundBox.minimum), new vec3(boundBox.maximum)), Mat4.IDENTITY, new vec4(0.0f, 1.0f, 0.0f, 1.0f));

		// try get nodes inside bound box
		var status = "Inside bound box:";
		bool res = World.GetIntersection(boundBox, Node.TYPE.OBJECT_MESH_STATIC, nodes);
		if (res)
		{
			// show nodes names
			foreach (Node n in nodes)
				status += $" {n.Name}";
		}
		else
			status += " empty";

		sampleDescriptionWindow.setStatus(status);
	}

	private void Shutdown()
	{
		Visualizer.Enabled = false;
		sampleDescriptionWindow.shutdown();
	}
}
