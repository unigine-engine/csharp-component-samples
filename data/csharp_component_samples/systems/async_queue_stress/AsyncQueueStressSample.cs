#region Math Variables
#if UNIGINE_DOUBLE
using Scalar = System.Double;
using Vec2 = Unigine.dvec2;
using Vec3 = Unigine.dvec3;
using Vec4 = Unigine.dvec4;
using Mat4 = Unigine.dmat4;
#else
using Scalar = System.Single;
using Vec2 = Unigine.vec2;
using Vec3 = Unigine.vec3;
using Vec4 = Unigine.vec4;
using Mat4 = Unigine.mat4;
using WorldBoundBox = Unigine.BoundBox;
using WorldBoundSphere = Unigine.BoundSphere;
using WorldBoundFrustum = Unigine.BoundFrustum;
#endif
#endregion

using System.Threading;
using Unigine;

[Component(PropertyGuid = "a8bfb39278ee1070f8d9f70d5e424754152a26ed")]
public class AsyncQueueStressSample : Component
{
	[ShowInEditor][ParameterFile]
	private string nodeToSpawn = null;

	private int numNodesLoaded;

	private WidgetLabel numNodesLoadedLabel;

	private SampleDescriptionWindow sampleDescriptionWindow = new SampleDescriptionWindow();

	void Init()
	{
		Profiler.Enabled = true;

		numNodesLoaded = 0;

		sampleDescriptionWindow.createWindow(Gui.ALIGN_RIGHT);

		WidgetGroupBox parameters = sampleDescriptionWindow.getParameterGroupBox();

		var numNodesHBox = new WidgetHBox(5);
		parameters.AddChild(numNodesHBox, Gui.ALIGN_EXPAND);

		var multithreadLabel = new WidgetLabel("Num nodes");
		numNodesHBox.AddChild(multithreadLabel);

		var spinboxHBox = new WidgetHBox();
		var editline = new WidgetEditLine();
		editline.Validator = Gui.VALIDATOR_INT;
		var spinbox = new WidgetSpinBox();
		editline.AddAttach(spinbox);
		spinbox.MinValue = 1;
		spinbox.MaxValue = 10000;
		spinbox.Value = 100;
		spinboxHBox.AddChild(editline);
		spinboxHBox.AddChild(spinbox);
		numNodesHBox.AddChild(spinboxHBox, Gui.ALIGN_RIGHT);

		var requestLoadNodesButton = new WidgetButton("Request Load Nodes Async");
		parameters.AddChild(requestLoadNodesButton, Gui.ALIGN_EXPAND);

		numNodesLoadedLabel = new WidgetLabel();
		parameters.AddChild(numNodesLoadedLabel, Gui.ALIGN_EXPAND);

		requestLoadNodesButton.EventClicked.Connect(() =>
		{
			AsyncQueue.RunAsync(AsyncQueue.ASYNC_THREAD.BACKGROUND, () => { LoadNodes(spinbox.Value); });
		});
	}

	void Update()
	{
		numNodesLoadedLabel.Text = "Num nodes loaded " + numNodesLoaded.ToString();
		if (numNodesLoaded > 2000)
			numNodesLoadedLabel.FontColor = vec4.RED;
	}

	void Shutdown()
	{
		Profiler.Enabled = false;

		sampleDescriptionWindow.shutdown();
	}

	private void LoadNodes(int num)
	{
		for (int i = 0; i < num; ++i)
		{
			// here we are loading the node not in the main thread, so it will not be added to the spatial tree
			Node loadedNode = World.LoadNode(nodeToSpawn, false);
			Vec3 position = new Vec3();
			position.x = Game.GetRandomFloat(-100.0f, 100.0f);
			position.y = Game.GetRandomFloat(-100.0f, 100.0f);
			position.z = Game.GetRandomFloat(0.0f, 50.0f);
			loadedNode.WorldPosition = position;

			Interlocked.Add(ref numNodesLoaded, 1);

			AsyncQueue.RunAsync(AsyncQueue.ASYNC_THREAD.MAIN, () =>
			{
				// call updateEnabled which will recursively go through all the children and add them to the spatial tree
				loadedNode.UpdateEnabled();
			});
	}
}
}
