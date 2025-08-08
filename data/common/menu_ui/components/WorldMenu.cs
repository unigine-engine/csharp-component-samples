#region Math Variables
#if UNIGINE_DOUBLE
using Scalar = System.Double;
using Vec2 = Unigine.dvec2;
using Vec3 = Unigine.dvec3;
using Vec4 = Unigine.dvec4;
using Mat4 = Unigine.dmat4;
#else
using Scalar = System.Single;
using Vec2 = Unigine.vec2;
using Vec3 = Unigine.vec3;
using Vec4 = Unigine.vec4;
using Mat4 = Unigine.mat4;
using WorldBoundBox = Unigine.BoundBox;
using WorldBoundSphere = Unigine.BoundSphere;
using WorldBoundFrustum = Unigine.BoundFrustum;
#endif
#endregion


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unigine;

[Component(PropertyGuid = "fccd3384dd4529130a2a9b56e5e31abd02139cf8")]
public class WorldMenu : Component
{
	public class UIFont
	{
		[ShowInEditor] public AssetLink font;
		[ShowInEditor] public int fontSize;
		[ParameterColor]
		[ShowInEditor] public vec4 fontColor = vec4.WHITE;
	}

	public class UIIcon
	{
		[ShowInEditor] public AssetLink icon;
		[ParameterColor]
		[ShowInEditor] public vec4 iconColor = vec4.WHITE;
		[ShowInEditor] public ivec2 iconSize;
	}


	public class UITooltip
	{
		[ShowInEditor] public AssetLink background;
		[ParameterColor]
		[ShowInEditor] public vec4 backgroundColor;
		[ShowInEditor] public int height;
		[ShowInEditor] public ivec4 padding;
		[ShowInEditor] public float timeDelay;
		[ShowInEditor] public ivec2 pos;
		public UIFont font;
	}


	public class UICommon
	{
		[ShowInEditor] public ivec4 padding;
		[ShowInEditor] public int bottomOffset;
		[ShowInEditor] public int rightOffset;
		[ShowInEditor] public int spaceBetweenHintAndNavigation;
	}

	public class UIHintPanel
	{
		[ShowInEditor] public int spaceX;
		[ShowInEditor] public AssetLink background;
		[ParameterColor]
		[ShowInEditor] public vec4 backgroundColor;
		[ShowInEditor] public vec4 backgroundSliceOffsets;
		[ShowInEditor] public float backgroundSliceScale;
		[ShowInEditor] public ivec4 backgroundPadding;
		[ShowInEditor] public int width;
		[ShowInEditor] public int height;


		public UIIcon icon ;
		public UIFont label ;
	};

	public class UIBackPanel
	{
		[ShowInEditor] public AssetLink background;
		[ShowInEditor] public AssetLink backgroundHover;
		[ParameterColor]
		[ShowInEditor] public vec4 backgroundColor;
		[ParameterColor]
		[ShowInEditor] public vec4 buttonTintColor;
		[ShowInEditor] public vec4 backgroundSliceOffsets;
		[ShowInEditor] public float backgroundSliceScale;
		[ShowInEditor] public ivec4 backgroundPadding;

		public UIIcon icon;
		public UIFont label;

		[ShowInEditor] public int spaceBetweenIconAndLabel;
	};
	public class UIButton
	{
		[ShowInEditor] public AssetLink background;
		[ShowInEditor] public AssetLink backgroundHover;
		[ParameterColor]
		[ShowInEditor] public vec4 backgroundColor = vec4.WHITE;
		[ParameterColor]
		[ShowInEditor] public vec4 backgroundTintColor = new vec4(0.68f, 0.68f, 0.68f, 1.0f);

		[ShowInEditor] public int buttonWidth;
		[ShowInEditor] public int buttonHeight;
	};

	public class UINavigationButton : UIButton
	{
		[ShowInEditor] public vec4 backgroundSliceOffsets;
		[ShowInEditor] public float backgroundSliceScale;

		public UIIcon icon ;
		[ShowInEditor] public string tooltipText;
	}


	public class UIWorldButton : UIButton
	{
		[ParameterColor]
		[ShowInEditor] public vec4 backgroundSelectedColor;

		public UIFont font;
	}

	public class UINavigationPanel
	{
		[ShowInEditor][Parameter(Group = "Selector Button")] public AssetLink background;
		[ShowInEditor][Parameter(Group = "Selector Button")] public AssetLink backgroundHover;
		[ShowInEditor][Parameter(Group = "Selector Button")] public vec4 backgroundSliceOffsets;
		[ShowInEditor][Parameter(Group = "Selector Button")] public float backgroundSliceScale;
		[ShowInEditor][Parameter(Group = "Selector Button")] public ivec4 backgroundPadding;
		[ShowInEditor][Parameter(Group = "Selector Button")] public int width;
		[ShowInEditor][Parameter(Group = "Selector Button")] public int height;
		[ParameterColor(Group = "Selector Button")]
		[ShowInEditor] public vec4 backgroundColor = vec4.WHITE;
		[ParameterColor(Group = "Selector Button")]
		[ShowInEditor] public vec4 backgroundTintColor = new vec4(0.68f, 0.68f, 0.68f, 1.0f);

		[ShowInEditor][Parameter(Group = "Selector Button")] public int spaceBetweenIconAndLabel;
		[ShowInEditor][Parameter(Group = "Selector Button")] public UIIcon icon;
		[ShowInEditor][Parameter(Group = "Selector Button")] public UIFont label;

		public UITooltip tooltip ;

		public UINavigationButton button_prev;
		public UINavigationButton button_next;
	}

	public class UISelectorList
	{
		[ShowInEditor][Parameter(Group = "Main Widget")] public AssetLink background;
		[ShowInEditor][Parameter(Group = "Main Widget")] public vec4 backgroundSliceOffsets;
		[ShowInEditor][Parameter(Group = "Main Widget")] public float backgroundSliceScale;
		[ShowInEditor][Parameter(Group = "Main Widget")] public ivec4 backgroundPadding;
		[ShowInEditor][Parameter(Group = "Main Widget")] public int height;

		[ShowInEditor][Parameter(Group = "Header")] public int headerHeight;
		[ShowInEditor][Parameter(Group = "Header")] public ivec4 headerPadding;
		[ShowInEditor][Parameter(Group = "Header")] public UIFont headerFont;

		[ShowInEditor][Parameter(Group = "Spacer")] public AssetLink spacerBackground;
		[ShowInEditor][Parameter(Group = "Spacer")] public int spacerHeight;

		[ShowInEditor][Parameter(Group = "List")] public int spaceBetweenButtons;
		[ShowInEditor][Parameter(Group = "List")] public ivec4 listPadding;
		[ShowInEditor] public UIWorldButton button = null;
	}

	[ShowInEditor] public string mainWorldName;
	[ShowInEditor] private UICommon common = null;
	[ShowInEditor] private UIHintPanel hint = null;
	[ShowInEditor] private UIBackPanel back = null;
	[ShowInEditor] private UINavigationPanel navigation = null;
	[ShowInEditor] private UISelectorList selector = null;

	private string currentWorldName = "";
	private string currentTitle = "";

	private WidgetHBox mainHBox = null;

	private WidgetHBox hintHBox = null;

	private WidgetHBox backHBox = null;
	private bool backPrevPressed = false;
	private bool backPrevHovered = false;
	private bool isPressed = false;

	private WidgetHBox navigationHBbox;
	private NavigationButton navPrev;
	private NavigationButton navNext;
	private SelectorButton navSelect;

	private WidgetHBox selectorHBox;
	private WidgetVBox selectorVBox;
	private WidgetScrollBox selectorScroll;
	private List<WorldButton> selectButtons = new();
	private bool isSelectorHovered = false;

	private Input.MOUSE_HANDLE mouseHandleAtClick = Input.MOUSE_HANDLE.USER;

	private static bool isSelectionActive;

	private void Init()
	{
		var sample = SamplesManager.Instance.getSampleByWorldPath(World.Path);
		if (sample != null)
		{
			currentTitle = sample.title;
			currentWorldName = sample.world_name;
		}
		else
		{
			currentTitle = SamplesManager.Instance.GetFileName(World.Path);
			currentWorldName = currentTitle;
		}

		mainHBox = new WidgetHBox();
		ivec4 padding = common.padding;
		mainHBox.SetPadding(padding.x, padding.y, padding.z, padding.w);

		// create hint box
		InitHintBox();

		// create back button
		InitBackButton();
		InitNavigationBar();
		navSelect.SetButtonPressed(isSelectionActive);

		var navigation = new WidgetVBox();
		navigation.AddChild(selectorHBox);
		navigation.AddChild(navigationHBbox);

		int space_y = common.spaceBetweenHintAndNavigation;
		var aux_vbox = new WidgetVBox(0, space_y);
		aux_vbox.SetPadding(0, 0, -space_y, -space_y);
		aux_vbox.AddChild(hintHBox);
		aux_vbox.AddChild(navigation);
		//	main_hbox.addChild(navigation_hbox, Gui.ALIGN_BOTTOM | Gui.ALIGN_CENTER);
		mainHBox.AddChild(aux_vbox, Gui.ALIGN_BOTTOM | Gui.ALIGN_CENTER);

		aux_vbox = new WidgetVBox();
		aux_vbox.AddChild(backHBox, Gui.ALIGN_BOTTOM /*, Gui.ALIGN_OVERLAP*/);

		mainHBox.AddChild(aux_vbox, Gui.ALIGN_BOTTOM | Gui.ALIGN_RIGHT);
		WindowManager.MainWindow.AddChild(mainHBox, Gui.ALIGN_OVERLAP);
	}

	private void Update()
	{
		bool down = Input.IsMouseButtonDown(Input.MOUSE_BUTTON.LEFT);
		bool up = Input.IsMouseButtonUp(Input.MOUSE_BUTTON.LEFT);

		hintHBox.Hidden = !Input.MouseGrab;
		hintHBox.Enabled = Input.MouseGrab;

		UpdateBackButton(up, down);
		UpdateNavigation(up, down);

		// adjust UI position in case if window size changed
		ivec2 size = WindowManager.MainWindow.ClientSize;

		mainHBox.Width = size.x;
		mainHBox.Arrange();

		mainHBox.PositionX = size.x - common.rightOffset - mainHBox.Width;
		mainHBox.PositionY = size.y - common.bottomOffset - mainHBox.Height;
	}

	private void Shutdown()
	{
		mainHBox.DeleteLater(); 
		hintHBox.DeleteLater();
		backHBox.DeleteLater();

		selectorHBox.DeleteLater();
		selectButtons.Clear();
	}

	private void InitBackButton()
	{
		int space = back.spaceBetweenIconAndLabel;
		backHBox = new WidgetHBox(space, 0);
		backHBox.Background = 1;
		backHBox.BackgroundTexture = back.background.Path;
		backHBox.BackgroundColor = back.backgroundColor;
		backHBox.Background9Sliced = true;
		vec4 back_offsets = back.backgroundSliceOffsets;
		backHBox.SetBackground9SliceOffsets(back_offsets.x, back_offsets.y, back_offsets.z,
			back_offsets.w);
		backHBox.Background9SliceScale = back.backgroundSliceScale;
		ivec4 back_padding = back.backgroundPadding;
		backHBox.SetPadding(back_padding.x - space, back_padding.y - space, back_padding.z,
			back_padding.w);

		var back_button_sprite = new WidgetSprite();
		back_button_sprite.AddLayer();
		back_button_sprite.SetLayerTexture(0, back.icon.icon.Path);
		back_button_sprite.SetLayerColor(0, back.icon.iconColor);
		back_button_sprite.Width = back.icon.iconSize.x;
		back_button_sprite.Height = back.icon.iconSize.y;
		backHBox.AddChild(back_button_sprite, Gui.ALIGN_LEFT);

		var back_label = new WidgetLabel("<p align=center>Back to Main Menu</p>");
		back_label.FontRich = 1;
		back_label.SetFont(back.label.font.Path);
		back_label.FontSize = back.label.fontSize;
		back_label.FontColor = back.label.fontColor;
		backHBox.AddChild(back_label, Gui.ALIGN_RIGHT);
	}

	private void InitHintBox()
	{
		int space = hint.spaceX;
		hintHBox = new WidgetHBox(space, 0);
		hintHBox.Background = 1;
		hintHBox.Height = hint.height;
		hintHBox.BackgroundTexture = hint.background.Path;
		hintHBox.BackgroundColor = hint.backgroundColor;
		hintHBox.Background9Sliced = true;
		vec4 hint_offsets = hint.backgroundSliceOffsets;
		hintHBox.SetBackground9SliceOffsets(hint_offsets.x, hint_offsets.y, hint_offsets.z,
			hint_offsets.w);
		hintHBox.Background9SliceScale = hint.backgroundSliceScale;
		ivec4 hint_padding = hint.backgroundPadding;
		hintHBox.SetPadding(hint_padding.x - space, hint_padding.y - space, hint_padding.z,
			hint_padding.w);

		var hint_sprite = new WidgetSprite();
		hint_sprite.AddLayer();
		hint_sprite.SetLayerTexture(0, hint.icon.icon.Path);
		hint_sprite.SetLayerColor(0, hint.icon.iconColor);
		hint_sprite.Width = hint.icon.iconSize.x;
		hint_sprite.Height = hint.icon.iconSize.y;
		hintHBox.AddChild(hint_sprite, Gui.ALIGN_LEFT);

		var hint_label = new WidgetLabel("Press <b>ESC</b> to interact with UI");
		hint_label.FontRich = 1;
		hint_label.SetFont(hint.label.font.Path);
		hint_label.FontSize = hint.label.fontSize;
		hint_label.FontColor = hint.label.fontColor;
		hintHBox.AddChild(hint_label);
	}

	private void InitNavigationBar()
	{
		// create selector box
		InitSelector();

		navigation.button_prev.buttonHeight = navigation.height;
		navigation.button_next.buttonHeight = navigation.height;

		// create selector button
		navSelect = new SelectorButton(currentTitle, navigation,
			(bool val) => { selectorHBox.Hidden = !val; });

		string prev_world = "", next_world = "";
		SamplesManager.Instance.getPrevNextSamplesID(currentWorldName, ref prev_world, ref next_world);

		// create button to the prev sample
		Sample s = SamplesManager.Instance.getSampleByID(prev_world);
		navPrev = new NavigationButton(prev_world, s != null ? s.title : prev_world,
		navigation.button_prev, navigation.tooltip);
		// create button to the next sample
		s = SamplesManager.Instance.getSampleByID(next_world);
		navNext = new NavigationButton(next_world, s != null ? s.title : next_world,
		navigation.button_next, navigation.tooltip);

		navigationHBbox = new WidgetHBox();
		navigationHBbox.AddChild(navPrev.GetWidget(), Gui.ALIGN_LEFT);
		navigationHBbox.AddChild(navSelect.GetWidget(), Gui.ALIGN_LEFT);
		navigationHBbox.AddChild(navNext.GetWidget(), Gui.ALIGN_LEFT);
	}

	private void InitSelector()
	{
		var category = SamplesManager.Instance.getCategoryBySampleID(currentWorldName);
		string title = category != null ? category.title : "No Category";

		int width = navigation.width;
		selector.button.buttonWidth = width;

		// create main box
		selectorHBox = new WidgetHBox();
		selectorVBox = new WidgetVBox();
		selectorVBox.Background = 1;
		selectorVBox.BackgroundTexture = selector.background.Path;
		selectorVBox.Background9Sliced = true;
		vec4 nav_offsets = selector.backgroundSliceOffsets;
		selectorVBox.SetBackground9SliceOffsets(nav_offsets.x, nav_offsets.y, nav_offsets.z,
			nav_offsets.w);
		selectorVBox.Background9SliceScale = selector.backgroundSliceScale;
		selectorVBox.Height = selector.height;
		selectorVBox.Width = width;

		// create header with category name
		var header = new WidgetHBox();
		ivec4 padding = selector.headerPadding;
		header.SetPadding(padding.x, padding.y, padding.z, padding.w);

		string text = "Samples C# - " + title;
		var selector_label = new WidgetLabel(text);
		selector_label.SetFont(selector.headerFont.font.Path);
		selector_label.FontSize = selector.headerFont.fontSize;
		selector_label.FontColor = selector.headerFont.fontColor;
		header.AddChild(selector_label);

		var spacer = new WidgetHBox();
		spacer.Background = 1;
		spacer.BackgroundTexture = selector.spacerBackground.Path;
		spacer.Height = selector.spacerHeight;
		spacer.Width = width - 2;

		// create samples scrollbox
		int space = selector.spaceBetweenButtons;
		selectorScroll = new WidgetScrollBox(0, space);
		selectorScroll.Border = 0;
		padding = selector.listPadding;
		selectorScroll.SetPadding(padding.x, padding.y, padding.z, padding.w);
		selectorScroll.HScrollEnabled = false;
		selectorScroll.VScrollHidden = WidgetScrollBox.SCROLL_RENDER_MODE.ALWAYS_HIDE_NO_BOUNDS;
		selectorScroll.Arrange();

		selectorVBox.AddChild(header);
		selectorVBox.AddChild(spacer);
		selectorVBox.AddChild(selectorScroll, Gui.ALIGN_EXPAND);

		selectorHBox.AddChild(selectorVBox);

		if (category != null)
		{
			foreach(var s in category.samples)
			{
				WorldButton button = new WorldButton(s.title, s.world_name, selector.button,
					s.world_name == currentWorldName);
				selectButtons.Add(button);
				selectorScroll.AddChild(button.GetWidget());
			}
		}
		else
		{
			WorldButton button = new WorldButton(currentTitle, currentWorldName,
				selector.button, true);
			selectButtons.Add(button);
			selectorScroll.AddChild(button.GetWidget());
		}
	}

	private	void UpdateNavigation(bool up, bool down)
	{
		navPrev.Update(up, down);
		navNext.Update(up, down);

		foreach (var b in selectButtons)
			b.Update(up, down);

		navSelect.Update(up, down);

		if (!selectorHBox.Hidden)
		{
			UpdateSelector();
		}
	}

	private void UpdateSelector()
	{
		bool hovered = IsHovered(selectorVBox);
		if (hovered && !isSelectorHovered)
		{
			mouseHandleAtClick = Input.MouseHandle;
			Input.MouseHandle = Input.MOUSE_HANDLE.USER;
			isSelectorHovered = hovered;
		}
		if (!hovered && isSelectorHovered)
		{
			Input.MouseHandle = mouseHandleAtClick;
			isSelectorHovered = hovered;
		}

		if (IsHovered(selectorScroll))
		{
			int wheel = Input.MouseWheel;
			if (wheel != 0)
			{
				int value = selectorScroll.VScrollValue;
				int step = selectorScroll.VScrollStepSize;
				selectorScroll.VScrollValue = value - wheel * step * 4;
			}
		}
		else
			selectorScroll.RemoveFocus();
	}

	private void UpdateBackButton(bool up, bool down)
	{
		bool hovered = IsHovered(backHBox);

		if (isPressed && up)
		{
			isPressed = false;
			Input.MouseHandle = mouseHandleAtClick;
			if (hovered)
			{
				World.LoadWorld(mainWorldName, true);
				isSelectionActive = false;
			}
		}

		if (!isPressed && down && hovered)
		{
			isPressed = true;
			// for samples with MOUSE_HANDLE_GRAB
			mouseHandleAtClick = Input.MouseHandle;
			Input.MouseHandle = Input.MOUSE_HANDLE.USER;
		}

		if (backPrevHovered != hovered)
		{
			backHBox.BackgroundTexture = hovered ? back.backgroundHover.Path : 
													back.background.Path;
			backPrevHovered = hovered;
		}
		if (backPrevPressed != isPressed)
		{
			backHBox.BackgroundColor = isPressed ? back.buttonTintColor
												  : back.backgroundColor;
			backPrevPressed = isPressed;
		}
	}

	private bool IsHovered(Widget widget)
	{
		bool is_hovered = widget.MouseX > 0
					&& widget.MouseX < widget.Width && widget.MouseY > 0
					&& widget.MouseY < widget.Height;
		return is_hovered;
	}

	public abstract class Button
	{
		public Button() { }
		protected WidgetHBox button_hbox = null;

		protected vec4 background_color;
		protected vec4 background_tint_color;

		protected bool pressed = false;
		protected bool prev_hovered = false;
		protected bool prev_pressed = false;

		protected Input.MOUSE_HANDLE mouse_handle_at_click;

		public abstract void Update(bool up, bool down);
		public WidgetHBox GetWidget() { return button_hbox; }
		public bool IsHovered()
		{
			if (!button_hbox)
				return false;

			bool is_hovered = button_hbox.MouseX > 0
				&& button_hbox.MouseX < button_hbox.Width && button_hbox.MouseY > 0
				&& button_hbox.MouseY < button_hbox.Height;
			return is_hovered;
		}
	}

	public class NavigationButton : Button
	{
		public NavigationButton() { }

		public NavigationButton(string world_path_, string title,
			UINavigationButton ui, UITooltip tooltip)
		{
			world_path = world_path_;
			button_hbox = new WidgetHBox(0, 0);
			button_hbox.Width = ui.buttonWidth;
			button_hbox.Height = ui.buttonHeight;
			button_hbox.Background = 1;
			button_hbox.BackgroundTexture = ui.background.Path;
			button_hbox.Background9Sliced = true;
			vec4 nav_offsets = ui.backgroundSliceOffsets;
			button_hbox.SetBackground9SliceOffsets(nav_offsets.x, nav_offsets.y, nav_offsets.z,
				nav_offsets.w);
			button_hbox.Background9SliceScale = ui.backgroundSliceScale;

			var sprite = new WidgetSprite();
			sprite.AddLayer();
			sprite.SetLayerTexture(0, ui.icon.icon.Path);
			sprite.Width = ui.icon.iconSize.x;
			sprite.Height = ui.icon.iconSize.y;
			button_hbox.AddChild(sprite, Gui.ALIGN_CENTER);
			button_hbox.Arrange();

			background = ui.background.Path;
			background_hover = ui.backgroundHover.Path;

			background_color = ui.backgroundColor;
			background_tint_color = ui.backgroundTintColor;

			tooltip_hbox = new WidgetHBox(0, 0);
			tooltip_hbox.Background = 1;
			tooltip_hbox.BackgroundTexture = tooltip.background.Path;
			tooltip_hbox.BackgroundColor = tooltip.backgroundColor;
			tooltip_hbox.Height = tooltip.height;
			ivec4 padding = tooltip.padding;
			tooltip_hbox.SetPadding(padding.x, padding.y, padding.z, padding.w);

			string text = ui.tooltipText + title;
			var label = new WidgetLabel(text);
			label.SetFont(tooltip.font.font.Path);
			label.FontSize = (tooltip.font.fontSize);
			label.FontColor = (tooltip.font.fontColor);
			label.TextAlign = (Gui.ALIGN_CENTER);

			tooltip_hbox.AddChild(label);
			tooltip_hbox.Hidden = true;
			tooltip_hbox.Order = 125;

			Gui.GetCurrent().AddChild(tooltip_hbox, Gui.ALIGN_OVERLAP);

			time_delay = tooltip.timeDelay < 0.0f ? 0.1f : tooltip.timeDelay;
			pos = tooltip.pos;
		}

		public override void Update(bool up, bool down)
		{
			if (!button_hbox)
				return;

			bool hovered = IsHovered();

			// update tooltip time
			if (prev_hovered && hovered && !(prev_pressed || pressed))
				current_time += Game.IFps;
			else
				current_time = 0.0f;

			// update tooltip state
			if (current_time >= time_delay && tooltip_hbox.Hidden)
			{
				ivec2 mouse_pos = Input.MousePosition - Gui.GetCurrent().Position;
				tooltip_hbox.Hidden = false;
				tooltip_hbox.PositionX = mouse_pos.x + pos.x;
				tooltip_hbox.PositionY = mouse_pos.y - tooltip_hbox.Height + pos.y;
			}

			if (current_time < time_delay && !tooltip_hbox.Hidden)
			{
				tooltip_hbox.Hidden = true;
			}

			// update button state
			if (pressed && up)
			{
				pressed = false;
				Input.MouseHandle = mouse_handle_at_click;
				if (hovered)
					World.LoadWorld(world_path, true);
			}

			if (!pressed && down && hovered)
			{
				pressed = true;
				// for samples with MOUSE_HANDLE_GRAB
				mouse_handle_at_click = Input.MouseHandle;
				Input.MouseHandle = Input.MOUSE_HANDLE.USER;
			}

			if (prev_hovered != hovered)
			{
				button_hbox.BackgroundTexture = hovered ? background_hover : background;
				prev_hovered = hovered;
			}
			if (prev_pressed != pressed)
			{
				button_hbox.BackgroundColor = pressed ? background_tint_color : background_color;
				prev_pressed = pressed;
			}

		}

		private string world_path = "";

		private string background = "";
		private string background_hover = "";

		private WidgetHBox tooltip_hbox;
		private float current_time = 0.0f;
		private float time_delay;
		private ivec2 pos;
	}

	public class SelectorButton : Button
	{
		public SelectorButton() { }
		public SelectorButton(string sample_name, UINavigationPanel config,
			 Action<bool> on_clicked_)
		{
			on_clicked = on_clicked_;

			button_hbox = new WidgetHBox(0, 0);
			button_hbox.Background = 1;
			button_hbox.BackgroundTexture = config.background.Path;
			button_hbox.Background9Sliced = false;
			button_hbox.Width = config.width;
			button_hbox.Height = config.height;

			var sprite = new WidgetSprite();
			sprite.AddLayer();
			sprite.SetLayerTexture(0, config.icon.icon.Path);
			sprite.Width = config.icon.iconSize.x;
			sprite.Height = config.icon.iconSize.y;

			var label = new WidgetLabel(sample_name);
			label.SetFont(config.label.font.Path);
			label.FontSize = config.label.fontSize;
			label.FontColor = config.label.fontColor;

			var aux_hbox = new WidgetHBox(config.spaceBetweenIconAndLabel, 0);
			aux_hbox.AddChild(sprite, Gui.ALIGN_LEFT);
			aux_hbox.AddChild(label, Gui.ALIGN_LEFT);
			button_hbox.AddChild(aux_hbox, Gui.ALIGN_CENTER);

			background = config.background.Path;
			background_hover = config.backgroundHover.Path;

			background_color = config.backgroundColor;
			background_tint_color = config.backgroundTintColor;
		}
		public override void Update(bool up, bool down)
		{
			if (!button_hbox)
				return;

			bool hovered = IsHovered();

			if (pressed && up)
			{
				pressed = false;
				Input.MouseHandle = mouse_handle_at_click;
				if (hovered)
				{
					enabled = !enabled;
					on_clicked(enabled);
					isSelectionActive = enabled;
				}
			}

			if (!pressed && down && hovered)
			{
				pressed = true;
				mouse_handle_at_click = Input.MouseHandle;
				Input.MouseHandle = Input.MOUSE_HANDLE.USER;
			}

			if (prev_hovered != hovered && !enabled)
			{
				button_hbox.BackgroundTexture = hovered ? background_hover : background;
				prev_hovered = hovered;
			}
			if (prev_pressed != pressed && !enabled)
			{
				button_hbox.BackgroundColor = pressed ? background_tint_color : background_color;
				prev_pressed = pressed;
			}
		}
		public void SetButtonPressed(bool is_pressed)
		{
			button_hbox.BackgroundTexture = is_pressed ? background_hover : background;
			button_hbox.BackgroundColor = is_pressed ? background_tint_color : background_color;
			enabled = is_pressed;
			isSelectionActive = enabled;
			pressed = false;
			prev_pressed = true;

			on_clicked(enabled);
		}

		private string background = "";
		private string background_hover = "";

		private Action<bool> on_clicked;
		private bool enabled = false;
	}

	public class WorldButton : Button
	{
		public WorldButton() { }
		public WorldButton(string text, string world_path_,
			UIWorldButton config, bool is_current = false)
		{
			world_path = world_path_;

			background_color = is_current ? config.backgroundSelectedColor
								  : config.backgroundColor;
			background_tint_color = config.backgroundTintColor;
			background_selected_color = config.backgroundSelectedColor;

			button_hbox = new WidgetHBox();
			button_hbox.Height = config.buttonHeight;
			button_hbox.Width = config.buttonWidth;
			button_hbox.Background = 1;
			button_hbox.BackgroundTexture = config.background.Path;
			button_hbox.BackgroundColor = background_color;

			var label = new WidgetLabel(text);
			label.SetFont(config.font.font.Path);
			label.FontSize = config.font.fontSize;
			label.FontColor = config.font.fontColor;
			button_hbox.AddChild(label, Gui.ALIGN_CENTER);
			button_hbox.Arrange();
		}
		public override void Update(bool up, bool down)
		{
			if (!button_hbox)
				return;

			bool hovered = IsHovered();

			if (pressed && up)
			{
				pressed = false;
				Input.MouseHandle = mouse_handle_at_click;
				if (hovered)
				{
					World.LoadWorld(world_path, true);
				}
			}

			if (!pressed && down && hovered)
			{
				pressed = true;
				// for samples with MOUSE_HANDLE_GRAB
				mouse_handle_at_click = Input.MouseHandle;
				Input.MouseHandle = Input.MOUSE_HANDLE.USER;
			}

			if (prev_hovered != hovered)
			{
				button_hbox.BackgroundColor = hovered ? background_tint_color : background_color;
				prev_hovered = hovered;
			}
			if (prev_pressed != pressed)
			{
				button_hbox.BackgroundColor = pressed ? background_selected_color : background_color;
				prev_pressed = pressed;
			}
		}

		private string world_path = "";

		private vec4 background_selected_color;
	}
}
