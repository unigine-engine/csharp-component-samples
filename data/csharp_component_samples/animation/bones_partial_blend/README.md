# Bones Partial Blend

This sample demonstrates partial blending between two animations using bone-specific interpolation.
The *AnimationPartialInterpolation.cs* component plays two skeletal animations simultaneously on a skinned mesh and blends them only for selected bones. The blend weight can be adjusted at runtime using the keyboard.

Use the *interpolatedBones* array in the *Parameters* window to define which bones are affected by blending. The *ObjectMeshSkinned* class is used to apply animation layers and interpolate transforms.