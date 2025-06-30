using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Unigine;

[Component(PropertyGuid = "24efca8e3d316e27c0870fada6cf2b5ea1511be4")]
public class MicroprofilerSleepyNode : Component
{
	private void Init()
	{
		int id = Profiler.BeginMicro("MicroprofilerSleepyUnit::Init()");
		Sleep(1000);
		Profiler.EndMicro(id);
	}

	private void UpdateAsyncThread()
	{
		int id = Profiler.BeginMicro("MicroprofilerSleepyUnit::UpdateAsyncThread()");
		Sleep(2000);
		Profiler.EndMicro(id);
	}

	private void UpdateSyncThread()
	{
		int id = Profiler.BeginMicro("MicroprofilerSleepyUnit::UpdateSyncThread()");
		Sleep(500);
		Profiler.EndMicro(id);
	}

	private void Update()
	{
		int id = Profiler.BeginMicro("MicroprofilerSleepyUnit::Update()");
		node.Rotate(0.0f, 0.0f, 3.0f);
		Sleep(750);
		Profiler.EndMicro(id);
	}

	private void PostUpdate()
	{
		int id = Profiler.BeginMicro("MicroprofilerSleepyUnit::PostUpdate()");
		Sleep(500);
		Profiler.EndMicro(id);
	}

	private void UpdatePhysics()
	{
		int id = Profiler.BeginMicro("MicroprofilerSleepyUnit::UpdatePhysics()");
		Sleep(20);
		Profiler.EndMicro(id);
	}

	private void Swap()
	{
		int id = Profiler.BeginMicro("MicroprofilerSleepyUnit::Swap()");
		Sleep(10);
		Profiler.EndMicro(id);
	}

	private void Shutdown()
	{
		int id = Profiler.BeginMicro("MicroprofilerSleepyUnit::Shutdown()");
		Sleep(1000);
		Profiler.EndMicro(id);
	}

	private static void Sleep(int microseconds)
	{
		Thread.Sleep(new TimeSpan(0, 0, 0, 0, 0, microseconds));
	}
}
