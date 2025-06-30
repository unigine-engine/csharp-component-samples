using Unigine;

#region Math Variables
#if UNIGINE_DOUBLE
using Vec3 = Unigine.dvec3;
using Mat4 = Unigine.dmat4;
#else
using Vec3 = Unigine.vec3;
using Mat4 = Unigine.mat4;
#endif
#endregion

[Component(PropertyGuid = "c02b0b3e3e273985a354dd20eff8edffd8cda352")]
public class ClusterSample : Component
{
	[Parameter(Title = "Cluster Node")]
	public ObjectMeshCluster cluster = null;

	[ParameterMask(MaskType = ParameterMaskAttribute.TYPE.INTERSECTION)]
	public int intersectionMask = 1;

	private WorldIntersection intersection = new WorldIntersection();

	private SampleDescriptionWindow sampleDescriptionWindow = new SampleDescriptionWindow();

	// z-coordinate for meshes
	private const float OFFSET_Z = 0.5f;

	private void Init()
	{
		if (!cluster)
			Log.Error("ClusterSample.Init(): cannot get Cluster property\n");
		InitGui();
	}
	
	private void Update()
	{
		if (Unigine.Console.Active)
			return;
		
		// remove a mesh from the cluster or add a new one to it
		if (Unigine.Input.IsMouseButtonDown(Unigine.Input.MOUSE_BUTTON.LEFT))
		{
			// select a mesh or empty space using the mouse
			ivec2 mouse = Unigine.Input.MousePosition;
			Vec3 p0 = Game.Player.WorldPosition;
			Vec3 p1 = p0 + Game.Player.GetDirectionFromMainWindow(mouse.x, mouse.y) * 100;

			// check for an intersection with the cluster or ground
			Unigine.Object obj = World.GetIntersection(p0, p1, intersectionMask, intersection);
			if (obj)
			{
				// if obj is ObjectMeshCluster then remove a mesh
				if (obj == cluster)
				{
					int num = intersection.Instance;
					cluster.RemoveMeshTransform(num);
				}
				else 
				{
					// create transformation matrix for the new mesh
					Vec3 point = intersection.Point;
					point.z = OFFSET_Z;

					// add a single mesh in local space
					int new_index = cluster.AddMeshTransform();
					cluster.SetMeshTransform(new_index, new mat4(cluster.IWorldTransform * MathLib.Translate(point)));
					// add multiple meshes in global space
					// Mat4[] tr = {MathLib.Translate(point)};
					// cluster.AppendMeshes(tr);
				}
			}
			UpdateGui();
		}
	}

	private void Shutdown()
	{
		ShutdownGui();
	}

	private void InitGui()
	{
		sampleDescriptionWindow.createWindow();
		sampleDescriptionWindow.setStatus($"Number of meshes in the cluster: {cluster.NumMeshes}");
	}

	private void UpdateGui()
	{
		string status = $"Number of meshes in the cluster: {cluster.NumMeshes}";
		sampleDescriptionWindow.setStatus(status);
	}

	private void ShutdownGui()
	{
		sampleDescriptionWindow.shutdown();
	}
}
