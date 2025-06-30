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

[Component(PropertyGuid = "126bc4774bce576ad6179fc11f21f62c6389fb9b")]
public class BoundSphereIntersection : Component
{
	public dvec3 center = dvec3.ZERO;
	public float radius = 1.0f;

	private WorldBoundSphere boundSphere;
	private List<Node> nodes = null;
	private SampleDescriptionWindow sampleDescriptionWindow;

	private void Init()
	{
		// create bound sphere based on center and radius in world coordinates
		boundSphere = new WorldBoundSphere(new Vec3(center), radius);

		// create collection for intersecting nodes
		nodes = new List<Node>();

		Visualizer.Enabled = true;
		sampleDescriptionWindow = new SampleDescriptionWindow();
		sampleDescriptionWindow.createWindow();
	}

	private void Update()
	{
		// show bound sphere
		Visualizer.RenderBoundSphere(new BoundSphere(new vec3(boundSphere.Center), (float)boundSphere.Radius), Mat4.IDENTITY, new vec4(0.0f, 1.0f, 0.0f, 1.0f));

		// try get nodes inside bound sphere
		var status = "Inside bound sphere:";
		bool res = World.GetIntersection(boundSphere, Node.TYPE.OBJECT_MESH_STATIC, nodes);
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
