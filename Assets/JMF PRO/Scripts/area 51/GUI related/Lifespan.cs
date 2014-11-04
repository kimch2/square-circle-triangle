using UnityEngine;
using System.Collections;
using PathologicalGames;

/// <summary> ##################################
/// 
/// NOTICE :
/// This script is a simple timer to kill the object when the animation is completed.
/// 
/// DO NOT TOUCH UNLESS REQUIRED
/// 
/// </summary> ##################################

public class Lifespan : MonoBehaviour {

	void OnEnable(){
		ParticleSystem psys = this.GetComponent<ParticleSystem>();
		if(JMFUtils.isPooling){
			// ------- POOLING CODE ---------
			PoolManager.Pools["Particles"].Despawn(gameObject.transform, psys.startLifetime + psys.duration);
		} else {
			// ------- NON POOLING CODE ---------
			Destroy(gameObject,psys.startLifetime + psys.duration);
		}
	}
}
