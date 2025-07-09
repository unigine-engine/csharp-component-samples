using System;
using System.Collections.Generic;
using Unigine;


public partial class InterpreterRegistrator
{
	private static InterpreterRegistrator instance = new InterpreterRegistrator();

#nullable enable
	private static List<Action>? actions;
#nullable disable

	private InterpreterRegistrator() { }

	private void init()
	{
		foreach (Action action in actions)
		{
			action?.Invoke();
		}
		actions.Clear();
	}

	protected class InterpreterRegistratorAction
	{
		public InterpreterRegistratorAction(Action action)
		{
			if (actions == null)
				actions = new List<Action>();
			actions.Add(action);
		}
	}

	public static void Initialize()
	{
		Log.Message("InterpreterRegistrator.Initialize()\n");
		instance.init();
	}
}
