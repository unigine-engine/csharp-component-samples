using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unigine;
using static Unigine.Console;

[Component(PropertyGuid = "4a09e0b28e49cc455a0eb7d09be530dfd3a97092")]
public class ConsoleSample : Component
{
	public Node controllable_node;

	void Init()
	{
		// show console
		Unigine.Console.Active = true;

		// here we add a new console command and add callback to process this command from our c# code
		Unigine.Console.AddCommand("my_console_command", "my_console_command decription", command_callback);

		// run console command
		Unigine.Console.Run("my_console_command \"Hello from C++\"");

		Unigine.Console.AddCommand("control_node", "With this command you can control node, pass desired position through the arguments",
		 move_node_callback);

		if (!controllable_node)
		{
			Log.Message("ConsoleSample.Init(): No node was provided!\n");
		}

		Log.Message("To move the node, you can use control_node command and 3 arguments to specify node position\n");
	}
	
	void Shutdown()
	{
		Unigine.Console.RemoveCommand("my_console_command");
		Unigine.Console.RemoveCommand("control_node");
		Unigine.Console.Active = false;
	}

	private void command_callback(int argc, string[] argv)
	{
		Log.Message("my_console_command(): called\n");
		for (int i = 0; i < argc; i++)
			Log.Message("{0}: {1}\n", i, argv[i]);
	}

	private void move_node_callback(int argc, string[] argv)
	{
		if (!controllable_node)
			return;

		vec3 node_position = new vec3();
		if (argc != 4)
		{
			Log.Warning("control_node was called incorrectly, you need to pass 3 coordinates to move the node\n");
			return;
		}

		var parse_arg = (int index) =>
		{
			string a_value = argv[index];
			bool is_number = float.TryParse(a_value, NumberStyles.Any, CultureInfo.InvariantCulture, out float res);
			float value = is_number ? res : 0.0f;
			return value;
		};

		node_position.x = parse_arg(1);
		node_position.y = parse_arg(2);
		node_position.z = parse_arg(3);

		controllable_node.WorldPosition = node_position;
	}
};
