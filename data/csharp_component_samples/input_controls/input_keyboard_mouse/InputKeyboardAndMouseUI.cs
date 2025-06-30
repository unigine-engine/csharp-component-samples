using System;
using Unigine;

[Component(PropertyGuid = "fce2c9811dd594ec64a98b5b827448fe5239dc45")]
public class InputKeyboardAndMouseUI : Component
{
	public InputKeyboardAndMouse inputComponent = null;

	[ParameterFile(Filter = ".ui")]
	public string uiFile = "";

	static public event Action<Input.MOUSE_HANDLE> mouseHandleChanged;

	private UserInterface ui = null;

	private WidgetHBox mainHBox = null;
	private WidgetVBox backgroundVBox = null;

	private WidgetLabel lastInputSymbolLabel = null;
	private WidgetLabel lastKeyDownLabel = null;
	private WidgetLabel lastKeyPressedLabel = null;
	private WidgetLabel lastKeyUpLabel = null;

	private WidgetLabel lastMouseDownLabel = null;
	private WidgetLabel lastMousePressedLabel = null;
	private WidgetLabel lastMouseUpLabel = null;

	private WidgetLabel mouseCoordXLabel = null;
	private WidgetLabel mouseCoordYLabel = null;

	private WidgetLabel lastMouseCoordDeltaXLabel = null;
	private WidgetLabel lastMouseCoordDeltaYLabel = null;

	private WidgetLabel lastMouseDeltaXLabel = null;
	private WidgetLabel lastMouseDeltaYLabel = null;

	private WidgetLabel lastWheelVerticalLabel = null;
	private WidgetLabel lastWheelHorizontalLabel = null;

	private WidgetLabel mouseHandleLabel = null;
	private WidgetComboBox mouseHandleCombobox = null;

	[MethodInit(Order = -1)]
	private void Init()
	{
		Input.MouseHandle = Input.MOUSE_HANDLE.USER;

		Gui gui = Gui.GetCurrent();

		ui = new UserInterface(gui, uiFile);

		InitWidget(nameof(mainHBox), out mainHBox);
		InitWidget(nameof(backgroundVBox), out backgroundVBox);
		InitWidget(nameof(lastInputSymbolLabel), out lastInputSymbolLabel);
		InitWidget(nameof(lastKeyDownLabel), out lastKeyDownLabel);
		InitWidget(nameof(lastKeyPressedLabel), out lastKeyPressedLabel);
		InitWidget(nameof(lastKeyUpLabel), out lastKeyUpLabel);
		InitWidget(nameof(lastMouseDownLabel), out lastMouseDownLabel);
		InitWidget(nameof(lastMousePressedLabel), out lastMousePressedLabel);
		InitWidget(nameof(lastMouseUpLabel), out lastMouseUpLabel);
		InitWidget(nameof(mouseCoordXLabel), out mouseCoordXLabel);
		InitWidget(nameof(mouseCoordYLabel), out mouseCoordYLabel);
		InitWidget(nameof(lastMouseCoordDeltaXLabel), out lastMouseCoordDeltaXLabel);
		InitWidget(nameof(lastMouseCoordDeltaYLabel), out lastMouseCoordDeltaYLabel);
		InitWidget(nameof(lastMouseDeltaXLabel), out lastMouseDeltaXLabel);
		InitWidget(nameof(lastMouseDeltaYLabel), out lastMouseDeltaYLabel);
		InitWidget(nameof(lastWheelVerticalLabel), out lastWheelVerticalLabel);
		InitWidget(nameof(lastWheelHorizontalLabel), out lastWheelHorizontalLabel);
		InitWidget(nameof(mouseHandleLabel), out mouseHandleLabel);
		InitWidget(nameof(mouseHandleCombobox), out mouseHandleCombobox);

		gui.AddChild(mainHBox);
		backgroundVBox.BackgroundColor = new vec4(0.0f, 0.0f, 0.0f, 0.5f);

		mouseHandleCombobox.CurrentItem = (int)Input.MOUSE_HANDLE.USER;

		mouseHandleCombobox.EventChanged.Connect(() =>
		{
			mouseHandleChanged?.Invoke((Input.MOUSE_HANDLE)mouseHandleCombobox.CurrentItem);
		});
	}

	private void Update()
	{
		if (inputComponent == null)
			return;

		lastInputSymbolLabel.Text = inputComponent.LastInputSymbol;
		lastKeyDownLabel.Text = inputComponent.LastKeyDown.ToString();
		lastKeyPressedLabel.Text = inputComponent.LastKeyPressed.ToString();
		lastKeyUpLabel.Text = inputComponent.LastKeyUp.ToString();

		lastMouseDownLabel.Text = inputComponent.LastMouseButtonDown.ToString();
		lastMousePressedLabel.Text = inputComponent.LastMouseButtonPressed.ToString();
		lastMouseUpLabel.Text = inputComponent.LastMouseButtonUp.ToString();

		mouseCoordXLabel.Text = inputComponent.MouseCoord?.x.ToString();
		mouseCoordYLabel.Text = inputComponent.MouseCoord?.y.ToString();

		lastMouseCoordDeltaXLabel.Text = inputComponent.LastMouseCoordDelta?.x.ToString();
		lastMouseCoordDeltaYLabel.Text = inputComponent.LastMouseCoordDelta?.y.ToString();

		lastMouseDeltaXLabel.Text = inputComponent.LastMouseDelta?.x.ToString("0.000");
		lastMouseDeltaYLabel.Text = inputComponent.LastMouseDelta?.y.ToString("0.000");

		lastWheelVerticalLabel.Text = inputComponent.LastMouseWheel?.ToString();
		lastWheelHorizontalLabel.Text = inputComponent.LastMouseWheelHorizontal?.ToString();

		mouseHandleLabel.Text = inputComponent.MouseHandle?.ToString();
		if (inputComponent.MouseHandle == Input.MOUSE_HANDLE.GRAB && Input.MouseGrab == true)
			mouseHandleLabel.Text += " (press ESC to show cursor)";
	}

	private void Shutdown()
	{
		Input.MouseHandle = Input.MOUSE_HANDLE.GRAB;

		if (mainHBox)
			Gui.GetCurrent().RemoveChild(mainHBox);
	}

	private void InitWidget<T>(string name, out T widget) where T : Widget
	{
		widget = null;
		int id = ui.FindWidget(name);

		if (id != -1)
			widget = ui.GetWidget(id) as T;
	}
}
