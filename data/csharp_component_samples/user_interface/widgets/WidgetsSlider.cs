using Unigine;

[Component(PropertyGuid = "9b7d1ea8fa6871e7b0adc61acaba5eb108e9485c")]
public class WidgetsSlider : Component
{
	public int x = 600;
	public int y = 150;
	public int width = 100;
	public int height = 50;
	public int buttonWidth = 30;

	private WidgetSlider slider = null;

	private void Init()
	{
		Gui gui = Gui.GetCurrent();

		// create slider
		slider = new WidgetSlider(gui);
		slider.Width = width;
		slider.Height = height;
		slider.ButtonWidth = buttonWidth;
		slider.SetPosition(x, y);
		slider.EventChanged.Connect(() => Unigine.Console.OnscreenMessageLine($"Slider: {slider.Value}"));

		// add slider to current gui
		gui.AddChild(slider, Gui.ALIGN_OVERLAP);

		Unigine.Console.Onscreen = true;
	}

	private void Shutdown()
	{
		// remove slider from current gui
		Gui.GetCurrent().RemoveChild(slider);

		Unigine.Console.Onscreen = false;
	}
}
