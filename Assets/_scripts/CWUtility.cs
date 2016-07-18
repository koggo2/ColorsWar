using UnityEngine;
using System.Collections;

public class CWUtility
{
	public static void Log(object message)
	{
		if(Debug.isDebugBuild)
			Debug.Log(message);
	}
}
