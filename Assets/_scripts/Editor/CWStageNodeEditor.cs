using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(CWStageNode))]
[CanEditMultipleObjects]
public class CWStageNodeEditor : Editor
{
	void OnSceneGUI()
	{
		CWStageNode node = target as CWStageNode;

		if (node == null || node.gameObject == null)
			return;

		Vector3 center = node.Position;

		for (int i = 0; i < node.NodeLinkList.Count; ++i)
		{
			if (node.NodeLinkList[i] != null)
			{
				Handles.DrawLine(center, node.NodeLinkList[i].Position);
			}
		}
	}
}
