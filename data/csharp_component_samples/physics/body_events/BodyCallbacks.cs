using System.Collections;
using System.Collections.Generic;
using Unigine;

#if UNIGINE_DOUBLE
using Vec3 = Unigine.dvec3;
using Vec4 = Unigine.dvec4;
using Mat4 = Unigine.dmat4;
#else
using Vec3 = Unigine.vec3;
using Vec4 = Unigine.vec4;
using Mat4 = Unigine.mat4;
#endif

[Component(PropertyGuid = "723d60191b1c46188aaf19df9e26724a53a6d73d")]
public class BodyCallbacks : Component
{
	// tower params
	public float space = 1.2f;
	public int tower_level = 10;
	// different materials for different callbacks
	public Material frozen_material;
	public Material position_material;
	// mesh used to build a tower
	[ParameterFile(Filter = ".mesh")]
	public string mesh_file = "";

	private List<Node> objects = new List<Node>();
	private EventConnections body_connections = new EventConnections();
	private bool visualizer_state;

	void Init()
	{
		visualizer_state = Visualizer.Enabled;
		Visualizer.Enabled = true;

		// parameters validation
		if (mesh_file.Length <= 0)
			Log.Error("BodyCallbacks.Init(): Mesh File parameter is empty!\n");

		if (!frozen_material)
			Log.Error("BodyCallbacks.Init(): Frozen Matreial parameter is empty!\n");

		if (!position_material)
			Log.Error("BodyCallbacks.Init(): Position Matreial parameter is empty!\n");

		// general Physics settings
		Physics.FrozenLinearVelocity = 0.1f;
		Physics.FrozenAngularVelocity = 0.1f;
		Physics.NumIterations = 4;

		// create object and physical body with a box shape
		ObjectMeshStatic obj = new ObjectMeshStatic(mesh_file);
		BodyRigid body = new BodyRigid(obj);
		ShapeBox shape = new ShapeBox(body, new vec3(1));
		obj.SetMaterial(position_material, "*");

		// create tower
		for (int i = 0; i < tower_level; i++)
		{
			for (int j = 0; j < tower_level - i; j++)
			{
				// clone created earlier object and calculate it's position in a tower
				ObjectMeshStatic mesh = obj.Clone() as ObjectMeshStatic;
				mesh.WorldTransform = MathLib.Translate(new Vec3(0.0f, j - 0.5f * (tower_level - i) + 0.5f, i + 0.5f) * space);
				
				// add Frozen, Position and Contact callbacks to new object's body
				body = mesh.BodyRigid;
				body.EventFrozen.Connect(body_connections, b => b.Object.SetMaterial(frozen_material, "*"));
				body.EventPosition.Connect(body_connections, b => b.Object.SetMaterial(position_material, "*"));
				body.EventContactEnter.Connect(body_connections, (b, num) => b.RenderContacts());
				objects.Add(mesh);
			}
		}
		obj.DeleteLater();
	}
	
	void Shutdown()
	{
		// remove all connections
		body_connections.DisconnectAll();
		objects.Clear();
		// restore visualizer state
		Visualizer.Enabled = visualizer_state;
	}
}
