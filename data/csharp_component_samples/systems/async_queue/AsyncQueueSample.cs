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

using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "17b88eee316007eac924ae078e5a90550c3746be")]
public class AsyncQueueSample : Component
{
	[ShowInEditor][ParameterFile]
	private string[] meshes = null;
	[ShowInEditor][ParameterFile]
	private string[] textures = null;

	struct AsyncLoadRequest
	{
		public string name;
		public int id;
	}

	private List<AsyncLoadRequest> meshLoadRequest = new List<AsyncLoadRequest>();

	private int objectsPlaced = 0;

	private List<WidgetSprite> sprites = new List<WidgetSprite>();

	private EventConnection imageLoadedConnection;

	void Init()
	{
		for(int i =0; i < meshes.Length; i++)
		{
			string name = meshes[i];
			AsyncLoadRequest request = new AsyncLoadRequest();
			request.name = name;
			request.id = AsyncQueue.LoadMesh(name);
			meshLoadRequest.Add(request);
		}
		
		for(int i =0; i < textures.Length; i++)
		{
			AsyncQueue.LoadImage(textures[i]);
		}

		imageLoadedConnection = AsyncQueue.EventImageLoaded.Connect(ImageLoadedCallback);

		Console.Onscreen = true;
	}
	
	void Update()
	{
		for(int i =0; i < meshLoadRequest.Count; i++)
		{
			AsyncLoadRequest request = meshLoadRequest[i];
			if (AsyncQueue.CheckMesh(request.id) == 0)
				continue;

			Mesh mesh = AsyncQueue.TakeMesh(request.id);
			if(mesh != null)
			{
				ObjectMeshDynamic objectMeshDynamic = new ObjectMeshDynamic(mesh);
				Scalar initialPos = -5;
				Scalar step = 5;

				objectMeshDynamic.Position = new Vec3(initialPos + (float)objectsPlaced * step, 0.0f, 0.0f);
				objectsPlaced++;

				AsyncQueue.RemoveMesh(request.id);
				Log.MessageLine($"Loaded mesh {request.name}");

				meshLoadRequest.RemoveAt(i--);
			}
		}
		
	}

	void Shutdown()
	{
		foreach(WidgetSprite sprite in sprites)
		{
			sprite.DeleteLater();
		}

		Console.Onscreen = false;

		imageLoadedConnection.Disconnect();
	}

	private void ImageLoadedCallback(string name, int id)
	{
		Image loadedImage = AsyncQueue.TakeImage(id);
		if (loadedImage == null)
			return;

		AsyncQueue.RemoveImage(id);
		Log.MessageLine($"Image {name} loaded");

		var sprite = new WidgetSprite();
		sprites.Add(sprite);
		sprite.SetImage(loadedImage);
		sprite.Width = 100;
		sprite.Height = 100;
		WindowManager.MainWindow.AddChild(sprite, Gui.ALIGN_OVERLAP | Gui.ALIGN_BACKGROUND);

		ivec2 initialSpritePosition = new ivec2(0, WindowManager.MainWindow.Size.y - 200);

		ivec2 newPos = new ivec2(initialSpritePosition.x + sprites.Count * 100, initialSpritePosition.y);
		sprite.SetPosition(newPos.x, newPos.y);
	}
}
