using UnityEngine;
using System.Collections;
using System;

public class TimeIt : System.IDisposable
{
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

	public TimeIt(string message) 
	{
		Start = DateTime.Now;
		Message = message;
		Debug.Log (string.Format ("{0} start {1}", Message, Start));
	}
	
	public void Dispose()
	{
		End = DateTime.Now;
		Debug.Log (string.Format ("{0} end {1}", Message, End));
		Debug.Log (string.Format ("{0} total {1}", Message, End - Start));
	}

	public static void Time(string message, Action action)
	{
		using (var timeIt = new TimeIt(message)) {
			action();
		}
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
		TerrainRenderer terrainRenderer = GetComponent<TerrainRenderer>();
		terrainRenderer.ClearImmediate();
		terrainRenderer.Render();
	}
}
