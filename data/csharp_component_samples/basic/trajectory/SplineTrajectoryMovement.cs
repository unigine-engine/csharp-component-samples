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
using System.Linq;
using static Unigine.Animations;

[Component(PropertyGuid = "24bbd2ad0b15daff35490849acddb59617ca6fe8")]
public class SplineTrajectoryMovement : Component
{
	[ShowInEditor]
	private Node pathNode = null;
	[ShowInEditor]
	private float velocity = 10.0f;
	public float Velocity { get { return velocity; } set { velocity = value; } }
	[ShowInEditor]
	private int quality = 25;
	[ShowInEditor]
	private bool debug;
	public bool Debug { get { return debug; } set { debug = value; } }

	private List<List<float>> lengths = new List<List<float>>();
	private List<Vec3> pointsPos = new List<Vec3>();
	private List<quat> pointsRot = new List<quat>();
	private int pointsIndex = 0;
	private float time = 0.0f;

	void Init()
	{
		// save positions and rotations
		int numChilds = pathNode.NumChildren;
		for (int i = 0; i < numChilds; i++)
		{
			Node nc = pathNode.GetChild(i);
			pointsPos.Add(nc.WorldPosition);
			pointsRot.Add(nc.GetWorldRotation());
		}

		int pointsCount = pointsPos.Count;
		for (int j = 0; j < pointsCount; j++)
		{
			int j_prev = (j - 1 < 0) ? (pointsCount - 1) : j - 1;
			int j_cur = j;
			int j_next = (j + 1) % pointsCount;
			int j_next_next = (j + 2) % pointsCount;

			Vec3 p0 = pointsPos[j_prev];
			Vec3 p1 = pointsPos[j_cur];
			Vec3 p2 = pointsPos[j_next];
			Vec3 p3 = pointsPos[j_next_next];

			lengths.Add(Utils.GetLengthCatmullRomCentripetal(p0, p1, p2, p3, quality));
		}
	}

	void Update()
	{
		float speed = velocity / (lengths[pointsIndex][(int)(time * (quality - 1))] * quality);
		UpdateTime(speed);

		Vec3[] p = GetCurrentPoints();
		quat[] q = GetCurrentQuats();

		Vec3 pos = Utils.CatmullRomCentripetal(p[0], p[1], p[2], p[3], time);
		quat rot = Utils.Squad(q[0], q[1], q[2], q[3], time);

		node.WorldPosition = pos;
		node.SetWorldRotation(rot, true);

		if (debug)
			VisualizePath();
	}

	private void VisualizePath()
	{
		int points_count = pointsPos.Count;
		for (int j = 0; j < points_count; j++)
		{
			int j_prev = (j - 1 < 0) ? (points_count - 1) : j - 1;
			int j_cur = j;
			int j_next = (j + 1) % points_count;
			int j_next_next = (j + 2) % points_count;

			Vec3 p0 = pointsPos[j_prev];
			Vec3 p1 = pointsPos[j_cur];
			Vec3 p2 = pointsPos[j_next];
			Vec3 p3 = pointsPos[j_next_next];

			// draw curve
			Vec3 start = Utils.CatmullRomCentripetal(p0, p1, p2, p3, 0);
			int quality = 10;
			for (int i = 1; i < quality; i++)
			{
				Vec3 end = Utils.CatmullRomCentripetal(p0, p1, p2, p3, (float)i / (quality - 1));
				Visualizer.RenderLine3D(start, end, vec4.WHITE);
				start = end;
			}
		}
	}

	private void UpdateTime(float speed)
	{
		time += speed * Game.IFps;
		if (time >= 1.0f)
		{
			pointsIndex = (pointsIndex + (int)time) % pointsPos.Count; // loop
			time = MathLib.Frac(time);
		}
	}

	private Vec3[] GetCurrentPoints()
	{
		int points_count = pointsPos.Count;
		int i_prev = (pointsIndex - 1 < 0) ? (points_count - 1) : pointsIndex - 1;
		int i_cur = pointsIndex;
		int i_next = (pointsIndex + 1) % points_count;
		int i_next_next = (pointsIndex + 2) % points_count;

		Vec3[] result = new Vec3[4];
		result[0] = pointsPos[i_prev];
		result[1] = pointsPos[i_cur];
		result[2] = pointsPos[i_next];
		result[3] = pointsPos[i_next_next];
		return result;
	}

	private quat[] GetCurrentQuats()
	{
		int points_count = pointsPos.Count;
		int i_prev = (pointsIndex - 1 < 0) ? (points_count - 1) : pointsIndex - 1;
		int i_cur = pointsIndex;
		int i_next = (pointsIndex + 1) % points_count;
		int i_next_next = (pointsIndex + 2) % points_count;

		quat[] result = new quat[4];
		result[0] = pointsRot[i_prev];
		result[1] = pointsRot[i_cur];
		result[2] = pointsRot[i_next];
		result[3] = pointsRot[i_next_next];
		return result;
	}
}
