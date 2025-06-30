using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using Unigine;
using static Unigine.Image;

[Component(PropertyGuid = "9f731f078717404b7f37b701d6863f28d2364131")]
public class ExternalPackageSample : Component
{
	private int num_files = 64;
	private ExternalPackage package;

	void Init()
	{
		package = new ExternalPackage(num_files);
		FileSystem.AddExternPackage("package", package);

		for (int i = 0; i < num_files; i += 1)
		{
			ObjectMeshStatic mesh_static = new ObjectMeshStatic(String.Format("{0}.mesh", i));

			vec3 position = random_vec3(new vec3(4.0f, 4.0f, 2.0f)) + vec3.UP * 2.0f;
			quat rotation = new quat(Game.GetRandomFloat(0.0f, 360.0f), Game.GetRandomFloat(0.0f, 360.0f), Game.GetRandomFloat(0.0f, 360.0f));

			mesh_static.WorldTransform = new mat4(rotation, position);
		}
	}

	void Shutdown()
	{
		FileSystem.RemovePackage("package");
		package.Dispose();
	}

	private vec3 random_vec3(vec3 size)
	{
		return random_vec3(-size * 0.5f, size * 0.5f);
	}

	private vec3 random_vec3(vec3 from, vec3 to)
	{
		return new vec3 {
			Game.GetRandomFloat(from.x, to.x),
			Game.GetRandomFloat(from.y, to.y),
			Game.GetRandomFloat(from.z, to.z)
		};
	}
}

class ExternalPackage : Package, IDisposable
{
	private Unigine.File file;
	private int num_files = 0;
	private bool disposed = false;

	public ExternalPackage(int num_files)
	{
		this.num_files = num_files;
		file = new Unigine.File();

		Mesh mesh = new Mesh();
		mesh.AddBoxSurface("box", vec3.ONE);

		string path = World.Path + @"/" + ".." + @"/" + ".temporary" + @"/" + "box.mesh";

		if (mesh.Save(path) > 0)
		{
			file.Open(path, "rb");
		}
	}

	// list of files
	public override int GetNumFiles()
	{
		return num_files;
	}

	public override string GetFilePath(int num)
	{
		return String.Format("{0}.mesh", num);
	}

	// select file
	public override bool SelectFile(string name, out ulong size)
	{
		bool exists = FindFile(name) == 1 ? true : false;

		size = 0;
		if (exists)
			size = file.GetSize();

		return exists;
	}

	// read file
	public override bool ReadFile(IntPtr data, ulong size)
	{
		if (!file.IsOpened)
			return false;

		file.SeekSet(0);
		ulong written = file.Read(data, size);

		return written == size;
	}

	public override int FindFile(string name)
	{
		for (int i = 0; i < num_files; i += 1)
		{
			if (String.Format("{0}.mesh", i) == name)
				return 1;
		}

		return 0;
	}

	public override ulong GetFileSize(int num)
	{
		return file.GetSize();
	}

	~ExternalPackage()
	{
		Dispose(disposing: false);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposed)
			return;

		if (disposing)
		{
			file.Dispose();
		}

		disposed = true;
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}
}

