using System;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "878de438f109da37b63ff22a9e44fb8151be1eec")]
public class InputGamePadComponent : Component
{
	public int CountGamePads { get; private set; } = 0;
	public int CountActiveGamePads { get; private set; } = 0;

	public class GamepadInfo
	{
		public string name;
		public int number;
		public int playerIndex;
		public Input.DEVICE deviceType;
		public InputGamePad.MODEL_TYPE modelType;
		public bool isAvailable;
		public float filter;
		public Input.GAMEPAD_BUTTON? lastButtonDown;
		public Input.GAMEPAD_BUTTON? lastButtonPressed;
		public Input.GAMEPAD_BUTTON? lastButtonUp;
		public vec2 axesLeft;
		public vec2 axesLeftLastDelta;
		public vec2 axesRight;
		public vec2 axesRightLastDelta;
		public float triggerLeft;
		public float triggerLeftLastDelta;
		public float triggerRight;
		public float triggerRightLastDelta;
	}

	private List<InputGamePad> activeGamepads = null;

	public List<GamepadInfo> GamepadsInfo { get; private set; } = null;

	private Array buttons = null;

	public Action<InputGamePad, int> onTouch;

	private void Init()
	{
		// get all available gamepad buttons
		buttons = Enum.GetValues(typeof(Input.GAMEPAD_BUTTON));

		// set collections of active gamepads and their states
		activeGamepads = new List<InputGamePad>();
		GamepadsInfo = new List<GamepadInfo>();

		InputGamepadUI.filterChanged += OnFilterChanged;
		InputGamepadUI.setVibration += OnSetVibration;
	}

	[MethodUpdate(Order=1)]
	private void Update()
	{
		// update current active gamepads
		if (CountActiveGamePads != Input.NumGamePads)
		{
			activeGamepads.Clear();
			GamepadsInfo.Clear();

			CountActiveGamePads = Input.NumGamePads;
			for (int i = 0; i < CountActiveGamePads; i++)
			{
				InputGamePad gamepad = Input.GetGamePad(i);
				activeGamepads.Add(gamepad);

				GamepadInfo info = new GamepadInfo()
				{
					number = gamepad.Number,
					isAvailable = gamepad.IsAvailable
				};
				GamepadsInfo.Add(info);
			}
		}

		// update information about gamepads
		for (int i = 0; i < CountActiveGamePads; i++)
		{
			GamepadsInfo[i].name = activeGamepads[i].Name;

			GamepadsInfo[i].playerIndex = activeGamepads[i].PlayerIndex;
			GamepadsInfo[i].deviceType = activeGamepads[i].DeviceType;
			GamepadsInfo[i].modelType = activeGamepads[i].ModelType;

			GamepadsInfo[i].isAvailable = activeGamepads[i].IsAvailable;

			if (!GamepadsInfo[i].isAvailable)
				continue;

			GamepadsInfo[i].filter = activeGamepads[i].Filter;

			foreach (var button in buttons)
			{
				Input.GAMEPAD_BUTTON currentButton = (Input.GAMEPAD_BUTTON)button;
				if (currentButton == Input.GAMEPAD_BUTTON.NUM_GAMEPAD_BUTTONS)
					continue;

				// update buttons
				if (activeGamepads[i].IsButtonDown(currentButton))
					GamepadsInfo[i].lastButtonDown = currentButton;

				if (activeGamepads[i].IsButtonPressed(currentButton))
					GamepadsInfo[i].lastButtonPressed = currentButton;

				if (activeGamepads[i].IsButtonUp(currentButton))
					GamepadsInfo[i].lastButtonUp = currentButton;
			}

			// update axes and deltas
			GamepadsInfo[i].axesLeft = activeGamepads[i].AxesLeft;
			if (activeGamepads[i].AxesLeftDelta.Length2 > 0.0f)
				GamepadsInfo[i].axesLeftLastDelta = activeGamepads[i].AxesLeftDelta;

			GamepadsInfo[i].axesRight = activeGamepads[i].AxesRight;
			if (activeGamepads[i].AxesRightDelta.Length2 > 0.0f)
				GamepadsInfo[i].axesRightLastDelta = activeGamepads[i].AxesRightDelta;

			// update tirggers and deltas
			GamepadsInfo[i].triggerLeft = activeGamepads[i].TriggerLeft;
			if (activeGamepads[i].TriggerLeftDelta > 0.0f)
				GamepadsInfo[i].triggerLeftLastDelta = activeGamepads[i].TriggerLeftDelta;

			GamepadsInfo[i].triggerRight = activeGamepads[i].TriggerRight;
			if (activeGamepads[i].TriggerRightDelta > 0.0f)
				GamepadsInfo[i].triggerRightLastDelta = activeGamepads[i].TriggerRightDelta;

			if (activeGamepads[i].NumTouches > 0)
			{
				onTouch.Invoke(activeGamepads[i], i);
			}
		}
	}

	private void Shutdown()
	{
		InputGamepadUI.filterChanged -= OnFilterChanged;
		InputGamepadUI.setVibration -= OnSetVibration;
	}

	private void OnFilterChanged(int number, float value)
	{
		if (0 <= number && number < activeGamepads.Count)
			activeGamepads[number].Filter = value;

		Log.MessageLine($"{value}");
	}

	private void OnSetVibration(int number, float lowFrequency, float highFrequency, float duration)
	{
		if (0 <= number && number < activeGamepads.Count)
			activeGamepads[number].SetVibration(lowFrequency, highFrequency, duration);
	}

}
