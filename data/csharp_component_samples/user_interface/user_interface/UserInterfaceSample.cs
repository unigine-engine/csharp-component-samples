using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "b4a1c14d6eb72caa79943c59f9360e9a2421d91f")]
public class UserInterfaceSample : Component
{
	public AssetLink ui_file;

	private UserInterface ui;
	private Widget window;

	void Init()
	{
		if (ui_file.Path.Length < 1)
		{
			Log.Warning("UserInterfaceSample::init(): ui_file is not assigned.\n");
			return;
		}

		Gui gui = Gui.GetCurrent();

		ui = new UserInterface(gui, ui_file.Path);
		if (!ui)
			Log.Error("UserInterfaceSample::init(): can't created UserInterface.\n");

		ui.GetWidgetByName("edittext").EventChanged.Connect(EdittextChanged);
		ui.GetWidgetByName("menubox_0").EventClicked.Connect(Menubox0Clicked);

		Widget window = ui.GetWidget(ui.FindWidget("window"));
		window.Arrange();
		gui.AddChild(window, Gui.ALIGN_OVERLAP | Gui.ALIGN_CENTER);

		Console.Onscreen = true;
	}

	void Shutdown()
	{
		if (window)
			window.DeleteLater();
		if (ui)
			ui.DeleteLater();

		Console.Onscreen = false;
	}

	private void EdittextChanged (Widget widget)
	{
		WidgetEditText edittext = widget as WidgetEditText;
		Log.Message("EditText changed: {0}\n", edittext.Text);
	}

	private void Menubox0Clicked(Widget widget)
	{
		WidgetMenuBox menubox = widget as WidgetMenuBox;
		Log.Message("MenuBox clicked: {0}\n", menubox.CurrentItemText);
		if (menubox.CurrentItem == 2)
			Unigine.Console.Run("quit");
	}

}
