using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWSingletonBehavior<T> : MonoBehaviour where T : class
{
	static public T Instance {
		get { return _instance; }
	}
	static private T _instance;

	virtual protected void Awake()
	{
		if (_instance != null) {
			Destroy (this);
			Debug.LogError ("Another instance has been created. Check your code..!");
			return;
		}
		_instance = this as T;
	}
}
