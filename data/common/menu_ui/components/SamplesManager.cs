using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection.Emit;
using Unigine;



public class SamplesManager
{
	private static SamplesManager instance = new SamplesManager();
	private SamplesManager() { }

	public static SamplesManager Instance => instance;

	private List<Category> categories = new();
	private List<string> tags = new();
	// <ID (world_name), Sample>
	private Dictionary<string, Sample> samples_map = new();

	private string meta_path = "";

	public static void Initialize()
	{
		Log.Message("InterpreterRegistrator.Initialize()\n");
		//instance.init();
	}


	public bool parseMetaXml(string path_relative_to_data) 
	{
		if (!isEmpty() && path_relative_to_data == meta_path)
			return true;

		string cpp_samples_xml_path = FileSystem.GetAbsolutePath(Engine.DataPath + @"\" + path_relative_to_data);

		Xml cpp_samples_xml = new Xml();
		if (!cpp_samples_xml.Load(cpp_samples_xml_path))
		{
			Unigine.Log.Warning($"MainMenu.parse_meta_xml(): cannot open {cpp_samples_xml_path} file\n");
			return false;
		}

		clear();
		meta_path = path_relative_to_data;

		Xml cpp_samples_samples_pack = cpp_samples_xml.GetChild("samples_pack");
		Xml categories_xml = cpp_samples_samples_pack.GetChild("categories");
		Xml samples_xml = cpp_samples_samples_pack.GetChild("samples");

		Dictionary<string, Category> categories_map = new Dictionary<string, Category>();
		List<string> categories_id = new();
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
			c.id = category_xml.GetArg("id");

			categories_id.Add(c.id);
			categories_map[c.id] = c;
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

			s.category_id = sample_xml.GetArg("category_id");

			if (categories_map.ContainsKey(s.category_id))
				categories_map[s.category_id].samples.Add(s);
			else
				Unigine.Log.Error($"Category with id {s.category_id} don't exists in .sample file\n");

			samples_map[s.world_name] = s;
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


		return false; }
	public bool isEmpty()
	{
		if (meta_path.Length <= 0 || categories.Count <= 0)
			return true;
		else
			return false;
	}

	public void clear() {
		meta_path = "";
		categories.Clear();
		samples_map.Clear();
		tags.Clear();
	}

	public List<Category> getCategories() { return categories; }
	public List<string> getTags() { return tags; }

	public Category getCategoryBySampleID(string sample_id) {
		if (isEmpty() || !samples_map.ContainsKey(sample_id))
			return null;

		Sample sample = samples_map[sample_id];

		foreach (var category in categories)
		{
			if (category.id == sample.category_id)
				return category;
		}

		return null;
	}

	public Sample getSampleByWorldPath(string world_path)
	{
		string sample_id = GetFileName(world_path);
		return getSampleByID(sample_id);
	}

	public Sample getSampleByID(string id) {
		if (isEmpty() || !samples_map.ContainsKey(id))
			return null;

		return samples_map[id];
	}

	public void getPrevNextSamplesID(string world_name, ref string prev_world, ref string next_world)
	{
		var category = getCategoryBySampleID(world_name);
		if (category == null)
		{
			prev_world = world_name;
			next_world = world_name;
			return;
		}

		var samples = category.samples;
		for (int i = 0; i < samples.Count; i++)
		{
			string id = samples[i].world_name;
			if (id != world_name)
				continue;

			prev_world = i == 0 ? samples.Last().world_name : samples[i - 1].world_name;
			next_world = i == samples.Count - 1 ? samples.First().world_name
												 : samples[i + 1].world_name;
			return;
		}
	}

	public string GetFileName(in string path)
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

public class Sample
{
	public string title;
	public string description;
	public List<string> tags = new List<string>();
	public string world_name;
	public string category_id;
};

public class Category
{
	public string id;
	public Unigine.Image icon;
	public string title;

	public List<Sample> samples = new List<Sample>();
};
