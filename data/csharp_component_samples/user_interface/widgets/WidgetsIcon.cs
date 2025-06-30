using Unigine;

[Component(PropertyGuid = "18ecb4706f48bd0f54eb8722968e5c1998cc96a4")]
public class WidgetsIcon : Component
{
	public int x = 500;
	public int y = 450;
	public int width = 32;
	public int height = 32;

	[ParameterFile]
	public string iconImage = "";

	private WidgetIcon icon = null;

	private void Init()
	{
		Gui gui = Gui.GetCurrent();

		// create icon
		icon = new WidgetIcon(gui, iconImage, width, height);
		icon.Toggleable = true;
		icon.SetPosition(x, y);
		icon.EventClicked.Connect(() => Unigine.Console.OnscreenMessageLine($"Icon: {icon.Toggled}"));

		// add icon to current gui
		gui.AddChild(icon, Gui.ALIGN_OVERLAP);

		Unigine.Console.Onscreen = true;
	}

	private void Shutdown()
	{
		// remove icon from current gui
		Gui.GetCurrent().RemoveChild(icon);

		Unigine.Console.Onscreen = false;
	}
}
