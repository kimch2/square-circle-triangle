using UnityEngine;
using System.Collections;
using PathologicalGames;

/// <summary> ##################################
/// 
/// NOTICE :
/// This script is the animation/particles section.
/// "Powers" and in "PowerMerge" references animations from this script;
/// which in turn will generate the called animations.
///  
/// </summary> ##################################

public enum animType{GLOBALDESTROY, NOMOREMOVES, ARROWH, ARROWV, ARROWVH, ARROWTX, STAR, RAINBOW, BOMB, ROCKHIT, LOCKHIT,
	ICEHIT, SHADEHIT, CONVERTSPEC, TREASURECOLLECTED};

public class CustomAnimations : MonoBehaviour {

	public GameObject PieceDestroyEffect; // global piece destroy effect
	public GameObject noMoreMoves; // no more moves effect
	public GameObject horizontalAnim;
	public GameObject verticalAnim;
	public GameObject starAnim;
	public GameObject rainbowAnim;
	public GameObject bombAnim;
	public GameObject rockAnim;
	public GameObject lockAnim;
	public GameObject iceAnim;
	public GameObject shadedAnim;
	public GameObject convertingAnim;
	public GameObject treasureCollectedAnim;
	
	GameManager gm;
	const string animPoolName = JMFUtils.particlePoolName;


	void Awake (){
        gm = GetComponent<GameManager>();
    }
	
	/*
	 * NOTES :
	 * 
	 * Use "gm.board[x,y].position" to get the origin location of the caller
	 * gm.boardWidth / gm.boardHeight    <--- the width and height of the current board
	 * 
	 * ---------------------------
	 * 
	 * IMPORTANT ~!!
	 * 
	 * Pool Manager version of the script has an auto-despawn function
	 * located in the "Lifespan.cs" script found in area 51/GUI Related/
	 * 
	 * 
	 */
	
	// External scripts will call this function
	// From here, CustomAnimations script will select the appropriate anim to use.
	public void doAnim(animType animType, int x, int y){
		switch (animType){
		case animType.GLOBALDESTROY :
			if(PieceDestroyEffect){
				if(JMFUtils.isPooling){
					PoolManager.Pools[animPoolName].Spawn(PieceDestroyEffect.transform, gm.board[x,y].position, Quaternion.identity);
				} else {
					Instantiate(PieceDestroyEffect, gm.board[x,y].position, Quaternion.identity);
				}
			}
			break;
		case animType.NOMOREMOVES :
			if(noMoreMoves){
				if(JMFUtils.isPooling){
					PoolManager.Pools[animPoolName].Spawn(noMoreMoves.transform);
				}
				else {
					Instantiate(noMoreMoves);
				}
			}
			break;
		case animType.ARROWH :
			if(horizontalAnim != null){
				if(JMFUtils.isPooling){
					PoolManager.Pools[animPoolName].Spawn(horizontalAnim.transform, gm.board[x,y].position, Quaternion.identity);
				} else {
					Instantiate(horizontalAnim,gm.board[x,y].position,Quaternion.identity);
				}
				
			}
			break;
		case animType.ARROWV :
			if(verticalAnim != null){
				if(JMFUtils.isPooling){
					PoolManager.Pools[animPoolName].Spawn(verticalAnim.transform,gm.board[x,y].position,Quaternion.identity);
				} else {
					Instantiate(verticalAnim,gm.board[x,y].position,Quaternion.identity);
				}
				
			}
			break;
		case animType.ARROWVH :
			if(verticalAnim != null && horizontalAnim != null){ // animation effect
				if(JMFUtils.isPooling){
					PoolManager.Pools[animPoolName].Spawn(verticalAnim.transform,gm.board[x,y].position,Quaternion.identity);
					PoolManager.Pools[animPoolName].Spawn(horizontalAnim.transform,gm.board[x,y].position,Quaternion.identity);
				} else {
					Instantiate(verticalAnim,gm.board[x,y].position,Quaternion.identity);
					Instantiate(horizontalAnim,gm.board[x,y].position,Quaternion.identity);
				}

			}
			break;
		case animType.ARROWTX : // is when match-4 power combine with match-T
			if(verticalAnim != null && horizontalAnim != null){ // animation effect
				if(JMFUtils.isPooling){
					PoolManager.Pools[animPoolName].Spawn(verticalAnim.transform,gm.board[x,y].position,Quaternion.identity);
					PoolManager.Pools[animPoolName].Spawn(horizontalAnim.transform,gm.board[x,y].position,Quaternion.identity);
					if(x+1 < gm.boardWidth){
						PoolManager.Pools[animPoolName].Spawn(verticalAnim.transform,gm.board[x+1,y].position,Quaternion.identity);}
					if(x-1 >= 0){
						PoolManager.Pools[animPoolName].Spawn(verticalAnim.transform,gm.board[x-1,y].position,Quaternion.identity);}
					if(y+1 < gm.boardHeight){
						PoolManager.Pools[animPoolName].Spawn(horizontalAnim.transform,gm.board[x,y+1].position,Quaternion.identity);}
					if(y-1 >= 0){
						PoolManager.Pools[animPoolName].Spawn(horizontalAnim.transform,gm.board[x,y-1].position,Quaternion.identity);}				
				} else {
					Instantiate(verticalAnim,gm.board[x,y].position,Quaternion.identity);
					Instantiate(horizontalAnim,gm.board[x,y].position,Quaternion.identity);
					if(x+1 < gm.boardWidth){
						Instantiate(verticalAnim,gm.board[x+1,y].position,Quaternion.identity);}
					if(x-1 >= 0){
						Instantiate(verticalAnim,gm.board[x-1,y].position,Quaternion.identity);}
					if(y+1 < gm.boardHeight){
						Instantiate(horizontalAnim,gm.board[x,y+1].position,Quaternion.identity);}
					if(y-1 >= 0){
						Instantiate(horizontalAnim,gm.board[x,y-1].position,Quaternion.identity);}				
				}
			}
			break;
		case animType.STAR :
			if(starAnim != null){
				if(JMFUtils.isPooling){
					PoolManager.Pools[animPoolName].Spawn(starAnim.transform,gm.board[x,y].position,Quaternion.identity);
				} else {
					Instantiate(starAnim,gm.board[x,y].position,Quaternion.identity);
				}
			}
			break;
		case animType.RAINBOW :
			if(rainbowAnim != null){
				if(JMFUtils.isPooling){
					PoolManager.Pools[animPoolName].Spawn(rainbowAnim.transform,gm.board[x,y].position,Quaternion.identity);
				} else {
					Instantiate(rainbowAnim,gm.board[x,y].position,Quaternion.identity);
				}
			}
			break;
		case animType.BOMB :
			if(bombAnim != null){
				if(JMFUtils.isPooling){
					PoolManager.Pools[animPoolName].Spawn(bombAnim.transform,gm.board[x,y].position,Quaternion.identity);
				} else {
					Instantiate(bombAnim,gm.board[x,y].position,Quaternion.identity);
				}
			}
			break;
		case animType.LOCKHIT :
			if(lockAnim != null){
				if(JMFUtils.isPooling){
					PoolManager.Pools[animPoolName].Spawn(lockAnim.transform,gm.board[x,y].position,Quaternion.identity);
				} else {
					Instantiate(lockAnim,gm.board[x,y].position,Quaternion.identity);
				}
			}
			break;
		case animType.ROCKHIT :
			if(rockAnim != null){
				if(JMFUtils.isPooling){
					PoolManager.Pools[animPoolName].Spawn(rockAnim.transform,gm.board[x,y].position,Quaternion.identity);
				} else {
					Instantiate(rockAnim,gm.board[x,y].position,Quaternion.identity);
				}
			}
			break;
		case animType.ICEHIT :
			if(iceAnim != null){
				if(JMFUtils.isPooling){
					PoolManager.Pools[animPoolName].Spawn(iceAnim.transform,gm.board[x,y].position,Quaternion.identity);
				} else {
					Instantiate(iceAnim,gm.board[x,y].position,Quaternion.identity);
				}
			}
			break;
		case animType.SHADEHIT :
			if(shadedAnim != null){
				if(JMFUtils.isPooling){
					PoolManager.Pools[animPoolName].Spawn(shadedAnim.transform,gm.board[x,y].position,Quaternion.identity);
				} else {
					Instantiate(shadedAnim,gm.board[x,y].position,Quaternion.identity);
				}
			}
			break;
		case animType.CONVERTSPEC :
			if(convertingAnim != null){
				if(JMFUtils.isPooling){
					PoolManager.Pools[animPoolName].Spawn(convertingAnim.transform,gm.board[x,y].position,Quaternion.identity);
				} else {
					Instantiate(convertingAnim,gm.board[x,y].position,Quaternion.identity);
				}
			}
			break;
		case animType.TREASURECOLLECTED :
			if(treasureCollectedAnim != null){
				if(JMFUtils.isPooling){
					PoolManager.Pools[animPoolName].Spawn(treasureCollectedAnim.transform,gm.board[x,y].position,Quaternion.identity);
				} else {
					Instantiate(treasureCollectedAnim,gm.board[x,y].position,Quaternion.identity);
				}
			}
			break;
		}
	}
}
