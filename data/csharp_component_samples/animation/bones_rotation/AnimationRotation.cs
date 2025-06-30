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

using Unigine;

[Component(PropertyGuid = "5de064783893dd77a6cd9c6ae78c20c10672832c")]
public class AnimationRotation : Component
{
	[ParameterFile(Filter = ".anim")]
	public string idleAnim = "";

	[ParameterFile(Filter = ".anim")]
	public string leftShootAnim = "";

	[ParameterFile(Filter = ".anim")]
	public string rightShootAnim = "";

	private ObjectMeshSkinned meshSkinned = null;
	private const int horizontalJointBone = 1;

	private enum SIDE
	{
		LEFT = 0,
		RIGHT
	}

	private enum LAYERS
	{
		IDLE = 0,
		LEFT,
		RIGHT,
		REFERENCE_FRAME,
		COUNT
	}

	private float animationSpeed = 30.0f;
	private float currentTime = 0.0f;
	private float shootAnimationTime = 0.6f;
	private float currentShootTime = 0.0f;
	private SIDE currentSide = SIDE.LEFT;
	private int leftFrameCount = 0;
	private int rightFrameCount = 0;

	private Mat4 boneTransform = Mat4.IDENTITY;

	private void Init()
	{
		meshSkinned = node as ObjectMeshSkinned;
		if (!meshSkinned)
			return;

		// set animation on different layers
		meshSkinned.NumLayers = (int)LAYERS.COUNT;
		meshSkinned.SetLayerAnimationFilePath((int)LAYERS.IDLE, idleAnim);
		meshSkinned.SetLayerAnimationFilePath((int)LAYERS.LEFT, leftShootAnim);
		meshSkinned.SetLayerAnimationFilePath((int)LAYERS.RIGHT, rightShootAnim);

		// set inverse transform for blend calculation on auxiliary layer
		meshSkinned.SetLayerFrame((int)LAYERS.IDLE, 0);
		meshSkinned.InverseLayer((int)LAYERS.REFERENCE_FRAME, (int)LAYERS.IDLE);

		// get number of frames in animations
		leftFrameCount = meshSkinned.GetLayerNumFrames((int)LAYERS.LEFT);
		rightFrameCount = meshSkinned.GetLayerNumFrames((int)LAYERS.RIGHT);

		boneTransform = meshSkinned.GetBoneWorldTransform(horizontalJointBone);
	}

	private void Update()
	{
		if (!meshSkinned)
			return;

		meshSkinned.SetLayerFrame((int)LAYERS.IDLE, currentTime * animationSpeed);
		currentTime += Game.IFps;

		// update side of shoot
		currentShootTime += Game.IFps;
		if (currentShootTime > shootAnimationTime)
		{
			currentShootTime = 0.0f;
			currentSide = (currentSide == SIDE.LEFT ? SIDE.RIGHT : SIDE.LEFT);
		}

		float k = MathLib.Saturate(currentShootTime / shootAnimationTime);
		switch (currentSide)
		{
			// 1. set the layer to the corresponding frame of animation
			// 2. find local transformations of all bones relative to reference frame
			// 3. add local transform﻿ations of shoot animation bones to the idle animation

			case SIDE.LEFT:
				meshSkinned.SetLayerFrame((int)LAYERS.LEFT, leftFrameCount * k);
				meshSkinned.MulLayer((int)LAYERS.LEFT, (int)LAYERS.REFERENCE_FRAME, (int)LAYERS.LEFT);
				meshSkinned.MulLayer((int)LAYERS.IDLE, (int)LAYERS.IDLE, (int)LAYERS.LEFT);
				break;

			case SIDE.RIGHT:
				meshSkinned.SetLayerFrame((int)LAYERS.RIGHT, rightFrameCount * k);
				meshSkinned.MulLayer((int)LAYERS.RIGHT, (int)LAYERS.REFERENCE_FRAME, (int)LAYERS.RIGHT);
				meshSkinned.MulLayer((int)LAYERS.IDLE, (int)LAYERS.IDLE, (int)LAYERS.RIGHT);
				break;
		}

		// set the bone transform for the horizontal joint
		boneTransform =  boneTransform * new Mat4(MathLib.RotateZ(45.0f * Game.IFps));
		meshSkinned.SetBoneWorldTransformWithChildren(horizontalJointBone, boneTransform);
	}
}
