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

[Component(PropertyGuid = "450a58bb1478b012e23ce0771dc58a21add5b15e")]
public class PathTrajectorySaver : Component
{
	[ShowInEditor][ParameterFile]
	private string pathFile = null;
	[ShowInEditor]
	private Node pathNode = null;
	[ShowInEditor]
	private int quality = 25;
	[ShowInEditor]
	private bool autosave = true;

	void Init()
	{
		if (autosave)
			Save();
	}
	
	private void Save()
	{
		Path path = new Path();
		path.Clear();

		int points_count = pathNode.NumChildren;
		double frame_time = 0;
		for (int j = 0; j < points_count; j++)
		{
			int j_prev = (j - 1 < 0) ? (points_count - 1) : j - 1;
			int j_cur = j;
			int j_next = (j + 1) % points_count;
			int j_next_next = (j + 2) % points_count;

			// get current control position and rotation
			Vec3 p0 = pathNode.GetChild(j_prev).WorldPosition;
			Vec3 p1 = pathNode.GetChild(j_cur).WorldPosition;
			Vec3 p2 = pathNode.GetChild(j_next).WorldPosition;
			Vec3 p3 = pathNode.GetChild(j_next_next).WorldPosition;

			quat q0 = pathNode.GetChild(j_prev).GetWorldRotation();
			quat q1 = pathNode.GetChild(j_cur).GetWorldRotation();
			quat q2 = pathNode.GetChild(j_next).GetWorldRotation();
			quat q3 = pathNode.GetChild(j_next_next).GetWorldRotation();

			// calculate curve
			Vec3 start = Utils.CatmullRomCentripetal(p0, p1, p2, p3, 0);
			for (int i = 1; i < quality; i++)
			{
				path.AddFrame();

				float time = (float)i / (quality - 1);

				// calculate segment position and rotation
				Vec3 end = Utils.CatmullRomCentripetal(p0, p1, p2, p3, time);
				quat rot = Utils.Squad(q0, q1, q2, q3, time);

				// calculate current frame time
				double len = MathLib.Length(start - end);
				frame_time += len;

				path.SetFramePosition(path.NumFrames - 1, end);
				path.SetFrameRotation(path.NumFrames - 1, rot);
				path.SetFrameTime(path.NumFrames - 1, (float)frame_time);

				start = end;
			}
		}

		// save to file
		path.Save(pathFile);
	}
}
