using System.Globalization;
using Unigine;

[Component(PropertyGuid = "89571ccf605ba9481e3600165449e4341baa6d50")]
public class TargetGui : Component
{
	private ObjectGui objectGui;
	private Gui gui;
	private WidgetLabel distanceLabel;

	void Init()
	{
		objectGui = node as ObjectGui;
		if (!objectGui)
		{
			Log.Error("TargetGui.Init is not ObjectGui!\n");
		}
		gui = objectGui.GetGui();

		WidgetWindow window = new WidgetWindow();
		gui.AddChild(window, Gui.ALIGN_CENTER);

		WidgetVBox vbox = new WidgetVBox();
		vbox.SetSpace(1, 3);
		window.AddChild(vbox, Gui.ALIGN_CENTER);
		window.Width = gui.Width;
		window.Height = gui.Height;


		WidgetLabel targetLabel = new WidgetLabel(node.Name);
		targetLabel.FontSize = 50;
		vbox.AddChild(targetLabel);

		double distance = (node.WorldPosition - Game.Player.WorldPosition).Length;

		distanceLabel = new WidgetLabel("Distance to label : " + distance.ToString("0.00", CultureInfo.InvariantCulture) + " units");
		distanceLabel.FontSize = 50;
		vbox.AddChild(distanceLabel);

		WidgetLabel sizeLabel = new WidgetLabel("Size of label : " + objectGui.PhysicalWidth + "x" + objectGui.PhysicalHeight);
		sizeLabel.FontSize = 50;
		vbox.AddChild(sizeLabel);
	}

	void Update()
	{
		double distance = (node.WorldPosition - Game.Player.WorldPosition).Length;
		distanceLabel.Text = "Distance to label : " + distance.ToString("0.00", CultureInfo.InvariantCulture) + " units";
	}
}
