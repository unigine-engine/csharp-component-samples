using Unigine;

[Component(PropertyGuid = "0b957b466bfa19ede69bb268679472a7d6f64fda")]
public class WidgetsListbox : Component
{
	public int x = 300;
	public int y = 300;
	public int fontSize = 16;

	private WidgetListBox listBox = null;

	private void Init()
	{
		Gui gui = Gui.GetCurrent();

		// create listbox
		listBox = new WidgetListBox(gui);
		listBox.SetPosition(x, y);
		listBox.AddItem("item 0");
		listBox.AddItem("item 1");
		listBox.AddItem("item 2");
		listBox.FontSize = fontSize;
		listBox.FontOutline = 1;
		listBox.EventChanged.Connect(() => Unigine.Console.OnscreenMessageLine($"Listbox: {listBox.GetCurrentItemText()}"));

		// add listbox to current gui
		gui.AddChild(listBox, Gui.ALIGN_OVERLAP);

		Unigine.Console.Onscreen = true;
	}

	private void Shutdown()
	{
		// remove listbox from current gui
		Gui.GetCurrent().RemoveChild(listBox);

		Unigine.Console.Onscreen = false;
	}
}
