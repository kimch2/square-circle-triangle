// Copyright (c) 2013 Sebastian Hein (nyaa.labs@gmail.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.


using UnityEngine;
using System.Collections;

public class VisualizedGrid : MonoBehaviour
{
	public enum GridOriginEnum
	{
	    TopLeft, TopRight, BottomLeft, BottomRight
	}
	
	[HideInInspector] public GameManager gm;
	[HideInInspector] public VectorFrame Corners = new VectorFrame(Vector2.zero);
	[HideInInspector] public GridOriginEnum GridOrigin = GridOriginEnum.BottomLeft;
	
	[HideInInspector]
    public Vector3 TileSize2D {
        get {
            return new Vector3(gm.size, gm.size, 0.01f);
        }
    }
    [HideInInspector]
    public Vector3 TileSize2DPadded {
        get {
            return new Vector3(gm.size - gm.boxPadding, gm.size - gm.boxPadding, 0.01f);
        }
    }

    [HideInInspector]
    public Vector3 TileSize3D {
        get {
            return new Vector3(gm.size, gm.size, gm.size);
        }
    }
    [HideInInspector]
    public Vector3 TileSize3DPadded {
        get {
            return new Vector3(gm.size - gm.boxPadding, gm.size - gm.boxPadding, gm.size - gm.boxPadding);
        }
    }
	public Vector3 this[int x, int y] {
        get {
			switch (GridOrigin) {
                case GridOriginEnum.BottomRight:
                    return Corners.CalculatePosFromBottomRight(x, y, gm.size);
                case GridOriginEnum.TopLeft:
                    return Corners.CalculatePosFromTopLeft(x, y, gm.size);
                case GridOriginEnum.TopRight:
                    return Corners.CalculatePosFromTopRight(x, y, gm.size);
                case GridOriginEnum.BottomLeft:
                default:
                    return Corners.CalculatePosFromBottomLeft(x, y, gm.size);
            }
        }
    }
	
	public struct VectorFrame
	{
	    public Vector2 TopL;
	    public Vector2 TopR;
	
	    public Vector2 BottomL;
	    public Vector2 BottomR;
	
	    public float z;
		
		public VectorFrame(Vector2 kickstart){
			TopL = TopR = BottomL = BottomR = kickstart;
	        z = 0;
		}
	
	    public VectorFrame(Vector2 topL, Vector2 topR, Vector2 bottomL, Vector2 bottomR)
	        : this(topL, topR, bottomL, bottomR, 0f)
	    {
	    }
	
	    public VectorFrame(Vector2 topL, Vector2 topR, Vector2 bottomL, Vector2 bottomR, float frameZ)
	    {
	        TopL = topL;
	        TopR = topR;
	        BottomL = bottomL;
	        BottomR = bottomR;
	        z = frameZ;
	    }
	
	    public VectorFrame Set(Vector2 topL, Vector2 topR, Vector2 bottomL, Vector2 bottomR, float frameZ = 0f)
	    {
	        TopL = topL;
	        TopR = topR;
	        BottomL = bottomL;
	        BottomR = bottomR;
	        z = frameZ;
	
	        return this;
	    }
	
	    public Vector3 CalculatePosFromTopLeft(int x, int y, float step)
	    {
	        return new Vector3(TopL.x + ((x * step) + (step * 0.5f)), TopL.y - ((y * step) + (step * 0.5f)), z);
	    }
	
	    public Vector3 CalculatePosFromBottomLeft(int x, int y, float step)
	    {
	        return new Vector3(BottomL.x + ((x * step) + (step * 0.5f)), BottomL.y + ((y * step) + (step * 0.5f)), z);
	    }
	
	    public Vector3 CalculatePosFromTopRight(int x, int y, float step)
	    {
	        return new Vector3(TopR.x - ((x * step) + (step * 0.5f)), TopR.y - ((y * step) + (step * 0.5f)), z);
	    }
	
	    public Vector3 CalculatePosFromBottomRight(int x, int y, float step)
	    {
	        return new Vector3(BottomR.x - ((x * step) + (step * 0.5f)), BottomR.y + ((y * step) + (step * 0.5f)), z);
	    }
	}
	
	//
	// start of functions
	//

	void OnEnable (){
		gm = GetComponent<GameManager>();
	}
	
	// values that is refreshed each call
	private void SetValues()
    {
        float halfWidth = gm.size * gm.boardWidth * 0.5f;
        float halfHeight = gm.size * gm.boardHeight * 0.5f;

        Vector3 TempPos = gm.transform.position;

        Corners.Set(
            new Vector2(TempPos.x - halfWidth, TempPos.y + halfHeight),
            new Vector2(TempPos.x + halfWidth, TempPos.y + halfHeight),
            new Vector2(TempPos.x - halfWidth, TempPos.y - halfHeight),
            new Vector2(TempPos.x + halfWidth, TempPos.y - halfHeight),
            TempPos.z
        );
		gm.boxPadding = gm.size*(gm.paddingPercentage/100); // adjust the boxPadding value
//		gm.maxPieces = 1;
    }
	
    public void OnDrawGizmos()
    {		
        if (gm != null) {
			SetValues(); // refresh all values that is needed
			
			// show corners
            if (gm.showCorners)
            {
				#region corners
                Gizmos.color = Color.red;
                Gizmos.DrawLine(
                    new Vector3(Corners.TopL.x, Corners.TopL.y, Corners.z - 0.5f),
                    new Vector3(Corners.TopL.x + gm.size, Corners.TopL.y, Corners.z - 0.5f)
                );
                Gizmos.DrawLine(
                    new Vector3(Corners.TopL.x, Corners.TopL.y, Corners.z - 0.5f),
                    new Vector3(Corners.TopL.x, Corners.TopL.y - gm.size, Corners.z - 0.5f)
                );
                Gizmos.DrawLine(
                    new Vector3(Corners.TopR.x, Corners.TopR.y, Corners.z - 0.5f),
                    new Vector3(Corners.TopR.x - gm.size, Corners.TopR.y, Corners.z - 0.5f)
                );
                Gizmos.DrawLine(
                    new Vector3(Corners.TopR.x, Corners.TopR.y, Corners.z - 0.5f),
                    new Vector3(Corners.TopR.x, Corners.TopR.y - gm.size, Corners.z - 0.5f)
                );
                Gizmos.DrawLine(
                    new Vector3(Corners.BottomL.x, Corners.BottomL.y, Corners.z - 0.5f),
                    new Vector3(Corners.BottomL.x + gm.size, Corners.BottomL.y, Corners.z - 0.5f)
                );
                Gizmos.DrawLine(
                    new Vector3(Corners.BottomL.x, Corners.BottomL.y, Corners.z - 0.5f),
                    new Vector3(Corners.BottomL.x, Corners.BottomL.y + gm.size, Corners.z - 0.5f)
                );
                Gizmos.DrawLine(
                    new Vector3(Corners.BottomR.x, Corners.BottomR.y, Corners.z - 0.5f),
                    new Vector3(Corners.BottomR.x - gm.size, Corners.BottomR.y, Corners.z - 0.5f)
                );
                Gizmos.DrawLine(
                    new Vector3(Corners.BottomR.x, Corners.BottomR.y, Corners.z - 0.5f),
                    new Vector3(Corners.BottomR.x, Corners.BottomR.y + gm.size, Corners.z - 0.5f)
                );
                #endregion
            }
			
			// show grid box
			if (gm.showGrid){
				for (int y = 0; y <= gm.boardHeight; y++)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(
                        new Vector3(Corners.BottomL.x, Corners.BottomL.y + (y * gm.size), Corners.z),
                        new Vector3(Corners.BottomR.x, Corners.BottomR.y + (y * gm.size), Corners.z)
                    );
                }

                for (int x = 0; x <= gm.boardWidth; x++)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(
                        new Vector3(Corners.BottomL.x + (x * gm.size), Corners.BottomL.y, Corners.z),
                        new Vector3(Corners.TopL.x + (x * gm.size), Corners.TopL.y, Corners.z)
                    );
                }
			}
			
			// show padded boxes
			if (gm.showPaddedTile)
            {
                for (int x = 0; x < gm.boardWidth; x++)
                {
                    for (int y = 0; y < gm.boardHeight; y++)
                    {
                        Gizmos.color = Color.blue;
                        Gizmos.DrawWireCube(this[x, y], TileSize2DPadded);
                    }
                }
            }
        }
    }
}