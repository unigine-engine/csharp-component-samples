using Unigine;

[Component(PropertyGuid = "16c02db76d472faacda41b01b39fb86c019258db")]
public class WidgetsTreebox : Component
{
	public int x = 775;
	public int y = 300;
	public int fontSize = 16;

	private WidgetTreeBox treeBox = null;

	private void Init()
	{
		Gui gui = Gui.GetCurrent();

		// create treebox
		treeBox = new WidgetTreeBox(gui);
		treeBox.SetPosition(x, y);
		treeBox.FontSize = fontSize;
		treeBox.FontOutline = 1;

		// add first parent and children
		treeBox.AddItem("parent 0");
		treeBox.AddItem("child 0");
		treeBox.AddItem("child 1");
		treeBox.AddItem("child 2");
		treeBox.AddItemChild(0, 1);
		treeBox.AddItemChild(0, 2);
		treeBox.AddItemChild(0, 3);

		// add second parent and children
		treeBox.AddItem("parent 1");
		treeBox.AddItem("child 0");
		treeBox.AddItem("child 1");
		treeBox.AddItem("child 2");
		treeBox.AddItemChild(4, 5);
		treeBox.AddItemChild(4, 6);
		treeBox.AddItemChild(4, 7);

		treeBox.EventChanged.Connect(() => Unigine.Console.OnscreenMessageLine($"Treebox: {treeBox.CurrentItemText}"));

		// add treebox to current gui
		gui.AddChild(treeBox, Gui.ALIGN_OVERLAP);

		Unigine.Console.Onscreen = true;
	}

	private void Shutdown()
	{
		// remove tree box from current gui
		Gui.GetCurrent().RemoveChild(treeBox);

		Unigine.Console.Onscreen = false;
	}
}
