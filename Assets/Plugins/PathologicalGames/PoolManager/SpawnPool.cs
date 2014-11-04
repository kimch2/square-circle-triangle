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
    /// Online Docs: 
    ///     http://docs.poolmanager2.path-o-logical.com/code-reference/spawnpool
    ///     
    ///	A special List class that manages object pools and keeps the scene 
    ///	organized.
    ///	
    ///  * Only active/spawned instances are iterable. Inactive/despawned
    ///    instances in the pool are kept internally only.
    /// 
    ///	 * Instanciated objects can optionally be made a child of this GameObject
    ///	   (reffered to as a 'group') to keep the scene hierachy organized.
    ///		 
    ///	 * Instances will get a number appended to the end of their name. E.g. 
    ///	   an "Enemy" prefab will be called "Enemy(Clone)001", "Enemy(Clone)002", 
    ///	   "Enemy(Clone)003", etc. Unity names all clones the same which can be
    ///	   confusing to work with.
    ///		   
    ///	 * Objects aren't destroyed by the Despawn() method. Instead, they are
    ///	   deactivated and stored to an internal queue to be used again. This
    ///	   avoids the time it takes unity to destroy gameobjects and helps  
    ///	   performance by reusing GameObjects. 
    ///		   
    ///  * Two events are implimented to enable objects to handle their own reset needs. 
    ///    Both are optional.
    ///      1) When objects are Despawned BroadcastMessage("OnDespawned()") is sent.
    ///		 2) When reactivated, a BroadcastMessage("OnRespawned()") is sent. 
    ///		    This 
    /// </description>
    [AddComponentMenu("Path-o-logical/PoolManager/SpawnPool")]
    public sealed class SpawnPool : MonoBehaviour, IList<Transform>
    {
        #region Inspector Parameters
        public string poolName = "";

        public bool matchPoolScale = false;

        public bool matchPoolLayer = false;

        public bool dontReparent = false;

        public bool dontDestroyOnLoad = false;

        public bool logMessages = false;

        public List<PrefabPool> _perPrefabPoolOptions = new List<PrefabPool>();

        public Dictionary<object, bool> prefabsFoldOutStates = new Dictionary<object, bool>();
        #endregion Inspector Parameters



        #region Public Code-only Parameters
        [HideInInspector]
        public float maxParticleDespawnTime = 60f;

        public Transform group { get; private set; }

        public PrefabsDict prefabs = new PrefabsDict();

        public Dictionary<object, bool> _editorListItemStates = new Dictionary<object, bool>();

        public Dictionary<string, PrefabPool> prefabPools
        {
            get
            {
                var dict = new Dictionary<string, PrefabPool>();
                foreach (PrefabPool pool in this._prefabPools)
                    dict[pool.prefabGO.name] = pool;

                return dict;
            }
        }
        #endregion Public Code-only Parameters



        #region Private Properties
        private List<PrefabPool> _prefabPools = new List<PrefabPool>();
        internal List<Transform> _spawned = new List<Transform>();
        #endregion Private Properties



        #region Constructor and Init
        private void Awake()
        {
            
        }


        private void OnDestroy()
        {
            
        }


        
        public void CreatePrefabPool(PrefabPool prefabPool)
        {
            
        }


       
        public void Add(Transform instance, string prefabName, bool despawn, bool parent)
        {
            
        }
        #endregion Constructor and Init



        #region List Overrides
        public void Add(Transform item)
        {
            string msg = "Use SpawnPool.Spawn() to properly add items to the pool.";
            throw new System.NotImplementedException(msg);
        }


       
        public void Remove(Transform item)
        {
            string msg = "Use Despawn() to properly manage items that should " +
                         "remain in the pool but be deactivated.";
            throw new System.NotImplementedException(msg);
        }

        #endregion List Overrides



        #region Pool Functionality
        
        public Transform Spawn(Transform prefab, Vector3 pos, Quaternion rot)
        {
			return null;
        }


        public Transform Spawn(Transform prefab, Vector3 pos, Quaternion rot, 
                               Transform parent)
        {
			return null;
        }

        public Transform Spawn(Transform prefab)
        {
            return this.Spawn(prefab, Vector3.zero, Quaternion.identity);
        }


        public Transform Spawn(Transform prefab, Transform parent)
        {
			return null;
        }

        public Transform Spawn(string prefabName)
        {
            Transform prefab = this.prefabs[prefabName];
            return this.Spawn(prefab, Vector3.zero, Quaternion.identity);
        }


        public Transform Spawn(string prefabName, Transform parent)
        {
			return null;
        }


        public Transform Spawn(string prefabName, Vector3 pos, Quaternion rot)
        {
            Transform prefab = this.prefabs[prefabName];
            return this.Spawn(prefab, pos, rot);
        }

        public Transform Spawn(string prefabName, Vector3 pos, Quaternion rot, 
                               Transform parent)
        {
			return null;
        }


        public ParticleSystem Spawn(ParticleSystem prefab,
                                    Vector3 pos, Quaternion quat)
        {
			return null;
        }

        public ParticleEmitter Spawn(ParticleEmitter prefab,
                                     Vector3 pos, Quaternion quat)
        {
            
			return null;
        }


        public ParticleEmitter Spawn(ParticleEmitter prefab,
                                     Vector3 pos, Quaternion quat,
                                     string colorPropertyName, Color color)
        {
			return null;
        }


        public void Despawn(Transform instance)
        {
            
        }

        public void Despawn(Transform instance, Transform parent)
        {
            
        }


       
        public void Despawn(Transform instance, float seconds)
        {
            
        }

        public void Despawn(Transform instance, float seconds, Transform parent)
        {
            
        }
		
        private IEnumerator DoDespawnAfterSeconds(Transform instance, float seconds, bool useParent, Transform parent)
        {
                yield return null;
        }


        public void DespawnAll()
        {
            
        }

        public bool IsSpawned(Transform instance)
        {
            return this._spawned.Contains(instance);
        }

        #endregion Pool Functionality



        #region Utility Functions
        public Transform GetPrefab(Transform prefab)
        {
            return null;
        }


        public GameObject GetPrefab(GameObject prefab)
        {
            
            return null;
        }



        private IEnumerator ListenForEmitDespawn(ParticleEmitter emitter)
        {
            
            yield return null;
            yield return new WaitForEndOfFrame();

            
        }

        private IEnumerator ListenForEmitDespawn(ParticleSystem emitter)
        {
            
            yield return new WaitForSeconds(emitter.startDelay + 0.25f);

        }

        #endregion Utility Functions



        public override string ToString()
        {
            
            return "";
        }


        
        public Transform this[int index]
        {
            get { return this._spawned[index]; }
            set { throw new System.NotImplementedException("Read-only."); }
        }

        public bool Contains(Transform item)
        {
            string message = "Use IsSpawned(Transform instance) instead.";
            throw new System.NotImplementedException(message);
        }


        public void CopyTo(Transform[] array, int arrayIndex)
        {
            this._spawned.CopyTo(array, arrayIndex);
        }


        public int Count
        {
            get { return this._spawned.Count; }
        }


        public IEnumerator<Transform> GetEnumerator()
        {
            foreach (Transform instance in this._spawned)
                yield return instance;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (Transform instance in this._spawned)
                yield return instance;
        }

        // Not implemented
        public int IndexOf(Transform item) { throw new System.NotImplementedException(); }
        public void Insert(int index, Transform item) { throw new System.NotImplementedException(); }
        public void RemoveAt(int index) { throw new System.NotImplementedException(); }
        public void Clear() { throw new System.NotImplementedException(); }
        public bool IsReadOnly { get { throw new System.NotImplementedException(); } }
        bool ICollection<Transform>.Remove(Transform item) { throw new System.NotImplementedException(); }

    }



    [System.Serializable]
    public class PrefabPool
    {

        #region Public Properties Available in the Editor
       
        public Transform prefab;

        internal GameObject prefabGO; 

        public int preloadAmount = 1;

        public bool preloadTime = false;

        public int preloadFrames = 2;

        public float preloadDelay = 0;

        public bool limitInstances = false;

        public int limitAmount = 100;

       
        public bool limitFIFO = false;  

       
        public bool cullDespawned = false;

        public int cullAbove = 50;

       
        public int cullDelay = 60;

        public int cullMaxPerPass = 5;

        
        public bool _logMessages = false;  // Used by the inspector
        public bool logMessages            // Read-only
        {
            get
            {
                if (forceLoggingSilent) return false;

                if (this.spawnPool.logMessages)
                    return this.spawnPool.logMessages;
                else
                    return this._logMessages;
            }
        }

        private bool forceLoggingSilent = false;


        public SpawnPool spawnPool;
        #endregion Public Properties Available in the Editor


        #region Constructor and Self-Destruction
        
        public PrefabPool(Transform prefab)
        {
            this.prefab = prefab;
            this.prefabGO = prefab.gameObject;
        }

        public PrefabPool() { }

        internal void inspectorInstanceConstructor()
        {
            
        }


        internal void SelfDestruct()
        {
            
        }
        #endregion Constructor and Self-Destruction


        #region Pool Functionality
    

        internal List<Transform> _spawned = new List<Transform>();
        public List<Transform> spawned { get { return new List<Transform>(this._spawned); } }

        
        internal List<Transform> _despawned = new List<Transform>();
        public List<Transform> despawned { get { return new List<Transform>(this._despawned); } }


        public int totalCount
        {
            get
            {
                // Add all the items in the pool to get the total count
                int count = 0;
                count += this._spawned.Count;
                count += this._despawned.Count;
                return count;
            }
        }


        private bool _preloaded = false;
        internal bool preloaded
        {
            get { return this._preloaded; }
            private set { this._preloaded = value; }
        }


        internal bool DespawnInstance(Transform xform)
        {
            return DespawnInstance(xform, true);
        }

        internal bool DespawnInstance(Transform xform, bool sendEventMessage)
        {
            
            return true;
        }



        
        internal IEnumerator CullDespawned()
        {
            
            yield return new WaitForSeconds(this.cullDelay);

            
            yield return null;
        }



       
        internal Transform SpawnInstance(Vector3 pos, Quaternion rot)
        {
            
            return null;
        }



        public Transform SpawnNew() { return this.SpawnNew(Vector3.zero, Quaternion.identity); }
        public Transform SpawnNew(Vector3 pos, Quaternion rot)
        {
           
            return null;
        }


        private void SetRecursively(Transform xform, int layer)
        {
            
        }


       
        internal void AddUnpooled(Transform inst, bool despawn)
        {
            
        }


        internal void PreloadInstances()
        {
            
        }

        private IEnumerator PreloadOverTime()
        {
            
                    yield return null;
                
        }

        #endregion Pool Functionality


        #region Utilities
        private void nameInstance(Transform instance)
        {
           
        }


        #endregion Utilities

    }



    public class PrefabsDict : IDictionary<string, Transform>
    {
        #region Public Custom Memebers
        public override string ToString()
        {
            return "";
        }
        #endregion Public Custom Memebers


        #region Internal Dict Functionality
        internal void _Add(string prefabName, Transform prefab)
        {
            this._prefabs.Add(prefabName, prefab);
        }

        internal bool _Remove(string prefabName)
        {
            return this._prefabs.Remove(prefabName);
        }

        internal void _Clear()
        {
            this._prefabs.Clear();
        }
        #endregion Internal Dict Functionality


        #region Dict Functionality
        private Dictionary<string, Transform> _prefabs = new Dictionary<string, Transform>();

        public int Count { get { return this._prefabs.Count; } }

        public bool ContainsKey(string prefabName)
        {
            return this._prefabs.ContainsKey(prefabName);
        }

        public bool TryGetValue(string prefabName, out Transform prefab)
        {
            return this._prefabs.TryGetValue(prefabName, out prefab);
        }

        #region Not Implimented

        public void Add(string key, Transform value)
        {
            throw new System.NotImplementedException("Read-Only");
        }

        public bool Remove(string prefabName)
        {
            throw new System.NotImplementedException("Read-Only");
        }

        public bool Contains(KeyValuePair<string, Transform> item)
        {
            string msg = "Use Contains(string prefabName) instead.";
            throw new System.NotImplementedException(msg);
        }

        public Transform this[string key]
        {
            get
            {
                Transform prefab;
                try
                {
                    prefab = this._prefabs[key];
                }
                catch (KeyNotFoundException)
                {
                    string msg = string.Format("A Prefab with the name '{0}' not found. " +
                                                "\nPrefabs={1}",
                                                key, this.ToString());
                    throw new KeyNotFoundException(msg);
                }

                return prefab;
            }
            set
            {
                throw new System.NotImplementedException("Read-only.");
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                return this._prefabs.Keys;
            }
        }


        public ICollection<Transform> Values
        {
            get
            {
                return this._prefabs.Values;
            }
        }


        #region ICollection<KeyValuePair<string, Transform>> Members
        private bool IsReadOnly { get { return true; } }
        bool ICollection<KeyValuePair<string, Transform>>.IsReadOnly { get { return true; } }

        public void Add(KeyValuePair<string, Transform> item)
        {
            
        }

        public void Clear() { throw new System.NotImplementedException(); }

        private void CopyTo(KeyValuePair<string, Transform>[] array, int arrayIndex)
        {
            
        }

        void ICollection<KeyValuePair<string, Transform>>.CopyTo(KeyValuePair<string, Transform>[] array, int arrayIndex)
        {
            
        }

        public bool Remove(KeyValuePair<string, Transform> item)
        {
            throw new System.NotImplementedException("Read-only");
        }
        #endregion ICollection<KeyValuePair<string, Transform>> Members
        #endregion Not Implimented




        #region IEnumerable<KeyValuePair<string, Transform>> Members
        public IEnumerator<KeyValuePair<string, Transform>> GetEnumerator()
        {
            return this._prefabs.GetEnumerator();
        }
        #endregion



        #region IEnumerable Members
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this._prefabs.GetEnumerator();
        }
        #endregion

        #endregion Dict Functionality

    }

}