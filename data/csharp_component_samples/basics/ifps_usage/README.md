# IFps Usage

This sample demonstrates the importance of using *Game.IFps* to implement movement logic independent of the frame rate.
The sample features two cubes moving back and forth along the X-axis. Their movement is implemented in the *IFpsMovementController* file. Use the *Max FPS* slider to change the target frame rate.

The green cube uses *Game.IFps* to scale its movement by the frame time delta. This ensures consistent speed across varying frame rates.

The red cube does not use *Game.IFps* and simply applies constant translation per frame, which results in inconsistent behavior when frame rate changes

This sample demonstrates why using time-based logic is essential for consistent results at different frame rates.