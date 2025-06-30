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

[Component(PropertyGuid = "ec557ee1fa8be1a22fe1df67565afa155e001e85")]
public class JointCallbacks : Component
{
	public int bridge_sections = 14;
	// different materials for still joint and already broken sections
	public Material broken_materal;
	public Material joint_materal;
	// mesh used as a section
	[ParameterFile(Filter = ".mesh")]
	public string mesh_file = "";

	private List<Node> objects = new List<Node>();
	private EventConnections joint_connections = new EventConnections();

	private float space = 1.1f;

	void Init()
	{
		// parameters validation
		if (mesh_file.Length <= 0)
			Log.Error("JointCallbacks.Init(): Mesh File parameter is empty!\n");

		if (!broken_materal)
			Log.Error("JointCallbacks.Init(): Broken Matreial parameter is empty!\n");

		if (!joint_materal)
			Log.Error("JointCallbacks.Init(): Joint Matreial parameter is empty!\n");

		// general Physics settings
		Physics.FrozenLinearVelocity = 0.1f;
		Physics.FrozenAngularVelocity = 0.1f;
		Physics.NumIterations = 4;

		// create object from mesh file
		ObjectMeshStatic orig_object = new ObjectMeshStatic(mesh_file);

		// create weights to break bridge
		BodyRigid body = new BodyRigid(orig_object);
		ShapeBox shape = new ShapeBox(body, new vec3(1));
		shape.Density = 80.0f;
		for (int i = 0; i < 3; i++)
		{
			ObjectMeshStatic mesh = orig_object.Clone() as ObjectMeshStatic;
			mesh.WorldTransform = MathLib.Translate(new Vec3(3.0f * (i - 1), 0.0f, 12.0f));
			objects.Add(mesh);
		}

		// remove body from object
		orig_object.Body = null;
		body.DeleteLater();

		// create bridge via boxes and joints
		orig_object.SetMaterial(joint_materal, "*");
		Body b0 = null, b1;
		for (int i = 0; i < bridge_sections; i++)
		{
			ObjectMeshStatic mesh = orig_object.Clone() as ObjectMeshStatic;
			float pos = space * (i - (bridge_sections - 1) / 2.0f);
			mesh.WorldTransform = MathLib.Translate(new Vec3(pos, 0.0f, 8.0f));

			// set first and last bridge section as BodyDummy so they won't fall
			if (i == 0 || i == bridge_sections - 1)
				b1 = new BodyDummy(mesh);
			else
				b1 = new BodyRigid(mesh);
			shape = new ShapeBox(b1, new vec3(1));
			objects.Add(mesh);

			// create joint between two neighbour boxes
			if (b0 != null)
			{
				JointHinge joint = new JointHinge(b0, b1, new Vec3(pos - space, 0.0f, 8.0f), new vec3(1.0f, 0.0f, 0.0f));
				joint.AngularDamping = 8.0f;
				joint.NumIterations = 2;
				joint.LinearRestitution = 0.02f;
				joint.AngularRestitution = 0.02f;
				joint.MaxForce = 1000.0f;
				joint.MaxTorque = 16000.0f;
				// subscribind to joint breaking event
				joint.EventBroken.Connect(joint_connections, j => {
					// change material of broken parts
					joint.Body0.Object.SetMaterial(broken_materal, "*");
					joint.Body1.Object.SetMaterial(broken_materal, "*");
				});
			}

			b0 = b1;
		}

		orig_object.DeleteLater();
	}
	
	void Shutdown()
	{
		// remove all connections
		joint_connections.DisconnectAll();
		objects.Clear();
	}
}
