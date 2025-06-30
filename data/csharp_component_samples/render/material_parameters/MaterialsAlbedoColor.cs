using Unigine;

[Component(PropertyGuid = "204cfc442f7edad38fab22160bda727d0e5991b8")]
public class MaterialsAlbedoColor : Component
{
	[ParameterColor]
	public vec4 beginColor = vec4.ZERO;

	[ParameterColor]
	public vec4 endColor = vec4.ONE;

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

		// change albedo color of material
		material.SetParameterFloat4("albedo_color", MathLib.Lerp(beginColor, endColor, MathLib.Saturate(currentTime / time)));
	}
}
