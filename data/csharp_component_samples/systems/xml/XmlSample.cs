using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "44c94e2c465fd46226bdd1babce978afdced818c")]
public class XmlSample : Component
{
	float onscreenTime;

	void Init()
	{
		Unigine.Console.Onscreen = true;
		onscreenTime = Unigine.Console.OnscreenTime;
		Unigine.Console.OnscreenTime = 100.0f;

		Log.Message("\n");

		// create the XML tree
		Xml xml = xml_create();

		// print xml tree
		xml_print(xml, 0);
	}
	
	void Shutdown()
	{
		Unigine.Console.Onscreen = false;
		Unigine.Console.OnscreenTime = onscreenTime;
	}

	private Xml xml_create()
	{
		Xml xml = new Xml("node");
		Xml xml_0 = xml.AddChild("child", "arg=\"0\"");
		Xml xml_1 = xml_0.AddChild("child", "arg=\"1\"");
		Xml xml_2 = xml_1.AddChild("child", "arg=\"2\"");
		xml_2.Data = "data";
		xml.APIInterfaceOwner = false;
		return xml;
	}

	private static void xml_print(Xml xml, int offset)
	{
		for (int i = 0; i < offset; i++)
		{
			Log.Message(" ");
		}

		Log.Message("{0}: ", xml.Name);
		for (int i = 0; i < xml.NumArgs; i++)
		{
			Log.Message("{0}={1} ", xml.GetArgName(i), xml.GetArgValue(i));
		}
		Log.Message(": {0}\n", xml.Data);

		for (int i = 0; i < xml.NumChildren; i++)
		{
			xml_print(xml.GetChild(i), offset + 1);
		}
	}
}
