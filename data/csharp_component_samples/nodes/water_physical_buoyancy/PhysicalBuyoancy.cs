using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "95dd6a8567f4cce4bdb16d41c8c0dc03fa138a93")]
public class PhysicalBuyoancy : Component
{
	private SampleDescriptionWindow window = new();
	public Node waves_node = null;
	public Node buoyancy_node = null;
	
	private Waves waves = null;
	private Buoyancy buoyancy = null;

	void Init()
	{
		if (waves_node == null)
			return;
		if (buoyancy_node == null)
			return;
		waves = GetComponent<Waves>(waves_node);
		buoyancy = GetComponent<Buoyancy>(buoyancy_node);
		buoyancy.SetVisualizer(false);

		window.createWindow();
		window.addFloatParameter("Beaufort", null, 0.0f, 0.0f, 8.00f, on_beaufort_slider_changed);
		window.addBoolParameter("Debug", null, false, on_debug_checkbox_clicked);
	}

	void Shutdown()
	{
		window.shutdown();
	}

	private void on_beaufort_slider_changed(float value)
	{
		waves.SetBeaufort(value);
	}

	private void on_debug_checkbox_clicked(bool value)
	{
		buoyancy.SetVisualizer(value);
	}
}
