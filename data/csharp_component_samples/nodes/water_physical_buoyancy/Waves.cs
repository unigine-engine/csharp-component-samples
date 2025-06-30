using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;
using Console = System.Console;

[Component(PropertyGuid = "f821909f5549a5721139c6297b53597471a2b643")]
public class Waves : Component
{
	private ObjectWaterGlobal water = null;

	private void Init()
	{
		water = World.GetNodeByType((int)Node.TYPE.OBJECT_WATER_GLOBAL) as ObjectWaterGlobal;
		if (water == null)
		{
			Log.Error("Now ObjectWaterGlobal in world!");
			return;
		}
		
		water.Beaufort = 0;
	}

	public void SetBeaufort(float beaufort)
	{
		water.Beaufort = beaufort;
	}
}
