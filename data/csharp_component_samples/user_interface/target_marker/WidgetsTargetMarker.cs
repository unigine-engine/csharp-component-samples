using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "af5207a29175e299b8a56b24fc09bc334ae4d3f6")]
public class WidgetsTargetMarker : Component
{
	[ShowInEditor] public AssetLink arrowSprite;
	[ShowInEditor] public AssetLink pointSprite;

	[ShowInEditor] public Node target;

	[ShowInEditor] private vec2 pointSpritePivot = new vec2(0.5f, 0.5f);

	private WidgetSprite arrow;
	private WidgetSprite point;

	private Player camera;

	private int pointWidth;
	private int pointHeight;
	private int arrowWidth;
	private int arrowHalfWidth;
	private int arrowHeight;
	private int arrowHalfHeight;

	private void Init()
	{
		if (!arrowSprite.IsFileExist)
		{
			Log.ErrorLine("WidgetsTargetMarker.Init(): Source file for the pointer sprite image is not found.");
			return;
		}
		arrow = new WidgetSprite(arrowSprite.Path);
		WindowManager.MainWindow.AddChild(arrow, Gui.ALIGN_OVERLAP);

		if (!pointSprite.IsFileExist)
		{
			Log.ErrorLine("WidgetsTargetMarker.Init(): Source file for the marker sprite image is not found.");
			return;
		}
		point = new WidgetSprite(pointSprite.Path);
		WindowManager.MainWindow.AddChild(point, Gui.ALIGN_OVERLAP);

		if (!target)
		{
			Log.ErrorLine("WidgetsTargetMarker.Init(): No target object specified.");
			return;
		}

		camera = node as Player;
		if (!camera)
		{
			Log.ErrorLine("WidgetsTargetMarker.Init(): Camera is not valid.");
			return;
		}
	}

	private void Update()
	{
		if (!arrow || !point || !camera || !target)
			return;

		arrowWidth = arrow.GetLayerWidth(0);
		arrowHalfWidth = arrowWidth / 2;
		arrowHeight = arrow.GetLayerHeight(0);
		arrowHalfHeight = arrowHeight / 2;

		pointWidth = point.GetLayerWidth(0);
		pointHeight = point.GetLayerHeight(0);
		mat4 translation = MathLib.Translate(new vec3(-pointWidth * pointSpritePivot.x, -pointHeight * pointSpritePivot.y, 0.0f) * WindowManager.MainWindow.DpiScale);
		point.Transform = translation;

		arrow.Hidden = true;
		point.Hidden = true;

		int width = WindowManager.MainWindow.ClientSize.x;
		int height = WindowManager.MainWindow.ClientSize.y;
		int halfWidth = width / 2;
		int halfHeight = height / 2;

		int x = 0;
		int y = 0;

		vec3 targetDirection = new vec3(target.WorldBoundBox.Center - camera.WorldPosition);

		bool behind = MathLib.Dot(camera.GetWorldDirection(), targetDirection) < 0;

		if(!behind)
		{
			camera.GetScreenPosition(out x, out y, target.WorldBoundBox.Center, width, height);
			x -= halfWidth;
			y -= halfHeight;
			y *= -1;
		}
		else
		{
			vec3 inverseScreenPlaneNormal = new vec3(camera.ViewDirection * -1);
			vec3 relativeToCameraTargetPosition = (vec3)(target.WorldBoundBox.Center - camera.WorldPosition);

			// ortho projection of vector relativeToCameraTargetPosition to vector inverseScreenPlaneNormal
			vec3 orthoProjectionTarget = inverseScreenPlaneNormal * MathLib.Dot(relativeToCameraTargetPosition, inverseScreenPlaneNormal);
			vec3 reflectedTargetPosition = (vec3)((relativeToCameraTargetPosition - orthoProjectionTarget * 2) + camera.WorldPosition);
			camera.GetScreenPosition(out x, out y, reflectedTargetPosition, width, height);
			x -= halfWidth;
			y -= halfHeight;
			if (y > 0)
				y *= -1;
		}

		if (!behind && x >= -halfWidth && x <= halfWidth && y >= -halfHeight && y <= halfHeight)
		{
			point.Hidden = false;
			point.SetPosition(x + halfWidth, -y + halfHeight);
		}
		else
		{
			int point_x = 0;
			int point_y = 0;
			GetIntersectionWithScreenRect(out point_x, out point_y, x, y, halfWidth, halfHeight);
			float angle = 0.0f;

			if (halfHeight - MathLib.Abs(point_y) <= arrowHalfHeight && halfWidth - MathLib.Abs(point_x) <= arrowHalfWidth)
			{
				float dx, dy;

				if (point_y > 0)
					dy = point_y - (halfHeight - arrowHalfWidth);
				else
					dy = point_y + (halfHeight - arrowHalfWidth);

				if (point_x > 0)
					dx = point_x - (halfWidth - arrowHalfWidth);
				else
					dx = point_x + (halfWidth - arrowHalfWidth);

				angle = -MathLib.Atan2(dy, dx) * MathLib.RAD2DEG;

				if (point_x > 0)
					point_x = halfWidth - arrowWidth;
				else
					point_x = -halfWidth;

				if (point_y > 0)
					point_y = halfHeight;
				else
					point_y = -halfHeight + arrowHeight;
			}
			else if (point_y == halfHeight)
			{
				point_x -= arrowHalfWidth;
				angle = -90;
			}
			else if (point_y == -halfHeight)
			{
				point_y += arrowHeight;
				point_x -= arrowHalfWidth;
				angle = 90;
			}
			else if (point_x == -halfWidth)
			{
				point_y += arrowHalfHeight;
				angle = 180;
			}
			else if (point_x == halfWidth)
			{
				point_x -= arrowWidth;
				point_y += arrowHalfHeight;
				angle = 0;
			}

			arrow.Hidden = false;
			arrow.SetPosition(point_x + halfWidth, -point_y + halfHeight);

			mat4 rotation = new mat4(
				MathLib.Translate(new vec3(arrowHalfWidth, arrowHalfHeight, 0.0f) * WindowManager.MainWindow.DpiScale) *
				MathLib.Rotate(new quat(vec3.UP, angle)) *
				MathLib.Translate(new vec3(-arrowHalfWidth, -arrowHalfHeight, 0.0f) * WindowManager.MainWindow.DpiScale)
			);

			arrow.Transform = rotation;
		}
	}

	private void Shutdown()
	{
		arrow.DeleteLater();
		point.DeleteLater();
	}

	private void GetIntersectionWithScreenRect(out int x, out int y, int vec_x, int vec_y, int halfWidth, int halfHeight)
	{
		if (vec_y >= 0)
		{
			if (vec_y == 0)
			{
				if (vec_x > 0)
				{
					x = halfWidth;
					y = 0;
				}
				else
				{
					x = -halfWidth;
					y = 0;
				}

				return;
			}

			x = (int)(halfHeight * ((float)vec_x / (float)vec_y));
			y = halfHeight;

			if (x >= -halfWidth && x <= halfWidth)
				return;

			if (vec_x >= 0)
			{
				if (vec_x == 0)
				{
					x = 0;
					y = halfHeight;

					return;
				}

				x = halfWidth;
				y = (int)(halfWidth * ((float)vec_y / (float)vec_x));

				return;
			}
			else
			{
				x = -halfWidth;
				y = (int)(-halfWidth * ((float)vec_y / (float)vec_x));

				return;
			}
		}
		else
		{
			x = (int)(-halfHeight * ((float)vec_x / (float)vec_y));
			y = -halfHeight;

			if (x >= -halfWidth && x <= halfWidth)
				return;

			if (vec_x >= 0)
			{
				if (vec_x == 0)
				{
					x = 0;
					y = -halfHeight;

					return;
				}

				x = halfWidth;
				y = (int)(halfWidth * ((float)vec_y / (float)vec_x));

				return;
			}
			else
			{
				x = -halfWidth;
				y = (int)(-halfWidth * ((float)vec_y / (float)vec_x));

				return;
			}
		}
	}
}
