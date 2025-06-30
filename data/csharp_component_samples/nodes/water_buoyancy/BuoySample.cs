using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unigine;

[Component(PropertyGuid = "0f0af9cec4a3b6d54e651a300eb067cf67f7b2e2")]
public class BuoySample : Component
{
	[ShowInEditor]
	[ParameterSlider(Min = 0.0f)]
	private float buoyancy = 1.0f;

	[ShowInEditor]
	private ObjectWaterGlobal water = null;

	static public float GlobalBuoancy { get; private set; }

	private SampleDescriptionWindow window = new();

	private void Init()
	{
		if (water != null)
		{	
			water.FetchSteepnessQuality = ObjectWaterGlobal.STEEPNESS_QUALITY.HIGH;
			water.FetchAmplitudeThreshold = 0.001f;
		}

		window.createWindow();
		window.addFloatParameter("Buoyancy", null, buoyancy, 0.01f, 1.0f, on_buoyancy_cahnged);
		window.addFloatParameter("Beaufort", null, 4.0f, 0.0f, 12.0f, on_beaufort_cahnged);

		GlobalBuoancy = buoyancy;
	}
	
	private void Shutdown()
	{
		window.shutdown();
	}

	private void on_buoyancy_cahnged(float value)
	{
		GlobalBuoancy = value;
	}

	private void on_beaufort_cahnged(float value)
	{
		if (water != null)
			water.Beaufort = value;
	}
}
