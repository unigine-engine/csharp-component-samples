using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unigine;

[Component(PropertyGuid = "de9cb9b47ac32b0e4f34b127fa6dedebf19a3fc7")]
public class AsyncQueueTasksSample : Component
{
	private SampleDescriptionWindow sampleDescriptionWindow = new SampleDescriptionWindow();

	void Init()
	{
		Console.Onscreen = true;

		// create sample UI
		sampleDescriptionWindow.createWindow(Gui.ALIGN_RIGHT);

		WidgetGroupBox parameters = sampleDescriptionWindow.getParameterGroupBox();

		var asyncThreadTypeHBox = new WidgetHBox(5);
		parameters.AddChild(asyncThreadTypeHBox, Gui.ALIGN_EXPAND);

		var asyncThreadTypeLabel = new WidgetLabel("Task Thread Type");
		asyncThreadTypeHBox.AddChild(asyncThreadTypeLabel);

		var asyncThreadTypeCombobox = new WidgetComboBox();
		asyncThreadTypeCombobox.AddItem("BACKGORUND");
		asyncThreadTypeCombobox.AddItem("ASYNC");
		asyncThreadTypeCombobox.AddItem("GPU STREAM");
		asyncThreadTypeCombobox.AddItem("FILE STREAM");
		asyncThreadTypeCombobox.AddItem("MAIN");
		asyncThreadTypeCombobox.AddItem("NEW");
		asyncThreadTypeHBox.AddChild(asyncThreadTypeCombobox, Gui.ALIGN_EXPAND);

		var runAsyncButton = new WidgetButton("Run Async");
		runAsyncButton.EventClicked.Connect(() =>
		{
			// run a task asynchronously in a specified thread
			// also you can specify priority of your task
			// ASYNC_PRIORITY_CRITICAL - hight
			// ASYNC_PRIORITY_DEFAULT - medium
			// ASYNC_PRIORITY_BACKGROUND - low
			AsyncQueue.RunAsync((AsyncQueue.ASYNC_THREAD)(asyncThreadTypeCombobox.CurrentItem), AsyncTask);
		});
		parameters.AddChild(runAsyncButton, Gui.ALIGN_EXPAND);

		var spacer = new WidgetSpacer();
		parameters.AddChild(spacer, Gui.ALIGN_EXPAND);

		var multithreadHBox = new WidgetHBox(5);
		parameters.AddChild(multithreadHBox, Gui.ALIGN_EXPAND);

		var multithreadLabel = new WidgetLabel("Num threads");
		multithreadHBox.AddChild(multithreadLabel);

		var spinboxHBox = new WidgetHBox();
		var multithreadEditline = new WidgetEditLine();
		multithreadEditline.Editable = false;
		var multithreadSpinbox = new WidgetSpinBox();
		multithreadEditline.AddAttach(multithreadSpinbox);
		multithreadSpinbox.MinValue = 1;
		multithreadSpinbox.MaxValue = 20;
		multithreadSpinbox.Value = 1;
		spinboxHBox.AddChild(multithreadEditline);
		spinboxHBox.AddChild(multithreadSpinbox);
		multithreadHBox.AddChild(spinboxHBox, Gui.ALIGN_RIGHT);

		var frame_checkbox = new WidgetCheckBox("Wait for multithreaded task to complete in frame");
		parameters.AddChild(frame_checkbox, Gui.ALIGN_LEFT);

		var run_async_multithread_button = new WidgetButton("Run Async Multithread");
		run_async_multithread_button.EventClicked.Connect(() =>
		{
			// run a task in a multithread mode, current thread number and total amount of thread are passed to the callback
			// does not block the thread from which it is called
			if (frame_checkbox.Checked)
				AsyncQueue.RunFrameAsyncMultiThread(MultithreadTask, multithreadSpinbox.Value);
			else
				AsyncQueue.RunAsyncMultiThread(MultithreadTask, multithreadSpinbox.Value);
		});
		parameters.AddChild(run_async_multithread_button, Gui.ALIGN_EXPAND);

		var run_sync_multithread_button = new WidgetButton("Run Sync Multithread");
		run_sync_multithread_button.EventClicked.Connect(() =>
		{
			// run a task in a multithread mode, current thread number and total amount of thread are passed to the callback
			// blocks the thread from which it was called (the calling thread will be unblocked after the task is completed in all threads)
			if (frame_checkbox.Checked)
				AsyncQueue.RunFrameSyncMultiThread(MultithreadTask, multithreadSpinbox.Value);
			else
				AsyncQueue.RunSyncMultiThread(MultithreadTask, multithreadSpinbox.Value);
		});
		parameters.AddChild(run_sync_multithread_button, Gui.ALIGN_EXPAND);
	}
	
	void Shutdown()
	{
		Console.Onscreen = false;
		sampleDescriptionWindow.shutdown();
	}

	private void AsyncTask()
	{
		// simulate task work
		Thread.Sleep(200);

		Log.MessageLine("This is async task, thread id: " + Thread.CurrentThread.ManagedThreadId.ToString());
	}

	private void MultithreadTask(int currentThread, int totalThreads)
	{
		// simulate task work
		Thread.Sleep(200);

		Log.MessageLine($"This is multithread task(current thread: {currentThread}, total number of threads: {totalThreads}), thread id: " + Thread.CurrentThread.ManagedThreadId.ToString());
	}
}
