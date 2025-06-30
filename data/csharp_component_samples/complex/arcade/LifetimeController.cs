using Unigine;

[Component(PropertyGuid = "70b43839418d8444ddf5c2d37424ea16b3b99a7d")]
public class LifetimeController : Component
{
	public float lifetime = 1.0f;

	private float starttime = 0.0f;

	private void Init()
	{
		starttime = Game.Time;
	}

	private void Update()
	{
		if (Game.Time - starttime > lifetime)
			node.DeleteLater();
	}

	private void Shutdown()
	{
		node.DeleteLater();
	}
}
