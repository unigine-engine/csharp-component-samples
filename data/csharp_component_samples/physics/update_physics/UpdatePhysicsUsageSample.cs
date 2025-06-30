using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unigine;

[Component(PropertyGuid = "e56cecddd17f9708f2cc09e6d846de17e4005379")]
public class UpdatePhysicsUsageSample : Component
{
	private SampleDescriptionWindow window = null;
	private WidgetSlider maxFpsSlider = null;

	private void Init()
	{
		window = new SampleDescriptionWindow();
		window.createWindow();
		maxFpsSlider = window.addIntParameter("Max render fps:", "Max render fps:", Render.MaxFPS, 15, 150, (int value) =>
		{
			Render.MaxFPS = value;
		});
	}
	private void Shutdown()
	{
		window.shutdown();
	}
}
