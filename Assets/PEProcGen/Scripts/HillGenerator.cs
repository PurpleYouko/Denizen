using UnityEngine;
using System.Collections;

public class HillGenerator : BaseGenerator 
{
	public float heightScale = 1.0F;
	public float xScale = 1.0F;
	public float yNoise = 0f;
	public float groundDepth = 10.0f;
	public float hillDepth = 20.0f;

	public AnimationCurve contourCurve = AnimationCurve.Linear (0f, 0f, 1f, 1f);


	public void GenerateMap()
	{
		ClearMap();
		for(int x = 0;x < MapWidth;x++)
		{
			float xf = (float)x / (float)(MapWidth-1);
			//height will evaluate to a value ~ between 0 and 1
			float height = contourCurve.Evaluate(xf) * heightScale * Mathf.PerlinNoise(xf * xScale, yNoise);
			height *= hillDepth;
			height += groundDepth;
			for(int y = 0;y < MapHeight;y++)
			{
				if(y <= height)
				{
					CurrentMap[x,y] = 1;
				}
				else
				{
					y = MapHeight; 
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
