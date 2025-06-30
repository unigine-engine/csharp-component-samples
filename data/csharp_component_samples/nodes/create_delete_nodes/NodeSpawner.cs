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

[Component(PropertyGuid = "3e8dfa296951fd7400e90a4322e62ac6e480036d")]
public class NodeSpawner : Component
{
	[ParameterFile(Filter = ".node")]
	public string instancePath = "";

	// collection of created nodes
	private List<Node> instances = null;

	// time between creation and deletion
	private const float spawnTimer = 0.5f;
	private float currentTime = 0.0f;

	private const int count = 20;
	private int currentIndex = 0;

	private void Init()
	{
		instances = new List<Node>();
		currentTime = spawnTimer;
	}

	private void Update()
	{
		currentTime -= Game.IFps;
		if (currentTime < 0)
		{
			// create a new instance of the node and add it to the collection
			Node newNode = World.LoadNode(instancePath);
			instances.Add(newNode);

			// set world position of node based on current index
			float x = 4.0f - 2.0f * (currentIndex / 4);
			float y = -4.0f + 2.0f * (currentIndex % 4);
			newNode.WorldPosition = new Vec3(x, y, 0.0f);

			if (instances.Count > count / 2)
			{
				// delete first node in collection
				instances[0].DeleteLater();
				instances.RemoveAt(0);
			}

			currentIndex++;
			if (currentIndex == count)
				currentIndex = 0;

			currentTime = spawnTimer;
		}
	}
}
