using UnityEngine;
using System.Collections;

public class HillGenerator : BaseGenerator 
{
	public float heightScale = 1.0F;
	public float xScale = 1.0F;
	public float yNoise = 0f;
	public float yOffset = 0f;

	public AnimationCurve contourCurve = AnimationCurve.Linear (0f, 0f, 1f, 1f);


	public void GenerateMap()
	{
		//PY new method. One loop for all plus it kicks out once the top of the ground is reached so even faster (in theory)
		//Clear it first. this is temporary and won't ever be called once the game is finished.
		ClearMap();
		//Had to reverse the nesting order
		for(int x = 0;x < MapWidth;x++)
		{
			float xf = (float)x / (float)(MapWidth-1); // moved these calcs out of the inner loop since they only evaluate with x and not y
			float height = contourCurve.Evaluate(xf) * heightScale * Mathf.PerlinNoise(xf * xScale, yNoise);
			for(int y = 0;y < MapHeight;y++)
			{
				float yf = (float)(y - yOffset) / (float)(MapHeight-1);
				if(yf <= height)
				{
					CurrentMap[x,y] = 1;
				}
				else
				{
					y = MapHeight; //kicks out to the next x as we are already in the sky
				}
			}
		}
	}

	public void Append()
	{
		GenerateMap();
		if(renderImmediate) Render ();
	}

	public void Generate () 
	{
		//PY This is pointless. We can do it all in one step in about a third the time
		/*
		for(int y=0;y<MapHeight;y++){
			for(int x=0;x<MapWidth;x++){
				CurrentMap[x,y] = 1;
			}
		}
		*/
		GenerateMap();
		if(renderImmediate) Render ();
	}

	public void ClearMap()
	{
		for(int y = 0; y < MapHeight; y++)
		{
			for(int x = 0; x < MapWidth; x++)
			{
				CurrentMap[x,y] = 0;
			}
		}
	}

}
