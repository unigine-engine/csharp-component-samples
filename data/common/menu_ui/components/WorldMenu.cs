using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unigine;

[Component(PropertyGuid = "fccd3384dd4529130a2a9b56e5e31abd02139cf8")]
public class WorldMenu : Component
{
	public class UICommon
	{
		[ShowInEditor] public ivec4 padding;
		[ShowInEditor] public int bottom_offset;
		[ShowInEditor] public int right_offset;
	};

	public class UIHintPanel
	{
		[ShowInEditor] public int spaceX;
		[ShowInEditor] public AssetLink background;
		[ParameterColor]
		[ShowInEditor] public vec4 backgroundColor;
		[ShowInEditor] public vec4 backgroundSliceOffsets;
		[ShowInEditor] public float backgroundSliceScale;
		[ShowInEditor] public ivec4 backgroundPadding;

		[ShowInEditor] public AssetLink labelFont;
		[ShowInEditor] public int labelFontSize;
		[ParameterColor]
		[ShowInEditor] public vec4 labelFontColor;

		[ShowInEditor] public AssetLink icon;
		[ParameterColor]
		[ShowInEditor] public vec4 iconColor;
		[ShowInEditor] public int iconWidth;
		[ShowInEditor] public int iconHeight;
	};

	public class UIBackPanel
	{
		[ShowInEditor] public AssetLink background;
		[ParameterColor]
		[ShowInEditor] public vec4 backgroundColor;
		[ShowInEditor] public vec4 backgroundSliceOffsets;
		[ShowInEditor] public float backgroundSliceScale;
		[ShowInEditor] public ivec4 backgroundPadding;

		[ShowInEditor] public AssetLink buttonBackground;
		[ParameterColor]
		[ShowInEditor] public vec4 buttonBackgroundColor;

		[ShowInEditor] public AssetLink buttonIcon;
		[ParameterColor]
		[ShowInEditor] public vec4 buttonIconColor;
		[ShowInEditor] public int buttonWidth;
		[ShowInEditor] public int buttonHeight;
		[ParameterColor]
		[ShowInEditor] public vec4 buttonTintColor;

		[ShowInEditor] public AssetLink labelFont;
		[ShowInEditor] public int labelFontSize;
		[ParameterColor]
		[ShowInEditor] public vec4 labelFontColor;

		[ShowInEditor] public int spaceBetweenButtonAndLabel;
	};

	[ShowInEditor] public string mainWorldName;
	[ShowInEditor] private UICommon common = null;
	[ShowInEditor] private UIHintPanel hint = null;
	[ShowInEditor] private UIBackPanel back = null;


	private WidgetHBox mainHBox = null;
	
	private WidgetHBox hintHBox = null;
	private WidgetLabel hintLabel = null;

	private WidgetHBox backHBox = null;
	private WidgetLabel backLabel = null;
	private WidgetSprite backButtonSprite = null;

	private bool isPressed = false;
	Input.MOUSE_HANDLE mouseHandleAtClick = Input.MOUSE_HANDLE.USER;


	private void Init()
	{
		mainHBox = new WidgetHBox();
		ivec4 padding = common.padding;
		mainHBox.SetPadding(padding.x, padding.y, padding.z, padding.w);

		int space = hint.spaceX;
		hintHBox = new WidgetHBox(space, 0);
		hintHBox.Background = 1;
		hintHBox.BackgroundTexture = hint.background.Path;
		hintHBox.BackgroundColor = hint.backgroundColor;
		hintHBox.Background9Sliced = true;
		vec4 hint_offsets = hint.backgroundSliceOffsets;
		hintHBox.SetBackground9SliceOffsets(hint_offsets.x, hint_offsets.y, hint_offsets.z, hint_offsets.w);
		hintHBox.Background9SliceScale = hint.backgroundSliceScale;
		ivec4 hint_padding = hint.backgroundPadding;
		hintHBox.SetPadding(hint_padding.x - space, hint_padding.y - space, hint_padding.z, hint_padding.w);

		var hintSprite = new WidgetSprite();
		hintSprite.AddLayer();
		hintSprite.SetLayerTexture(0, hint.icon.Path);
		hintSprite.SetLayerColor(0, hint.iconColor);
		hintSprite.Width = hint.iconWidth;
		hintSprite.Height = hint.iconHeight;
		hintHBox.AddChild(hintSprite, Gui.ALIGN_LEFT);

		hintLabel = new WidgetLabel("Press <b>ESC</b> to interact with UI");
		hintLabel.FontRich = 1;
		hintLabel.SetFont(hint.labelFont.Path);
		hintLabel.FontSize = hint.labelFontSize;
		hintLabel.FontColor = hint.labelFontColor;
		hintHBox.AddChild(hintLabel);

		backHBox = new WidgetHBox(back.spaceBetweenButtonAndLabel, 0);
		backHBox.Background = 1;
		backHBox.BackgroundTexture = back.background.Path;
		backHBox.BackgroundColor = back.backgroundColor;
		backHBox.Background9Sliced = false;
		vec4 back_offsets = back.backgroundSliceOffsets;
		backHBox.SetBackground9SliceOffsets(back_offsets.x, back_offsets.y, back_offsets.z, back_offsets.w);
		backHBox.Background9SliceScale = back.backgroundSliceScale;
		ivec4 back_padding = back.backgroundPadding;
		backHBox.SetPadding(back_padding.x - back.spaceBetweenButtonAndLabel, back_padding.y - back.spaceBetweenButtonAndLabel, back_padding.z, back_padding.w);

		backButtonSprite = new WidgetSprite();
		backButtonSprite.AddLayer();
		backButtonSprite.SetLayerTexture(0, back.buttonIcon.Path);
		backButtonSprite.SetLayerColor(0, back.buttonIconColor);
		backButtonSprite.Width = back.buttonWidth;
		backButtonSprite.Height = back.buttonHeight;
		backHBox.AddChild(backButtonSprite, Gui.ALIGN_LEFT);

		backLabel = new WidgetLabel("<p align=center>Back to Main Menu</p>");
		backLabel.FontRich = 1;
		backLabel.SetFont(back.labelFont.Path);
		backLabel.FontSize = back.labelFontSize;
		backLabel.FontColor = back.labelFontColor;
		backHBox.AddChild(backLabel, Gui.ALIGN_RIGHT);

		mainHBox.AddChild(hintHBox, Gui.ALIGN_BOTTOM | Gui.ALIGN_CENTER);
		mainHBox.AddChild(backHBox, Gui.ALIGN_BOTTOM | Gui.ALIGN_RIGHT);

		WindowManager.MainWindow.AddChild(mainHBox, Gui.ALIGN_OVERLAP);
	}

	private void Update()
	{
		ivec2 size = WindowManager.MainWindow.ClientSize;

		hintHBox.Hidden = !Input.MouseGrab;
		hintHBox.Enabled = Input.MouseGrab;

		mainHBox.Width = size.x;
		mainHBox.Arrange();

		mainHBox.PositionX = size.x - common.right_offset - mainHBox.Width;
		mainHBox.PositionY = size.y - common.bottom_offset - mainHBox.Height;

		bool down = Input.IsMouseButtonDown(Input.MOUSE_BUTTON.LEFT);
		bool up = Input.IsMouseButtonUp(Input.MOUSE_BUTTON.LEFT);

		bool hovered = backHBox.MouseX >= 0 && backHBox.MouseX < backHBox.Width
			&& backHBox.MouseY >= 0 && backHBox.MouseY < backHBox.Height;

		if (isPressed && up)
		{
			isPressed = false;
			Input.MouseHandle = mouseHandleAtClick;
			if (hovered)
			{
				World.LoadWorld(mainWorldName, true);
			}
		}

		if (!isPressed && down && hovered)
		{
			isPressed = true;
			// for samples with MOUSE_HANDLE_GRAB
			mouseHandleAtClick = Input.MouseHandle;
			Input.MouseHandle = Input.MOUSE_HANDLE.USER;
		}

		backHBox.Color = isPressed ? back.buttonTintColor : back.backgroundColor;
	}

	private void Shutdown()
	{
		mainHBox.DeleteLater();
	}
}
