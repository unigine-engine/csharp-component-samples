using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "3dbf3dd482e5e570fb4168689d535522a7a0fc17")]
public class TrajectoryLogic : Component
{
	[ShowInEditor]
	private Node airplane1 = null;
	[ShowInEditor]
	private Node airplane2 = null;
	[ShowInEditor]
	private Node airplane3 = null;

	private WidgetCheckBox enableVisualizePath;

	private enum Players
	{
		MAIN = 0,
		ONE,
		TWO,
		THREE,
		TOTAL_PLAYERS
	}

	private List<Player> mainPlayers = new List<Player>();
	Players currentPlayer = Players.MAIN;

	SampleDescriptionWindow sampleDescriptionWindow = new SampleDescriptionWindow();

	void Init()
	{
		Game.GetMainPlayers(mainPlayers);

		Game.Player = mainPlayers[(int)currentPlayer];

		InitGui();
		Visualizer.Enabled = true;
	}

	void Shutdown()
	{
		sampleDescriptionWindow.shutdown();
		Visualizer.Enabled = false;
	}

	private void InitGui()
	{
		sampleDescriptionWindow.createWindow();

		sampleDescriptionWindow.addFloatParameter("Velocity", "Velicity", 5.0f, 1.0f, 50.0f, (float v) => {
			GetComponent<SimpleTrajectoryMovement>(airplane1).Velocity = v;
			GetComponent<SplineTrajectoryMovement>(airplane2).Velocity = v;
			GetComponent<SavedPathTrajectory>(airplane3).Velocity = v;
		});

		WidgetGroupBox parameters = sampleDescriptionWindow.getParameterGroupBox();

		var changeCameraButton = new WidgetButton();
		parameters.AddChild(changeCameraButton, Gui.ALIGN_LEFT);
		changeCameraButton.Text = "Switch Camera";
		changeCameraButton.EventClicked.Connect(SwitchTrajectoryCallback);

		var gridbox = new WidgetGridBox();
		parameters.AddChild(gridbox, Gui.ALIGN_LEFT);
		enableVisualizePath = new WidgetCheckBox();
		enableVisualizePath.Checked = false;
		enableVisualizePath.EventClicked.Connect(EnableVisualizeCallback);
		var visualizeLabel = new WidgetLabel("Visualize Path");
		gridbox.AddChild(visualizeLabel);
		gridbox.AddChild(enableVisualizePath);
	}

	private void SwitchTrajectoryCallback()
	{
		currentPlayer = (Players)(((int)currentPlayer + 1) % (int)Players.TOTAL_PLAYERS);
		Game.Player = mainPlayers[(int)currentPlayer];
	}

	private void EnableVisualizeCallback()
	{
		bool enabled = enableVisualizePath.Checked;
		GetComponent<SimpleTrajectoryMovement>(airplane1).Debug = enabled;
		GetComponent<SplineTrajectoryMovement>(airplane2).Debug = enabled;
		GetComponent<SavedPathTrajectory>(airplane3).Debug = enabled;
	}
}
