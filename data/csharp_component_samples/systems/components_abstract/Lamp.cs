using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "6e8e87bcdefeca656e6329c53069243985a5d2f2")]
public class Lamp : Toggleable
{
	[ParameterColor]
	public vec4 emission_color = vec4.WHITE;

	protected override bool On()
	{
		Log.MessageLine("Lamp::On()");
		return SetEmissionColor(emission_color);
	}

	protected override bool Off()
	{
		Log.MessageLine("Lamp::Off()");
		return SetEmissionColor(vec4.ZERO);
	}

	private bool SetEmissionColor(vec4 emission_color)
	{
		Object obj = (Object)node;

		if (obj == null)
			return false;

		for (var surface = 0; surface < obj.NumSurfaces; surface += 1)
			obj.SetMaterialParameterFloat4("emission_color", emission_color, surface);

		return true;
	}

	private void Init()
	{
		SetEmissionColor(Toggled ? emission_color : vec4.ZERO);
	}
}
