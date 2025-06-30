using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Unigine;

[Component(PropertyGuid = "a10fa015460f72701fc0630f2d22f5226532f219")]
public class InputKeyboardAndMouse : Component
{
	public string LastInputSymbol { get; private set; } = null;
	public Input.KEY? LastKeyDown { get; private set; } = null;
	public Input.KEY? LastKeyPressed { get; private set; } = null;
	public Input.KEY? LastKeyUp { get; private set; } = null;

	public Input.MOUSE_BUTTON? LastMouseButtonDown { get; private set; } = null;
	public Input.MOUSE_BUTTON? LastMouseButtonPressed { get; private set; } = null;
	public Input.MOUSE_BUTTON? LastMouseButtonUp { get; private set; } = null;

	public ivec2? MouseCoord { get; private set; } = null;
	public ivec2? LastMouseCoordDelta { get; private set; } = null;
	public vec2? LastMouseDelta { get; private set; } = null;

	public int? LastMouseWheel { get; private set; } = null;
	public int? LastMouseWheelHorizontal { get; private set; } = null;

	public Input.MOUSE_HANDLE? MouseHandle { get; private set; } = null;

	private Array keys = null;
	private Array mouseButtons = null;

	private HashSet<Input.KEY> pressedKeys = null;
	private HashSet<Input.MOUSE_BUTTON> pressedMouseButtons = null;

	private void Init()
	{
		keys = Enum.GetValues(typeof(Input.KEY));
		mouseButtons = Enum.GetValues(typeof(Input.MOUSE_BUTTON));

		pressedKeys = new HashSet<Input.KEY>();
		pressedMouseButtons = new HashSet<Input.MOUSE_BUTTON>();

		InputKeyboardAndMouseUI.mouseHandleChanged += OnMouseHandleChanged;

		Input.EventTextPress.Connect(OnTextPressed);
	}

	private void Update()
	{
		// update keyboards
		foreach (var key in keys)
		{
			Input.KEY currentKey = (Input.KEY)key;

			// skip any ANY_* keys
			if (currentKey >= Input.KEY.ANY_SHIFT)
				continue;

			if (Input.IsKeyDown(currentKey))
				LastKeyDown = currentKey;

			if (Input.IsKeyPressed(currentKey) && !pressedKeys.Contains(currentKey))
			{
				LastKeyPressed = currentKey;
				pressedKeys.Add(currentKey);
			}

			if (Input.IsKeyUp(currentKey))
			{
				LastKeyUp = currentKey;
				pressedKeys.Remove(currentKey);
			}
		}

		// update mouse buttons
		foreach (var button in mouseButtons)
		{
			Input.MOUSE_BUTTON currentButton = (Input.MOUSE_BUTTON)button;
			if (currentButton == Input.MOUSE_BUTTON.MOUSE_NUM_BUTTONS)
				continue;

			if (Input.IsMouseButtonDown(currentButton))
				LastMouseButtonDown = currentButton;

			if (Input.IsMouseButtonPressed(currentButton) && !pressedMouseButtons.Contains(currentButton))
			{
				LastMouseButtonPressed = currentButton;
				pressedMouseButtons.Add(currentButton);
			}

			if (Input.IsMouseButtonUp(currentButton))
			{
				LastMouseButtonUp = currentButton;
				pressedMouseButtons.Remove(currentButton);
			}
		}

		// update mouse coords and deltas
		MouseCoord = Input.MousePosition;

		if (Input.MouseDeltaPosition.Length2 > 0)
			LastMouseCoordDelta = Input.MouseDeltaPosition;

		if (Input.MouseDeltaPosition.Length2 > 0)
			LastMouseDelta = Input.MouseDeltaPosition;

		if (Input.MouseWheel != 0)
			LastMouseWheel = Input.MouseWheel;

		if (Input.MouseWheelHorizontal != 0)
			LastMouseWheelHorizontal = Input.MouseWheelHorizontal;

		MouseHandle = Input.MouseHandle;
	}

	private void Shutdown()
	{
		InputKeyboardAndMouseUI.mouseHandleChanged -= OnMouseHandleChanged;
	}

	private void OnMouseHandleChanged(Input.MOUSE_HANDLE handle)
	{
		Input.MouseHandle = handle;
	}

	private void OnTextPressed(uint unicode)
	{
		byte[] bytes = BitConverter.GetBytes(unicode);
		LastInputSymbol = Encoding.Unicode.GetString(bytes);
	}
}
