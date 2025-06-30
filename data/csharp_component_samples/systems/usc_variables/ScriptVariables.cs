using System.Collections;
using System.Collections.Generic;
using Unigine;


public partial class InterpreterRegistrator
{
	private InterpreterRegistrator.InterpreterRegistratorAction my_variable_action
		= new InterpreterRegistrator.InterpreterRegistratorAction(() =>
		{
			// export functions
			Interpreter.AddExternVariable("my_variable_int", 13);
			Interpreter.AddExternVariable("my_variable_float", 13.17f);
			Interpreter.AddExternVariable("my_variable_double", 13.17);
			Interpreter.AddExternVariable("my_variable_string", "13.17s");
			Interpreter.AddExternVariable("my_variable_vec3", new Variable(new vec3(13.0f, 17.0f, 137.0f)));
			Interpreter.AddExternVariable("my_variable_vec4", new Variable(new vec4(13.0f, 17.0f, 137.0f, 173.0f)));
		}
	);
}


[Component(PropertyGuid = "acf95e6aeb518c3dfe47d9943ed37d78894211b3")]
public class ScriptVariables : Component
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

	void Shutdown()
	{
		Unigine.Console.Onscreen = false;
		Unigine.Console.OnscreenHeight = 30;
		Unigine.Console.OnscreenTime = onscreenTime;
	}
}
