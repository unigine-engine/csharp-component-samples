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

[Component(PropertyGuid = "de217aaf7595a5baab09a9b2f5bcb45b4870c8ee")]
public class UpdatePhysicsUsageController : Component
{
	[ShowInEditor]
	[Parameter(Title = "Use update function")]
	private bool useUdpate=false;

	[ShowInEditor]
	[Parameter(Title = "Linear force applied to body")]
	private float linearForce=5.0f;

	private BodyRigid rigidBody;
	private float currentForce = 0.0f;


	void Init()
	{
		rigidBody = node.ObjectBodyRigid;

		if (!rigidBody)
		{
			Log.Error("PhysicsIFpsController.Init() can't find rigid body on the node!\n");
		}
		currentForce = linearForce;
	}

	void Update()
	{
		//visualizing current linear velocity
		Visualizer.Enabled = true;
		Visualizer.RenderVector(rigidBody.Position, rigidBody.Position + new Vec3(rigidBody.LinearVelocity), vec4.RED,0.5f);

		//NOTICE that methods: Update and UdpatePhysics registered in different component Macros and code is the same for both usage examples
		// using Update() to move node with physics
		if (useUdpate)
		{
			Movement();
		}
	}
	void UpdatePhysics()
	{
		//NOTICE that methods: Update and UdpatePhysics registered in different component Macros and code is the same for both usage examples
		// using Update() to move node with physics
		if (!useUdpate)
		{
			Movement();
		}
	}
	private void Movement()
	{
		rigidBody.AddForce(vec3.RIGHT * currentForce);

		if (node.WorldPosition.x > 5)
			currentForce= -linearForce;
		if (node.WorldPosition.x < -5)
			currentForce = linearForce;
	}
}
