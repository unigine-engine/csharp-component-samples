using Unigine;

[Component(PropertyGuid = "2c37e3b1fc47095c43c1150ba9b266eefcbf0175")]
public class TransformRotate : Component
{
	public vec3 angularVelocity = new vec3(0.0f, 0.0f, 45.0f);

	private void Update()
	{
		node.Rotate(angularVelocity * Game.IFps);
	}
}
