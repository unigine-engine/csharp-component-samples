using Unigine;

[Component(PropertyGuid = "9b2c8cbb9c079ade503444a5d68d2dc0d994c61f")]
public class MaterialsCastWorldShadow : Component
{
	public float time = 5.0f;

	private Material material = null;
	private float currentTime = 0.0f;
	private float timeSign = 1.0f;

	private void Init()
	{
		// try cast node to object
		Unigine.Object obj = node as Unigine.Object;
		if (!obj)
			return;

		// try get material from 0 surface
		if (obj.NumSurfaces != 0)
			material = obj.GetMaterial(0);
	}

	private void Update()
	{
		if (!material)
			return;

		// update current time
		currentTime += Game.IFps * timeSign;
		if (currentTime < 0 || time < currentTime)
		{
			// change world cast shadow
			material.CastWorldShadow = !material.CastWorldShadow;

			timeSign = -timeSign;
		}
	}
}
