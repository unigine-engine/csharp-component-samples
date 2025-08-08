using Unigine;

[Component(PropertyGuid = "736f477ad32150586cf44810ad19da883434cf17")]
public class IFpsMovementSample : Component
{
	private SampleDescriptionWindow window = null;
	private WidgetSlider maxFpsSlider = null;

	private void Init()
	{
		window = new SampleDescriptionWindow();
		window.createWindow();
		WidgetGroupBox parameters = window.getParameterGroupBox();
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
