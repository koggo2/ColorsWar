using UnityEngine;
using System.Text;

public class CWUtility
{
	public static void Log(params object[] values)
	{
		if(Debug.isDebugBuild && values != null && values.Length > 0)
			ExLog(values);
	}

	private static void ExLog(params object[] values)
	{
		StringBuilder sb = new StringBuilder();

		for (int i = 0; i < values.Length; ++i)
		{
			sb.Append(values[i].ToString());
			if(i + 1 != values.Length)
				sb.Append(", "); 
		}

		Debug.Log(sb.ToString());
	}
}
