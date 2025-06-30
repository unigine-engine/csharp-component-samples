using Unigine;

[Component(PropertyGuid = "00ce6ffe7757d74551675fe0ebc211bf4402f412")]
public class WidgetsEdittext : Component
{
	public int x = 250;
	public int y = 150;
	public int width = 150;
	public int height = 100;
	public string text = "Enter text...";
	public int fontSize = 16;

	private WidgetEditText editText = null;

	private void Init()
	{
		Gui gui = Gui.GetCurrent();

		// create edittext
		editText = new WidgetEditText(gui, text);
		editText.SetPosition(x, y);
		editText.Width = width;
		editText.Height = height;
		editText.FontSize = fontSize;
		editText.FontOutline = 1;
		editText.EventChanged.Connect(() => Unigine.Console.OnscreenMessageLine($"Edittext: {editText.Text}"));

		// add edittext to current gui
		gui.AddChild(editText, Gui.ALIGN_OVERLAP);

		Unigine.Console.Onscreen = true;
	}

	private void Shutdown()
	{
		// remove edittext from current gui
		Gui.GetCurrent().RemoveChild(editText);

		Unigine.Console.Onscreen = false;
	}
}
