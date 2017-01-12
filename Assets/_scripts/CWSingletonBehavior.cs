using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWSingletonBehavior : MonoBehaviour
{
	static public CWSingletonBehavior Instance {
		get { return _instance; }
	}
	static private CWSingletonBehavior _instance = null;

	virtual protected void Awake()
	{
		if (_instance != null) {
			Destroy (this);
			Debug.LogError ("Another instance has been created. Check your code..!");
			return;
		}
		_instance = this;
	}
}
