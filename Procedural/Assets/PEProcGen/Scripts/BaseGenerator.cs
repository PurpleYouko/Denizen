﻿using UnityEngine;
using System.Collections;

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
