using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "3272b1603a8da87d8f3af3cef30e0951d5b1fe40")]
public class FFPSample : Component
{
	EventConnections connection = new EventConnections();

	void Init()
	{
		Engine.EventEndPluginsGui.Connect(connection, Render);
	}
	
	void Render()
	{
		// screen size
		int width = 0;
		int height = 0;

		float time = Game.Time;

		EngineWindow main_window = WindowManager.MainWindow;
		if (main_window != null)
		{
			width = main_window.ClientRenderSize.x;
			height = main_window.ClientRenderSize.y;
		}

		float radius = height / 2.0f;

		Ffp.Enable(Ffp.MODE_SOLID);
		Ffp.SetOrtho(width, height);

		// begin triangles
		Ffp.BeginTriangles();

		// vertex colors
		uint[] colors = { 0xffff0000, 0xff00ff00, 0xff0000ff };

		// create vertices
		int num_vertices = 16;
		for (int i = 0; i < num_vertices; i++)
		{
			float angle = MathLib.PI2 * i / (num_vertices - 1) - time;
			float x = width / 2 + (float)Math.Sin(angle) * radius;
			float y = height / 2 + (float)Math.Cos(angle) * radius;
			Ffp.AddVertex(x, y);
			Ffp.SetColor(colors[i % 3]);
		}

		// create indices
		for (int i = 1; i < num_vertices; i++)
		{
			Ffp.AddIndex(0);
			Ffp.AddIndex(i);
			Ffp.AddIndex(i - 1);
		}

		// end triangles
		Ffp.EndTriangles();

		Ffp.Disable();
	}

	void Shutdown()
	{
		connection.DisconnectAll();
	}
}
