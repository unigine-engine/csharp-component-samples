using Unigine;

[Component(PropertyGuid = "b3a398566ad5f3bdb5dea1f8fe7dfcf70fbe35ea")]
public class WidgetsMenu : Component
{
	public int x = 450;
	public int y = 300;
	public int fontSize = 16;

	[ParameterColor]
	public vec4 selectionColor = vec4.ZERO;

	private WidgetMenuBar menuBar = null;

	private void Init()
	{
		Gui gui = Gui.GetCurrent();

		// create menubars and menuboxes
		menuBar = new WidgetMenuBar(gui);
		menuBar.SelectionColor = selectionColor;
		menuBar.AddItem("File");
		menuBar.AddItem("Edit");
		menuBar.AddItem("Help");
		menuBar.SetPosition(x, y);
		menuBar.FontSize = fontSize;
		menuBar.FontOutline = 1;

		// add file menubox
		WidgetMenuBox fileMenuBox = new WidgetMenuBox(gui);
		fileMenuBox.FontSize = fontSize;
		fileMenuBox.FontOutline = 1;
		fileMenuBox.AddItem("File 0");
		fileMenuBox.AddItem("File 1");
		fileMenuBox.AddItem("File 2");
		fileMenuBox.EventClicked.Connect(() => Unigine.Console.OnscreenMessageLine($"Menubar: {fileMenuBox.CurrentItemText}"));
		menuBar.SetItemMenu(0, fileMenuBox);

		// add edit menubox
		WidgetMenuBox editMenuBox = new WidgetMenuBox(gui);
		editMenuBox.FontSize = fontSize;
		editMenuBox.FontOutline = 1;
		editMenuBox.AddItem("Edit 0");
		editMenuBox.AddItem("Edit 1");
		editMenuBox.AddItem("Edit 2");
		editMenuBox.EventClicked.Connect(() => Unigine.Console.OnscreenMessageLine($"Menubar: {editMenuBox.CurrentItemText}"));
		menuBar.SetItemMenu(1, editMenuBox);

		// add help menubox
		WidgetMenuBox helpMenuBox = new WidgetMenuBox(gui);
		helpMenuBox.FontSize = fontSize;
		helpMenuBox.FontOutline = 1;
		helpMenuBox.AddItem("Help 0");
		helpMenuBox.AddItem("Help 1");
		helpMenuBox.AddItem("Help 2");
		helpMenuBox.EventClicked.Connect(() => Unigine.Console.OnscreenMessageLine($"Menubar: {helpMenuBox.CurrentItemText}"));
		menuBar.SetItemMenu(2, helpMenuBox);

		// add menu to current gui
		gui.AddChild(menuBar, Gui.ALIGN_OVERLAP);

		Unigine.Console.Onscreen = true;
	}

	private void Shutdown()
	{
		// remove menu from current gui
		Gui.GetCurrent().RemoveChild(menuBar);

		Unigine.Console.Onscreen = false;
	}
}
