using UnityEngine;
using System.Collections;

/// <summary> ##################################
/// 
/// NOTICE :
/// This script is for the audio fx and bgm~!
/// You can add more Audioclips as well as modify the enums as you see fit.
/// 
/// Note that this is just where all the clips are stored. There are various places in other scripts
/// that will reference this script to play the appropriate audio using the defined enum.
/// 
/// </summary> ##################################


// audio enum
public enum PlayFx{ MATCHFX, SPECIALMATCH, SWITCHFX, DROPFX, BADMOVEFX,
	ARROWFX, STAR, RAINBOW, GAMEOVER, ROCKP, LOCKP, ICEP, SHADEDP, CONVERTSPEC, TREASURECOLLECTED};

public class AudioPlayer : MonoBehaviour {
	
	// the audio source is a gameObject so that you can move it around for various effect.
	// the listener is attached to the UIroot2D.
	public AudioSource player;
	
	public bool enableMusic = true;
	public AudioClip bgm;
	
	public bool enableSoundFX = true;
	public AudioClip gameOverSoundFx;
	public AudioClip switchSoundFx;
	public AudioClip DropSoundFx;
	public AudioClip matchSoundFx;
	public AudioClip specialMatchSoundFx;
	public AudioClip badMoveSoundFx;
	public AudioClip arrowSoundFx;
	public AudioClip starSoundFx;
	public AudioClip rainbowSoundFx;
	public AudioClip rockPanelHitFx;
	public AudioClip shadedPanelHitFx;
	public AudioClip icePanelHitFx;
	public AudioClip lockedPanelHitFx;
	public AudioClip convertingSpecialFx;
	public AudioClip treasureCollectedFx;
	
	// created a custom class to store a bool as a reference,
	// and to simulate a cooldown function with "x" seconds.
	class customBool{
		public bool state = false;
		
		// causes a delayed state transition
		public IEnumerator coolDown(float timer){
			state = !state; // reverse the state
			yield return new WaitForSeconds(timer);
			state = !state; // back to original
		}
	}
	
	// custom boolean cooldown objects for limiting the sound to play once in a given time
	customBool dropStatus = new customBool();
	customBool rockHitCD = new customBool();
	customBool lockHitCD = new customBool();
	customBool iceHitCD = new customBool();
	customBool shadeHitCD = new customBool();
	
	
	
	// Externally called function by all other scripts requiring audio feedback
	// plays audio source
	public void playSound(PlayFx thisFx){
		
		if(enableSoundFX){
			switch(thisFx){
			case PlayFx.GAMEOVER :
				if(gameOverSoundFx != null){
					player.PlayOneShot(gameOverSoundFx);
				}
				break;
			case PlayFx.DROPFX :
				if(DropSoundFx != null && !dropStatus.state){
					player.PlayOneShot(DropSoundFx);
					StartCoroutine( dropStatus.coolDown(0.01f) ); // cooldown so it doesn't spam the sound
				}
				break;
			case PlayFx.MATCHFX :
				if(matchSoundFx != null){
					player.PlayOneShot(matchSoundFx);
				}
				break;
			case PlayFx.SPECIALMATCH :
				if(matchSoundFx != null){
					player.PlayOneShot(matchSoundFx);
				}
				break;
			case PlayFx.SWITCHFX :
				if(switchSoundFx != null){
					player.PlayOneShot(switchSoundFx);
				}
				break;
			case PlayFx.BADMOVEFX :
				if(badMoveSoundFx != null){
					player.PlayOneShot(badMoveSoundFx);
				}
				break;
			case PlayFx.ARROWFX :
				if(arrowSoundFx != null){
					player.PlayOneShot(arrowSoundFx);
				}
				break;
			case PlayFx.STAR :
				if(starSoundFx != null){
					player.PlayOneShot(starSoundFx);
				}
				break;
			case PlayFx.RAINBOW :
				if(rainbowSoundFx != null){
					player.PlayOneShot(rainbowSoundFx);
				}
				break;
			
			case PlayFx.LOCKP :
				if(lockedPanelHitFx != null && !lockHitCD.state){
					player.PlayOneShot(lockedPanelHitFx);
					StartCoroutine( lockHitCD.coolDown(0.01f) ); // cooldown so it doesn't spam the sound
				}
				break;
			case PlayFx.ROCKP :
				if(rockPanelHitFx != null && !rockHitCD.state){
					player.PlayOneShot(rockPanelHitFx);
					StartCoroutine( rockHitCD.coolDown(0.01f) ); // cooldown so it doesn't spam the sound
				}
				break;
			case PlayFx.SHADEDP :
				if(shadedPanelHitFx != null && !shadeHitCD.state){
					player.PlayOneShot(shadedPanelHitFx);
					StartCoroutine( shadeHitCD.coolDown(0.01f) ); // cooldown so it doesn't spam the sound
				}
				break;
			case PlayFx.ICEP :
				if(icePanelHitFx != null && !iceHitCD.state){
					player.PlayOneShot(icePanelHitFx);
					StartCoroutine( iceHitCD.coolDown(0.01f) ); // cooldown so it doesn't spam the sound
				}
				break;
			case PlayFx.CONVERTSPEC :
				if(convertingSpecialFx != null){
					player.PlayOneShot(convertingSpecialFx);
				}
				break;
			case PlayFx.TREASURECOLLECTED :
				if(treasureCollectedFx != null){
					player.PlayOneShot(treasureCollectedFx);
				}
				break;
			}
		}
	}
	
	// function to play the bgm if enabled
	void loadBGM(){
		player.audio.clip = bgm; // set the clip
		if(enableMusic && player.audio.clip != null){ // if music is enabled
			player.audio.Play(); // play
		}
	}
	
	// function to toggle the bgm on/off
	public void toggleBGM(){
		if(player.audio.clip != null && player.audio.isPlaying){ // if music is playing
			player.audio.Pause(); // pause the music
		} else {
			player.audio.Play(); // play
		}
	}
	
	// function to toggle the FX on/off
	void toggleFX(){
		enableSoundFX = !enableSoundFX;
	}
	
	void Awake(){
		if(player == null){ // try and get it manually if player forgot to assign an AudioSource
			player = GameObject.Find("AudioSource").GetComponent<AudioSource>();
		}
		loadBGM();
	}
}
