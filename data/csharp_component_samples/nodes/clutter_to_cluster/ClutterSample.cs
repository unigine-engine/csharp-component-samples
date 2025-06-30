using Unigine;

[Component(PropertyGuid = "6734288337043406c8f5fbad0718a928f0fd7a67")]
public class ClutterSample : Component
{
	public Node converterNode = null;

	private ClutterConverter clutter_converter = null;

	private SampleDescriptionWindow sampleDescriptionWindow = new SampleDescriptionWindow();

	private void Init()
	{
		clutter_converter = GetComponent<ClutterConverter>(converterNode);
		if (!clutter_converter)
			Log.Error("ClutterSample.Init(): cannot find ClutterConverter component!\n");
		InitGui();
	}

	private void Shutdown()
	{
		ShutdownGui();
	}
	
	private void InitGui()
	{
		sampleDescriptionWindow.createWindow();
		var parameters = sampleDescriptionWindow.getParameterGroupBox();

		WidgetHBox hbox = new WidgetHBox(5, 0);
		parameters.AddChild(hbox, Gui.ALIGN_CENTER);

		WidgetButton button = new WidgetButton("Generate Clutter");
		button.EventClicked.Connect(GenerateButtonCallback);
		hbox.AddChild(button, Gui.ALIGN_LEFT);

		button = new WidgetButton("Convert to Cluster");
		button.EventClicked.Connect(ConvertButtonCallback);
		hbox.AddChild(button, Gui.ALIGN_LEFT);
	}

	private void ShutdownGui()
	{
		sampleDescriptionWindow.shutdown();
	}

	private void GenerateButtonCallback()
	{
		clutter_converter.generateClutter();
	}

	private void ConvertButtonCallback()
	{
		clutter_converter.ConvertToCluster();
	}
}
