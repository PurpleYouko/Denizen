using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainRenderer : MonoBehaviour {
//	public int [,] terrainMap;
	public string tilesetName;
	public GameObject prefabDefault;//If left blank the sprite_basic prefab in the Resources folder will be used
	public List<TileSet> tileSets;
	public bool reuse;

	public int [,] TerrainMap {
		get{
			TerrainMap terrainMap = GetComponent<TerrainMap>();
			return terrainMap.Map;
		}
	}

	void Start () {
		LoadSprites();
	}
	void LateUpdate(){
		if(Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
	}
	public void LoadSprites(){//Load the sprites from the resources folder
		foreach(var t in tileSets){
			Sprite[] sprites = Resources.LoadAll<Sprite>(t.Name);
			for (int i = 0; i < sprites.Length; i++) {
				if(i >= t.tileData.Count){
					TileData td = new TileData(){sprite = sprites[i]};
					t.tileData.Add(td);
				}else{
					t.tileData[i].sprite = sprites[i];
				}

			}
		}
	}
	public void Render(){

		if (TerrainMap == null)
			return;
		if (tileSets == null || tileSets.Count == 0) {
			Debug.Log ("You must set up a tileset");
			return;
		}
		TileSet currenTileSet = tileSets.Find (t => t.Name == tilesetName);//Find the currently specified tileset
		if (currenTileSet == null)
			currenTileSet = tileSets [0];//Couldnt find the tileset so just use the first one
		var timeIt = new TimeIt ("Render - For Loop");

		int numberOfBlank = 0;
		for (int y = 0; y < TerrainMap.GetLength(1); y++) {
			for (int x = 0; x < TerrainMap.GetLength(0); x++) {
				TileData tiledata =currenTileSet.tileData [TerrainMap [x, y]];
				if(tiledata.blank) numberOfBlank++;
			}
		}

		List<GameObject> available = timeIt.Aggregate ("Get All Objects", () => {
			return GetAllAvailable (TerrainMap.GetLength (1) * TerrainMap.GetLength (0) - numberOfBlank);
		});

		int nextAvailable = 0;

		for (int y = 0; y < TerrainMap.GetLength(1); y++) {
			for (int x = 0; x < TerrainMap.GetLength(0); x++) {
				TileData tiledata = timeIt.Aggregate("Tile Data", () => {
					return currenTileSet.tileData [TerrainMap [x, y]];//Get all the info needed on this tile to render it
				});
				if (tiledata.blank)
					continue;
				timeIt.Aggregate("Object Manipulation", () => {
					GameObject go = available[nextAvailable++];
					go.transform.parent = this.transform;
					go.transform.localPosition = new Vector3 (x, y, 0);
					SpriteRenderer spriteRenderer = go.GetComponent<SpriteRenderer> ();
					spriteRenderer.sprite = tiledata.sprite;
					go.GetComponent<BoxCollider2D> ().enabled = tiledata.boxCollider;
					go.name = "x: " + x + " y: " + y + " " + tiledata.sprite.name;
				});
			}
		}

		timeIt.CalculateTime ();
		timeIt.DisplayMessage ();
		timeIt.DisplayAggregates ();
	}
	
	List<GameObject> GetAllAvailable(int totalSize)
	{
		List<GameObject> allAvailable = new List<GameObject> ();

		if (reuse) {
			foreach (Transform child in this.transform) {
				GameObject go = Returnable (child);
				if(go != null) allAvailable.Add(go);
			}
		}

		while(allAvailable.Count < totalSize)
		{
			if(prefabDefault == null) prefabDefault = Resources.Load<GameObject>("sprite_basic");
			allAvailable.Add ((GameObject)Instantiate (prefabDefault)) ;
		}

		return allAvailable;
	}

	static GameObject Returnable (Transform child)
	{
		if (!child.gameObject.activeSelf) {
			child.gameObject.SetActive (true);
			return child.gameObject;
		}
		return null;
	}

	public void ClearImmediate(){
		if(reuse){
			foreach(Transform child in this.transform){
				child.gameObject.SetActive(false);
			}
			return;
		}
		int retrycount = 0;
		while(this.transform.childCount > 0 && retrycount <20){//Really tries to make sure all the child gameobjects are destroyed!
			foreach(Transform child in this.transform){
				DestroyImmediate(child.gameObject);
			}
			retrycount++;
		}
	}
}
