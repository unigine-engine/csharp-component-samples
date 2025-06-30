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

[Component(PropertyGuid = "151c20a121dd9f07fca828d340bb5962f855e124")]
public class IFpsMovementController : Component
{
	[ShowInEditor]
	[Parameter(Title = "Use IFps")]
	private bool useIFps = false;

	[ShowInEditor]
	[Parameter(Title = "Movement speed")]
	private float movementSpeed = 1.0f;

	Vec3 current_dir = Vec3.RIGHT;
	void Update()
	{
		if (useIFps)
		{
			node.Translate(current_dir * movementSpeed * Game.IFps);
		}
		else
		{
			node.Translate(current_dir * movementSpeed);
		}

		if (node.WorldPosition.x > 5)
			current_dir = Vec3.LEFT;
		if (node.WorldPosition.x < -5)
			current_dir = Vec3.RIGHT;
	}
}
