using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "2041c058f0c844eadd8476060da062b46f82f438")]
public class ImagesSample : Component
{
	private float size;
	private float velocity;
	private float radius;

	private int num_fields;
	private vec3[] positions;
	private vec3[] velocities;
	private float[] radiuses;

	private Image image;
	Material material;

	void Init()
	{
		image = new Image();
		image.Create3D(32, 32, 32, Image.FORMAT_RGBA8);

		image_init();

		ObjectVolumeBox obj = new ObjectVolumeBox(new vec3(20.0f));
		obj.SetMaterial(Materials.FindManualMaterial("Unigine::volume_cloud_base"), "*");
		obj.SetMaterialState("samples", 2, 0);
		obj.Transform = MathLib.Translate(new vec3(0.0f, 0.0f, 1.0f));
		material = obj.GetMaterialInherit(0);
	}
	
	void Update()
	{
		image_update();
		material.SetTextureImage(material.FindTexture("density_3d"), image);
	}

	private void image_init()
	{
		size = 2.0f;
		velocity = 1.0f;
		radius = 0.5f;

		num_fields = 16;
		positions = new vec3[num_fields];
		velocities = new vec3[num_fields];
		radiuses = new float[num_fields];

		for (int i = 0; i < num_fields; i++)
		{
			positions[i] = MathLib.RandVec3(0.0f, size);
			velocities[i] = MathLib.RandVec3(-velocity, velocity);
			radiuses[i] = MathLib.RandFloat(radius / 2.0f, radius);
		}
	}

	private void image_update()
	{
		float ifps = Game.IFps;

		for (int i = 0; i < num_fields; i++)
		{
			vec3 p = positions[i] + velocities[i] * ifps;
			if (p.x < 0.0f || p.x > size) velocities[i].x = -velocities[i].x;
			if (p.y < 0.0f || p.y > size) velocities[i].y = -velocities[i].y;
			if (p.z < 0.0f || p.z > size) velocities[i].z = -velocities[i].z;
			positions[i] += velocities[i] * ifps;
		}

		int width = image.Width;
		int height = image.Height;
		int depth = image.Depth;

		float iwidth = size / width;
		float iheight = size / height;
		float idepth = size / depth;
		vec3 position = new vec3(0.0f);


		Image.Pixel pixel = new Image.Pixel();

		for (int z = 0; z < depth; z++)
		{
			position.z = z * idepth;
			for (int y = 0; y < height; y++)
			{
				position.y = y * iheight;
				for (int x = 0; x < width; x++)
				{
					position.x = x * iwidth;
					float field = 0.0f;
					for (int i = 0; i < num_fields; i++)
					{
						float distance = MathLib.Distance2(positions[i], position);
						if (distance < radiuses[i])
							field += radiuses[i] - distance;

						pixel.i.x = pixel.i.y = pixel.i.z = pixel.i.w = (int)(MathLib.Saturate(field) * 255.0f);
						image.Set3D(x, y, z, pixel);
					}
				}
			}
		}
	}
}
