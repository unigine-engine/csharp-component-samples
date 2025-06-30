using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "f1ef0201346f7487ef96082467dee7850d6d5d2e")]
public class BodyFractureExplosionSample : Component
{
	public BodyFractureExplosion explosion = null;
	public SampleDescriptionWindow sampleDescriptionWindow = new();
	void Init()
	{
		sampleDescriptionWindow.createWindow();
		var btn = new WidgetButton("Explode!");
		btn.EventClicked.Connect(() => explosion?.Explode());
		sampleDescriptionWindow.getParameterGroupBox().AddChild(btn);

		Visualizer.Enabled = true;
	}
	void Shutdown()
	{
		Visualizer.Enabled = false;

		sampleDescriptionWindow.shutdown();
	}

}
