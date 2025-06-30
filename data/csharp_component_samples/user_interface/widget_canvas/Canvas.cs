using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "2eae4a513d0b30b551ded61db2489c740c493f43")]
public class Canvas : Component
{
	private WidgetCanvas canvas;

	void Init()
	{
		// create canvas
		canvas = new WidgetCanvas();

		canvas.SetLineColor(create_line(canvas, 0, 200.0f, 200.0f, 100.0f, 3, 360.0f), new vec4(0.0f, 0.0f, 1.0f, 1.0f));
		canvas.SetLineColor(create_line(canvas, 0, 200.0f, 200.0f, 100.0f, 4, 360.0f), new vec4(0.0f, 1.0f, 0.0f, 1.0f));
		canvas.SetLineColor(create_line(canvas, 0, 200.0f, 200.0f, 100.0f, 5, 360.0f), new vec4(1.0f, 0.0f, 0.0f, 1.0f));

		canvas.SetLineColor(create_line(canvas, 0, 800.0f, 400.0f, 100.0f, 16, 360.0f * 9.0f), new vec4(1.0f, 1.0f, 1.0f, 1.0f));

		canvas.SetPolygonColor(create_polygon(canvas, 0, 600.0f, 200.0f, 100.0f, 6, 360.0f), new vec4(1.0f, 0.0f, 0.0f, 1.0f));
		canvas.SetPolygonColor(create_polygon(canvas, 1, 600.0f, 200.0f, 100.0f, 3, 360.0f), new vec4(0.0f, 0.0f, 1.0f, 1.0f));

		canvas.SetPolygonColor(create_polygon(canvas, 0, 400.0f, 400.0f, 100.0f, 8, 360.0f), new vec4(0.0f, 1.0f, 0.0f, 1.0f));
		canvas.SetPolygonColor(create_polygon(canvas, 1, 400.0f, 400.0f, 100.0f, 4, 360.0f), new vec4(1.0f, 0.0f, 0.0f, 1.0f));

		create_text(canvas, 0, 200.0f - 64.0f, 200.0f - 30.0f, "This is C# canvas text");

		Gui.GetCurrent().AddChild(canvas, Gui.ALIGN_OVERLAP | Gui.ALIGN_BACKGROUND);

		Engine.BackgroundUpdate = Engine.BACKGROUND_UPDATE.BACKGROUND_UPDATE_RENDER_NON_MINIMIZED;
	}

	void Update()
	{
		// get gui
		Gui gui = Gui.GetCurrent();

		float fov = 2.0f;
		float time = Game.Time;
		float x = gui.Width / 2.0f;
		float y = gui.Height / 2.0f;
		canvas.Transform = MathLib.Translate(new vec3(x, y, 0.0f)) * MathLib.Perspective(fov, 1.0f, 0.01f, 100.0f) * MathLib.RotateY((float)Math.Sin(time)) * MathLib.RotateX((float)Math.Cos(time * 0.5f)) * MathLib.Translate(new vec3(-x, -y, -1.0f / (float)Math.Tan(fov * MathLib.DEG2RAD * 0.5f)));
	}

	void Shutdown()
	{
		canvas.DeleteLater();
	}

	private int create_line(WidgetCanvas canvas, int order, float x, float y, float radius, int num, float angle)
	{
		int line = canvas.AddLine(order);
		for (int i = 0; i <= num; i++)
		{
			float s = (float)Math.Sin(angle / num * MathLib.DEG2RAD * i) * radius + x;
			float c = (float)Math.Cos(angle / num * MathLib.DEG2RAD * i) * radius + y;
			canvas.AddLinePoint(line, new vec3(s, c, 0.0f));
		}
		return line;
	}

	private int create_polygon(WidgetCanvas canvas, int order, float x, float y, float radius, int num, float angle)
	{
		int polygon = canvas.AddPolygon(order);
		for (int i = 0; i < num; i++)
		{
			float s = (float)Math.Sin(angle / num * MathLib.DEG2RAD * i) * radius + x;
			float c = (float)Math.Cos(angle / num * MathLib.DEG2RAD * i) * radius + y;
			canvas.AddPolygonPoint(polygon, new vec3(s, c, 0.0f));
		}
		return polygon;
	}

	private int create_text(WidgetCanvas canvas, int order, float x, float y, string str)
	{
		int text = canvas.AddText(order);
		canvas.SetTextPosition(text, new vec2(x, y));
		canvas.SetTextText(text, str);
		return text;
	}
}
