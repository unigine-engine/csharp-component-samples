using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Unigine;
using static Unigine.Input;

[Component(PropertyGuid = "329841cf19529c597e1d5eecb92603dc8b4f216e")]
public class DefaultPlayer : Component
{
	[ShowInEditor]
	[ParameterSwitch(Items = "UNKNOWN,LEFT,MIDDLE,RIGHT,DCLICK,AUX_0,AUX_1,AUX_2,AUX_3")]
	private int mouse_button = 3;

	[ShowInEditor]
	private Player player;

	private Input.MOUSE_BUTTON spectator_mode_button = Input.MOUSE_BUTTON.RIGHT;
	private bool prev_state = false;

	private bool init_player_controlled = false;
	private bool init_mouse_enabled = false;

	void Init()
	{
		if (!player)
			player = Game.Player;

		if (!player)
		{
			Log.Error("DefaultSpectator::init(): must be at least one player in world!\n");
			return;
		}

		init_player_controlled = player.Controlled;
		init_mouse_enabled = ControlsApp.MouseEnabled;

		var button = (Input.MOUSE_BUTTON)mouse_button;
		if (button != Input.MOUSE_BUTTON.UNKNOWN)
			spectator_mode_button = button;
	}
	
	void Update()
	{
		if (Console.Active || !player || (Game.Player != player))
			return;

		bool current_state = Input.IsMouseButtonPressed(spectator_mode_button);
		if (prev_state != current_state)
		{
			ControlsApp.MouseEnabled = current_state ? true : init_mouse_enabled;
			player.Controlled = current_state ? true : init_player_controlled;
			prev_state = current_state;
		}

		Input.MouseCursorHide = current_state;
	}

	void Shutdown()
	{
		if (player)
			player.Controlled = init_player_controlled;

		ControlsApp.MouseEnabled = init_mouse_enabled;
		Input.MouseCursorHide = false;
	}
}
