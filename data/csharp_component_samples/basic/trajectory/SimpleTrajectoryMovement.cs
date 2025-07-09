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
using System;

[Component(PropertyGuid = "e54b808ee042217251a38fc2f09ce89441d44429")]
public class SimpleTrajectoryMovement : Component
{
	[ShowInEditor]
	private Node pathNode = null;
	[ShowInEditor]
	private float velocity = 10.0f;
	public float Velocity { get { return velocity; } set { velocity = value; } }
	[ShowInEditor]
	private bool debug;
	public bool Debug { get { return debug; } set { debug = value; } }

	private List<Vec3> pointsPos = new List<Vec3>();
	private List<quat> pointsRot = new List<quat>();
	private Vec3 prevPoint = Vec3.ZERO;
	private quat prevRot = quat.IDENTITY;
	private int pointsIndex = 0;
	private float time = 0.0f;

	void Init()
	{
		int num_childs = pathNode.NumChildren;
		for (int i = 0; i < num_childs; i++)
		{
			Node nc = pathNode.GetChild(i);
			pointsPos.Add(nc.WorldPosition);
			pointsRot.Add(nc.GetWorldRotation());
		}
		prevPoint = node.WorldPosition;
	}
	
	void Update()
	{
		UpdateTime();

		node.WorldPosition = MathLib.Lerp(prevPoint, pointsPos[pointsIndex], time);
		node.SetWorldRotation(MathLib.Slerp(prevRot, pointsRot[pointsIndex], time));

		if (debug)
			VisualizePath();
	}

	private void VisualizePath()
	{
		for (int i = 0; i < pointsPos.Count; i++)
		{
			int next = (i + 1) % pointsPos.Count;
			Visualizer.RenderLine3D(pointsPos[i], pointsPos[next], vec4.WHITE);
		}
	}

	private void UpdateTime()
	{
		float len = (float)MathLib.Length(pointsPos[pointsIndex] - prevPoint);

		// update time with stable velocity
		time += velocity / len * Game.IFps;
		if (time >= 1.0f)
		{
			prevPoint = pointsPos[pointsIndex];
			prevRot = pointsRot[pointsIndex];

			// next point
			pointsIndex = (pointsIndex + Convert.ToInt32(time)) % pointsPos.Count;
			time = MathLib.Frac(time);
		}
	}
}
