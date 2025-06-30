using Unigine;

[Component(PropertyGuid = "0bf2c8c22c3dc582e4207a776cee495b07b0b055")]
public class MicroprofilerSample : Component
{
	private Engine.BACKGROUND_UPDATE previous_bg_update = Engine.BACKGROUND_UPDATE.BACKGROUND_UPDATE_DISABLED;
	private SampleDescriptionWindow sampleDescriptionWindow = new();

	void Init()
	{	
		string description;

		if (Profiler.MicroprofileUrl == "")
		{
			WindowManager.DialogError("Warning", "Microprofiler is not available!");
			description = "<font color=\"#de4a14\"><p>Microprofiler is not compiled.</p><p>Use development-release binaries.</p></font>";
		}
		else
		{
			description = "<p>Microprofiler url <font color=\"#de4a14\">" + Profiler.MicroprofileUrl + "</font></p>";
		}

		sampleDescriptionWindow.createWindow();

		WidgetLabel label = new(description)
		{
			FontRich = 1,
			FontWrap = 1,
			Width = 300
		};
		sampleDescriptionWindow.getParameterGroupBox().AddChild(label, Gui.ALIGN_LEFT);
		
		previous_bg_update = Engine.BackgroundUpdate;
		Engine.BackgroundUpdate = Engine.BACKGROUND_UPDATE.BACKGROUND_UPDATE_RENDER_NON_MINIMIZED;		
	}

	void Shutdown()
	{
		Engine.BackgroundUpdate = previous_bg_update;
		sampleDescriptionWindow.shutdown();
	}
}
