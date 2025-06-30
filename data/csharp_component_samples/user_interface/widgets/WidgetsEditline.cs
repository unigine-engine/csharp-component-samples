using Unigine;

[Component(PropertyGuid = "9113b1e5d2ad3a009e56e55f4c837332bee3b8e9")]
public class WidgetsEditline : Component
{
	public int x = 750;
	public int y = 50;
	public int width = 150;
	public int height = 30;
	public string text = "Enter text...";
	public int fontSize = 16;

	private WidgetEditLine editLine = null;

	private void Init()
	{
		Gui gui = Gui.GetCurrent();

		// create editline
		editLine = new WidgetEditLine(gui, text);
		editLine.SetPosition(x, y);
		editLine.Width = width;
		editLine.Height = height;
		editLine.FontSize = fontSize;
		editLine.FontOutline = 1;
		editLine.EventChanged.Connect(() => Unigine.Console.OnscreenMessageLine($"Editline text: {editLine.Text}"));

		// add editline to current gui
		gui.AddChild(editLine, Gui.ALIGN_OVERLAP);

		Unigine.Console.Onscreen = true;
	}

	private void Shutdown()
	{
		// remove editline form current gui
		Gui.GetCurrent().RemoveChild(editLine);

		Unigine.Console.Onscreen = false;
	}
}
