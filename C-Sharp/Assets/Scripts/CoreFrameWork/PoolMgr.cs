using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;


// A general pool object for reusable game objects.
//
// It supports spawning and unspawning game objects that are
// instantiated from a common prefab. Can be used preallocate
// objects to avoid calls to Instantiate during gameplay. Can
// also create objects on demand (which it does if no objects
// are available in the pool).
public class PoolMgr
{

    public GameObject parent;


    // The prefab that the game objects will be instantiated from.
    private GameObject prefab;

    // The list of available game objects (initially empty by default).
    public List<GameObject> available;
    public List<GameObject> active;

    // The list of all game objects created thus far (used for efficiently
    // unspawning all of them at once, see UnspawnAll).
    public List<GameObject> all;


    // An optional function that will be called whenever a new object is instantiated.
    // The newly instantiated object is passed to it, which allows users of the pool
    // to do custom initialization.
    private Callback<GameObject> initializeFunction;

    private Callback<GameObject> destroyFunction;

    //// Indicates whether the pool's game objects should be activated/deactivated
    //// recursively (i.e. the game object and all its children) or non-recursively (just the
    //// game object).
    //private var setActiveRecursively : boolean;

    // Creates a pool.
    // The initialCapacity is used to initialize the .NET collections, and determines
    // how much space they pre-allocate behind the scenes. It does not pre-populate the
    // collection with game objects. For that, see the PrePopulate function.
    // If an initialCapacity that is <= to zero is provided, the pool uses the default
    // initial capacities of its internal .NET collections.
    public PoolMgr(GameObject prefab, Callback<GameObject> initializeFunction, Callback<GameObject> destroyFunction)
    {
        this.prefab = prefab;

        this.parent = new GameObject(prefab.name + "Pool");
        UnityEngine.Object.DontDestroyOnLoad(this.parent);


        this.available = new List<GameObject>();
        this.active = new List<GameObject>();
        this.all = new List<GameObject>();

        this.initializeFunction = initializeFunction;
        this.destroyFunction = destroyFunction;
    }

    // Spawn a game object with the specified position/rotation.
    public GameObject Spawn(Vector3 position, Quaternion rotation)
    {
        GameObject result = null;

        if (available.Count == 0)
        {
            // Create an object and initialize it.
            result = GameObject.Instantiate(prefab, position, rotation) as GameObject;
            UnityEngine.Object.DontDestroyOnLoad(result);

            result.transform.parent = parent.transform;

            // Keep track of it.
            all.Add(result);
        }
        else
        {
            result = available[0] as GameObject;
            var resultTrans = result.transform;
            resultTrans.position = position;
            resultTrans.rotation = rotation;
            result.SetActive(true);
            
            available.RemoveAt(0);
        }

        active.Add(result);

        if (initializeFunction != null) initializeFunction(result);

        return result;
    }

    // Unspawn the provided game object.
    // The function is idempotent. Calling it more than once for the same game object is
    // safe, since it first checks to see if the provided object is already unspawned.
    // Returns true if the unspawn succeeded, false if the object was already unspawned.
    public bool Unspawn(GameObject obj)
    {
        if (!available.Contains(obj))
        {
            // Make sure we don't insert it twice.
            available.Add(obj);
            obj.SetActive(false);

            active.Remove(obj);

            if (destroyFunction != null) destroyFunction(obj);

            return true; // Object inserted back in stack.
        }
        else
        {
            Debug.LogError("Object already in stack.");
            return false; // Object already in stack.
        }

    }

    // Pre-populates the pool with the provided number of game objects.
    public void PrePopulate(int count)
    {
        GameObject[] array = new GameObject[count];
        for (var i = 0; i < count; i++)
        {
            array[i] = Spawn(Vector3.zero, Quaternion.identity);
            //this.SetActive(array[i], false);
        }
        for (var j = 0; j < count; j++)
        {
            Unspawn(array[j]);
        }
    }

    // Unspawns all the game objects created by the pool.
    public void UnspawnAll()
    {
        for (int i = (active.Count - 1); i >= 0; i--)
        {
            Unspawn(active[i]);
        }
    }
    // Returns the number of active objects.

    int GetActiveCount()
    {
        return active.Count;
    }

    // Returns the number of available objects.
    int GetAvailableCount()
    {
        return available.Count;
    }
}


