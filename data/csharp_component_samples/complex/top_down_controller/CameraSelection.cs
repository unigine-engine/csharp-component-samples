using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unigine;
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

[Component(PropertyGuid = "ca0b0bfe1193c345127a5be0e620ddbfac1b5012")]
public class CameraSelection : Component
{
	private Vec3 selectedObjectsBoundSpherePosition;
	public Vec3 Center
	{
		get 
		{
			UpdateBoundSphere();
			return selectedObjectsBoundSpherePosition; 
		}
	}

	private Scalar selectedObjectsBoundSphereRadius;
	public Scalar BoundRadius
	{
		get 
		{
			UpdateBoundSphere();
			return selectedObjectsBoundSphereRadius; 
		}
	}

	public bool Selection
	{
		get { return selectedObjects.Count != 0; }
	}

	private bool isSelection;

	private ivec2 selectionStartMousePosition;
	private vec2 upperLeftSelectionCorner;
	private vec2 bottomRightSelectionCorner;

	private WorldBoundFrustum frustum;
	private List<Unigine.Object> selectedObjects = new List<Unigine.Object>(0);

	private void UpdateBoundSphere()
	{
		WorldBoundSphere bs = new WorldBoundSphere();
		for(int i =0; i < selectedObjects.Count; i++)
		{
			bs.Expand(selectedObjects[i].WorldBoundSphere);
		}
		selectedObjectsBoundSpherePosition = bs.Center;
		selectedObjectsBoundSphereRadius = bs.Radius * 4.0f;
	}

	private void Update()
	{
		if(!Unigine.Console.Active)
		{
			Visualizer.Enabled = true;

			if(Input.IsMouseButtonDown(Input.MOUSE_BUTTON.LEFT))
			{
				isSelection = true;
				selectionStartMousePosition = Input.MousePosition - WindowManager.MainWindow.ClientPosition;
			}

			if(isSelection)
			{
				ivec2 windowSize = WindowManager.MainWindow.ClientRenderSize;
				upperLeftSelectionCorner.x = selectionStartMousePosition.x * 1.0f / windowSize.x;
				upperLeftSelectionCorner.y = 1.0f - selectionStartMousePosition.y * 1.0f / windowSize.y;
				ivec2 currentMousePosition = Input.MousePosition - WindowManager.MainWindow.ClientPosition;
				bottomRightSelectionCorner.x = currentMousePosition.x * 1.0f / windowSize.x;
				bottomRightSelectionCorner.y = 1.0f - currentMousePosition.y * 1.0f / windowSize.y;

				Visualizer.RenderRectangle(new vec4(upperLeftSelectionCorner.x , upperLeftSelectionCorner.y, bottomRightSelectionCorner.x, bottomRightSelectionCorner.y), vec4.GREEN);
			}

			if(Input.IsMouseButtonUp(Input.MOUSE_BUTTON.LEFT))
			{
				Player camera = Game.Player;

				foreach(var it in selectedObjects)
				{
					var cameraUnitSelectionComponent = GetComponent<CameraUnitSelection>(it);
					cameraUnitSelectionComponent.Selected = false;
				}

				isSelection = false;
				ivec2 selectionFinishPosition = Input.MousePosition - WindowManager.MainWindow.ClientPosition;
				float width = MathLib.Abs(selectionStartMousePosition.x - selectionFinishPosition.x);
				float height = MathLib.Abs(selectionStartMousePosition.y - selectionFinishPosition.y);

				mat4 perspective = camera.GetProjectionFromMainWindow(selectionStartMousePosition.x, selectionStartMousePosition.y, selectionFinishPosition.x, selectionFinishPosition.y);

				frustum.Set(perspective, MathLib.Inverse(camera.WorldTransform));

				World.GetIntersection(frustum, selectedObjects);
				if(selectedObjects.Count == 0)
				{
					ivec2 mouse = Input.MousePosition;
					vec3 dir = camera.GetDirectionFromMainWindow(mouse.x, mouse.y);
					Unigine.Object obj = World.GetIntersection(camera.WorldPosition, camera.WorldPosition + dir * 10000, ~0);
					if(obj != null)
					{
						selectedObjects.Add(obj);
					}
				}

				for (int i =0; i < selectedObjects.Count; i++)
				{
					var cameraUnitSelectionComponent = GetComponent<CameraUnitSelection>(selectedObjects[i]);
					if (cameraUnitSelectionComponent)
					{
						cameraUnitSelectionComponent.Selected = true;
					}
					else
					{
						selectedObjects.Remove(selectedObjects[i]);
						i--;
					}
				}
				UpdateBoundSphere();
			}
		}
	}
}
