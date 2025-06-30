using Unigine;

[Component(PropertyGuid = "a09052cffaf8a1e2daa137d907103f6442480373")]
public class WidgetsLabel : Component
{
	public int x = 800;
	public int y = 150;
	public string text = "Label";
	public int fontSize = 16;

	private WidgetLabel label = null;

	private void Init()
	{
		Gui gui = Gui.GetCurrent();

		// create label
		label = new WidgetLabel(gui, text);
		label.SetPosition(x, y);
		label.FontSize = fontSize;
		label.FontOutline = 1;

		// add label to current gui
		gui.AddChild(label, Gui.ALIGN_OVERLAP);
	}

	private void Shutdown()
	{
		// remove label from current gui
		Gui.GetCurrent().RemoveChild(label);
	}
}
