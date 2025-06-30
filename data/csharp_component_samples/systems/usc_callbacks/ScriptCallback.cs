using System.Collections;
using System.Collections.Generic;
using Unigine;


public class MyCallback
{
	public const string sourse_str = "From [C++]:";

	public static Variable runWorldFunction(Variable name, Variable v)
	{
		Log.Message("{0} runWorldFunction({1},{2}): called\n", sourse_str, name.TypeName, v.TypeName);
		return Engine.RunWorldFunction(name, v);
	}
}

public partial class InterpreterRegistrator
{
	private InterpreterRegistrator.InterpreterRegistratorAction my_callback_action
		= new InterpreterRegistrator.InterpreterRegistratorAction(() =>
		{
			// export functions
			Interpreter.AddExternFunction("runWorldFunction", new Interpreter.Function2v(MyCallback.runWorldFunction));
		}
	);
}

[Component(PropertyGuid = "0619b8285b053d9ed75eeb42cc54c484aeb84189")]
public class ScriptCallback : Component
{
	private float onscreenTime;

	void Init()
	{
		Unigine.Console.Onscreen = true;
		Unigine.Console.OnscreenFontSize = 15;
		Unigine.Console.OnscreenHeight = 100;
		onscreenTime = Unigine.Console.OnscreenTime;
		Unigine.Console.OnscreenTime = 1000;
	}

	void Update()
	{
		Variable ret = Engine.RunWorldFunction(new Variable("counter"));
		if (ret.Int != -1)
			Log.Message("{0} counter is: {1}\n", MyCallback.sourse_str, ret.Int);
		if (ret.Int == 3)
			Log.Message("\n{0} world path is: \"{1}\"\n", MyCallback.sourse_str, Engine.RunWorldFunction(new Variable("engine.world.getPath")).String);

	}

	void Shutdown()
	{
		Unigine.Console.Onscreen = false;
		Unigine.Console.OnscreenHeight = 30;
		Unigine.Console.OnscreenTime = onscreenTime;
	}
}
