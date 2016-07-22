using UnityEngine;
using System.Collections;

public class CWAI : MonoBehaviour
{
	private CWStageNode _stageNode = null;

	void Start()
	{
		_stageNode = GetComponent<CWStageNode>();
	}
}
