using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class TimeIt
{
	Dictionary<string, List<TimeIt>> aggregates = new Dictionary<string, List<TimeIt>> ();

	public string Message {
		get;
		set;
	}

	public DateTime Start {
		get;
		set;
	}

	public DateTime End {
		get;
		set;
	}

	public TimeSpan Total {
		get{ return End - Start; }
	}

	public void DisplayMessage() {
		Debug.Log (string.Format ("{0} total {1}", Message, Total));

	}

	public void DisplayAggregates()
	{
		foreach (var aggregate in aggregates) {
			TimeSpan total = new TimeSpan ();
			TimeSpan high = new TimeSpan();
			TimeSpan low = new TimeSpan(1000000000);
			foreach (var timeIt in aggregate.Value) {
				total += timeIt.Total;
				if(high < timeIt.Total) high = timeIt.Total;
				if(low > timeIt.Total) low = timeIt.Total;
			}
			Debug.Log (string.Format ("{0} total {1}, low {2}, high {3}", aggregate.Key, total, low, high));
		}
	}
	
	public TimeIt(string message) 
	{
		Start = DateTime.Now;
		Message = message;
	}

	public void CalculateTime()
	{
		End = DateTime.Now;
	}

	public static T Time<T>(string message, Func<T> action)
	{
		TimeIt timeIt = new TimeIt (message);
		try {
			return timeIt.Time (action);
		} finally {
			timeIt.DisplayMessage ();
		}
	}
	
	public T Time<T>(Func<T> action)
	{
		try {
			return action ();
		} finally {
			CalculateTime ();
		}
	}
	
	public T Aggregate<T>(string bucket_name, Func<T> action)
	{
		if (!aggregates.ContainsKey (bucket_name)) {
			aggregates[bucket_name] = new List<TimeIt>();
		}
		var timeIt = new TimeIt (bucket_name);
		aggregates [bucket_name].Add (timeIt);
		return timeIt.Time (action);
	}


	public static void Time(string message, Action action)
	{
		TimeIt timeIt = new TimeIt (message);
		try {
			timeIt.Time (action);
		} finally {
			timeIt.DisplayMessage ();
		}
	}

	public void Time(Action action)
	{
		try {
			action ();
		} finally {
			CalculateTime ();
		}
	}
	
	public void Aggregate(string bucket_name, Action action)
	{
		if (!aggregates.ContainsKey (bucket_name)) {
			aggregates[bucket_name] = new List<TimeIt>();
		}
		var timeIt = new TimeIt (bucket_name);
		aggregates [bucket_name].Add (timeIt);
		timeIt.Time (action);
	}
}

public class BaseGenerator : MonoBehaviour {
	public bool renderImmediate = true;
	//public int [,] currentMap;
	private TerrainMap terrainMap = null;



	public int MapWidth{
		get{
			if(terrainMap == null) terrainMap = GetComponent<TerrainMap>();
			return terrainMap.mapWidth;
		}
	}

	public int MapHeight{
		get{
			if(terrainMap == null) terrainMap = GetComponent<TerrainMap>();
			return terrainMap.mapHeight;
		}
	}

	public int [,] CurrentMap {
		get{
			if(terrainMap == null) terrainMap = GetComponent<TerrainMap>();
			return terrainMap.Map;
		}
		set{
			if(terrainMap == null) terrainMap = GetComponent<TerrainMap>();
			terrainMap.Map = value;
		}
	}

	public void Render(){
		TimeIt.Time ("Render", () => {
			TerrainRenderer terrainRenderer = GetComponent<TerrainRenderer> ();
			TimeIt.Time ("Render - Clear", () => {
				terrainRenderer.ClearImmediate ();
			});
			TimeIt.Time ("Render - Render", () => {
				terrainRenderer.Render ();
			});
		});
	}
}
