# Joint Events

This sample demonstrates how to use the *Broken* event of the *Joint* class via the C# API. This event allows you to react when a joint is broken due to physical forces during the simulation.
A simple bridge structure is created by cloning a mesh and connecting multiple sections using hinge joints. Some sections are dynamic (*BodyRigid*) and others are static (*BodyDummy*) to anchor the ends. Additionally, a few weights are dropped onto the bridge to cause joint breakage. The scene is configured to showcase physically reactive behavior through joints under load.

The Broken event of each joint is connected to a *lambda* expression using an *EventConnections* class instance, which stores all connections in one place and ensures automatic cleanup when the component is shut down. When a joint breaks, the lambda is triggered, changing the material of the connected objects to visually indicate the break.

You can use this for detecting breakage in joint-based systems or adding visual feedback to destruction mechanics.