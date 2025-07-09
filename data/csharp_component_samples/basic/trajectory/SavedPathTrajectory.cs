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

[Component(PropertyGuid = "b9943918b39397a29c8751ec6b1d21fb4398cc4b")]
public class SavedPathTrajectory : Component
{
	[ShowInEditor][ParameterFile]
	private string trajectoryFilePath = null;
	[ShowInEditor]
	private float velocity = 10.0f;
	public float Velocity { get { return velocity; } set { velocity = value; } }
	[ShowInEditor]
	private bool debug;
	public bool Debug { get { return debug; } set { debug = value; } }

	private WorldTransformPath transformPath;

	void Init()
	{
		node.WorldPosition = new Vec3(0, 0, 0);

		transformPath = new WorldTransformPath(trajectoryFilePath);
		transformPath.Loop = 1;
		transformPath.Time = 0.0f;
		transformPath.Speed = velocity;
		transformPath.Play();
		transformPath.AddChild(node);
	}
	
	void Update()
	{
		transformPath.Speed = velocity;

		if (debug)
			VisualizePath();
	}
	private void VisualizePath()
	{
		Path path = transformPath.Path;
		int num_frames = path.NumFrames;
		for (int i = 0; i < num_frames; i++)
		{
			Vec3 curr_point = path.GetFramePosition(i);
			Vec3 next_point = path.GetFramePosition((i + 1) % num_frames);

			Visualizer.RenderLine3D(curr_point, next_point, vec4.WHITE);
		}
	}
}
