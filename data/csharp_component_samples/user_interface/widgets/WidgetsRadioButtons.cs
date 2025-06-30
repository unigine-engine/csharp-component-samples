using Unigine;

[Component(PropertyGuid = "e4bb213d04b6d5fb523bd6459d00da4eb01a4354")]
public class WidgetsRadioButtons : Component
{
	[ShowInEditor]
	vec2 widgetPosition = new vec2(600, 450);

	[ShowInEditor]
	int fontSize = 16;

	[ShowInEditor]
	int horizontaLayoutSpace = 4;

	[ShowInEditor]
	int verticalLayoutSpace = 4;

	[ShowInEditor]
	string firstButtontext = "Check Me";

	[ShowInEditor]
	string secondButtontext = "Or Me";

	WidgetVBox _verticalLayout = null;

	WidgetCheckBox _firstCheckBox = null;

	WidgetCheckBox _secondCheckBox = null;

	bool _consoleOnScreenState = false;

	void Init()
	{
		var gui = Gui.GetCurrent();

		_verticalLayout = new WidgetVBox(horizontaLayoutSpace, verticalLayoutSpace)
		{
			PositionX = (int)widgetPosition.x,
			PositionY = (int)widgetPosition.y,
			Background = 1 // 1 = true
		};

		gui.AddChild(_verticalLayout, Gui.ALIGN_OVERLAP);

		_firstCheckBox = new WidgetCheckBox(firstButtontext)
		{
			Checked = true, // Set the first checkbox as selected by default
			FontSize = fontSize
		};

		_verticalLayout.AddChild(_firstCheckBox, Gui.ALIGN_LEFT);

		_firstCheckBox.EventClicked.Connect(() =>
		{
			if (_firstCheckBox.Checked)
				Console.OnscreenMessageLine("Radio buttons: first option");
		});
		;

		_secondCheckBox = new WidgetCheckBox(secondButtontext)
		{
			FontSize = fontSize
		};

		_verticalLayout.AddChild(_secondCheckBox, Gui.ALIGN_LEFT);
		_firstCheckBox.AddAttach(_secondCheckBox); // Attach the second checkbox to the first to group them as radio buttons

		_secondCheckBox.EventClicked.Connect(() =>
		{
			if (_secondCheckBox.Checked)
				Console.OnscreenMessageLine("Radio buttons: second option");
		});

		_consoleOnScreenState = Console.Onscreen;
		Console.Onscreen = true; // Enable to see messages in the viewport
	}

	void Shutdown()
	{
		Console.Onscreen = _consoleOnScreenState; // Restore to default state to avoid breaking logic in other worlds
		_verticalLayout.DeleteLater();
	}
}
