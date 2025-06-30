using Unigine;

[Component(PropertyGuid = "1d252ff437419ac34b788146b4e567edf9a2a868")]
public class MaterialsMetalness : Component
{
	public float beginMetalness = 0.0f;
	public float endMetalness = 1.0f;
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
			timeSign = -timeSign;

		// change metalness of material
		material.SetParameterFloat("metalness", MathLib.Lerp(beginMetalness, endMetalness, MathLib.Saturate(currentTime / time)));
	}
}
