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

[Component(PropertyGuid = "33a82db84a2489447af346085b2b1c2e128d4d4c")]
public class MathTriggerComponent : Component
{
	[ShowInEditor]
	private float boundSphereSize = 5.0f;
	[ShowInEditor]
	private float boundBoxSize = 5.0f;
	[ShowInEditor]
	private bool isSphere = false;
	[ShowInEditor]
	private bool debug = false;

	private List<Node> objects = new List<Node>();
	public int NumObjects { get { return objects.Count; } }

	private List<Node> entered = new List<Node>();

	private WorldBoundBox boundBox;
	private WorldBoundSphere boundSphere;

	private EventInvoker<Node> eventEnter = new EventInvoker<Node>();
	public Event<Node> EventEnter {  get { return eventEnter; } }

	private EventInvoker<Node> eventLeave = new EventInvoker<Node>();
	public Event<Node> EventLeave {  get { return eventLeave; } }

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

		CheckEnter();
		CheckLeave();
	}

	public void AddObject(Node obj)
	{
		objects.Add(obj);
	}

	public void AddObjects(ICollection<Node> inputObjects)
	{
		objects.AddRange(inputObjects);
	}

	public Node GetObjectByIndex(int index)
	{
		return objects[index];
	}

	public void RemoveObject(Node obj)
	{
		objects.Remove(obj);
	}

	public void RemoveObjectByIndex(int index)
	{
		objects.RemoveAt(index);
	}

	public void ClearObjects()
	{
		objects.Clear();
	}

	private void CheckEnter()
	{
		foreach(Node obj in objects)
		{
			if (entered.Contains(obj))
				continue;

			bool isInside = isSphere ? CheckSphere(obj) : CheckBox(obj);
			if(isInside)
			{
				entered.Add(obj);
				eventEnter.Run(obj);
			}
		}
	}

	private void CheckLeave()
	{
		foreach (Node obj in objects)
		{
			if (!entered.Contains(obj))
				continue;

			if(obj.IsDeleted)
			{
				entered.Remove(obj);
				continue;
			}

			bool isInside = isSphere ? CheckSphere(obj) : CheckBox(obj);
			if (!isInside)
			{
				entered.Remove(obj);
				eventLeave.Run(obj);
			}
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

	private bool CheckSphere(Node obj)
	{
		return boundSphere.InsideValid(new vec3(obj.Transform.Translate));
	}

	private bool CheckBox(Node obj)
	{
		return boundBox.InsideValid(new vec3(obj.Transform.Translate));
	}
}

