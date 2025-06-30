using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "4a3bf23b8bc0c25e1dd84625c5fa41405f63c39f")]
public class Spinner : Component
{
	[Parameter(Title = "Angular Speed", Tooltip = "Rotation speed, in degrees per second", Group = "General")]
	public float rotationSpeed = 50.0f;

	[ParameterFile(Filter = ".node")]
	public string bulletAsset = "";

	public List<Node> spawnPoints = null;

	public float spawnDelay = 2.0f;

	private float spawnTimer = 0.0f;

	private void Init()
	{
		spawnTimer = spawnDelay;
	}

	private void Update()
	{
		node.Rotate(0, 0, rotationSpeed * Game.IFps);

		if (spawnPoints == null)
			return;

		spawnTimer -= Game.IFps;
		if (spawnTimer < 0)
		{
			spawnTimer = spawnDelay;

			foreach (Node point in spawnPoints)
			{
				Node spawnedBullet = World.LoadNode(bulletAsset);
				spawnedBullet.WorldTransform = point.WorldTransform;
			}
		}
	}
}
