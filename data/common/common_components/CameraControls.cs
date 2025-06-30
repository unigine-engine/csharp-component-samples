using Unigine;

[Component(PropertyGuid = "20ff288c8937ca7b34f43d9e41d10865af419e80")]
public class CameraControls : Component
{
	public float mouseSensitivity = 1.25f;
	public float mouseWheelSensitivity = 10.0f;
	public Input.KEY forwardKey = Input.KEY.W;
	public Input.KEY backwardKey = Input.KEY.S;
	public Input.KEY rightKey = Input.KEY.D;
	public Input.KEY leftKey = Input.KEY.A;
	public Input.KEY upKey = Input.KEY.E;
	public Input.KEY downKey = Input.KEY.Q;
	public Input.KEY accelerationKey = Input.KEY.ANY_SHIFT;

	public float gamepadDeadZone = 0.2f;

	public GAMEPAD_AXES gamepadForwardAxis = GAMEPAD_AXES.LEFT_AXIS_UP;
	public GAMEPAD_AXES gamepadBackwardAxis = GAMEPAD_AXES.LEFT_AXIS_DOWN;
	public GAMEPAD_AXES gamepadRightAxis = GAMEPAD_AXES.LEFT_AXIS_RIGHT;
	public GAMEPAD_AXES gamepadLeftAxis = GAMEPAD_AXES.LEFT_AXIS_LEFT;
	public GAMEPAD_AXES gamepadUpAxis = GAMEPAD_AXES.RIGHT_TRIGGER;
	public GAMEPAD_AXES gamepadDownAxis = GAMEPAD_AXES.LEFT_TRIGGER;
	public GAMEPAD_AXES gamepadTurnRightAxis = GAMEPAD_AXES.RIGHT_AXIS_RIGHT;
	public GAMEPAD_AXES gamepadTurnLeftAxis = GAMEPAD_AXES.RIGHT_AXIS_LEFT;
	public GAMEPAD_AXES gamepadTurnUpAxis = GAMEPAD_AXES.RIGHT_AXIS_UP;
	public GAMEPAD_AXES gamepadTurnDownAxis = GAMEPAD_AXES.RIGHT_AXIS_DOWN;
	public Input.GAMEPAD_BUTTON gamepadAccelerationButton = Input.GAMEPAD_BUTTON.SHOULDER_RIGHT;
	public GAMEPAD_AXES gamepadZoomInAxis = GAMEPAD_AXES.RIGHT_TRIGGER;
	public GAMEPAD_AXES gamepadZoomOutAxis = GAMEPAD_AXES.LEFT_TRIGGER;

	public float Forward { get; private set; } = 0.0f;
	public float Backward { get; private set; } = 0.0f;
	public float Right { get; private set; } = 0.0f;
	public float Left { get; private set; } = 0.0f;
	public float Up { get; private set; } = 0.0f;
	public float Down { get; private set; } = 0.0f;
	public float TurnRight { get; private set; } = 0.0f;
	public float TurnLeft { get; private set; } = 0.0f;
	public float TurnUp { get; private set; } = 0.0f;
	public float TurnDown { get; private set; } = 0.0f;
	public float Acceleration { get; private set; } = 1.0f;
	public float ZoomIn { get; private set; } = 0.0f;
	public float ZoomOut { get; private set; } = 0.0f;

	public enum GAMEPAD_AXES
	{
		LEFT_AXIS_UP,
		LEFT_AXIS_DOWN,
		LEFT_AXIS_RIGHT,
		LEFT_AXIS_LEFT,
		RIGHT_AXIS_UP,
		RIGHT_AXIS_DOWN,
		RIGHT_AXIS_RIGHT,
		RIGHT_AXIS_LEFT,
		LEFT_TRIGGER,
		RIGHT_TRIGGER,
		NONE
	}

	private InputGamePad gamepad = null;

	private void Init()
	{
		for (int i = 0; i < Input.NumGamePads; i++)
		{
			Unigine.InputGamePad game_pad = Input.GetGamePad(i);
			if (game_pad.IsAvailable)
			{
				gamepad = game_pad;
				break;
			}
		}
	}

	private void Update()
	{
		Forward = 0.0f;
		Backward = 0.0f;
		Right = 0.0f;
		Left = 0.0f;
		Up = 0.0f;
		Down = 0.0f;
		TurnRight = 0.0f;
		TurnLeft = 0.0f;
		TurnUp = 0.0f;
		TurnDown = 0.0f;
		Acceleration = 1.0f;
		ZoomIn = 0.0f;
		ZoomOut = 0.0f;

		UpdateKeyboardAndMouse();
		UpdateGamepad();
	}

	private void UpdateKeyboardAndMouse()
	{
		if (!Input.MouseGrab)
			return;

		if (Input.IsKeyPressed(forwardKey))
			Forward = MathLib.Max(Forward, 1.0f);

		if (Input.IsKeyPressed(backwardKey))
			Backward = MathLib.Max(Backward, 1.0f);

		if (Input.IsKeyPressed(rightKey))
			Right = MathLib.Max(Right, 1.0f);

		if (Input.IsKeyPressed(leftKey))
			Left = MathLib.Max(Left, 1.0f);

		if (Input.IsKeyPressed(upKey))
			Up = MathLib.Max(Up, 1.0f);

		if (Input.IsKeyPressed(downKey))
			Down = MathLib.Max(Down, 1.0f);

		vec2 delta = Input.MouseDeltaPosition * 0.1f;

		if (delta.x > 0)
			TurnRight = MathLib.Max(TurnRight, delta.x * mouseSensitivity);
		else
			TurnLeft = MathLib.Max(TurnLeft, -delta.x * mouseSensitivity);

		if (delta.y > 0)
			TurnDown = MathLib.Max(TurnUp, delta.y * mouseSensitivity);
		else
			TurnUp = MathLib.Max(TurnDown, -delta.y * mouseSensitivity);

		if (Input.IsKeyPressed(accelerationKey))
			Acceleration = MathLib.Max(Acceleration, 2.0f);

		if (Input.MouseWheel > 0)
			ZoomIn = MathLib.Max(ZoomIn, Input.MouseWheel * mouseWheelSensitivity);
		else
			ZoomOut = MathLib.Max(ZoomOut, -Input.MouseWheel * mouseWheelSensitivity);
	}

	private void UpdateGamepad()
	{
		if (!gamepad)
			return;

		Right = MathLib.Max(GetGamepadAxis(gamepadRightAxis), Right);
		Left = MathLib.Max(GetGamepadAxis(gamepadLeftAxis), Left);

		Forward = MathLib.Max(GetGamepadAxis(gamepadForwardAxis), Forward);
		Backward = MathLib.Max(GetGamepadAxis(gamepadBackwardAxis), Backward);

		Up = MathLib.Max(GetGamepadAxis(gamepadUpAxis), Up);
		Down = MathLib.Max(GetGamepadAxis(gamepadDownAxis), Down);

		TurnRight = MathLib.Max(GetGamepadAxis(gamepadTurnRightAxis), TurnRight);
		TurnLeft = MathLib.Max(GetGamepadAxis(gamepadTurnLeftAxis), TurnLeft);

		TurnUp = MathLib.Max(GetGamepadAxis(gamepadTurnUpAxis), TurnUp);
		TurnDown = MathLib.Max(GetGamepadAxis(gamepadTurnDownAxis), TurnDown);

		if (gamepad.IsButtonPressed(gamepadAccelerationButton))
			Acceleration = MathLib.Max(Acceleration, 2.0f);

		ZoomIn = MathLib.Max(GetGamepadAxis(gamepadZoomInAxis), ZoomIn);
		ZoomOut = MathLib.Max(GetGamepadAxis(gamepadZoomOutAxis), ZoomOut);
	}

	private float GetGamepadAxis(GAMEPAD_AXES axis)
	{
		if (!gamepad)
			return 0.0f;

		float value = 0.0f;
		switch (axis)
		{
			case GAMEPAD_AXES.LEFT_AXIS_UP: value = MathLib.Clamp(gamepad.AxesLeft.y, 0.0f, 1.0f); break;
			case GAMEPAD_AXES.LEFT_AXIS_DOWN: value = -MathLib.Clamp(gamepad.AxesLeft.y, -1.0f, 0.0f); break;
			case GAMEPAD_AXES.LEFT_AXIS_RIGHT: value = MathLib.Clamp(gamepad.AxesLeft.x, 0.0f, 1.0f); break;
			case GAMEPAD_AXES.LEFT_AXIS_LEFT: value = -MathLib.Clamp(gamepad.AxesLeft.x, -1.0f, 0.0f); break;
			case GAMEPAD_AXES.RIGHT_AXIS_UP: value = MathLib.Clamp(gamepad.AxesRight.y, 0.0f, 1.0f); break;
			case GAMEPAD_AXES.RIGHT_AXIS_DOWN: value = -MathLib.Clamp(gamepad.AxesRight.y, -1.0f, 0.0f); break;
			case GAMEPAD_AXES.RIGHT_AXIS_RIGHT: value = MathLib.Clamp(gamepad.AxesRight.x, 0.0f, 1.0f); break;
			case GAMEPAD_AXES.RIGHT_AXIS_LEFT: value = -MathLib.Clamp(gamepad.AxesRight.x, -1.0f, 0.0f); break;
			case GAMEPAD_AXES.LEFT_TRIGGER: value = gamepad.TriggerLeft; break;
			case GAMEPAD_AXES.RIGHT_TRIGGER: value = gamepad.TriggerRight; break;
			default: break;
		}

		return MathLib.Max(gamepadDeadZone, value);
	}
}
