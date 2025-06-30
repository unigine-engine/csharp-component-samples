using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "5d3fa800bd9a7b5f76b611327c69bee9549f7b34")]
public class TrackPlayback : Component
{
	[ShowInEditor]
	[ParameterFile(Filter = ".track")]
	private string scaleTrackPath = "";

	private int positionTrackID = -1;
	private int scaleTrackID = -1;

	private float positionTrackTime = 0.0f;
	private float rotationTrackTime = 0.0f;
	private float scaleTrackTime = 0.0f;

	private void Init()
	{
		if (!Tracker.IsInitialized)
			return;

		// get position track id that was added to tracker in editor
		if (Tracker.ContainsTrack("position_track"))
		{
			positionTrackID = Tracker.GetTrackID("position_track");
			positionTrackTime = Tracker.GetMinTime(positionTrackID);
		}

		// get rotation track time using track name
		if (Tracker.ContainsTrack("rotation_track"))
			rotationTrackTime = Tracker.GetMinTime("rotation_track");

		// add new track to tracker
		scaleTrackID = Tracker.AddTrack(scaleTrackPath);
		if (scaleTrackID != -1)
			scaleTrackTime = Tracker.GetMinTime(scaleTrackID);
	}
	
	private void Update()
	{
		if (!Tracker.IsInitialized)
			return;

		// set position track time using id
		if (positionTrackID != -1)
		{
			float minTime = Tracker.GetMinTime(positionTrackID);
			float maxTime = Tracker.GetMaxTime(positionTrackID);
			float unitTime = Tracker.GetUnitTime(positionTrackID);

			positionTrackTime += Game.IFps / unitTime;
			if (positionTrackTime >= maxTime)
				positionTrackTime = minTime;

			Tracker.SetTime(positionTrackID, positionTrackTime);
		}

		// set rotation track time using track name
		if (Tracker.ContainsTrack("rotation_track"))
		{
			float minTime = Tracker.GetMinTime("rotation_track");
			float maxTime = Tracker.GetMaxTime("rotation_track");
			float unitTime = Tracker.GetUnitTime("rotation_track");

			rotationTrackTime += Game.IFps / unitTime;
			if (rotationTrackTime >= maxTime)
				rotationTrackTime = minTime;

			Tracker.SetTime("rotation_track", rotationTrackTime);
		}

		// update scale track time
		if (scaleTrackID != -1)
		{
			float minTime = Tracker.GetMinTime(scaleTrackID);
			float maxTime = Tracker.GetMaxTime(scaleTrackID);
			float unitTime = Tracker.GetUnitTime(scaleTrackID);

			scaleTrackTime += Game.IFps / unitTime;
			if (scaleTrackTime >= maxTime)
				scaleTrackTime = minTime;

			Tracker.SetTime(scaleTrackID, scaleTrackTime);
		}
	}
}
