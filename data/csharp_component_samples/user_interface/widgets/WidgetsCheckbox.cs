using Unigine;

[Component(PropertyGuid = "19255e459ba13ae020f430bff6e887129cd0fb49")]
public class WidgetsCheckbox : Component
{
	public int x = 450;
	public int y = 50;
	public string text = "Check Me";
	public int fontSize = 16;

	private WidgetCheckBox checkBox = null;

	private void Init()
	{
		Gui gui = Gui.GetCurrent();

		// create checkbox
		checkBox = new WidgetCheckBox(gui, text);
		checkBox.SetPosition(x, y);
		checkBox.FontSize = fontSize;
		checkBox.FontOutline = 1;
		checkBox.EventChanged.Connect(() => Unigine.Console.OnscreenMessageLine($"Checkbox: {checkBox.Checked}"));

		// add checkbox to current gui
		gui.AddChild(checkBox, Gui.ALIGN_OVERLAP);

		Unigine.Console.Onscreen = true;
	}

	private void Shutdown()
	{
		// remove checkbox from current gui
		Gui.GetCurrent().RemoveChild(checkBox);

		Unigine.Console.Onscreen = false;
	}
}
