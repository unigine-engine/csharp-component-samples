using Unigine;

[Component(PropertyGuid = "3a4bfa242c0f8179abfaded08f54d57662e23496")]
public class VisualizerSample : Component
{
	[ShowInEditor]
	[Parameter(Title = "Visualizer usage")]
	private VisualizerUsage visualizer_usage = null;
	private SampleDescriptionWindow window = null;

	void Init()
	{
		window = new SampleDescriptionWindow();
		window.createWindow();
		var parameters = window.getParameterGroupBox();
		//========== Enable visualizer checkbox =========//
		WidgetCheckBox visualizer_check_box = new WidgetCheckBox("Enable visualizer");
		parameters.AddChild(visualizer_check_box, Gui.ALIGN_LEFT);
		visualizer_check_box.EventChanged.Connect(() =>
		{
			Visualizer.Enabled = visualizer_check_box.Checked;
		});
		visualizer_check_box.Checked = true;

		//========== Enable visualizer checkbox =========//
		WidgetCheckBox depth_test_check_box = new WidgetCheckBox("Enable depth test");
		parameters.AddChild(depth_test_check_box, Gui.ALIGN_LEFT);
		depth_test_check_box.EventChanged.Connect(() =>
		{
			if (depth_test_check_box.Checked)
			{
				Visualizer.Mode = Visualizer.MODE.ENABLED_DEPTH_TEST_ENABLED;
			}
			else
			{
				Visualizer.Mode = Visualizer.MODE.ENABLED_DEPTH_TEST_DISABLED;
			}
		});
		depth_test_check_box.Checked = true;

		//========== Enable point2D checkbox =========//
		WidgetCheckBox point2D_check_box = new WidgetCheckBox("Point2D");
		parameters.AddChild(point2D_check_box, Gui.ALIGN_LEFT);
		point2D_check_box.EventChanged.Connect(() =>
		{
			visualizer_usage.renderPoint2D = point2D_check_box.Checked;
		});
		point2D_check_box.Checked = visualizer_usage.renderPoint2D;

		//========== Enable line2D checkbox =========//
		WidgetCheckBox line2D_check_box = new WidgetCheckBox("Line2D");
		parameters.AddChild(line2D_check_box, Gui.ALIGN_LEFT);
		line2D_check_box.EventChanged.Connect(() =>
		{
			visualizer_usage.renderLine2D = line2D_check_box.Checked;
		});
		line2D_check_box.Checked = visualizer_usage.renderPoint2D;

		//========== Enable triangle2D checkbox =========//
		WidgetCheckBox triangle2D_check_box = new WidgetCheckBox("Triangle2D");
		parameters.AddChild(triangle2D_check_box, Gui.ALIGN_LEFT);
		triangle2D_check_box.EventChanged.Connect(() =>
		{
			visualizer_usage.renderTriangle2D = triangle2D_check_box.Checked;
		});
		triangle2D_check_box.Checked = visualizer_usage.renderTriangle2D;

		//========== Enable quad2D checkbox =========//
		WidgetCheckBox quad2D_check_box = new WidgetCheckBox("Quad2D");
		parameters.AddChild(quad2D_check_box, Gui.ALIGN_LEFT);
		quad2D_check_box.EventChanged.Connect(() =>
		{
			visualizer_usage.renderQuad2D = quad2D_check_box.Checked;
		});
		quad2D_check_box.Checked = visualizer_usage.renderQuad2D;

		//========== Enable rectangle checkbox =========//
		WidgetCheckBox rectangle_check_box = new WidgetCheckBox("Rectangle");
		parameters.AddChild(rectangle_check_box, Gui.ALIGN_LEFT);
		rectangle_check_box.EventChanged.Connect(() =>
		{
			visualizer_usage.renderRectangle = rectangle_check_box.Checked;
		});
		rectangle_check_box.Checked = visualizer_usage.renderRectangle;

		//========== Enable message2D checkbox =========//
		WidgetCheckBox message2D_check_box = new WidgetCheckBox("Message2D");
		parameters.AddChild(message2D_check_box, Gui.ALIGN_LEFT);
		message2D_check_box.EventChanged.Connect(() =>
		{
			visualizer_usage.renderMessage2D = message2D_check_box.Checked;
		});
		message2D_check_box.Checked = visualizer_usage.renderMessage2D;
	}
	void Shutdown()
	{
		window.shutdown();
	}
}
