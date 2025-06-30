using Unigine;

[Component(PropertyGuid = "32f19f98ae229e174f6d248a5302ca6153aee04c")]
public class WidgetsSprite : Component
{
	public int x = 275;
	public int y = 450;
	public int width = 100;
	public int height = 50;

	[ParameterFile]
	public string spriteImage = "";

	private WidgetSprite sprite = null;

	private void Init()
	{
		Gui gui = Gui.GetCurrent();

		// create sprite
		sprite = new WidgetSprite(gui, spriteImage);
		sprite.Width = width;
		sprite.Height = height;
		sprite.SetPosition(x, y);

		// add sprite to current gui
		gui.AddChild(sprite, Gui.ALIGN_OVERLAP);
	}

	private void Shutdown()
	{
		// remove sprite form current gui
		Gui.GetCurrent().RemoveChild(sprite);
	}
}
