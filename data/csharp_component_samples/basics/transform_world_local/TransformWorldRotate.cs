using Unigine;

[Component(PropertyGuid = "8b0e57758d6f0250dfcdf16f3bed7002d2506f79")]
public class TransformWorldRotate : Component
{
	public vec3 angularVelocity = new vec3(0.0f, 0.0f, 45.0f);

	private void Update()
	{
		vec3 offset = angularVelocity * Game.IFps;
		node.WorldRotate(offset.x, offset.y, offset.z);
	}
}
