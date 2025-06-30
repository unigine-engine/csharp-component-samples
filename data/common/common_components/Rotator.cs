using Unigine;

[Component(PropertyGuid = "397da4203e5508ff293d0252dd76bd894d5c2954")]
public class Rotator : Component
{
	public vec3 angularVelocity = vec3.ZERO;

	private void Update()
	{
		vec3 delta = angularVelocity * Game.IFps;
		node.SetRotation(node.GetRotation() * new quat(delta.x, delta.y, delta.z));
	}
}
