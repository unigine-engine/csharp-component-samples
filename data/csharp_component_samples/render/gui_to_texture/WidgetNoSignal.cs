using System.Collections;
using System.Collections.Generic;
using Unigine;

/// <summary>
/// This sample demonstrates how the <c> GuiToTexture </c> component can be used to render custom widgets
/// In this component we're going to use <c> GuiToTexture </c> with auto update enabled and will update the widget state only
/// </summary>
[Component(PropertyGuid = "acf913707bbb76a1b995c159da51b3bec3aaab94")]
public class WidgetNoSignal : Component
{
	void Init()
	{

		// get GuiToTexture component
		GuiToTexture guiToTexture = ComponentSystem.GetComponent<GuiToTexture>(node);

		// get gui from GuiToTexture component
		Gui gui = guiToTexture.Gui;

		// create a widget that you want to render in gui
		label = new WidgetLabel(gui) { FontSize = 150, Text = "No Signal", FontColor = vec4.RED };

		container = new WidgetVBox() { Background = 1, BackgroundColor = vec4.BLUE };
		container.AddChild(label, Gui.ALIGN_EXPAND | Gui.ALIGN_BACKGROUND);

		// add the widget to gui children
		gui.AddChild(container, Gui.ALIGN_OVERLAP | Gui.ALIGN_CENTER);

		// now we don't need to interact with GuiToTexture, it will be updated on its own
		// starting from here, we will just update the state of our custom widget
	}

	void Update()
	{
		float frameSpeed = labelSpeed * Game.IFps;
		vec2 delta = direction * frameSpeed;
		int posX = container.PositionX;
		int posY = container.PositionY;

		if (new ivec2(posX, posY) + new ivec2(accumulatedDelta) == new ivec2(posX, posY))
		{
			accumulatedDelta += delta;
			return;
		}

		container.PositionX = posX + (int)accumulatedDelta.x;
		container.PositionY = posY + (int)accumulatedDelta.y;

		accumulatedDelta = new vec2(0, 0);

		ReflectDirection();
	}


	private void ReflectDirection()
	{
		ivec2 size = container.ParentGui.Size;

		int labelPosX = container.PositionX;
		int labelPosY = container.PositionY;

		int xRightCornerDelta = label.GetTextRenderSize(label.Text).x;
		int yBottomCornerDelta = label.GetTextRenderSize(label.Text).y;

		var leftTopCornerPos = new ivec2(labelPosX, labelPosY);
		var rightTopCornerPos = new ivec2(labelPosX + xRightCornerDelta, labelPosY);
		var leftBottomCornerPos = new ivec2(labelPosX, labelPosY + yBottomCornerDelta);
		var rightBottomCornerPos = new ivec2(labelPosX + xRightCornerDelta,
			labelPosY + yBottomCornerDelta);

		// check the top left corner
		{
			// intersecting with top
			if (leftTopCornerPos.x > 0 && leftTopCornerPos.y < 0)
			{
				container.PositionY = 0;
				direction = ReflectVector(direction, new vec2(0, 1));
				return;
			}

			if (leftTopCornerPos.x < 0 && leftTopCornerPos.y > 0)
			{
				container.PositionX = 0;
				direction = ReflectVector(direction, new vec2(1, 0));
				return;
			}

			// intersecting with corner
			if (leftTopCornerPos.x < 0 && leftTopCornerPos.y < 0)
			{
				direction = MathLib.Normalize(new vec2(1, 1));
				container.PositionX = 0;
				container.PositionY = 0;
				return;
			}
		}

		// check the top right corner
		{
			// right corner
			if (rightTopCornerPos.x > size.x && rightTopCornerPos.y > 0)
			{
				container.PositionX = size.x - xRightCornerDelta;
				direction = ReflectVector(direction, new vec2(-1, 0));
				return;
			}

			if (rightTopCornerPos.x > size.x && rightTopCornerPos.y < 0)
			{
				container.PositionX = size.x - xRightCornerDelta;
				container.PositionY = 0;
				direction = new vec2(-1, 1);
				return;
			}
		}

		// check the bottom left corner
		{
			if (leftBottomCornerPos.x < 0 && leftBottomCornerPos.y < size.y)
			{
				container.PositionX = 0;
				direction = ReflectVector(direction, new vec2(1, 0));
				return;
			}

			if (leftBottomCornerPos.x > 0 && leftBottomCornerPos.y > size.y)
			{
				container.PositionY = size.y - yBottomCornerDelta;
				direction = ReflectVector(direction, new vec2(0, -1));
				return;
			}

			if (leftBottomCornerPos.x < 0 && leftBottomCornerPos.y > size.y)
			{
				container.PositionX = 0;
				container.PositionY = yBottomCornerDelta;
				direction = new vec2(1, -1);
				return;
			}
		}

		// bottom right corner
		{
			if (rightBottomCornerPos.x > size.x && rightBottomCornerPos.y < size.y)
			{
				container.PositionX = size.x - xRightCornerDelta;
				direction = ReflectVector(direction, new vec2(-1, 0));
				return;
			}

			if (rightBottomCornerPos.x > size.x && rightBottomCornerPos.y > size.y)
			{
				container.PositionX = size.x - xRightCornerDelta;
				container.PositionY = size.y - yBottomCornerDelta;
				direction = new vec2(-1, -1);
				return;
			}
		}
	}

	static vec2 ReflectVector(vec2 vector, vec2 normal)
	{
		return MathLib.Normalize(vector - normal * MathLib.Dot(vector, normal) * 2);
	}


	[ShowInEditor] private float labelSpeed = 1000;

	private WidgetVBox container;
	private WidgetLabel label;

	private vec2 accumulatedDelta;
	private vec2 direction = new vec2(1, 1);
}
