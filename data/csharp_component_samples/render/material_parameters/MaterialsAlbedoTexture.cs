using Unigine;

[Component(PropertyGuid = "c92fe3f5cb9e24d02e71b1ef45713682319883f9")]
public class MaterialsAlbedoTexture : Component
{
	[ParameterFile]
	public string firstTextureImage;

	[ParameterFile]
	public string secondTextureImage;

	public float time = 5.0f;

	private Material material = null;
	private float currentTime = 0.0f;
	private float timeSign = 1.0f;

	private Texture firstTexture = null;
	private Texture secondTexture = null;

	private void Init()
	{
		// try cast node to object
		Unigine.Object obj = node as Unigine.Object;
		if (!obj)
			return;

		// try get material from 0 surface
		if (obj.NumSurfaces != 0)
			material = obj.GetMaterial(0);

		if (!material)
			return;

		// create first texture
		firstTexture = new Texture();
		firstTexture.Load(firstTextureImage);

		// create second texture
		secondTexture = new Texture();
		secondTexture.Load(secondTextureImage);

		// apply first texture to material
		material.SetTexture("albedo", firstTexture);
	}

	private void Update()
	{
		if (!material)
			return;

		// update current time
		currentTime += Game.IFps * timeSign;
		if (currentTime < 0 || time < currentTime)
		{
			// change albedo texture
			if (timeSign > 0)
				material.SetTexture("albedo", secondTexture);
			else
				material.SetTexture("albedo", firstTexture);

			timeSign = -timeSign;
		}
	}
}
