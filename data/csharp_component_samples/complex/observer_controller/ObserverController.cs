using System;
using System.Collections.Generic;
using Unigine;
using System.Linq;

#region Math
#if UNIGINE_DOUBLE
using Vec3 = Unigine.dvec3;
using Mat4 = Unigine.dmat4;
#else
using Vec3 = Unigine.vec3;
using Mat4 = Unigine.mat4;
#endif
#endregion

[Component(PropertyGuid = "3bf2472427c699325eb25ef019b9a56619cd1934")]
public class ObserverController : Component
{
	[ShowInEditor, Parameter(Group = "Input", Title = "Toggle Camera Menu", Tooltip = "Key to toggle the visibility of the camera menu.")]
	Input.KEY toggleCameraMenu = Input.KEY.F3;

	[ShowInEditor, Parameter(Group = "Input", Title = "Focusing Key", Tooltip = "Key to focus the camera on the object, zooming in for a closer view.")]
	Input.KEY focusKey = Input.KEY.F;

	[ShowInEditor, Parameter(Group = "Input", Title = "Camera Control Modifier", Tooltip = "Modifier key for switching between different camera control modes.")]
	Input.MODIFIER altCameraModKey = Input.MODIFIER.ANY_ALT;

	[ShowInEditor, Parameter(Group = "Input", Title = "Acceleration Key", Tooltip = "Key to activate acceleration mode, typically used for faster movement.")]
	Input.KEY accelerationKey = Input.KEY.ANY_SHIFT;

	[ShowInEditor, Parameter(Group = "Input", Title = "Spectator Mode Mouse Button", Tooltip = "Mouse button to toggle spectator mode.")]
	Input.MOUSE_BUTTON spectatorModeMouseButton = Input.MOUSE_BUTTON.RIGHT;

	[ShowInEditor, Parameter(Group = "Input", Title = "Rail Mode Mouse Button", Tooltip = "Mouse button to activate rail mode.")]
	Input.MOUSE_BUTTON railModeMouseButton = Input.MOUSE_BUTTON.RIGHT;

	[ShowInEditor, Parameter(Group = "Input", Title = "Panning Mode Mouse Button", Tooltip = "Mouse button to initiate panning mode.")]
	Input.MOUSE_BUTTON panningModeMouseButton = Input.MOUSE_BUTTON.MIDDLE;

	[ShowInEditor, Parameter(Group = "Input", Title = "Panning/Rail Scale", Tooltip = "Scale factor for panning and rail modes, controlling the movement speed.")]
	float panningRailScale = 0.01f;

	[ShowInEditor, Parameter(Group = "Velocity", Title = "First Gear Key", Tooltip = "Key to activate the first gear (low speed).")]
	Input.KEY firstGearKey = Input.KEY.DIGIT_1;

	[ShowInEditor, Parameter(Group = "Velocity", Title = "First Gear Speed", Tooltip = "Velocity for the first gear.")]
	float firstGearVelocity = 5.0f;

	[ShowInEditor, Parameter(Group = "Velocity", Title = "Second Gear Key", Tooltip = "Key to activate the second gear (medium speed).")]
	Input.KEY secondGearKey = Input.KEY.DIGIT_2;

	[ShowInEditor, Parameter(Group = "Velocity", Title = "Second Gear Speed", Tooltip = "Velocity for the second gear.")]
	float secondGearVelocity = 50.0f;

	[ShowInEditor, Parameter(Group = "Velocity", Title = "Third Gear Key", Tooltip = "Key to activate the third gear (high speed).")]
	Input.KEY thirdGearKey = Input.KEY.DIGIT_3;

	[ShowInEditor, Parameter(Group = "Velocity", Title = "Third Gear Speed", Tooltip = "Velocity for the third gear.")]
	float thirdGearVelocity = 500.0f;

	[ShowInEditor, Parameter(Group = "Velocity", Title = "Acceleration Multiplier", Tooltip = "Multiplier for acceleration when the assigned key is held.")]
	float accelerationMultiplier = 2.0f;

	float _defaultVelocity = 5.0f;
	float _defaultPositionValue = 0f;

	bool _tryEndFocusing = false;
	bool _editText = false;

	WidgetHBox _menuLayout = null;
	WidgetCheckBox _firstGearCheckbox = null;
	WidgetCheckBox _secondGearCheckbox = null;
	WidgetCheckBox _thirdGearCheckBox = null;
	List<WidgetEditLine> _widgetEditLines = [];
	WidgetEditLine _currentGearVelocityTextField = null;
	WidgetEditLine _widgetCameraPositionX = null;
	WidgetEditLine _widgetCameraPositionY = null;
	WidgetEditLine _widgetCameraPositionZ = null;

	WorldIntersection _intersection = new WorldIntersection();

	Vec3 _targetPoint;

	PlayerSpectator _playerSpectator = null;

	bool _enterMouseGrabMode = false;

	public enum PlayerMovementState
	{
		IDLE,
		SPECTATOR,
		RAIL,
		FOCUSING,
		PANNING
	}

	PlayerMovementState _playerState = PlayerMovementState.IDLE;


	private readonly struct StateTransition(Func<bool> condition, PlayerMovementState tragetState)
	{
		public Func<bool> Condition { get; } = condition;
		public PlayerMovementState TargetState { get; } = tragetState;
	}

	private readonly struct MovementState(IEnumerable<StateTransition> transitions, Action onEnter = null, Action onExit = null, Action onUpdate = null)
	{
		public Action OnEnter { get; } = onEnter;
		public Action OnExit { get; } = onExit;
		public Action OnUpdate { get; } = onUpdate;

		public List<StateTransition> Transitions { get; } = transitions.ToList();
	}


	private Dictionary<PlayerMovementState, MovementState> _stateMap = null;

	public enum VelocityGear
	{
		GEAR_FIRST = 1,
		GEAR_SECOND,
		GEAR_THIRD
	}

	VelocityGear _velocityGear = VelocityGear.GEAR_FIRST;

	bool TryFocusing => Input.IsKeyDown(focusKey);

	bool TryEnterSpectatorMode => !Input.IsModifierEnabled(altCameraModKey) && Input.IsMouseButtonPressed(spectatorModeMouseButton);

	bool TryEnterRailMode => Input.IsModifierEnabled(altCameraModKey) && Input.IsMouseButtonPressed(railModeMouseButton);

	bool TryEnterPannigMode => Input.IsModifierEnabled(altCameraModKey) && Input.IsMouseButtonPressed(panningModeMouseButton);

	bool TryExitRailMode => !Input.IsMouseButtonPressed(railModeMouseButton);

	bool TryExitPanningMode => !Input.IsMouseButtonPressed(panningModeMouseButton);

	void Init()
	{
		_playerSpectator = Game.Player as PlayerSpectator;

		_enterMouseGrabMode = Input.MouseGrab;

		CreateStateTable();
		UpdateGear(_velocityGear);
		GenerateMenu();
	}

	void Update()
	{
		if (Unigine.Console.Active)
			return;
		if (_editText)
		{
			UpdateEditFieldSubmission();
			return;
		}
		UpdateVelocityGear();
		UpdateStates();
		UpdateMenuParams();
	}

	void Shutdown()
	{
		_menuLayout.DeleteLater();
		Input.MouseGrab = _enterMouseGrabMode;
	}

	// This method sets up the state transitions and associated actions for each movement state of the player.
	// Each state (e.g., IDLE, SPECTATOR, FOCUSING, etc.) is mapped to a set of conditions for state transitions, 
	// as well as initialization, update, and end functions for when the state is entered or exited.
	private void CreateStateTable()
	{
		_stateMap = new Dictionary<PlayerMovementState, MovementState>
		{
			[PlayerMovementState.IDLE] = new MovementState
			(
				transitions:
				[
					new (() => TryFocusing, PlayerMovementState.FOCUSING),
					new (() => TryEnterSpectatorMode, PlayerMovementState.SPECTATOR),
					new (() => TryEnterRailMode, PlayerMovementState.RAIL),
					new (() => TryEnterPannigMode, PlayerMovementState.PANNING)
				]
			),

			[PlayerMovementState.FOCUSING] = new MovementState
			(
				transitions:
				[
					new (() => _tryEndFocusing, PlayerMovementState.IDLE),
					new (() => TryEnterSpectatorMode, PlayerMovementState.SPECTATOR),
					new (() => TryEnterPannigMode, PlayerMovementState.PANNING),
					new (() => TryEnterRailMode, PlayerMovementState.RAIL)
				],
				onEnter: InitFocusing,
				onExit: EndFocusing,
				onUpdate: UpdateFocusing
			),

			[PlayerMovementState.SPECTATOR] = new MovementState
			(
				transitions:
				[
					new (() => !TryEnterSpectatorMode, PlayerMovementState.IDLE)
				],
				onEnter: InitSpectator,
				onExit: EndSpectator,
				onUpdate: UpdateSpectator
			),

			[PlayerMovementState.RAIL] = new MovementState
			(
				transitions:
				[
					new (() => TryExitRailMode, PlayerMovementState.IDLE)
				],
				onUpdate: UpdateRail
			),

			[PlayerMovementState.PANNING] = new MovementState
			(
				transitions:
				[
					new (() => TryExitPanningMode, PlayerMovementState.IDLE)
				],
				onEnter: InitPanning,
				onExit: EndPanning,
				onUpdate: UpdatePanning
			)
		};
	}

	private void UpdateStates()
	{
		var state = _stateMap[_playerState];

		foreach (var transition in state.Transitions)
		{
			if (transition.Condition())
			{
				SwitchState(transition.TargetState);
				return;
			}
		}
		state.OnUpdate?.Invoke();
	}

	private void SwitchState(PlayerMovementState newState)
	{
		if (_playerState == newState)
			return;

		_stateMap[_playerState].OnExit?.Invoke();
		_playerState = newState;
		_stateMap[_playerState].OnEnter?.Invoke();
	}

	void UpdateVelocityGear()
	{
		if (Input.IsKeyDown(firstGearKey))
		{
			_firstGearCheckbox.Checked = true;
		}

		if (Input.IsKeyDown(secondGearKey))
		{
			_secondGearCheckbox.Checked = true;
		}

		if (Input.IsKeyDown(thirdGearKey))
		{
			_thirdGearCheckBox.Checked = true;
		}
	}

	void UpdateGear(VelocityGear newGear)
	{
		_velocityGear = newGear;
		_playerSpectator.MinVelocity = GetVelocity();
		_playerSpectator.MaxVelocity = GetVelocityAcceleration();
	}

	float GetVelocity()
	{
		return _velocityGear switch
		{
			VelocityGear.GEAR_FIRST => firstGearVelocity,
			VelocityGear.GEAR_SECOND => secondGearVelocity,
			VelocityGear.GEAR_THIRD => thirdGearVelocity,
			_ => 0.0f
		};
	}

	float GetVelocityAcceleration()
	{
		return GetVelocity() * accelerationMultiplier;
	}

	void SetVelocity(VelocityGear targetGear, float velocity)
	{
		switch (targetGear)
		{
			case VelocityGear.GEAR_FIRST:
				firstGearVelocity = velocity;
				break;
			case VelocityGear.GEAR_SECOND:
				secondGearVelocity = velocity;
				break;
			case VelocityGear.GEAR_THIRD:
				thirdGearVelocity = velocity;
				break;
		}
		UpdateGear(targetGear);
	}

	void InitSpectator()
	{
		_playerSpectator.Controlled = true;

		ControlsApp.MouseEnabled = true;
	}

	void UpdateSpectator()
	{
		Input.MouseCursorHide = true;
	}

	void EndSpectator()
	{
		_playerSpectator.Controlled = false;
	}

	void UpdateRail()
	{
		Input.MouseCursorHide = true;

		ivec2 mouseDelta = Input.MouseDeltaPosition;

		if (mouseDelta != ivec2.ZERO)
			MathLib.Normalize(mouseDelta);

		float currentAcceleration = Input.IsKeyPressed(accelerationKey) ? GetVelocity() : GetVelocityAcceleration();

		float delta = -(mouseDelta.x + mouseDelta.y) * currentAcceleration * ControlsApp.MouseSensitivity * panningRailScale;

		_playerSpectator.Translate(new Vec3(0, 0, delta));
	}

	void InitPanning()
	{
		Input.MouseGrab = true;
		ControlsApp.MouseEnabled = true;
	}

	void UpdatePanning()
	{
		ivec2 mouse_delta = Input.MouseDeltaPosition;

		float current_acceleration = Input.IsKeyPressed(accelerationKey) ? GetVelocity() : GetVelocityAcceleration();

		_playerSpectator.Translate(new Vec3(-mouse_delta.x, mouse_delta.y, 0) * current_acceleration * ControlsApp.MouseSensitivity * panningRailScale);

		Input.MouseCursorHide = true;
	}

	void EndPanning()
	{
		Input.MouseGrab = false;
	}

	void InitFocusing()
	{
		_playerSpectator.GetDirectionFromMainWindow(out Vec3 startPoint, out Vec3 endPoint, Input.MousePosition.x, Input.MousePosition.y);

		Vec3 objPosition;
		double focusRadius;

		Node obj = World.GetIntersection(startPoint, endPoint, ~0, _intersection);

		if (!obj)
		{
			_tryEndFocusing = true;
			return;
		}

		objPosition = obj.WorldPosition;
		focusRadius = obj.WorldBoundSphere.Radius;

		// Block, with an example for focusing specifically on instances of ObjectMeshCluster
		if (obj as ObjectMeshCluster != null)
		{
			ObjectMeshCluster cluster = obj as ObjectMeshCluster;
			int instanceIndex = _intersection.Instance;

			Mesh mesh = cluster.GetMeshCurrentRAM();
			Mat4 transform = cluster.GetMeshTransform(instanceIndex);

			objPosition = cluster.WorldTransform * transform.Translate;

			//When searching the radius, we take into account both the scale of the instance and the ObjectMeshCluster itself.
			focusRadius = mesh.BoundSphere.Radius * cluster.Scale.Maximum * transform.Scale.Maximum;
		}

		//We take the doubled radius so that the camera is at a distance from the focus point.
		_targetPoint = objPosition - (Vec3)(_playerSpectator.GetWorldDirection() * focusRadius * 2);
	}

	void UpdateFocusing()
	{
		_playerSpectator.WorldPosition = MathLib.Lerp(_playerSpectator.WorldPosition, _targetPoint, Game.IFps * 5);
		if (MathLib.Distance2(_playerSpectator.WorldPosition, _targetPoint) < 0.01f)
			_tryEndFocusing = true;
	}

	void EndFocusing()
	{
		_tryEndFocusing = false;
	}

	void GenerateMenu()
	{
		_menuLayout = new WidgetHBox(0, 4)
		{
			Background = 1,
		};
		Gui.GetCurrent().AddChild(_menuLayout, Gui.ALIGN_TOP);	

		WidgetHBox gearsLayout = new (5, 0);
		_menuLayout.AddChild(gearsLayout,Gui.ALIGN_LEFT);

		WidgetLabel labelName = new WidgetLabel("Player speed:");
		gearsLayout.AddChild(labelName, Gui.ALIGN_LEFT);

		_currentGearVelocityTextField = new WidgetEditLine(GetVelocity().ToString("F5"))
		{
			Validator = 3,
			Width = 100
		};
		gearsLayout.AddChild(_currentGearVelocityTextField, Gui.ALIGN_LEFT);
		_widgetEditLines.Add(_currentGearVelocityTextField);
		_currentGearVelocityTextField.EventFocusIn.Connect(() => _editText = true);
		_currentGearVelocityTextField.EventFocusOut.Connect(() =>
		{
			_editText = false;
			SetVelocity
			(
				_velocityGear,
				_currentGearVelocityTextField.Text.Length == 0 ? _defaultVelocity : float.Parse(_currentGearVelocityTextField.Text)
			);
		});

		_firstGearCheckbox = new WidgetCheckBox("1")
		{
			Checked = true
		};
		gearsLayout.AddChild(_firstGearCheckbox, Gui.ALIGN_LEFT);
		_firstGearCheckbox.EventChanged.Connect(() =>
		{
			if (_firstGearCheckbox.Checked)
				ChangeGearTextField(VelocityGear.GEAR_FIRST);
		});

		_secondGearCheckbox = new WidgetCheckBox("2");
		_firstGearCheckbox.AddAttach(_secondGearCheckbox);
		gearsLayout.AddChild(_secondGearCheckbox, Gui.ALIGN_LEFT);
		_secondGearCheckbox.EventChanged.Connect(() =>
		{
			if (_secondGearCheckbox.Checked)
				ChangeGearTextField(VelocityGear.GEAR_SECOND);
		});

		_thirdGearCheckBox = new WidgetCheckBox("3");
		_firstGearCheckbox.AddAttach(_thirdGearCheckBox);
		gearsLayout.AddChild(_thirdGearCheckBox, Gui.ALIGN_LEFT);
		_thirdGearCheckBox.EventChanged.Connect(() =>
		{
			if (_thirdGearCheckBox.Checked)
				ChangeGearTextField(VelocityGear.GEAR_THIRD);
		});

		WidgetSpacer gearSpacer = new WidgetSpacer()
		{
			Orientation = 0
		};
		_menuLayout.AddChild(gearSpacer,Gui.ALIGN_LEFT);

		WidgetHBox positionLayout = new WidgetHBox(5,0);
		_menuLayout.AddChild(positionLayout, Gui.ALIGN_LEFT);

		labelName = new WidgetLabel("X:");
		positionLayout.AddChild(labelName, Gui.ALIGN_LEFT);

		_widgetCameraPositionX = new WidgetEditLine(_playerSpectator.WorldPosition.x.ToString("F5"))
		{
			Validator = 3,
			Width = 100
		};
		positionLayout.AddChild(_widgetCameraPositionX, Gui.ALIGN_LEFT);
		_widgetEditLines.Add(_widgetCameraPositionX);
		_widgetCameraPositionX.EventFocusIn.Connect(() =>  _editText = true);
		_widgetCameraPositionX.EventKeyPressed.Connect(() => 
		{
			float value = _widgetCameraPositionX.Text.Length == 0 ? _defaultPositionValue : float.Parse(_widgetCameraPositionX.Text);
			Vec3 position = _playerSpectator.WorldPosition;
			_playerSpectator.WorldPosition = new Vec3(value, position.y, position.z);
		});
		_widgetCameraPositionX.EventFocusOut.Connect(() =>
		{
			_editText = false;
			if (_widgetCameraPositionX.Text.Length == 0)
				_widgetCameraPositionX.Text = _defaultPositionValue.ToString("F5");
		});

		labelName = new WidgetLabel("Y:");
		positionLayout.AddChild(labelName, Gui.ALIGN_LEFT);

		_widgetCameraPositionY = new WidgetEditLine(_playerSpectator.WorldPosition.y.ToString("F5"))
		{
			Validator = 3,
			Width = 100
		};
		positionLayout.AddChild(_widgetCameraPositionY, Gui.ALIGN_LEFT);
		_widgetEditLines.Add(_widgetCameraPositionY);
		_widgetCameraPositionY.EventFocusIn.Connect(() => _editText = true);
		_widgetCameraPositionY.EventKeyPressed.Connect(() =>
		{
			float value = _widgetCameraPositionY.Text.Length == 0 ? _defaultPositionValue : float.Parse(_widgetCameraPositionY.Text);
			Vec3 position = _playerSpectator.WorldPosition;
			_playerSpectator.WorldPosition = new Vec3(position.x, value, position.z);
		});
		_widgetCameraPositionY.EventFocusOut.Connect(() =>
		{
			_editText = false;
			if (_widgetCameraPositionY.Text.Length == 0)
			_widgetCameraPositionY.Text = _defaultPositionValue.ToString("F5");
		});

		labelName = new WidgetLabel("Z:");
		positionLayout.AddChild(labelName, Gui.ALIGN_LEFT);

		_widgetCameraPositionZ = new WidgetEditLine(_playerSpectator.WorldPosition.z.ToString("F5"))
		{
			Validator = 3,
			Width = 100
		};
		positionLayout.AddChild(_widgetCameraPositionZ, Gui.ALIGN_LEFT);
		_widgetEditLines.Add(_widgetCameraPositionZ);
		_widgetCameraPositionZ.EventFocusIn.Connect(() => _editText = true);
		_widgetCameraPositionZ.EventKeyPressed.Connect(() =>
		{
			float value = _widgetCameraPositionZ.Text.Length == 0 ? _defaultPositionValue : float.Parse(_widgetCameraPositionZ.Text);
			Vec3 position = _playerSpectator.WorldPosition;
			_playerSpectator.WorldPosition = new Vec3(position.x, position.y, value);
		});
		_widgetCameraPositionZ.EventFocusOut.Connect(() => 
		{
			_editText = false;
			if (_widgetCameraPositionY.Text.Length == 0)
				_widgetCameraPositionY.Text = _defaultPositionValue.ToString("F5");
		});
	}

	void UpdateMenuParams()
	{
		if (Input.IsKeyDown(toggleCameraMenu))
			_menuLayout.Hidden = !_menuLayout.Hidden;

		_widgetCameraPositionX.Text = _playerSpectator.WorldPosition.x.ToString("F5");
		_widgetCameraPositionY.Text = _playerSpectator.WorldPosition.y.ToString("F5");
		_widgetCameraPositionZ.Text = _playerSpectator.WorldPosition.z.ToString("F5");
	}

	void UpdateEditFieldSubmission()
	{
		if (_widgetEditLines.Count != 0 && Input.IsKeyDown(Input.KEY.ENTER))
		{
			foreach (var widget in _widgetEditLines)
				widget.RemoveFocus();
		}
	}

	void ChangeGearTextField(VelocityGear newGear)
	{
		UpdateGear(newGear);
		_currentGearVelocityTextField.Text = GetVelocity().ToString();
	}
}
