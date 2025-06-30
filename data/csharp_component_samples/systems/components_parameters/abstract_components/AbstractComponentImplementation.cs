using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "75fc620fe6076a02782bc4674dba55bcc1e0a699")]
public class AbstractComponentImplementation : AbstractComponent
{
	public override void DoSomething()
	{
		Log.MessageLine("AbstractComponentImplementation::DoSomething()");
	}
}
