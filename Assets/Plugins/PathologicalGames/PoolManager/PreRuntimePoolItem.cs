/// <Licensing>
/// © 2011 (Copyright) Path-o-logical Games, LLC
/// Licensed under the Unity Asset Package Product License (the "License");
/// You may not use this file except in compliance with the License.
/// You may obtain a copy of the License at: http://licensing.path-o-logical.com
/// </Licensing>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace PathologicalGames
{
    /// <description>
    /// Adds an instance which is already a part of a scene to a pool on game start
    /// 
    /// Online Docs: http://docs.poolmanager.path-o-logical.com
    /// </description>
    [AddComponentMenu("Path-o-logical/PoolManager/Pre-Runtime Pool Item")]
    public class PreRuntimePoolItem : MonoBehaviour
    {
        #region Public Properties
        public string poolName = "";


        public string prefabName = "";

        public bool despawnOnStart = true;


        public bool doNotReparent = false;
        #endregion Public Properties


        private void Start()
        {
            SpawnPool pool;
            if (!PoolManager.Pools.TryGetValue(this.poolName, out pool))
            {

                string msg = "PreRuntimePoolItem Error ('{0}'): " +
                        "No pool with the name '{1}' exists! Create one using the " +
                        "PoolManager Inspector interface or PoolManager.CreatePool()." +
                        "See the online docs for more information at " +
                        "http://docs.poolmanager.path-o-logical.com";

                Debug.LogError(string.Format(msg, this.name, this.poolName));
                return;
            }

            pool.Add(this.transform, this.prefabName,
                     this.despawnOnStart, !this.doNotReparent);
        }
    }

}

