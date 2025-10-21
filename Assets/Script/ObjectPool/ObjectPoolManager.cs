using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
	[Header("ObjectPoolName"), SerializeField]
	private TextAsset hint;

	public Dictionary<string, Queue<GameObject>> poolDicionary;
	public List<Pool> poolsList;

	public Transform home;

	public static ObjectPoolManager instance;

	void Awake()
	{
		if (instance != null)
		{
			//Debug.LogWarning("Found more than one ObjectPoolManager");
			Destroy(gameObject);
			return;
		}

		instance = this;

		home = this.transform;
		poolDicionary = new Dictionary<string, Queue<GameObject>>();

		foreach (var pool in poolsList)
		{
			if (pool.parent == null) { pool.parent = this.transform; }
			Queue<GameObject> objectPool = new Queue<GameObject>();
			for (int i = 0; i < pool.size; i++)
			{
				GameObject obj = null;
				obj = Instantiate(pool.perfabs, pool.parent);

				obj.SetActive(false);

				objectPool.Enqueue(obj);
			}

			poolDicionary.Add(pool.tag, objectPool);
		}
	}

	public static ObjectPoolManager GetInstance()
	{
		return instance;
	}

	public GameObject GetEnemyHealthCanvas() { return transform.Find("EnemyHealthCanvas").gameObject; }

	public GameObject SpwanFromPool(string tag, Vector3 position, Quaternion rotation, Transform parent)
	{
		if (!poolDicionary.ContainsKey(tag))
		{
			Debug.LogWarning("Pool with tag" + tag + "dosen't exist");
			return null;
		}

		GameObject objectToSpawn = poolDicionary[tag].Dequeue();

		objectToSpawn.SetActive(true);
		objectToSpawn.transform.position = position;
		objectToSpawn.transform.rotation = rotation;
		objectToSpawn.transform.SetParent(parent);

		poolDicionary[tag].Enqueue(objectToSpawn);
		return objectToSpawn;
	}
	public GameObject SpwanFromPool(string tag, Vector3 position, Quaternion rotation)
	{
		if (!poolDicionary.ContainsKey(tag))
		{
			Debug.LogWarning("Pool with tag" + tag + "dosen't exist");
			return null;
		}

		GameObject objectToSpawn = poolDicionary[tag].Dequeue();

		objectToSpawn.SetActive(true);
		objectToSpawn.transform.position = position;
		objectToSpawn.transform.rotation = rotation;

		poolDicionary[tag].Enqueue(objectToSpawn);
		return objectToSpawn;
	}
	public GameObject SpwanFromPool(string tag, Vector3 position)
	{
		if (!poolDicionary.ContainsKey(tag))
		{
			Debug.LogWarning("Pool with tag" + tag + "dosen't exist");
			return null;
		}
		//Debug.Log(position);
		GameObject objectToSpawn = poolDicionary[tag].Dequeue();

		objectToSpawn.SetActive(true);
		objectToSpawn.transform.position = position;

		poolDicionary[tag].Enqueue(objectToSpawn);
		return objectToSpawn;
	}

	public GameObject SpwanFromPool(string tag)
	{
		if (!poolDicionary.ContainsKey(tag))
		{
			Debug.LogWarning("Pool with tag" + tag + "dosen't exist");
			return null;
		}

		GameObject objectToSpawn = poolDicionary[tag].Dequeue();

		objectToSpawn.SetActive(true);

		poolDicionary[tag].Enqueue(objectToSpawn);
		return objectToSpawn;
	}

	public void ReturnAllToPool(string tag, Transform newParent)
	{
		if (!poolDicionary.ContainsKey(tag))
		{
			Debug.LogWarning("Pool with tag" + tag + "dosen't exist");
			return;
		}

		foreach (var obj in poolDicionary[tag])
		{
			obj.transform.SetParent(newParent);
			obj.SetActive(false);
		}
	}
	public void ReturnAllToPool(string tag)
	{
		if (!poolDicionary.ContainsKey(tag))
		{
			Debug.LogWarning("Pool with tag" + tag + "dosen't exist");
			return;
		}

		foreach (var obj in poolDicionary[tag])
		{
			obj.SetActive(false);
		}
	}

	public void ReturnToPool(string tag, GameObject obj) 
	{
		if (!poolDicionary.ContainsKey(tag))
		{
			Debug.LogWarning("Pool with tag" + tag + "dosen't exist");
			return;
		}

		obj.SetActive(false);
	}
}

[System.Serializable]
public class Pool
{
	public string tag;
	public GameObject perfabs;
	public int size;
	public Transform parent;
}


