using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "881e74a476453dee3919dabfdaf3f4bf1a0b16ed")]
public class Fan : Toggleable
{
	public float rotation_speed = 120;

	private float target_speed = 0;
	private float actual_speed = 0;

	protected override bool On()
	{
		Log.MessageLine("Fan::On()");
		target_speed = rotation_speed;
		return true;
	}

	protected override bool Off()
	{
		Log.MessageLine("Fan::Off()");
		target_speed = 0;
		return true;
	}

	private void Init()
	{
		target_speed = Toggled ? rotation_speed : 0;
	}

	private void Update()
	{
		actual_speed = MathLib.Lerp(actual_speed, target_speed, Game.IFps);
		node.Rotate(0, 0, actual_speed * Game.IFps);
	}
}
