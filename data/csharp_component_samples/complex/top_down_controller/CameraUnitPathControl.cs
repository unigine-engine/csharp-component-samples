using System;
using System.Collections;
using System.Collections.Generic;
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

[Component(PropertyGuid = "ca65a6e04ff5acbf0d63d3124d5bd749c800f814")]
public class CameraUnitPathControl : Component
{
	public float speed = 13;
	public float torque = 1;
	public Node pathNode;
	public float epsD = 0.1f;

	private List<Mat4> path = new List<Mat4>(0);
	private int currentPathIndex = 0;
	private int dir = 1;
	private float rotAcceleration;

	private void Init()
	{
		float minDistance = 1e+9f;
		if (pathNode != null)
		{
			for (int i = 0; i < pathNode.NumChildren; i++)
			{
				var p = pathNode.GetChild(i).Transform;
				float distance = (float)MathLib.Length2(node.WorldPosition - p.Translate);
				if (distance < minDistance)
				{
					currentPathIndex = i;
					minDistance = distance;
				}
				path.Add(p);
			}
		}
	}
	
	private void Update()
	{
		if (path.Count == 0)
			return;

		var currentPosition = node.WorldPosition;
		var currentRot = node.GetWorldRotation();
		var route = path[currentPathIndex];

		float ifps = Game.IFps;
		if (ifps == 0)
			return;

		vec3 ddir = new vec3(route.Translate - currentPosition);
		ddir.Normalize();


		float d = MathLib.Dot(currentRot.Mat3.AxisX, ddir);
		if (MathLib.IsNaN(d))
			d = 0;
		rotAcceleration += (d > rotAcceleration ? ifps : -ifps);

		currentRot = currentRot * new quat(vec3.UP, torque * ifps * -rotAcceleration);
		currentPosition = currentPosition + new vec3(currentRot.Mat3.AxisY * ifps * speed);

		node.WorldPosition = currentPosition;
		node.SetWorldRotation(currentRot);

		float distanceToTarget = (float)MathLib.Length(route.Translate - currentPosition);
		if(distanceToTarget < epsD)
		{
			currentPathIndex += dir;

			if (currentPathIndex == path.Count)
				currentPathIndex = 0;

			if(currentPathIndex == -1)
				currentPathIndex = path.Count - 1;
		}
	}
}
