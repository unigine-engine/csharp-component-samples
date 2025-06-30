using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;
using static MainMenu;

[Component(PropertyGuid = "1858030bd6307ceea1ea1f75d0482a50b2c4f96f")]
public class MainMenu : Component
{
	public class UICommon
	{
		[Parameter(Group = "Main")] public AssetLink white_image;
		[Parameter(Group = "Main")] public int min_window_height = 400;
	};

	public class UIControlPanel
	{
		public int control_panel_space_x;
		[Parameter(Group = "Close Button")] public int close_button_width;
		[Parameter(Group = "Close Button")] public int close_button_height;
		[Parameter(Group = "Close Button")] public AssetLink close_button_background;
		[ParameterColor(Group = "Close Button")]
		public vec4 close_button_background_color;
		[Parameter(Group = "Close Button")] public String close_button_text;
		[ParameterColor(Group = "Close Button")]
		public vec4 close_button_tint_color;
		[Parameter(Group = "Close Button")] public AssetLink close_font;
		[Parameter(Group = "Close Button")] public int close_font_size;
		[ParameterColor(Group = "Close Button")]
		public vec4 close_font_color;

		[Parameter(Group = "Collapse All Button")] public int collaps_button_size;
		[Parameter(Group = "Collapse All Button")] public AssetLink collaps_button_icon;
		[Parameter(Group = "Collapse All Button")] public AssetLink collaps_button_hovered_icon;

		[Parameter(Group = "Expand All Button")] public int expand_button_size;
		[Parameter(Group = "Expand All Button")] public AssetLink expand_button_icon;
		[Parameter(Group = "Expand All Button")] public AssetLink expand_button_hovered_icon;
	};

	public class UISerachField
	{
		[ShowInEditor] public AssetLink background;
		[ParameterColor]
		[ShowInEditor] public vec4 backgroundColor = vec4.WHITE;
		[ShowInEditor] public vec4 backgroundSliceOffsets;
		[ShowInEditor] public float backgroundSliceScale;

		[ParameterColor]
		[ShowInEditor] public vec4 selection_color = vec4.WHITE;
		[ShowInEditor] public int height = 50;
		[ShowInEditor] public ivec4 padding;

		[ShowInEditor] public AssetLink icon;
		[ParameterColor]
		[ShowInEditor] public vec4 iconColor = vec4.WHITE;
		[ShowInEditor] public int iconSize;
		[ShowInEditor] public int iconHeight;

		[ShowInEditor] public AssetLink font;
		[ShowInEditor] public int fontSize;
		[ParameterColor]
		[ShowInEditor] public vec4 fontColor;

		public int spaceBetweenTagsInRow;
		public int spaceBetweenRowsOfTags;
	};

	public class UITagCloud
	{
		public int spaceBetweenTagsInRow;
		public int spaceBetweenRows;

		[ShowInEditor] public AssetLink font;
		[ShowInEditor] public int fontSize;
		[ParameterColor]
		[ShowInEditor] public vec4 fontColor;
	};

	public class UISearchSection
	{
		[ShowInEditor] public AssetLink background;
		[ParameterColor]
		[ShowInEditor] public vec4 backgroundColor = vec4.WHITE;
		[ShowInEditor] public vec4 backgroundSliceOffsets;
		[ShowInEditor] public float backgroundSliceScale;

		[ShowInEditor] public ivec4 padding;
		[ShowInEditor] public int width = 350;

		[ShowInEditor] public float searchFieldReleativeSize = 0.3f;
		public int spaceBetweenChildSections = 30;

		public UIControlPanel control_panel;
		public UISerachField searchField;
		public UITagCloud tagCloud;
	};

	public class UISampleListCategory
	{
		public int vertical_spacing;

		[ShowInEditor] public AssetLink background;
		[ParameterColor]
		[ShowInEditor] public vec4 backgroundColor = vec4.WHITE;
		[ShowInEditor] public vec4 backgroundSliceOffsets;
		[ShowInEditor] public float backgroundSliceScale;
		[ShowInEditor] public ivec4 background_padding;

		[ShowInEditor] public ivec2 iconSize;

		[ShowInEditor] public AssetLink arrowIcon;
		[ParameterColor]
		[ShowInEditor] public vec4 arrowIconColor = vec4.WHITE;
		[ShowInEditor] public int arrowIconSize;
		[ShowInEditor] public ivec4 arrowOffset;

		[ShowInEditor] public AssetLink titleFont;
		[ShowInEditor] public int titleFontSize;
		[ParameterColor]
		[ShowInEditor] public vec4 titleFontColor;
		public int titleOffset;
	};

	public class UISampleListSample
	{
		public int sideOffset;
		public int contentVerticalSpacing;

		[ShowInEditor] public AssetLink background;
		[ParameterColor]
		[ShowInEditor] public vec4 backgroundColor = vec4.WHITE;
		[ShowInEditor] public vec4 backgroundSliceOffsets;
		[ShowInEditor] public float backgroundSliceScale;
		[ShowInEditor] public ivec4 background_padding;

		public int spaceBetweenTagsInRrow;
		public int spaceBetweenRowsOfTags;

		[ShowInEditor] public AssetLink titleFont;
		[ShowInEditor] public int titleFontSize;
		[ParameterColor]
		[ShowInEditor] public vec4 titleFontColor;


		[ShowInEditor] public AssetLink descriptionFont;
		[ShowInEditor] public int descriptionFontSize;
		[ParameterColor]
		[ShowInEditor] public vec4 descriptionFontColor;
		public int descriptionFontVspacing;
	};

	public class UISampleList
	{
		[ParameterColor]
		[ShowInEditor] public vec4 sampleListBackgroundColor = vec4.WHITE;
		[ShowInEditor] public int sampleListMinWidth = 800;
		[ShowInEditor] public int verticalSpacing;
		[ShowInEditor] public ivec4 padding;
		[ParameterColor]
		[ShowInEditor] public vec4 tintColor = vec4.WHITE;

		public UISampleListCategory category;
		public UISampleListSample sample;
	};

	public class UITagStyle
	{
		[ShowInEditor] public AssetLink background;
		[ShowInEditor] public AssetLink selectedBackground;
		[ParameterColor]
		[ShowInEditor] public vec4 backgroundColor = vec4.WHITE;
		[ShowInEditor] public vec4 backgroundSliceOffsets;
		[ShowInEditor] public float backgroundSliceScale;
		[ShowInEditor] public ivec4 backgroundPadding;

		[ShowInEditor] public AssetLink font;
		[ShowInEditor] public AssetLink selectedFont;
		[ShowInEditor] public int fontSize;
		[ParameterColor]
		[ShowInEditor] public vec4 fontColor = vec4.WHITE;
		[ParameterColor]
		[ShowInEditor] public vec4 selectedFontColor;

		[ParameterColor]
		[ShowInEditor] public vec4 tintColor = vec4.WHITE;

		public int removeButtonSize;
		public AssetLink removeButtonBackground;
		[ParameterColor]
		[ShowInEditor] public vec4 removeButtonBackgroundColor = vec4.WHITE;

		[ShowInEditor] public AssetLink removeButtonIcon;
		[ParameterColor]
		[ShowInEditor] public vec4 removeButtonIconColor = vec4.WHITE;
		[ParameterColor]
		[ShowInEditor] public vec4 removeButtonTintColor = vec4.WHITE;

		public float searchFieldScale;
		public float sampleListScale;
		public float tagCloudScale;
	};

	public class UIConfiguration
	{
		public string pathToMeta;
		public UICommon common;
		public UISearchSection searchSection;
		public UISampleList sampleList;
		public UITagStyle tagStyle;
	}
	[ShowInEditor] UIConfiguration uiConfiguration;

	private WidgetHBox samples_and_tags_hbox = null;

	private WidgetVBox side_panel_vbox = null;
	private WidgetSearchField search_field = null;
	private WidgetTagCloud tag_cloud = null;

	// side panel buttons
	private WidgetSprite collaps_button_sprite;
	private WidgetSprite expand_button_sprite;
	private WidgetHBox close_button;
	private vec4 close_pressed_color;
	private vec4 close_color;
	private bool close_button_pressed = false;

	private bool search_editline_focused = false;

	// sample list
	private WidgetSampleList sample_list = null;

	EventConnections connections = new EventConnections();
	private static Unigine.Blob saved_state = new Unigine.Blob();

	private void Init()
	{
		// parse meta
		List<Category> categories = new List<Category>();
		List<string> tags = new List<string>();
		parse_meta_xml(uiConfiguration.pathToMeta, categories, tags);

		Tag.setTagConfig(uiConfiguration.tagStyle);

		// create UI
		// samples list
		samples_and_tags_hbox = new WidgetHBox();

		create_sample_list(categories);
		samples_and_tags_hbox.AddChild(sample_list.getWidget(), Gui.ALIGN_LEFT | Gui.ALIGN_EXPAND);

		create_side_panel();
		samples_and_tags_hbox.AddChild(side_panel_vbox, Gui.ALIGN_LEFT);

		// create search field
		search_field = new WidgetSearchField(uiConfiguration);
		side_panel_vbox.AddChild(search_field.getWidget(), Gui.ALIGN_TOP);

		// tags area
		create_tag_cloud(tags);
		side_panel_vbox.AddChild(tag_cloud.getWidget(), Gui.ALIGN_EXPAND);

		// setup main window
		var main_window = WindowManager.MainWindow;
		main_window.AddChild(samples_and_tags_hbox, Gui.ALIGN_EXPAND);
		main_window.MinSize = new ivec2(uiConfiguration.sampleList.sampleListMinWidth
				+ uiConfiguration.searchSection.width,
			uiConfiguration.common.min_window_height);

		// setup callbacks
		search_field.getEventSearchRequest().Connect(connections, sample_list.filter);
		search_field.getEventTagRemoved().Connect(connections, (string str) =>
		{
			tag_cloud.setTagSelected(str, false);
			sample_list.setTagSelected(str, false);
		});
		search_field.getEventTagAdded().Connect(connections, (string str) =>
		{
			tag_cloud.setTagSelected(str, true);
			sample_list.setTagSelected(str, true);
		});
		search_field.getEventEditlineFocused().Connect(connections, (bool focused) => { search_editline_focused = focused; });
		Tag.getEventClicked().Connect(connections, (string str) =>
		{
			search_field.changeTagState(str, uiConfiguration.tagStyle);
		});
		expand_button_sprite.EventClicked.Connect(connections, () => { sample_list.setCollapseAll(false); });
		collaps_button_sprite.EventClicked.Connect(connections, () => { sample_list.setCollapseAll(true); });

		Input.MouseHandle = Input.MOUSE_HANDLE.USER;

		if (saved_state.IsValidPtr)
			restore(saved_state);
	}

	private void Update()
	{
		EngineWindowViewport window = WindowManager.MainWindow;
		int window_width = window.ClientSize.x;
		int window_height = window.ClientSize.y;
		//	close_button_sprite->setHidden(!window->isFullscreen());
		//	close_button_sprite->setEnabled(!close_button_sprite->isHidden());

		tag_cloud.update(!search_editline_focused);
		sample_list.update(!search_editline_focused);
		search_field.update();

		side_panel_vbox.Height = window_height;
		search_field.getWidget().Height = MathLib.ToInt(side_panel_vbox.Height * uiConfiguration.searchSection.searchFieldReleativeSize);

		update_close_button();
	}

	private void Shutdown()
	{
		save(saved_state);
		samples_and_tags_hbox.DeleteLater();
		connections.DisconnectAll();
	}

	private void save(Unigine.Blob blob)
		{
		blob.Clear();
		search_field.save(blob);
		sample_list.save(blob);
	}

	private void restore(Unigine.Blob blob)
	{
		if (blob.GetSize() <= 0)
			return;

		blob.SeekSet(0);
		search_field.restore(blob);
		sample_list.restore(blob);
	}

	private void update_close_button()
	{
		bool down = Input.IsMouseButtonDown(Input.MOUSE_BUTTON.LEFT);
		bool up = Input.IsMouseButtonUp(Input.MOUSE_BUTTON.LEFT);

		bool hovered = MenuUtils.isHovered(close_button);

		if (!hovered && !up)
			return;

		if (close_button_pressed && up)
		{
			close_button_pressed = false;
			if (hovered)
			{
				Engine.Quit();
			}
		}

		if (!close_button_pressed && down && hovered)
		{
			close_button_pressed = true;
		}

		close_button.BackgroundColor = close_button_pressed ? close_pressed_color : close_color;
	}

	private void parse_meta_xml(string path_relative_to_data, List<Category> categories, List<string> tags)
	{
		string cpp_samples_xml_path = FileSystem.GetAbsolutePath(Engine.DataPath + @"\" + path_relative_to_data);
		Log.Message($"{path_relative_to_data}\n");
		
		Xml cpp_samples_xml = new Xml();
		if (!cpp_samples_xml.Load(cpp_samples_xml_path))
		{
			Unigine.Log.Warning($"MainMenu.parse_meta_xml(): cannot open {cpp_samples_xml_path} file\n");
			return;
		}

		Xml cpp_samples_samples_pack = cpp_samples_xml.GetChild("samples_pack");
		Xml categories_xml = cpp_samples_samples_pack.GetChild("categories");
		Xml samples_xml = cpp_samples_samples_pack.GetChild("samples");

		Dictionary<string, Category> categories_map = new Dictionary<string, Category>();
		HashSet<string> tags_set = new HashSet<string>();

		for (int i = 0; i < categories_xml.NumChildren; ++i)
		{
			Xml category_xml = categories_xml.GetChild(i);
						
			string icon_path = category_xml.GetArg("img");
			if (icon_path.StartsWith("data/"))
			{
				icon_path = icon_path.Substring("data/".Length);
			}

			Category c = new Category();
			c.icon = new Unigine.Image(icon_path);
			c.title = category_xml.GetArg("name");

			string category_id = category_xml.GetArg("id");

			categories_map[category_id] = c;
		}

		for (int i = 0; i < samples_xml.NumChildren; ++i)
		{
			Xml sample_xml = samples_xml.GetChild(i);

			Sample s = new Sample();
			s.title = sample_xml.GetArg("title");
			s.description = sample_xml.GetChild("sdk_desc").Data;
			s.world_name = sample_xml.GetArg("id");

			Xml tags_xml = sample_xml.GetChild("tags");
			for (int j = 0; j < tags_xml.NumChildren; ++j)
			{
				string tag = tags_xml.GetChild(j).Data;

				tags_set.Add(tag);

				s.tags.Add(tag);
			}

			string category_id = sample_xml.GetArg("category_id");

			if (categories_map.ContainsKey(category_id))
				categories_map[category_id].samples.Add(s);
			else
				Unigine.Log.Error($"Category with id {category_id} don't exists in .sample file\n");
		}

		categories.Clear();
		foreach (var c in categories_map)
	{
			if (c.Value.samples.Count > 0)
				categories.Add(c.Value);
		}

		tags.Clear();
		foreach (var t in tags_set)
	{
			tags.Add(t);
		}
	}

	private void create_side_panel()
	{
		int space_y = uiConfiguration.searchSection.spaceBetweenChildSections;
		side_panel_vbox =new WidgetVBox(0, space_y);
		side_panel_vbox.Background = 1;
		side_panel_vbox.BackgroundTexture = uiConfiguration.searchSection.background.Path;
		side_panel_vbox.BackgroundColor = uiConfiguration.searchSection.backgroundColor;
		side_panel_vbox.Background9Sliced = true;
		vec4 offsets = uiConfiguration.searchSection.backgroundSliceOffsets;
		side_panel_vbox.SetBackground9SliceOffsets(offsets.x, offsets.y, offsets.z, offsets.w);
		side_panel_vbox.Background9SliceScale = uiConfiguration.searchSection.backgroundSliceScale;
		side_panel_vbox.Width = uiConfiguration.searchSection.width;
		ivec4 padding = uiConfiguration.searchSection.padding;
		side_panel_vbox.SetPadding(padding.x, padding.y, padding.z - space_y, padding.w - space_y);

		// create control buttons box
		var control_config = uiConfiguration.searchSection.control_panel;
		int space_x = control_config.control_panel_space_x;
		var control_hbox = new WidgetHBox(space_x, 0);
		control_hbox.SetPadding(-space_x, -space_x, 0, 0);
		control_hbox.Width = uiConfiguration.searchSection.width - padding.x - padding.y;
		side_panel_vbox.AddChild(control_hbox, Gui.ALIGN_TOP);

		// create expand button
		expand_button_sprite = new WidgetSprite();
		expand_button_sprite.Texture = control_config.expand_button_icon.Path;
		expand_button_sprite.Width = control_config.expand_button_size;
		expand_button_sprite.Height = control_config.expand_button_size;
		expand_button_sprite.SetToolTip("Expand all categories");

		expand_button_sprite.EventEnter.Connect(connections, () => { expand_button_sprite.Texture = uiConfiguration.searchSection.control_panel.expand_button_hovered_icon.Path; });
		expand_button_sprite.EventLeave.Connect(connections, () => { expand_button_sprite.Texture = uiConfiguration.searchSection.control_panel.expand_button_icon.Path; });
		control_hbox.AddChild(expand_button_sprite, Gui.ALIGN_LEFT);

		// create collapse button
		collaps_button_sprite = new WidgetSprite();
		collaps_button_sprite.Texture = control_config.collaps_button_icon.Path;
		collaps_button_sprite.Width = control_config.collaps_button_size;
		collaps_button_sprite.Height = control_config.collaps_button_size;
		collaps_button_sprite.SetToolTip("Collapse all categories");

		collaps_button_sprite.EventEnter.Connect(connections, () => { collaps_button_sprite.Texture = uiConfiguration.searchSection.control_panel.collaps_button_hovered_icon.Path; });
		collaps_button_sprite.EventLeave.Connect(connections, () => { collaps_button_sprite.Texture = uiConfiguration.searchSection.control_panel.collaps_button_icon.Path; });
		control_hbox.AddChild(collaps_button_sprite, Gui.ALIGN_LEFT);

		// create close button
		close_color = control_config.close_button_background_color;
		close_pressed_color = close_color * control_config.close_button_tint_color;

		var close_label = new WidgetLabel(control_config.close_button_text);
		close_label.SetFont(control_config.close_font.Path);
		close_label.FontSize = control_config.close_font_size;
		close_label.FontColor = control_config.close_font_color;

		close_button = new WidgetHBox();
		close_button.Background = 1;
		close_button.BackgroundTexture = control_config.close_button_background.Path;
		close_button.BackgroundColor = control_config.close_button_background_color;
		close_button.Width = control_config.close_button_width;
		close_button.Height = control_config.close_button_height;

		close_button.AddChild(close_label, Gui.ALIGN_CENTER);
		control_hbox.AddChild(close_button, Gui.ALIGN_RIGHT | Gui.ALIGN_TOP);
	}

	private void create_sample_list(List<Category> categories)
	{
		sample_list = new WidgetSampleList(categories, uiConfiguration);
		sample_list.getWidget().Arrange();
	}

	private void create_tag_cloud(List<string> tags)
	{
		tag_cloud = new WidgetTagCloud(tags, uiConfiguration);
		tag_cloud.getWidget().Arrange();
	}
}

internal class WidgetSearchField
{
	MainMenu.UITagStyle tag_config;

	private WidgetVBox main_vbox = null;
	private WidgetEditLine editline = null;
	private WidgetSprite icon_sprite = null;


	private WidgetVBox tags_vbox = null;
	private WidgetScrollBox tags_scrollbox = null;
	private List<Widget> tag_widgets_ordered = new List<Widget>();

	private Dictionary<string, Widget> tag_widgets = new Dictionary<string, Widget>();

	private int space_between_tags_in_row = 0;

	private EventConnections editline_callback_connections = new EventConnections();
	private EventConnections tag_remove_button_connections = new EventConnections();

	// lower case words and tags parsed from edit text
	private EventInvoker<List<string>, List<string>> event_search_request = new EventInvoker<List<string>, List<string>>();
	private EventInvoker<string> event_tag_removed = new EventInvoker<string>();
	private EventInvoker<string> event_tag_added = new EventInvoker<string>();
	private EventInvoker<bool> event_editline_focus = new EventInvoker<bool>();
	bool pressed = false;


	public WidgetSearchField(MainMenu.UIConfiguration config)
	{
		tag_config = config.tagStyle;

		var search_field_params = config.searchSection.searchField;
		space_between_tags_in_row = search_field_params.spaceBetweenTagsInRow;

		var auxiliary_hbox = new WidgetHBox(0, 0);
		auxiliary_hbox.Background = 1;
		auxiliary_hbox.BackgroundTexture = search_field_params.background.Path;
		auxiliary_hbox.BackgroundColor = search_field_params.backgroundColor;
		auxiliary_hbox.Background9Sliced = true;
		vec4 offsets = search_field_params.backgroundSliceOffsets;
		auxiliary_hbox.SetBackground9SliceOffsets(offsets.x, offsets.y, offsets.z, offsets.w);
		auxiliary_hbox.Background9SliceScale = search_field_params.backgroundSliceScale;

		ivec4 padding = search_field_params.padding;
		auxiliary_hbox.SetPadding(padding.x, padding.y, padding.z, padding.w);

		editline = new WidgetEditLine();
		editline.Height = search_field_params.height - padding.z - padding.w - 5;
		editline.BorderColor = new vec4(0.0f, 0.0f, 0.0f, 0.0f);
		editline.Background = 0;
		editline.SelectionColor = search_field_params.selection_color;
		editline.SetFont(search_field_params.font.Path);
		editline.FontSize = search_field_params.fontSize;
		editline.FontColor = search_field_params.fontColor;

		auxiliary_hbox.AddChild(editline, Gui.ALIGN_EXPAND);

		icon_sprite = new WidgetSprite();
		icon_sprite.Texture = search_field_params.icon.Path;
		icon_sprite.Width = search_field_params.iconSize;
		icon_sprite.Height = search_field_params.iconSize;
		icon_sprite.Color = search_field_params.iconColor;

		auxiliary_hbox.AddChild(icon_sprite, Gui.ALIGN_RIGHT);
		auxiliary_hbox.Height = search_field_params.height;
		padding = config.searchSection.padding;
		auxiliary_hbox.Width = config.searchSection.width - padding.x - padding.y;

		int space_y = search_field_params.spaceBetweenRowsOfTags;
		main_vbox = new WidgetVBox(0, space_y);
		main_vbox.AddChild(auxiliary_hbox, Gui.ALIGN_TOP);
		main_vbox.SetPadding(0, 0, -space_y, -space_y);

		tags_scrollbox = new WidgetScrollBox(0,
			search_field_params.spaceBetweenRowsOfTags);
		tags_scrollbox.HScrollEnabled = false;
		tags_scrollbox.VScrollHidden = WidgetScrollBox.SCROLL_RENDER_MODE.ALWAYS_HIDE_NO_BOUNDS;
		tags_scrollbox.Border = 0;
		tags_scrollbox.SetPadding(0, 0, -space_y, -space_y);
		main_vbox.AddChild(tags_scrollbox, Gui.ALIGN_EXPAND);
		main_vbox.Arrange();
		tags_scrollbox.Width = main_vbox.Width;

		editline.EventFocusOut.Connect(editline_callback_connections, run_event_search_request);
		editline.EventFocusOut.Connect(editline_callback_connections, () => { event_editline_focus.Run(false); });
		editline.EventFocusIn.Connect(editline_callback_connections, () => { event_editline_focus.Run(true); });
		editline.EventChanged.Connect(editline_callback_connections, run_event_search_request);
	}

	~WidgetSearchField()
	{
		main_vbox.DeleteLater();
		editline_callback_connections.DisconnectAll();
		tag_remove_button_connections.DisconnectAll();
	}

	public Widget getWidget()
	{
		return main_vbox;
	}

	public void changeTagState(string str, MainMenu.UITagStyle config)
	{
		if (tag_widgets.ContainsKey(str))
			removeTagFromSearch(str);
		else
			addTagToSerach(str, config);
	}

	public void addTagToSerach(string str, MainMenu.UITagStyle config)
	{
		if (tag_widgets.ContainsKey(str))
			return;

		Widget tag = MenuUtils.createTagWidget(str, config, MenuUtils.TAG_PLACE.SEARCH_FIELD);

		tag_widgets_ordered.Add(tag);
		tag_widgets[str] = tag;

		for (int i = 0; i < tag.NumChildren; ++i)
		{
			WidgetSprite sprite = tag.GetChild(i) as WidgetSprite;

			if (sprite)
			{
				sprite.EventPressed.Connect(tag_remove_button_connections, () =>
				{
					pressed = true;
					sprite.SetLayerColor(1,
						config.removeButtonIconColor * config.removeButtonTintColor);
				});
				sprite.EventReleased.Connect(tag_remove_button_connections, () =>
				{
					pressed = false;
					sprite.SetLayerColor(1, config.removeButtonIconColor);
				});
				sprite.EventClicked.Connect(tag_remove_button_connections,
					() => { removeTagFromSearch(str); });

				break;
			}
		}

		event_tag_added.Run(str);
		Arrange_tags();
		run_event_search_request();
	}


	public void removeTagFromSearch(string str)
	{
		if (!tag_widgets.ContainsKey(str))
			return;

		tag_widgets_ordered.Remove(tag_widgets[str]);
		tag_widgets.Remove(str);

		event_tag_removed.Run(str);
		Arrange_tags();
		run_event_search_request();
	}

	public void save(Unigine.Blob blob)
	{
		blob.WriteInt(tag_widgets.Count);
		foreach (var it in tag_widgets)
			blob.WriteString(it.Key);

		blob.WriteString(editline.Text);
	}

	public void restore(Unigine.Blob blob)
	{
		event_search_request.Enabled = false;

		int tags_size = blob.ReadInt();
		for (int i = 0; i < tags_size; i++)
			addTagToSerach(blob.ReadString(), tag_config);

		editline.EventChanged.Enabled = false;
		editline.Text = blob.ReadString();
		editline.EventChanged.Enabled = true;

		event_search_request.Enabled = true;
		run_event_search_request();
	}

	public Event<List<string>, List<string>> getEventSearchRequest() { return event_search_request; }
	public Event<string> getEventTagRemoved() { return event_tag_removed; }
	public Event<string> getEventTagAdded() { return event_tag_added; }
	public Event<bool> getEventEditlineFocused() { return event_editline_focus; }

	private void Arrange_tags()
	{
		for (int i = tags_scrollbox.NumChildren; i > 0; --i)
		{
			var child = tags_scrollbox.GetChild(i - 1);
			tags_scrollbox.RemoveChild(child);
			child.DeleteLater();
		}

		var current_tags_hbox = new WidgetHBox(space_between_tags_in_row);
		current_tags_hbox.SetPadding(-space_between_tags_in_row, -space_between_tags_in_row, 0, 0);
		tags_scrollbox.AddChild(current_tags_hbox, Gui.ALIGN_LEFT);

		for (int i = 0; i < tag_widgets_ordered.Count; ++i)
		{
			Widget tag = tag_widgets_ordered[i];

			current_tags_hbox.Arrange();
			tag.Arrange();
			int current_width = current_tags_hbox.Width + tag.Width
				+ space_between_tags_in_row;
			if (current_width < tags_scrollbox.Width)
			{
				current_tags_hbox.AddChild(tag);
			}
			else
			{
				current_tags_hbox = new WidgetHBox(space_between_tags_in_row);
				current_tags_hbox.SetPadding(-space_between_tags_in_row, -space_between_tags_in_row, 0,
					0);
				tags_scrollbox.AddChild(current_tags_hbox, Gui.ALIGN_LEFT);
				current_tags_hbox.AddChild(tag);
			}
		}
		current_tags_hbox.Arrange();
	}
	private void run_event_search_request()
	{
		string str = editline.Text;

		List<string> words = new List<string>();
		string buffer = "";
		for (int i = 0; i < str.Length; ++i)
		{
			char c = Char.ToLower(str[i]);
			
			if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
			{
				buffer += c;
			}
			else
			{
				if (buffer.Length > 0)
					words.Add(buffer);
				buffer = "";
			}
		}

		if (buffer.Length > 0)
			words.Add(buffer);


		List<string> tags = new List<string>();
		foreach (var it in tag_widgets)
	{
			string lower_tag = it.Key;
			tags.Add(lower_tag.ToLower());
		}

		event_search_request.Run(words, tags);
	}

	public void update()
	{
		if (pressed)
			return;

		if (editline.IsFocused() == 1
		&& (Input.IsKeyDown(Input.KEY.ESC) || Input.IsKeyDown(Input.KEY.ENTER)))
			editline.RemoveFocus();

		if (editline.IsFocused() == 0 && MenuUtils.isHovered(tags_scrollbox))
			tags_scrollbox.SetFocus();
	}
}

internal class WidgetTagCloud
{
	private WidgetVBox main_vbox = null;
	private WidgetScrollBox main_scrollbox = null;

	private Dictionary<string, Tag> tag_widgets = new Dictionary<string, Tag>();

	private EventInvoker<string> event_tag_clicked = new EventInvoker<string>();


	public WidgetTagCloud(List<string> tags, MainMenu.UIConfiguration config)
	{
		var tag_cloud_params = config.searchSection.tagCloud;
		main_vbox = new WidgetVBox();

		var title = new WidgetLabel("TAGS");
		title.SetFont(tag_cloud_params.font.Path);
		title.FontSize = tag_cloud_params.fontSize;
		title.FontColor = tag_cloud_params.fontColor;
		main_vbox.AddChild(title, Gui.ALIGN_LEFT);

		main_scrollbox = new WidgetScrollBox(0, tag_cloud_params.spaceBetweenRows);
		main_scrollbox.HScrollEnabled = false;
		main_scrollbox.VScrollHidden = WidgetScrollBox.SCROLL_RENDER_MODE.ALWAYS_HIDE_NO_BOUNDS;
		main_scrollbox.Border = 0;
		main_vbox.AddChild(main_scrollbox, Gui.ALIGN_EXPAND);

		ivec4 pAdding = config.searchSection.padding;
		int max_tags_hbox_size = config.searchSection.width - pAdding.x - pAdding.y;

		int x_space = tag_cloud_params.spaceBetweenTagsInRow;
		var current_tags_hbox = new WidgetHBox(x_space);
		current_tags_hbox.SetPadding(-x_space, -x_space, 0, 0);
		main_scrollbox.AddChild(current_tags_hbox, Gui.ALIGN_LEFT);

		for (int i = 0; i < tags.Count; ++i)
		{
			Tag tag = new Tag(tags[i], MenuUtils.TAG_PLACE.TAGS_CLOUD);
			tag_widgets[tags[i]] = tag;

			main_scrollbox.Arrange();
			current_tags_hbox.Arrange();
			Widget tag_widget = tag.getWidget();
			tag_widget.Arrange();
			int current_width = current_tags_hbox.Width + tag_widget.Width + x_space;
			if (current_width < max_tags_hbox_size)
			{
				current_tags_hbox.AddChild(tag_widget, Gui.ALIGN_LEFT);
			}
			else
			{
				// create new
				current_tags_hbox = new WidgetHBox(x_space);
				current_tags_hbox.SetPadding(-x_space, -x_space, 0, 0);
				main_scrollbox.AddChild(current_tags_hbox, Gui.ALIGN_LEFT);
				current_tags_hbox.AddChild(tag_widget);
			}
		}
	}
	~WidgetTagCloud()
	{
		main_scrollbox.DeleteLater();
	}

	public Widget getWidget()
	{
		return main_vbox;
	}

	public void update(bool enable_focus)
	{
		if (Unigine.Console.Active)
			return;

		bool down = Input.IsMouseButtonDown(Input.MOUSE_BUTTON.LEFT);
		bool up = Input.IsMouseButtonUp(Input.MOUSE_BUTTON.LEFT);

		foreach (var tag_widget in tag_widgets)
		{
			tag_widget.Value.update(up, down);
		}

		if (enable_focus && MenuUtils.isHovered(main_scrollbox))
			main_scrollbox.SetFocus();
	}

	public void setTagSelected(string str, bool selected = true)
	{
		if (!tag_widgets.ContainsKey(str))
			return;

		var widget = tag_widgets[str];
		widget.setTagSelected(selected);
	}

	public Event<string> getEventTagClicked() { return event_tag_clicked; }
}


public class Sample
{
	public string title;
	public string description;
	public List<string> tags = new List<string>();
	public string world_name;
};


public class Category
{
	public Unigine.Image icon;
	public string title;

	public List<Sample> samples = new List<Sample>();
};


internal abstract class WidgetSampleListNode
{
	protected WidgetVBox main_vbox = null;
	protected WidgetHBox header_hbox = null;
	protected WidgetVBox children_container_vbox = null;

	protected bool is_hidden = false;
	protected bool is_children_hidden = false;

	protected WidgetSampleListNode parent = null;
	protected List<WidgetSampleListNode> children = new List<WidgetSampleListNode>();

	protected bool pressed = false;
	protected vec4 tint_color;

	public WidgetSampleListNode(MainMenu.UIConfiguration config)
	{
		main_vbox = new WidgetVBox();
		header_hbox = new WidgetHBox();
		children_container_vbox = new WidgetVBox();

		main_vbox.AddChild(header_hbox, Gui.ALIGN_EXPAND);
		main_vbox.AddChild(children_container_vbox, Gui.ALIGN_EXPAND);

		tint_color = config.sampleList.tintColor;
	}
	~WidgetSampleListNode()
	{
		children.Clear();
		main_vbox.DeleteLater();
	}

	public void addChild(WidgetSampleListNode node)
	{
		children_container_vbox.AddChild(node.getWidget(), Gui.ALIGN_EXPAND);
		children.Add(node);
		node.parent = this;
	}

	public int getNumChildren() { return children.Count; }
	public WidgetSampleListNode getChild(int index) { return children[index]; }
	public Widget getWidget() { return main_vbox; }

	public abstract void getSearchInfo(ref string main_info, ref List<string> tags);
	public abstract string getTitle();

	public void setHidden(bool hide) {
		if (hide == is_hidden)
			return;

		is_hidden = hide;

		main_vbox.Hidden = is_hidden;
		main_vbox.Enabled = !is_hidden;
	}
	public bool isHidden() { return is_hidden; }

	public virtual void setChildrenHidden(bool hide)
	{
		if (hide == is_children_hidden)
			return;

		is_children_hidden = hide;

		children_container_vbox.Hidden = is_children_hidden;
		children_container_vbox.Enabled = !is_children_hidden;
	}
	public bool isChildrenHidden() { return is_children_hidden; }
	
	public virtual void update()
	{
		if (!isChildrenHidden())
			for (int i = 0; i < children.Count; ++i)
				children[i].update();

		if (Unigine.Console.Active)
			return;

		bool hovered = MenuUtils.isHovered(header_hbox);

		bool mouse_pressed = Input.IsMouseButtonPressed(Input.MOUSE_BUTTON.LEFT);

		bool down = Input.IsMouseButtonDown(Input.MOUSE_BUTTON.LEFT);
		bool up = Input.IsMouseButtonUp(Input.MOUSE_BUTTON.LEFT);

		if (pressed && up)
		{
			pressed = false;
			if (hovered)
				on_clicked();
		}

		if (!pressed && down && hovered)
			pressed = true;

		show_press_effect(pressed);
	}

	public virtual void setTagSelected(string str, bool selected = true) { }

	protected abstract void on_clicked();
	protected void show_press_effect(bool show)
	{
		header_hbox.Color = show ? tint_color : vec4.WHITE;
	}
}

internal class WidgetSample : WidgetSampleListNode
{
	private WidgetVBox header_vbox = null;

	private WidgetLabel title_label = null;
	private WidgetLabel description_label = null;

	private WidgetVBox tags_vbox = null;
	private Dictionary<string, Tag> tag_widgets = new Dictionary<string, Tag>();

	Sample sample_info;
	string search_info;

	public WidgetSample(Sample sample, MainMenu.UIConfiguration config) : base(config)
	{
		var sample_params = config.sampleList.sample;
		sample_info = sample;

		header_hbox.Background = 1;
		header_hbox.BackgroundTexture = sample_params.background.Path;
		header_hbox.BackgroundColor = sample_params.backgroundColor;
		header_hbox.Background9Sliced = true;
		vec4 offsets = sample_params.backgroundSliceOffsets;
		header_hbox.SetBackground9SliceOffsets(offsets.x, offsets.y, offsets.z, offsets.w);
		header_hbox.Background9SliceScale = sample_params.backgroundSliceScale;

		ivec4 padding = sample_params.background_padding;
		header_hbox.SetPadding(padding.x, padding.y, padding.z, padding.w);


		header_vbox = new WidgetVBox();
		header_vbox.SetSpace(0, sample_params.contentVerticalSpacing);
		header_hbox.AddChild(header_vbox, Gui.ALIGN_EXPAND);

		var title_hbox = new WidgetHBox();

		title_label = new WidgetLabel(sample_info.title);
		title_label.SetFont(sample_params.titleFont.Path);
		title_label.FontSize = sample_params.titleFontSize;
		title_label.FontColor = sample_params.titleFontColor;
		//	title_label.setFontHSpacing(1);
		title_hbox.AddChild(title_label, Gui.ALIGN_CENTER);
		header_vbox.AddChild(title_hbox, Gui.ALIGN_LEFT);

		var description_hbox = new WidgetHBox();
		description_label = new WidgetLabel(sample_info.description);
		description_label.SetFont(sample_params.descriptionFont.Path);
		description_label.FontSize = sample_params.descriptionFontSize;
		description_label.FontColor = sample_params.descriptionFontColor;
		description_label.FontRich = 1;
		description_label.FontWrap = 1;
		description_label.FontVSpacing = sample_params.descriptionFontVspacing;
		description_hbox.AddChild(description_label, Gui.ALIGN_EXPAND);
		header_vbox.AddChild(description_hbox, Gui.ALIGN_EXPAND);

		string tag_info = "";
		var tags_vbox = new WidgetVBox(0, sample_params.spaceBetweenRowsOfTags);
		int x_space = sample_params.spaceBetweenTagsInRrow;
		var current_tags_hbox = new WidgetHBox(x_space);
		current_tags_hbox.SetPadding(-x_space, -x_space, 0, 0);
		tags_vbox.AddChild(current_tags_hbox, Gui.ALIGN_LEFT);
		int max_tags_hbox_width = config.sampleList.sampleListMinWidth
			- config.sampleList.padding.x - config.sampleList.padding.y
			- sample_params.sideOffset - sample_params.background_padding.x
			- sample_params.background_padding.y;
		for (int i = 0; i < sample.tags.Count; ++i)
		{
			tag_info += " " + sample.tags[i];
			var tag = new Tag(sample.tags[i], MenuUtils.TAG_PLACE.SAMPLES_LIST);
			tag_widgets[sample.tags[i]] = tag;
			var tag_widget = tag.getWidget();
			tag_widget.Arrange();
			current_tags_hbox.Arrange();

			if (tag_widget.Width + current_tags_hbox.Width < max_tags_hbox_width)
			{
				current_tags_hbox.AddChild(tag_widget, Gui.ALIGN_LEFT);
			}
			else
			{
				current_tags_hbox = new WidgetHBox(x_space);
				current_tags_hbox.SetPadding(-x_space, -x_space, 0, 0);
				tags_vbox.AddChild(current_tags_hbox, Gui.ALIGN_LEFT);

				current_tags_hbox.AddChild(tag_widget, Gui.ALIGN_LEFT);
			}
		}
		header_vbox.AddChild(tags_vbox, Gui.ALIGN_LEFT);

		search_info += sample_info.title.ToLower();
		search_info += " ";
		search_info += sample_info.description.ToLower();
		search_info += " ";
		search_info += tag_info.ToLower();
	}
	~WidgetSample()	{}

	public override void getSearchInfo(ref string main_info, ref List<string> tags)
	{
		string category_search_info = "";
		List<string> empty = new List<string>();
		parent.getSearchInfo(ref category_search_info, ref empty);

		main_info = "";
		main_info += search_info;
		main_info += category_search_info.ToLower();

		tags.Clear();
		tags = sample_info.tags;
	}

	public override string getTitle() { return sample_info.title; }

	public override void setTagSelected(string str, bool selected = true) {
		if (!tag_widgets.ContainsKey(str))
			return;

		var widget = tag_widgets[str];
		widget.setTagSelected(selected);
	}

	public override void update()
	{
		if (isHidden())
			return;

		if (pressed)
		{
			base.update();
			return;
		}

		bool down = Input.IsMouseButtonDown(Input.MOUSE_BUTTON.LEFT);
		bool up = Input.IsMouseButtonUp(Input.MOUSE_BUTTON.LEFT);

		bool some_hovered = false;
		foreach (var tag in tag_widgets)
		{
			some_hovered |= MenuUtils.isHovered(tag.Value.getWidget());
			tag.Value.update(up, down);
		}

		if (!some_hovered)
			base.update();
	}

	protected override void on_clicked()
	{
		World.LoadWorld(sample_info.world_name, true);
	}
}


internal class WidgetCategory : WidgetSampleListNode
{
	private bool was_pressed = false;

	private WidgetSprite icon_sprite;
	private WidgetLabel title_label;
	private WidgetSprite arrow_sprite;

	private Category category_info;

	public WidgetCategory(Category category, MainMenu.UIConfiguration config) : base(config)
	{
		var category_params = config.sampleList.category;
		category_info = category;

		header_hbox.Background = 1;
		header_hbox.BackgroundTexture = category_params.background.Path;
		header_hbox.BackgroundColor = category_params.backgroundColor;
		header_hbox.Background9Sliced = true;
		vec4 offsets = category_params.backgroundSliceOffsets;
		header_hbox.SetBackground9SliceOffsets(offsets.x, offsets.y, offsets.z, offsets.w);
		header_hbox.Background9SliceScale = category_params.backgroundSliceScale;

		ivec4 padding = category_params.background_padding;
		header_hbox.SetPadding(padding.x, padding.y, padding.z, padding.w);

		children_container_vbox.SetSpace(0, category_params.vertical_spacing);
		children_container_vbox.SetPadding(config.sampleList.sample.sideOffset, 0, 0, 0);

		icon_sprite = new WidgetSprite();
		icon_sprite.SetImage(category_info.icon);
		icon_sprite.Height = category_params.iconSize.y;
		icon_sprite.Width = category_params.iconSize.x;
		header_hbox.AddChild(icon_sprite, Gui.ALIGN_LEFT);

		title_label = new WidgetLabel(category_info.title);
		title_label.SetFont(category_params.titleFont.Path);
		title_label.FontSize = category_params.titleFontSize;
		title_label.FontColor = category_params.titleFontColor;
		var title_box = new WidgetVBox();
		title_box.SetPadding(category_params.titleOffset, 0, 0, 0);
		title_box.AddChild(title_label, Gui.ALIGN_LEFT);
		header_hbox.AddChild(title_box, Gui.ALIGN_LEFT);

		arrow_sprite = new WidgetSprite();
		arrow_sprite.Texture = category_params.arrowIcon.Path;
		arrow_sprite.Color = category_params.arrowIconColor;
		arrow_sprite.Width = category_params.arrowIconSize;
		arrow_sprite.Height = category_params.arrowIconSize;
		var icon_padding = category_params.arrowOffset;
		var arrow_box = new WidgetVBox();
		arrow_box.SetPadding(icon_padding.x, icon_padding.y, icon_padding.z, icon_padding.w);
		arrow_box.AddChild(arrow_sprite, Gui.ALIGN_CENTER);
		header_hbox.AddChild(arrow_box, Gui.ALIGN_RIGHT);

		List<Sample> samples = category.samples;
		for (int i = 0; i < samples.Count; ++i)
		{
			Sample sample = samples[i];

			WidgetSample widget = new WidgetSample(sample, config);
			addChild(widget);
		}
	}
	~WidgetCategory() { }

	public override void getSearchInfo(ref string main_info, ref List<string> tags)
	{
		main_info = "";
		tags.Clear();

		main_info += title_label.Text;
	}

	public override string getTitle() { return category_info.title; }

	public override void setTagSelected(string str, bool selected)
	{
		for (int i = 0; i < children.Count; ++i)
			children[i].setTagSelected(str, selected);
	}

	public override void setChildrenHidden(bool hide)
	{
		base.setChildrenHidden(hide);

		if (!children_container_vbox.Hidden)
			arrow_sprite.TexCoord = new vec4(0.0f, 1.0f, 1.0f, 0.0f);
		else
			arrow_sprite.TexCoord = new vec4(0.0f, 0.0f, 1.0f, 1.0f);
	}

	protected override void on_clicked()
	{
		setChildrenHidden(!isChildrenHidden());
	}
}


internal class WidgetSampleList
{
	private WidgetScrollBox main_scrollbox;

	List<WidgetSampleListNode> roots = new List<WidgetSampleListNode>();

	public WidgetSampleList(List<Category> categories, MainMenu.UIConfiguration config)
	{
		main_scrollbox = new WidgetScrollBox(0, config.sampleList.verticalSpacing);
		main_scrollbox.HScrollEnabled = false;
		main_scrollbox.Width = config.sampleList.sampleListMinWidth;
		main_scrollbox.VScrollHidden = WidgetScrollBox.SCROLL_RENDER_MODE.ALWAYS_HIDE_NO_BOUNDS;
		main_scrollbox.Border = 0;

		ivec4 padding = config.sampleList.padding;
		main_scrollbox.SetPadding(padding.x, padding.y, padding.z, padding.w);

		// temp widget to get scrollbox vbox widget
		var temp = new WidgetVBox();
		main_scrollbox.AddChild(temp);

		var background_vbox = temp.Parent as WidgetVBox;
		background_vbox.Background = 1;
		background_vbox.BackgroundTexture = config.common.white_image.Path;
		background_vbox.BackgroundColor = config.sampleList.sampleListBackgroundColor;

		for (int i = 0; i < categories.Count; ++i)
		{
			Category category = categories[i];

			WidgetCategory widget = new WidgetCategory(category, config);
			widget.setChildrenHidden(true);

			background_vbox.AddChild(widget.getWidget());

			roots.Add(widget);
		}

		main_scrollbox.RemoveChild(temp);
		temp.DeleteLater();
	}
	~WidgetSampleList()
	{
		roots.Clear();
		main_scrollbox.DeleteLater();
	}

	public void setTagSelected(string str, bool selected)
	{
		for (int i = 0; i < roots.Count; ++i)
			roots[i].setTagSelected(str, selected);
	}

	public Widget getWidget() { return main_scrollbox; }

	public void filter(List<string> search_words, List<string> search_tags)
	{
		for (int i = 0; i < roots.Count; ++i)
			filter_node(roots[i], search_words, search_tags);
	}

	public void update(bool enable_focus)
	{
		for (int i = 0; i < roots.Count; ++i)
			roots[i].update();

		bool hovered = MenuUtils.isHovered(main_scrollbox);

		if (enable_focus && hovered)
			main_scrollbox.SetFocus();
	}

	public void setCollapseAll(bool collapse = true)
	{
		for (int i = 0; i < roots.Count; ++i)
			roots[i].setChildrenHidden(collapse);
	}

	public void save(Unigine.Blob blob)
	{
		int num_roots = roots.Count;
		blob.WriteInt(num_roots);

		foreach (var root in roots)
	{
			blob.WriteString(root.getTitle());
			blob.WriteBool(root.isChildrenHidden());
		}

		int scroll = main_scrollbox.VScrollValue;
		blob.WriteInt(scroll);
	}

	public void restore(Unigine.Blob blob)
	{
		int num_roots = blob.ReadInt();

		Dictionary<string, bool> hidden_children = new Dictionary<string, bool>();
		hidden_children.Clear();
		hidden_children.EnsureCapacity(num_roots);

		for (int i = 0; i < num_roots; i++)
		{
			String title = blob.ReadString();
			bool is_hidden = blob.ReadBool();
			hidden_children[title] = is_hidden;
		}

		foreach (var root in roots)
		{
			String title = root.getTitle();
			if (hidden_children.ContainsKey(title))
			{
				root.setChildrenHidden(hidden_children[title]);
			}
		}

		int scroll_value = blob.ReadInt();
		main_scrollbox.Arrange();
		main_scrollbox.VScrollValue = scroll_value;
	}

	private void filter_node(WidgetSampleListNode node, List<string> search_words, List<string> search_tags)
	{
		int num_children = node.getNumChildren();

		if (num_children > 0)
		{
			int num_enabled = 0;
			for (int i = 0; i < num_children; ++i)
			{
				filter_node(node.getChild(i), search_words, search_tags);

				if (!node.getChild(i).isHidden())
					++num_enabled;
			}

			node.setHidden(num_enabled == 0);
			if (!node.isHidden())
				node.setChildrenHidden(false);
		}
		else
		{
			string main_info = "";
			List<string> node_tags = new List<string>();
			node.getSearchInfo(ref main_info, ref node_tags);

			if (search_words.Count <= 0 && search_tags.Count <= 0)
			{
				node.setHidden(false);
				return;
			}

			int num_tag_matches = 0;
			// check tags
			for (int i = 0; i < node_tags.Count; ++i)
			{
				for (int j = 0; j < search_tags.Count; ++j)
				{
					if (node_tags[i].ToLower() == search_tags[j])
					{
						++num_tag_matches;
						break;
					}
				}
			}

			bool tags_search = num_tag_matches == search_tags.Count;
			if (!tags_search)
			{
				node.setHidden(true);
				return;
			}

			bool words_search = search_words.Count <= 0;
			// check words
			for (int i = 0; i < search_words.Count; ++i)
			{
				if (MenuUtils.isWordSuitable(main_info, search_words[i]))
				{
					words_search = true;
					break;
				}
			}

			node.setHidden(!words_search);
		}
	}
}


public class MenuUtils
{
	public enum TAG_PLACE
	{
		SEARCH_FIELD,
		SAMPLES_LIST,
		TAGS_CLOUD
	}

	public static WidgetHBox createTagWidget(string text, MainMenu.UITagStyle config, TAG_PLACE place)
	{
		var main_hbox = new WidgetHBox();

		ivec4 padding = config.backgroundPadding;
		switch (place)
		{
			case TAG_PLACE.SEARCH_FIELD:
				padding = new ivec4(MathLib.ToInt(padding.x * config.searchFieldScale),
					MathLib.ToInt(padding.y * config.searchFieldScale),
					MathLib.ToInt(padding.z * config.searchFieldScale),
					MathLib.ToInt(padding.w * config.searchFieldScale));
				break;
			case TAG_PLACE.SAMPLES_LIST:
				padding = new ivec4(MathLib.ToInt(padding.x * config.sampleListScale),
					MathLib.ToInt(padding.y * config.sampleListScale),
					MathLib.ToInt(padding.z * config.sampleListScale),
					MathLib.ToInt(padding.w * config.sampleListScale));
				break;
			case TAG_PLACE.TAGS_CLOUD:
				padding = new ivec4(MathLib.ToInt(padding.x * config.tagCloudScale),
					MathLib.ToInt(padding.y * config.tagCloudScale),
					MathLib.ToInt(padding.z * config.tagCloudScale),
					MathLib.ToInt(padding.w * config.tagCloudScale));
				break;
		}

		main_hbox.SetPadding(padding.x, padding.y, padding.z, padding.w);

		main_hbox.Background = 1;
		main_hbox.BackgroundColor = (config.backgroundColor);
		main_hbox.BackgroundTexture = (config.background.Path);
		main_hbox.Background9Sliced = (true);
		vec4 offsets = config.backgroundSliceOffsets;
		main_hbox.SetBackground9SliceOffsets(offsets.x, offsets.y, offsets.z, offsets.w);
		main_hbox.Background9SliceScale = config.backgroundSliceScale;

		var label = new WidgetLabel(text);
		label.SetFont(config.font.Path);
		int font_size = config.fontSize;
		var font_color = config.fontColor;
		switch (place)
		{
			case TAG_PLACE.SEARCH_FIELD:
				font_size = MathLib.ToInt(font_size * config.searchFieldScale);
				font_color = config.selectedFontColor;
				label.SetFont(config.selectedFont.Path);
				break;
			case TAG_PLACE.SAMPLES_LIST:
				font_size = MathLib.ToInt(font_size * config.sampleListScale);
				break;
			case TAG_PLACE.TAGS_CLOUD:
				font_size = MathLib.ToInt(font_size * config.tagCloudScale);
				break;
		}
		label.FontSize = font_size;
		label.FontColor = font_color;

		main_hbox.AddChild(label);

		if (place == TAG_PLACE.SEARCH_FIELD)
		{
			main_hbox.BackgroundTexture = config.selectedBackground.Path;
			main_hbox.SetSpace(5, 0);
			main_hbox.SetPadding(padding.x, padding.y - 5, padding.z, padding.w);

			var remove_button_sprite = new WidgetSprite();
			remove_button_sprite.SetLayerTexture(0, config.removeButtonBackground.Path);
			remove_button_sprite.SetLayerColor(0, config.removeButtonBackgroundColor);
			remove_button_sprite.AddLayer();
			remove_button_sprite.SetLayerTexture(1, config.removeButtonIcon.Path);
			remove_button_sprite.SetLayerColor(1, config.removeButtonIconColor);
			remove_button_sprite.Width = MathLib.ToInt(config.removeButtonSize * config.searchFieldScale);
			remove_button_sprite.Height = MathLib.ToInt(config.removeButtonSize * config.searchFieldScale);

			main_hbox.AddChild(remove_button_sprite, Gui.ALIGN_RIGHT);
		}

		return main_hbox;
	}

	public static bool isWordSuitable(string text, string word)
	{
		for (int i = 0; i < text.Length - word.Length + 1; ++i)
		{
			int num_matches = 0;
			for (int j = 0; j < word.Length; ++j)
			{
				if (text[i + j] == word[j])
					++num_matches;
				else
					break;
			}

			if (num_matches == word.Length)
				return true;
		}

		return false;
	}

	public static bool isHovered(Widget widget)
	{
		bool hovered = widget.MouseX >= 0 && widget.MouseX < widget.Width
		&& widget.MouseY >= 0 && widget.MouseY < widget.Height;
		return hovered;
	}
}


internal class Tag
{
	private string text;
	private WidgetHBox tagWidget = null;
	private WidgetLabel label = null;

	private bool isPressed = false;
	private bool isSelected = false;

	static private MainMenu.UITagStyle config;
	static private EventInvoker<string> eventClicked = new EventInvoker<string>();


	public Tag(string text_, MenuUtils.TAG_PLACE place)
	{
		text = text_;
		tagWidget = MenuUtils.createTagWidget(text_, config, place);
		for (int i = 0; i < tagWidget.NumChildren; i++)
		{
			var child = tagWidget.GetChild(i) as WidgetLabel;
			if (child)
			{
				label = child;
				break;
			}
		}
	}
	~Tag()
	{
		tagWidget.DeleteLater();
	}

	public Widget getWidget()
	{
		return tagWidget;
	}
	public string getText() { return text; }

	public void update(bool up, bool down)
	{
		bool hovered = MenuUtils.isHovered(tagWidget);
		if (!hovered && !up)
			return;

		if (isPressed && up)
		{
			isPressed = false;
			if (hovered)
			{
				eventClicked.Run(text);
				//setTagSelected(true);
			}
		}

		if (!isPressed && down && hovered)
		{
			isPressed = true;
		}

		tagWidget.Color = isPressed ? config.tintColor : vec4.WHITE;
	}

	public void setTagSelected(bool selected = true)
	{
		if (isSelected == selected)
			return;

		isSelected = selected;
		vec4 color = config.fontColor;
		if (selected)
		{
			tagWidget.BackgroundTexture = config.selectedBackground.Path;
			color = config.selectedFontColor;
		}
		else
		{
			tagWidget.BackgroundTexture = config.background.Path;
		}

		if (label)
			label.FontColor = color;
	}

	static public Event<string> getEventClicked() { return eventClicked; }
	static public void setTagConfig(MainMenu.UITagStyle tag_config) { config = tag_config; }
}
