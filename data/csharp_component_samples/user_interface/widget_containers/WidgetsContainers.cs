using Unigine;

[Component(PropertyGuid = "04560204ebfea593d9b0edd603144b3dd3661378")]
public class WidgetsContainers : Component
{
	private WidgetVBox vBox = null;
	private WidgetVPaned vPaned = null;

	private WidgetHBox hBoxTop = null;
	private WidgetHBox hBoxBottom = null;

	private WidgetHPaned hPanedTop = null;

	private WidgetGridBox gridBox = null;
	private WidgetGroupBox groupBox = null;

	private WidgetTabBox tabBox = null;
	private WidgetScrollBox scrollBox = null;
	[MethodInit(Order = -1)]
	private void Init()
	{
		Gui gui = Gui.GetCurrent();

		// add vbox
		vBox = new WidgetVBox(gui);
		vBox.Background = 1;
		gui.AddChild(vBox, Gui.ALIGN_EXPAND);

		// add vpaned to vbox
		vPaned = new WidgetVPaned(gui);
		vPaned.Width = 450;
		vBox.AddChild(vPaned, Gui.ALIGN_EXPAND);

		// add top hbox to vpaned
		hBoxTop = new WidgetHBox(gui);
		hBoxTop.Background = 1;
		hBoxTop.BackgroundColor = new vec4(0.0f, 0.0f, 1.0f, 0.5f);
		vPaned.AddChild(hBoxTop, Gui.ALIGN_EXPAND);

		// add bottom hbox to vpaned
		hBoxBottom = new WidgetHBox(gui);
		hBoxBottom.Background = 1;
		hBoxBottom.BackgroundColor = new vec4(0.0f, 1.0f, 1.0f, 0.5f);
		vPaned.AddChild(hBoxBottom, Gui.ALIGN_EXPAND);

		// add hpaned to top hbox
		hPanedTop = new WidgetHPaned(gui);
		hBoxTop.AddChild(hPanedTop, Gui.ALIGN_EXPAND);

		// add gridbox to top hpaned
		gridBox = new WidgetGridBox(gui, 3, 100, 100);
		gridBox.Background = 1;
		gridBox.BackgroundColor = new vec4(1.0f, 0.0f, 0.0f, 0.5f);
		gridBox.AddChild(new WidgetLabel(gui, "Item 0") { FontSize = 30 }, Gui.ALIGN_CENTER);
		gridBox.AddChild(new WidgetLabel(gui, "Item 1") { FontSize = 30 }, Gui.ALIGN_CENTER);
		gridBox.AddChild(new WidgetLabel(gui, "Item 2") { FontSize = 30 }, Gui.ALIGN_CENTER);
		gridBox.AddChild(new WidgetLabel(gui, "Item 3") { FontSize = 30 }, Gui.ALIGN_CENTER);
		gridBox.AddChild(new WidgetLabel(gui, "Item 4") { FontSize = 30 }, Gui.ALIGN_CENTER);
		gridBox.AddChild(new WidgetLabel(gui, "Item 5") { FontSize = 30 }, Gui.ALIGN_CENTER);
		gridBox.AddChild(new WidgetLabel(gui, "Item 6") { FontSize = 30 }, Gui.ALIGN_CENTER);
		gridBox.AddChild(new WidgetLabel(gui, "Item 7") { FontSize = 30 }, Gui.ALIGN_CENTER);
		gridBox.AddChild(new WidgetLabel(gui, "Item 8") { FontSize = 30 }, Gui.ALIGN_CENTER);

		hPanedTop.AddChild(gridBox, Gui.ALIGN_OVERLAP);

		// add groupbox to top hpaned
		groupBox = new WidgetGroupBox(gui, "Group Box", 30, 30);
		groupBox.Background = 1;
		groupBox.BackgroundColor = new vec4(0.0f, 1.0f, 0.0f, 0.5f);
		groupBox.AddChild(new WidgetLabel(gui, "Item 0") { FontSize = 30 });
		groupBox.AddChild(new WidgetLabel(gui, "Item 1") { FontSize = 30 });
		groupBox.AddChild(new WidgetLabel(gui, "Item 2") { FontSize = 30 });
		groupBox.AddChild(new WidgetLabel(gui, "Item 3") { FontSize = 30 });
		hPanedTop.AddChild(groupBox, Gui.ALIGN_EXPAND);

		// add tabbox to bottom hbox
		tabBox = new WidgetTabBox(gui);
		tabBox.AddTab("Tab 0");
		tabBox.AddChild(new WidgetLabel(gui, "Tab 0 Content") { FontSize = 50 });
		tabBox.AddTab("Tab 1");
		tabBox.AddChild(new WidgetLabel(gui, "Tab 1 Content") { FontSize = 50 });
		tabBox.AddTab("Tab 2");
		tabBox.AddChild(new WidgetLabel(gui, "Tab 2 Content") { FontSize = 50 });
		hBoxBottom.AddChild(tabBox, Gui.ALIGN_EXPAND);

		// add scroll box to bottom hbox
		scrollBox = new WidgetScrollBox(gui, 30, 30);
		scrollBox.BackgroundColor = new vec4(0.0f, 0.0f, 1.0f, 0.5f);

		scrollBox.Width = 250;
		scrollBox.Height = 250;
		scrollBox.AddChild(new WidgetLabel(gui, "Item 0") { FontSize = 20 });
		scrollBox.AddChild(new WidgetLabel(gui, "Item 1") { FontSize = 20 });
		scrollBox.AddChild(new WidgetLabel(gui, "Item 2") { FontSize = 20 });
		scrollBox.AddChild(new WidgetLabel(gui, "Item 3") { FontSize = 20 });
		scrollBox.AddChild(new WidgetLabel(gui, "Item 4") { FontSize = 20 });
		scrollBox.AddChild(new WidgetLabel(gui, "Item 5") { FontSize = 20 });
		scrollBox.AddChild(new WidgetLabel(gui, "Item 6") { FontSize = 20 });
		scrollBox.AddChild(new WidgetLabel(gui, "Item 7") { FontSize = 20 });
		scrollBox.AddChild(new WidgetLabel(gui, "Item 8") { FontSize = 20 });
		hBoxBottom.AddChild(scrollBox, Gui.ALIGN_EXPAND);

		vBox.SetFocus();
	}
	private void Shutdown()
	{
		Gui.GetCurrent().RemoveChild(vBox);
	}
}
