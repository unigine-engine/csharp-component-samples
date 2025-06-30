using Unigine;

[Component(PropertyGuid = "8cfeb36765e8199a63759c31666c67dcc5cde18b")]
public class WidgetsSpinbox : Component
{
	public int x = 625;
	public int y = 300;

	private WidgetSpinBox spinBox = null;
	private WidgetEditLine spinBoxLine = null;

	private void Init()
	{
		Gui gui = Gui.GetCurrent();

		// create spinbox line
		spinBoxLine = new WidgetEditLine(gui, "0");
		spinBoxLine.SetPosition(x, y);
		spinBoxLine.FontOutline = 1;

		// add spinbox line to current gui
		gui.AddChild(spinBoxLine, Gui.ALIGN_OVERLAP);

		// create spinbox
		spinBox = new WidgetSpinBox(gui);
		spinBox.Order = spinBoxLine.Order + 1;
		spinBoxLine.AddAttach(spinBox);

		spinBox.EventChanged.Connect(() => Unigine.Console.OnscreenMessageLine($"Spinbox: {spinBox.Value}"));

		// add spinbox to current gui
		gui.AddChild(spinBox, Gui.ALIGN_OVERLAP);

		Unigine.Console.Onscreen = true;
	}

	private void Shutdown()
	{
		// remove spinbox line and spinbox from current gui
		Gui.GetCurrent().RemoveChild(spinBox);
		Gui.GetCurrent().RemoveChild(spinBoxLine);

		Unigine.Console.Onscreen = false;
	}
}
