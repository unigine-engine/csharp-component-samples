using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "3ae25d97a71237e98e1e42efe9611bc8a3afd961")]
public class InterfaceComponentImplementation : Component, Interface
{
	public void DoSomething()
	{
		Log.MessageLine("InterfaceComponentImplementation::DoSomething()");
	}
}
