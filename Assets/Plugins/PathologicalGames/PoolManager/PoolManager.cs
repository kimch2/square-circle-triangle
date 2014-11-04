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
    /// PoolManager v2.0
    ///  - PoolManager.Pools is not a complete implimentation of the IDictionary interface
    ///    Which enabled:
    ///        * Much more acurate and clear error handling
    ///        * Member access protection so you can't run anything you aren't supposed to.
    ///  - Moved all functions for working with Pools from PoolManager to PoolManager.Pools
    ///    which enabled shorter names to reduces the length of lines of code.
    /// Online Docs: http://docs.poolmanager2.path-o-logical.com
    /// </description>
    public static class PoolManager
    {
		public static readonly SpawnPoolsDict Pools = new SpawnPoolsDict();
    }


    public static class PoolManagerUtils
    {
        internal static void SetActive(GameObject obj, bool state)
        {
        }

        internal static bool activeInHierarchy(GameObject obj)
        {
			return false;
        }
    }


    public class SpawnPoolsDict : IDictionary<string, SpawnPool>
    {
        
        public SpawnPool Create(string poolName)
        {
            return null;
        }


        public SpawnPool Create(string poolName, GameObject owner)
        {
			return null;
        }

        private bool assertValidPoolName(string poolName)
        {
            return true;
        }


        
        public override string ToString()
        {
            return "";
        }

        public bool Destroy(string poolName)
        {
            
            return true;
        }

        public void DestroyAll()
        {
            
        }



        
        private Dictionary<string, SpawnPool> _pools = new Dictionary<string, SpawnPool>();

        internal void Add(SpawnPool spawnPool)
        {
            
        }

        // Keeping here so I remember we have a NotImplimented overload (original signature)
        public void Add(string key, SpawnPool value)
        {
           
        }

		
        internal bool Remove(SpawnPool spawnPool)
        {
            
            return true;
        }

        // Keeping here so I remember we have a NotImplimented overload (original signature)
        public bool Remove(string poolName)
        {
			return true;
        }

        public int Count { get { return this._pools.Count; } }

        
        public bool ContainsKey(string poolName)
        {
            return this._pools.ContainsKey(poolName);
        }

        public bool TryGetValue(string poolName, out SpawnPool spawnPool)
        {
            return this._pools.TryGetValue(poolName, out spawnPool);
        }



        #region Not Implimented
        public bool Contains(KeyValuePair<string, SpawnPool> item)
        {
            string msg = "Use PoolManager.Pools.Contains(string poolName) instead.";
            throw new System.NotImplementedException(msg);
        }

        public SpawnPool this[string key]
        {
            get
            {
                SpawnPool pool;
                try
                {
                    pool = this._pools[key];
                }
                catch (KeyNotFoundException)
                {
                    string msg = string.Format("A Pool with the name '{0}' not found. " +
                                                "\nPools={1}",
                                                key, this.ToString());
                    throw new KeyNotFoundException(msg);
                }

                return pool;
            }
            set
            {
                string msg = "Cannot set PoolManager.Pools[key] directly. " +
                    "SpawnPools add themselves to PoolManager.Pools when created, so " +
                    "there is no need to set them explicitly. Create pools using " +
                    "PoolManager.Pools.Create() or add a SpawnPool component to a " +
                    "GameObject.";
                throw new System.NotImplementedException(msg);
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                string msg = "If you need this, please request it.";
                throw new System.NotImplementedException(msg);
            }
        }


        public ICollection<SpawnPool> Values
        {
            get
            {
                string msg = "If you need this, please request it.";
                throw new System.NotImplementedException(msg);
            }
        }


        #region ICollection<KeyValuePair<string,SpawnPool>> Members
        private bool IsReadOnly { get { return true; } }
        bool ICollection<KeyValuePair<string, SpawnPool>>.IsReadOnly { get { return true; } }

        public void Add(KeyValuePair<string, SpawnPool> item)
        {
            string msg = "SpawnPools add themselves to PoolManager.Pools when created, so " +
                         "there is no need to Add() them explicitly. Create pools using " +
                         "PoolManager.Pools.Create() or add a SpawnPool component to a " +
                         "GameObject.";
            throw new System.NotImplementedException(msg);
        }

        public void Clear()
        {
            string msg = "Use PoolManager.Pools.DestroyAll() instead.";
            throw new System.NotImplementedException(msg);

        }

        private void CopyTo(KeyValuePair<string, SpawnPool>[] array, int arrayIndex)
        {
            string msg = "PoolManager.Pools cannot be copied";
            throw new System.NotImplementedException(msg);
        }

        void ICollection<KeyValuePair<string, SpawnPool>>.CopyTo(KeyValuePair<string, SpawnPool>[] array, int arrayIndex)
        {
            string msg = "PoolManager.Pools cannot be copied";
            throw new System.NotImplementedException(msg);
        }

        public bool Remove(KeyValuePair<string, SpawnPool> item)
        {
            string msg = "SpawnPools can only be destroyed, not removed and kept alive" +
                         " outside of PoolManager. There are only 2 legal ways to destroy " +
                         "a SpawnPool: Destroy the GameObject directly, if you have a " +
                         "reference, or use PoolManager.Destroy(string poolName).";
            throw new System.NotImplementedException(msg);
        }
        #endregion ICollection<KeyValuePair<string, SpawnPool>> Members
        #endregion Not Implimented




        #region IEnumerable<KeyValuePair<string,SpawnPool>> Members
        public IEnumerator<KeyValuePair<string, SpawnPool>> GetEnumerator()
        {
            return this._pools.GetEnumerator();
        }
        #endregion



        #region IEnumerable Members
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this._pools.GetEnumerator();
        }
        #endregion

    }
}