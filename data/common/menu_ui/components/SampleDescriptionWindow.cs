using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Unigine;

public class SampleDescriptionWindow
{
	private WidgetWindow mainWindow = null;
	private WidgetGroupBox aboutGroup = null;
	private WidgetLabel aboutLabel = null;
	private WidgetGroupBox controlsGroup = null;
	private WidgetLabel controlsLabel = null;
	private WidgetGroupBox parametersGroup = null;
	private WidgetGridBox parametersGrid = null;
	private WidgetGroupBox statusGroup = null;
	private WidgetLabel statusLabel = null;

	private EventConnections connections;


	public void createWindow(int align = Gui.ALIGN_LEFT, int width = 400)
	{
		string worldPath = World.Path;
		string worldName = GetFileName(worldPath);

		String cppSamplesXmlPath = FileSystem.GetAbsolutePath(Engine.DataPath + "../csharp_component_samples.sample");

		Xml cppSamplesXml = new Xml();
		if (!cppSamplesXml.Load(cppSamplesXmlPath))
		{
			Log.Warning($"SampleDescriptionWindow::createWindow(): cannot open {cppSamplesXmlPath} file\n");
			return;
		}

		Xml cppSamplesSamplesPack = cppSamplesXml.GetChild("samples_pack");
		Xml samplesXml = cppSamplesSamplesPack.GetChild("samples");

		string title = "";
		string description = "";
		string controls = "";

		for (int i = 0; i < samplesXml.NumChildren; ++i)
		{
			Xml sample_xml = samplesXml.GetChild(i);

			if (sample_xml.GetArg("id") == worldName)
			{
				title = sample_xml.GetArg("title");
				description = sample_xml.GetChild("desc").GetChildData("brief");
				if (sample_xml.IsChild("controls"))
					controls = sample_xml.GetChildData("controls");
				break;
			}
		}

		mainWindow = new WidgetWindow(title);
		mainWindow.Width = width;
		mainWindow.Arrange();
		WindowManager.MainWindow.AddChild(mainWindow, Gui.ALIGN_OVERLAP | align);

		if (description.Length > 0)
		{
			aboutGroup = new WidgetGroupBox("About", 8, 8);
			mainWindow.AddChild(aboutGroup, Gui.ALIGN_LEFT);

			aboutLabel = new WidgetLabel(description);
			aboutLabel.FontWrap = 1;
			aboutLabel.FontRich = 1;
			//aboutLabel.Width = 300;
			aboutGroup.AddChild(aboutLabel, Gui.ALIGN_EXPAND);
		}
		if (controls.Length > 0)
		{
			controlsGroup = new WidgetGroupBox("Controls", 8, 8);
			mainWindow.AddChild(controlsGroup, Gui.ALIGN_LEFT);

			controlsLabel = new WidgetLabel(controls);
			controlsLabel.FontWrap = 1;
			controlsLabel.FontRich = 1;
			//controlsLabel.Width = 300;
			controlsGroup.AddChild(controlsLabel, Gui.ALIGN_EXPAND);
		}

		connections = new EventConnections();
	}

	public void shutdown()
	{
		connections.DisconnectAll();
		mainWindow.DeleteLater();
	}


	public WidgetLabel addLabel(string label_text)
	{
		if (!parametersGrid)
			init_parameter_box();

		var label = new WidgetLabel(label_text);
		parametersGrid.AddChild(new WidgetLabel());
		parametersGrid.AddChild(label);
		parametersGrid.AddChild(new WidgetLabel());
		return label;
	}


	public WidgetSlider addFloatParameter(string name, string tooltip, float default_value, float min_value, float max_value, Action<float> on_change)
	{
		if (!parametersGrid)
			init_parameter_box();

		var label = new WidgetLabel(name);
		label.Width = 100;
		parametersGrid.AddChild(label, Gui.ALIGN_LEFT);
		label.SetToolTip(tooltip);

		var slider = new WidgetSlider();
		slider.MinValue = (int)(min_value * 100);
		slider.MaxValue = (int)(max_value * 100);
		slider.Value = (int)(default_value * 100);

		slider.Width = 200;
		slider.ButtonWidth = 20;
		slider.ButtonHeight = 20;
		slider.SetToolTip(tooltip);
		parametersGrid.AddChild(slider, Gui.ALIGN_LEFT);

		label = new WidgetLabel(default_value.ToString("0.00"));
		label.Width = 20;
		label.SetToolTip(tooltip);
		parametersGrid.AddChild(label);

		slider.EventChanged.Connect(connections, () =>
		{
			float v = slider.Value / 100.0f;
			label.Text = v.ToString("0.00");
			on_change(v);
		});

		return slider;
	}


	public WidgetSlider addIntParameter(string name, string tooltip, int default_value, int min_value, int max_value, Action<int> on_change)
	{
		if (!parametersGrid)
			init_parameter_box();

		var label = new WidgetLabel(name);
		label.Width = 100;
		label.SetToolTip(tooltip);
		parametersGrid.AddChild(label, Gui.ALIGN_LEFT);

		var slider = new WidgetSlider();
		slider.MinValue = min_value;
		slider.MaxValue = max_value;
		slider.Value = default_value;

		slider.Width = 200;
		slider.ButtonWidth = 20;
		slider.ButtonHeight = 20;
		slider.SetToolTip(tooltip);
		parametersGrid.AddChild(slider, Gui.ALIGN_LEFT);

		label = new WidgetLabel(default_value.ToString());
		label.Width = 20;
		label.SetToolTip(tooltip);
		parametersGrid.AddChild(label);

		slider.EventChanged.Connect(connections, () =>
		{
			int v = slider.Value;
			label.Text = v.ToString();
			on_change(v);
		});

		return slider;
	}

	public WidgetCheckBox addBoolParameter(string name, string tooltip, bool default_value, Action<bool> on_change)
	{
		if (!parametersGrid)
			init_parameter_box();

		var label = new WidgetLabel(name);
		label.Width = 100;
		label.SetToolTip(tooltip);
		var checkbox = new WidgetCheckBox();
		checkbox.SetToolTip(tooltip);
		checkbox.Checked = default_value;

		checkbox.EventChanged.Connect(connections, (Widget widget) =>
		{
			if (checkbox.Checked)
				on_change(true);
			else
				on_change(false);
		});

		parametersGrid.AddChild(label, Gui.ALIGN_LEFT);
		parametersGrid.AddChild(checkbox, Gui.ALIGN_CENTER);
		parametersGrid.AddChild(new WidgetLabel(), Gui.ALIGN_LEFT);
		return checkbox;
	}

	public void setStatus(string status)
	{
		if (!statusLabel)
			init_status_box();

		statusLabel.Text = status;
		statusLabel.Arrange();
	}

	public WidgetGroupBox getParameterGroupBox()
	{
		if (!parametersGrid)
			init_parameter_box();
		return parametersGroup;
	}

	private void init_parameter_box()
	{
		parametersGroup = new WidgetGroupBox("Parameters", 8, 8);
		mainWindow.AddChild(parametersGroup, Gui.ALIGN_LEFT);
		parametersGrid = new WidgetGridBox(3);
		parametersGroup.AddChild(parametersGrid);
	}

	private void init_status_box()
	{
		statusGroup = new WidgetGroupBox("Status", 8, 8);
		mainWindow.AddChild(statusGroup, Gui.ALIGN_LEFT);
		statusLabel = new WidgetLabel();
		// w_status_lbl->setFontWrap(1);
		// w_status_lbl->setFontRich(1);
		statusLabel.Width = 300;
		statusGroup.AddChild(statusLabel, Gui.ALIGN_EXPAND);
	}

	public void addParameterSpacer()
	{
		for (int i = 0; i < 3; ++i)
		{
			var spacer = new WidgetSpacer();
			parametersGrid.AddChild(spacer);
		}
	}

	private string GetFileName(in string path)
	{
		string[] parts = path.Split("/");
		if (parts.Length > 0)
		{
			string name = parts.Last();
			parts = name.Split(".");

			if (parts.Length > 0)
				return parts.First();
		}

		return "UNKNOW";
	}
}
