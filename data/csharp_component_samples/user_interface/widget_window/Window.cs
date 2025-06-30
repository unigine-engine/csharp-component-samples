using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "5dbaec014a558a2f578248cdc24add62a2707e73")]
public class Window : Component
{
	private WidgetWindow window;
	
	void Init()
	{
		Gui gui = Gui.GetCurrent();

		window = new WidgetWindow(gui, "Hello from C#", 4, 4);
		window.Flags = Gui.ALIGN_OVERLAP | Gui.ALIGN_CENTER;
		window.Width = 320;
		window.Sizeable = true;

		var editline = new WidgetEditLine(gui, "Edit me");
		editline.Flags = Gui.ALIGN_EXPAND;
		window.AddChild(editline);
		editline.EventChanged.Connect(widget => {
			WidgetEditLine el = widget as WidgetEditLine;
			Log.Message("EditLine changed: {0}\n", el.Text);
		});
		editline.FontSize = 16;

		var button = new WidgetButton(gui, "Press me");
		button.Flags = Gui.ALIGN_EXPAND;
		window.AddChild(button);
		button.EventClicked.Connect(() => Log.Message("Button pressed\n"));
		button.FontSize = 18;

		window.Arrange();
		Gui.GetCurrent().AddChild(window);

		Console.Onscreen = true;
	}
	
	void Shutdown()
	{
		Console.Onscreen = false;

		window.DeleteLater();
	}
}
