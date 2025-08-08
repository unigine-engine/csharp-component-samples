# Bones Rotation

This sample demonstrates how to control animation playback and directly modify bone transforms.
The *AnimationRotation.cs* component plays skeletal animations using `.anim` files and programmatically applies additional transformations to specific bones. In this example, the shooting animation is blended with the idle animation, while the turret is rotated via code by modifying the transform of a specific joint.

Animations are assigned to separate layers using the *ObjectMeshSkinned* class. Blending is performed between the idle animation and a dynamically selected shooting animation