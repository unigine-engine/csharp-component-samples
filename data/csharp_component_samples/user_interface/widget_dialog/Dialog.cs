using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "b8f11faa3a1d55832158966ef902f118084e7538")]
public class Dialog : Component
{
	public AssetLink image;

	private WidgetWindow window;

	void Init()
	{
		Gui gui = Gui.GetCurrent();

		window = new WidgetWindow(gui, "Dialogs", 4, 4);
		window.Flags = Gui.ALIGN_OVERLAP | Gui.ALIGN_CENTER;

		var button_0 = new WidgetButton(gui, "Message");
		button_0.Flags = Gui.ALIGN_EXPAND;
		button_0.EventClicked.Connect(() => button_message_clicked("DialogMessage", "Message"));
		window.AddChild(button_0);

		var button_1 = new WidgetButton(gui, "File");
		button_1.Flags = Gui.ALIGN_EXPAND;
		button_1.EventClicked.Connect(() => button_file_clicked("DialogFile", "./"));
		window.AddChild(button_1);

		var button_2 = new WidgetButton(gui, "Color");
		button_2.Flags = Gui.ALIGN_EXPAND;
		button_2.EventClicked.Connect(() => button_color_clicked("DialogColor", new vec4(1.0f)));
		window.AddChild(button_2);

		if (image.Path.Length > 0)
		{
			var button_3 = new WidgetButton(gui, "Image");
			button_3.Flags = Gui.ALIGN_EXPAND;
			button_3.EventClicked.Connect(() => button_image_clicked("DialogImage", image.Path));
			window.AddChild(button_3);
		}
		else
			Log.Warning("Dialogs.Init(): image parameter not specified.");

		window.Arrange();
		gui.AddChild(window);

		Console.Onscreen = true;
	}
	
	void Shutdown()
	{
		window.DeleteLater();

		Console.Onscreen = false;
	}

	private void dialog_ok_clicked(WidgetDialog dialog, int type)
	{
		Log.Message("{0} ok clicked\n", dialog.Text);
		if (type == 1)
			Log.Message("{0}\n", (dialog as WidgetDialogFile).File);
		if (type == 2)
			Log.Message("{0}\n", (dialog as WidgetDialogColor).WebColor);
		Gui.GetCurrent().RemoveChild(dialog);
	}

	private void dialog_cancel_clicked(Widget widget, WidgetDialog dialog)
	{
		Log.Message("{0} cancel clicked\n", dialog.Text);
		Gui.GetCurrent().RemoveChild(dialog);
	}

	private void dialog_show(WidgetDialog dialog, int type)
	{
		dialog.GetOkButton().EventClicked.Connect(() => dialog_ok_clicked(dialog, type));
		dialog.GetCancelButton().EventClicked.Connect(widget => dialog_cancel_clicked(widget, dialog));
		Gui.GetCurrent().AddChild(dialog, Gui.ALIGN_OVERLAP | Gui.ALIGN_CENTER);
		dialog.SetPermanentFocus();
	}

	private void button_message_clicked(string str, string message)
	{
		var dialog_message = new WidgetDialogMessage(Gui.GetCurrent(), str);
		dialog_message.MessageText = message;
		dialog_show(dialog_message, 0);
	}

	private void button_file_clicked(string str, string path)
	{
		var dialog_file = new WidgetDialogFile(Gui.GetCurrent(), str);
		dialog_file.Path = path;
		dialog_show(dialog_file, 1);
	}

	private void button_color_clicked(string str, vec4 color)
	{
		var dialog_color = new WidgetDialogColor(Gui.GetCurrent(), str);
		dialog_color.Color = color;
		dialog_show(dialog_color, 2);
	}

	private void button_image_clicked(string str, string name)
	{
		var dialog_image = new WidgetDialogImage(Gui.GetCurrent(), str);
		dialog_image.Texture = name;
		dialog_show(dialog_image, 3);
	}
}
