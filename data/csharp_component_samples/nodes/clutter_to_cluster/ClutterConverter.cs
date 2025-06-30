using System.Collections.Generic;
using Unigine;

#region Math Variables
#if UNIGINE_DOUBLE
using Mat4 = Unigine.dmat4;
#else
using Mat4 = Unigine.mat4;
#endif
#endregion

[Component(PropertyGuid = "5f874774f719cc4302ac04c66068156db1e677e8")]
public class ClutterConverter : Component
{
	[Parameter(Title = "Clutter Node")]
	public ObjectMeshClutter clutter;
	public ObjectMeshStatic clusterParent;

	private bool is_convertred = false;
	private ObjectMeshCluster cluster;
	
	public void ConvertToCluster()
	{
		RemoveCluster();

		cluster = ConvertMesh(clutter);
		if (cluster)
		{
			is_convertred = true;
			// move cluster to another parent
			cluster.Parent = clusterParent;
		}
	}

	public void generateClutter()
	{
		clutter.Seed = Unigine.Random.Get().Int();
	}
	
	private void RemoveCluster()
	{
		if (!is_convertred)
			return;

		cluster.DeleteLater();
		is_convertred = false;
	}

	private ObjectMeshCluster ConvertMesh(ObjectMeshClutter clutter)
	{
		ObjectMeshCluster cluster = new ObjectMeshCluster(clutter.MeshPath);
		cluster.Name = clutter.Name + "_Cluster";
		// copy the hierarchy
		cluster.Parent = clutter.Parent;
		// copy node transformation
		cluster.WorldTransform = clutter.WorldTransform;

		// copy necessary parameters for surfaces
		cluster.VisibleDistance = clutter.VisibleDistance;
		cluster.FadeDistance = clutter.FadeDistance;

		int suf_num = clutter.NumSurfaces;
		for (int suf_index = 0; suf_index < suf_num; suf_index++)
		{
			cluster.SetEnabled(cluster.IsEnabled(suf_index), suf_index);
			cluster.SetViewportMask(cluster.GetViewportMask(suf_index), suf_index);
			cluster.SetShadowMask(clutter.GetShadowMask(suf_index), suf_index);
			cluster.SetCastShadow(clutter.GetCastShadow(suf_index), suf_index);
			cluster.SetCastWorldShadow(clutter.GetCastWorldShadow(suf_index), suf_index);
			cluster.SetBakeToEnvProbe(clutter.GetBakeToEnvProbe(suf_index), suf_index);
			cluster.SetBakeToGI(clutter.GetBakeToGI(suf_index), suf_index);
			cluster.SetCastEnvProbeShadow(clutter.GetCastEnvProbeShadow(suf_index), suf_index);
			cluster.SetShadowMode(clutter.GetShadowMode(suf_index), suf_index);
			cluster.SetMinVisibleDistance(clutter.GetMinVisibleDistance(suf_index), suf_index);
			cluster.SetMaxVisibleDistance(clutter.GetMaxVisibleDistance(suf_index), suf_index);
			cluster.SetMinFadeDistance(clutter.GetMinFadeDistance(suf_index), suf_index);
			cluster.SetMaxFadeDistance(clutter.GetMaxFadeDistance(suf_index), suf_index);
			cluster.SetMinParent(clutter.GetMinParent(suf_index), suf_index);
			cluster.SetMaxParent(clutter.GetMaxParent(suf_index), suf_index);
			cluster.SetIntersection(clutter.GetIntersection(suf_index), suf_index);
			cluster.SetIntersectionMask(clutter.GetIntersectionMask(suf_index), suf_index);
			cluster.SetCollision(clutter.GetCollision(suf_index), suf_index);
			cluster.SetCollisionMask(clutter.GetCollisionMask(suf_index), suf_index);
			cluster.SetPhysicsIntersection(clutter.GetPhysicsIntersection(suf_index), suf_index);
			cluster.SetPhysicsIntersectionMask(clutter.GetPhysicsIntersectionMask(suf_index),
			suf_index);
			cluster.SetSoundOcclusion(clutter.GetSoundOcclusion(suf_index), suf_index);
			cluster.SetSoundOcclusionMask(clutter.GetSoundOcclusionMask(suf_index), suf_index);
			cluster.SetPhysicsFriction(clutter.GetPhysicsFriction(suf_index), suf_index);
			cluster.SetPhysicsRestitution(clutter.GetPhysicsRestitution(suf_index), suf_index);
			cluster.SetMaterial(clutter.GetMaterial(suf_index), suf_index);
			cluster.SetSurfaceProperty(clutter.GetSurfaceProperty(suf_index), suf_index);
		}

		// copy transforms from the clutter to the cluster
		List<Mat4> transforms = new List<Mat4>();
		clutter.CreateClutterTransforms();
		if(!clutter.GetClutterWorldTransforms(transforms))
		{
			Log.Warning("ClutterConverter.ConvertMesh(): empty set of transforms\n");
			return cluster;
		}
		cluster.CreateMeshes(transforms.ToArray());
		return cluster;
	}

	
}
