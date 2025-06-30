using Unigine;

[Component(PropertyGuid = "2b37ec3ac80f0e51cacff37c2b1db8ab4d38760e")]
public class WidgetsCombobox : Component
{
	public int x = 600;
	public int y = 50;
	public int fontSize = 16;

	private WidgetComboBox comboBox = null;

	private void Init()
	{
		Gui gui = Gui.GetCurrent();

		// create combobox
		comboBox = new WidgetComboBox(gui);
		comboBox.SetPosition(x, y);
		comboBox.FontSize = fontSize;

		// add items
		comboBox.AddItem("item 0");
		comboBox.AddItem("item 1");
		comboBox.AddItem("item 2");

		comboBox.EventChanged.Connect(() => Unigine.Console.OnscreenMessageLine($"Combobox: {comboBox.GetCurrentItemText()}"));

		// add combobox to current gui
		gui.AddChild(comboBox, Gui.ALIGN_OVERLAP);

		Unigine.Console.Onscreen = true;
	}

	private void Shutdown()
	{
		// remove combobox from current gui
		Gui.GetCurrent().RemoveChild(comboBox);

		Unigine.Console.Onscreen = false;
	}
}
