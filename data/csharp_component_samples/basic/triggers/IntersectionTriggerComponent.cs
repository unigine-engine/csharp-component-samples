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
using System.Collections.Generic;

[Component(PropertyGuid = "6bd4a7db685ab8df907188cb5ecf4bcf1054599f")]
public class IntersectionTriggerComponent : Component
{
	[ShowInEditor]
	private float boundSphereSize = 5.0f;
	[ShowInEditor]
	private float boundBoxSize = 5.0f;
	[ShowInEditor]
	private bool isSphere = false;
	[ShowInEditor]
	private bool debug = false;
	[ShowInEditor][ParameterMask]
	private int materialBallIntersectionMask = 0x7;
	public int MaterialBallIntersectionMask { get { return materialBallIntersectionMask; } }

	private List<Node> entered = new List<Node>();
	private List<Node> inside = new List<Node>();

	private WorldBoundBox boundBox;
	private WorldBoundSphere boundSphere;

	private EventInvoker<Node> eventEnter = new EventInvoker<Node>();
	public Event<Node> EventEnter { get { return eventEnter; } }

	private EventInvoker<Node> eventLeave = new EventInvoker<Node>();
	public Event<Node> EventLeave { get { return eventLeave; } }

	void Init()
	{
		Vec3 translation = node.WorldTransform.Translate;
		Vec3 coordinatesForBoundBoxMin = new Vec3(-boundBoxSize * 0.5f) + translation;
		Vec3 coordinatesForBoundBoxMax = new Vec3(boundBoxSize * 0.5f) + translation;
		boundBox.Set(new vec3(coordinatesForBoundBoxMin), new vec3(coordinatesForBoundBoxMax));
		boundSphere.Set(new vec3(translation), boundSphereSize * 0.5f);
	}
	
	void Update()
	{
		ReplaceBounds();

		if(debug)
		{
			VisualizeBounds();
		}

		GetInsideNodes();
		CheckEnter();
		CheckLeave();
	}

	private void CheckEnter()
	{
		foreach(Node obj in inside)
		{
			if(!entered.Contains(obj))
			{
				entered.Add(obj);
				eventEnter.Run(obj);
			}
		}
	}

	private void CheckLeave()
	{
		List<Node> toRemove = new List<Node>();

		foreach (Node obj in entered)
		{
			if (obj.IsDeleted)
				toRemove.Add(obj);

			if (!inside.Contains(obj))
			{
				toRemove.Add(obj);
				eventLeave.Run(obj);
			}
		}

		foreach(Node obj in toRemove)
		{
			entered.Remove(obj);
		}
	}

	private void ReplaceBounds()
	{
		Vec3 translation = node.WorldTransform.Translate;
		Vec3 coordinatesForBoundBoxMin = new Vec3(-boundBoxSize * 0.5f) + translation;
		Vec3 coordinatesForBoundBoxMax = new Vec3(boundBoxSize * 0.5f) + translation;
		boundBox.Set(new vec3(coordinatesForBoundBoxMin), new vec3(coordinatesForBoundBoxMax));
		boundSphere.Set(new vec3(translation), boundSphereSize * 0.5f);
	}

	private void VisualizeBounds()
	{
		if (isSphere)
			Visualizer.RenderSphere(boundSphereSize * 0.5f, node.WorldTransform, vec4.RED);
		else
			Visualizer.RenderBoundBox(new BoundBox(new vec3(boundBox.minimum), new vec3(boundBox.maximum)), Mat4.IDENTITY, vec4.RED);
	}

	private void GetInsideNodes()
	{
		if (isSphere)
			World.GetIntersection(boundSphere, inside);
		else
			World.GetIntersection(boundBox, inside);
	}
}
