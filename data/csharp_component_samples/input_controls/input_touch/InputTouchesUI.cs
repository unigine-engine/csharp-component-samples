using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "8166e8007718bb5f6f9f51324fc1fee9799abb34")]
public class InputTouchesUI : Component
{
	public InputTouches touchesComponent = null;

	[ParameterFile(Filter = ".ui")]
	public string uiFile = "";

	private UserInterface ui = null;

	private WidgetHBox mainHBox = null;
	private WidgetVBox backgroundVBox = null;
	private WidgetLabel maxTouchesCountLabel = null;
	private WidgetLabel touchesCountLabel = null;

	private List<WidgetLabel> xValues = null;
	private List<WidgetLabel> yValues = null;

	private WidgetCanvas canvas = null;
	private List<int> polygonsId = null;
	private List<int> textsId = null;
	private int polygonsHalsSize = 25;

	private vec4[] colors =
	{
		new vec4(0.0f, 0.0f, 0.0f, 1.0f),
		new vec4(0.0f, 0.0f, 1.0f, 1.0f),
		new vec4(0.0f, 1.0f, 0.0f, 1.0f),
		new vec4(0.0f, 1.0f, 1.0f, 1.0f),
		new vec4(1.0f, 0.0f, 0.0f, 1.0f),
		new vec4(1.0f, 0.0f, 1.0f, 1.0f),
		new vec4(1.0f, 1.0f, 0.0f, 1.0f),
		new vec4(1.0f, 1.0f, 1.0f, 1.0f),
		new vec4(0.5f, 0.0f, 0.0f, 1.0f),
		new vec4(0.0f, 0.5f, 1.0f, 1.0f),
		new vec4(0.0f, 1.0f, 0.5f, 1.0f),
		new vec4(0.5f, 1.0f, 1.0f, 1.0f),
		new vec4(1.0f, 0.5f, 0.0f, 1.0f),
		new vec4(1.0f, 0.5f, 1.0f, 1.0f),
		new vec4(1.0f, 1.0f, 0.5f, 1.0f),
		new vec4(1.0f, 0.5f, 1.0f, 1.0f),
		new vec4(0.0f, 0.0f, 0.0f, 0.0f)
	};

	private int lastAppWidth = 0;
	private int lastAppHeight = 0;

	[MethodInit(Order = -1)]
	private void Init()
	{
		EngineWindow main_window = WindowManager.MainWindow;
		if (!main_window)
		{
			Engine.Quit();
			return;
		}

		ivec2 main_size = main_window.Size;

		Input.MouseHandle = Input.MOUSE_HANDLE.USER;

		Gui gui = Gui.GetCurrent();

		ui = new UserInterface(gui, uiFile);

		InitWidget(nameof(mainHBox), out mainHBox);
		InitWidget(nameof(backgroundVBox), out backgroundVBox);
		InitWidget(nameof(canvas), out canvas);
		InitWidget(nameof(maxTouchesCountLabel), out maxTouchesCountLabel);
		InitWidget(nameof(touchesCountLabel), out touchesCountLabel);

		maxTouchesCountLabel.Text = Input.NUM_TOUCHES.ToString();

		xValues = new List<WidgetLabel>();
		for (int i = 0; i < Input.NUM_TOUCHES; i++)
		{
			WidgetLabel xLabel = null;
			InitWidget($"xTouch{i}Label", out xLabel);
			xValues.Add(xLabel);
		}

		yValues = new List<WidgetLabel>();
		for (int i = 0; i < Input.NUM_TOUCHES; i++)
		{
			WidgetLabel yLabel = null;
			InitWidget($"yTouch{i}Label", out yLabel);
			yValues.Add(yLabel);
		}

		gui.AddChild(mainHBox, Gui.ALIGN_CENTER);
		backgroundVBox.BackgroundColor = new vec4(0.0f, 0.0f, 0.0f, 0.5f);

		gui.AddChild(canvas);
		canvas.Width = main_size.x;
		canvas.Height = main_size.y;

		// create polygons and texts for visualization of touches
		polygonsId = new List<int>(Input.NUM_TOUCHES);
		textsId = new List<int>(Input.NUM_TOUCHES);
		for (int i = 0; i < Input.NUM_TOUCHES; i++)
		{
			int id = canvas.AddPolygon();
			polygonsId.Add(id);

			canvas.AddPolygonPoint(id, new vec3(-polygonsHalsSize, -polygonsHalsSize, 0));
			canvas.AddPolygonPoint(id, new vec3(-polygonsHalsSize, polygonsHalsSize, 0));
			canvas.AddPolygonPoint(id, new vec3(polygonsHalsSize, polygonsHalsSize, 0));
			canvas.AddPolygonPoint(id, new vec3(polygonsHalsSize, -polygonsHalsSize, 0));

			canvas.AddPolygonIndex(id, 0);
			canvas.AddPolygonIndex(id, 1);
			canvas.AddPolygonIndex(id, 2);
			canvas.AddPolygonIndex(id, 2);
			canvas.AddPolygonIndex(id, 3);
			canvas.AddPolygonIndex(id, 0);

			canvas.SetPolygonColor(id, vec4.ZERO);

			id = canvas.AddText();
			textsId.Add(id);

			canvas.SetTextText(id, $"Touch {i}");
			canvas.SetTextSize(id, 16);
			canvas.SetTextPosition(id, new vec2(-30, -polygonsHalsSize * 2));
		}
	}

	private void Update()
	{
		EngineWindow mainWindow = WindowManager.MainWindow;
		if (!mainWindow)
		{
			Engine.Quit();
			return;
		}

		ivec2 mainSize = mainWindow.Size;
		ivec2 mainPosition = mainWindow.Position;

		// resize canvas
		int width = mainSize.x;
		int height = mainSize.y;

		if (width != lastAppWidth || height != lastAppHeight)
		{
			lastAppWidth = width;
			lastAppHeight = height;

			canvas.Width = lastAppWidth;
			canvas.Height = lastAppHeight;
		}

		// update polygons, texts and values of touches
		int touchesCount = 0;
		for (int i = 0; i < Input.NUM_TOUCHES; i++)
		{
			int x = touchesComponent.TouchesPositions[i].x;
			int y = touchesComponent.TouchesPositions[i].y;

			vec4 touchColor = vec4.ZERO;
			if (x != -1 || y != -1)
			{
				touchColor = colors[i % (colors.Length - 1)];
				touchesCount++;
			}

			xValues[i].Text = x.ToString();
			yValues[i].Text = y.ToString();

			canvas.SetPolygonTransform(polygonsId[i], MathLib.Translate(new vec3(x - mainPosition.x, y - mainPosition.y, 0)));
			canvas.SetPolygonColor(polygonsId[i], touchColor);

			canvas.SetTextTransform(textsId[i], MathLib.Translate(new vec3(x - mainPosition.x, y - mainPosition.y, 0)));
			canvas.SetTextColor(textsId[i], touchColor);
		}

		touchesCountLabel.Text = touchesCount.ToString();
	}

	private void Shutdown()
	{
		Input.MouseHandle = Input.MOUSE_HANDLE.GRAB;

		if (mainHBox)
			Gui.GetCurrent().RemoveChild(mainHBox);

		if (canvas)
			Gui.GetCurrent().RemoveChild(canvas);
	}

	private void InitWidget<T>(string name, out T widget) where T : Widget
	{
		widget = null;
		int id = ui.FindWidget(name);

		if (id != -1)
			widget = ui.GetWidget(id) as T;
	}
}
