using Unigine;

[Component(PropertyGuid = "abe43a0d916c1408a3af27c7389c427adcc4ef91")]
public class WidgetsButton : Component
{
	public int x = 250;
	public int y = 50;
	public int width = 100;
	public int height = 50;
	public string text = "Press Me";
	public int fontSize = 16;

	private WidgetButton button = null;

	private void Init()
	{
		Gui gui = Gui.GetCurrent();

		// create button
		button = new WidgetButton(gui, text);
		button.SetPosition(x, y);
		button.Width = width;
		button.Height = height;
		button.FontSize = fontSize;
		button.EventClicked.Connect(() => Unigine.Console.OnscreenMessageLine("Button Clicked!"));

		// add button to current gui
		gui.AddChild(button, Gui.ALIGN_OVERLAP);

		Unigine.Console.Onscreen = true;
	}

	private void Shutdown()
	{
		// remove button from current gui
		Gui.GetCurrent().RemoveChild(button);

		Unigine.Console.Onscreen = false;
	}
}
