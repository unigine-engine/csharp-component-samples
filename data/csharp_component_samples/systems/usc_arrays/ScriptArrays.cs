using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;


public partial class InterpreterRegistrator
{
	private InterpreterRegistrator.InterpreterRegistratorAction my_array_action
		= new InterpreterRegistrator.InterpreterRegistratorAction(() =>
		{
			// export functions
			Interpreter.AddExternFunction("my_array_vector_set", new Interpreter.Function3(MyArray.my_array_vector_set), "[]");
			Interpreter.AddExternFunction("my_array_vector_get", new Interpreter.Function2v(MyArray.my_array_vector_get), "[]");
			Interpreter.AddExternFunction("my_array_map_set", new Interpreter.Function3(MyArray.my_array_map_set), "[]");
			Interpreter.AddExternFunction("my_array_map_get", new Interpreter.Function2v(MyArray.my_array_map_get), "[]");
			Interpreter.AddExternFunction("my_array_vector_generate", new Interpreter.Function1(MyArray.my_array_vector_generate), "[]");
			Interpreter.AddExternFunction("my_array_map_generate", new Interpreter.Function1(MyArray.my_array_map_generate), "[]");
			Interpreter.AddExternFunction("my_array_vector_enumerate", new Interpreter.Function1(MyArray.my_array_vector_enumerate), "[]");
		}
	);
}


public class MyArray
{
	private const string sourse_str = "From [C++]:";

	public static void my_array_vector_set(Variable id, Variable index, Variable val)
	{
		ArrayVector vector = ArrayVector.Get(Interpreter.Get(), id);
		vector.Set(index.Int, val);
	}

	public static Variable my_array_vector_get(Variable id, Variable index)
	{
		ArrayVector vector = ArrayVector.Get(Interpreter.Get(), id);
		return vector.Get(index.Int);
	}

	public static void my_array_map_set(Variable id, Variable key, Variable val)
	{
		ArrayMap map = ArrayMap.Get(Interpreter.Get(), id);
		map.Set(key, val);
	}

	public static Variable my_array_map_get(Variable id, Variable key)
	{
		ArrayMap map = ArrayMap.Get(Interpreter.Get(), id);
		return map.Get(key);
	}

	public static void my_array_vector_generate(Variable id)
	{
		ArrayVector vector = ArrayVector.Get(Interpreter.Get(), id);
		vector.Clear();
		for (int i = 0; i < 4; i++)
		{
			vector.Append(new Variable(i * i));
		}
		vector.Remove(0);
		vector.Append(new Variable("128"));
	}

	public static void my_array_map_generate(Variable id)
	{
		ArrayMap map = ArrayMap.Get(Interpreter.Get(), id);
		map.Clear();
		for (int i = 0; i < 4; i++)
		{
			map.Append(new Variable(i * i), new Variable(i * i));
		}
		map.Remove(new Variable(0));
		map.Append(new Variable(128), new Variable("128"));
	}

	public static void my_array_vector_enumerate(Variable id)
	{
		ArrayVector vector = ArrayVector.Get(Interpreter.Get(), id);
		for (int i = 0; i < vector.Size; i++)
		{
			Log.Message("{0} {1}: {2}\n", sourse_str, i, vector.Get(i).TypeInfo);
		}
	}
}


[Component(PropertyGuid = "e48b0c1950f63af408657bb35c6d0aed736bedc7")]
public class ScriptArrays : Component
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

