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

[Component(PropertyGuid = "3be055ec0d1b585b6ce9b1fe43063145eb1df1e0")]
public class BoundFrustumIntersection : Component
{
	public float fov = 100.0f;
	public float aspect = 1.0f;
	public float zNear = 0.001f;
	public float zFar = 10.0f;
	public vec3 position = vec3.ZERO;
	public vec3 rotation = vec3.ZERO;

	private mat4 frustumTransform = mat4.IDENTITY;
	private WorldBoundFrustum boundFrustum;
	private List<Node> nodes = null;
	private SampleDescriptionWindow sampleDescriptionWindow;

	private void Init()
	{
		// create frustum transform based on fov, aspect, znear and zfar in world coordinates
		frustumTransform = MathLib.Perspective(fov, aspect, zNear, zFar);
		frustumTransform = frustumTransform * MathLib.RotateX(-90);
		frustumTransform = frustumTransform * MathLib.Translate(-position);
		frustumTransform = frustumTransform * MathLib.Rotate(new quat(rotation.x, rotation.y, rotation.z));

		// create bound frustum
		boundFrustum = new WorldBoundFrustum(frustumTransform, Mat4.IDENTITY);

		// create collection for intersecting nodes
		nodes = new List<Node>();

		Visualizer.Enabled = true;
		sampleDescriptionWindow = new SampleDescriptionWindow();
		sampleDescriptionWindow.createWindow();
	}

	private void Update()
	{
		// show bound frustum
		Visualizer.RenderFrustum(frustumTransform, Mat4.IDENTITY, new vec4(0.0f, 1.0f, 0.0f, 1.0f));

		var status = "Inside bound frustum:";
		// try get nodes inside bound frustum
		bool res = World.GetIntersection(boundFrustum, Node.TYPE.OBJECT_MESH_STATIC, nodes);
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
