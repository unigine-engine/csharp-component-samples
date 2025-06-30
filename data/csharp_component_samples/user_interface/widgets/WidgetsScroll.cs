using Unigine;

[Component(PropertyGuid = "392801192f57f7a4df95deb30d4194768970815d")]
public class WidgetsScroll : Component
{
	public int x = 500;
	public int y = 150;

	private WidgetScroll scroll = null;

	private void Init()
	{
		Gui gui = Gui.GetCurrent();

		// create scroll
		scroll = new WidgetScroll(gui);
		scroll.SetPosition(x, y);
		scroll.Orientation = 0;
		scroll.EventChanged.Connect(() => Unigine.Console.OnscreenMessageLine($"Scroll: {scroll.Value}"));

		// add scroll to current gui
		gui.AddChild(scroll, Gui.ALIGN_OVERLAP);

		Unigine.Console.Onscreen = true;
	}

	private void Shutdown()
	{
		// remove scroll from current gui
		Gui.GetCurrent().RemoveChild(scroll);

		Unigine.Console.Onscreen = false;
	}
}
