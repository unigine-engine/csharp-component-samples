using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "a6d021071cb8ceb159c8c170226cb50b1b3e0a39")]
public class BodyFractureSpawner : Component
{
	[ParameterFile(Filter = ".node")]
	public string InstancePath;
	public float Cooldown = 3.0f;
	public bool Instant = true;

	private float counter;

	void Init()
	{
		counter = Instant ? Cooldown : 0;
	}
	
	void Update()
	{
		counter += Game.IFps;
		if (counter >= Cooldown)
		{
			counter = 0;
			var inst = World.LoadNode(InstancePath);
			inst.WorldPosition = node.WorldPosition;
		}
	}
}
