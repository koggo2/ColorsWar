using UnityEngine;
using System.Collections;

public class NodeConnectionData
{
	public enum NodeLinkState
	{
		OneDirection,
		Dispersion,
	}

	public NodeLinkState LinkState = NodeLinkState.OneDirection;
	public CWStageNode Origin = null;
	public CWStageNode Target = null;

	private int _dispersionIndex = 0;

	public NodeConnectionData(NodeLinkState linkState, CWStageNode origin)
	{
		LinkState = linkState;
		Origin = origin;
	}

	public NodeConnectionData(CWStageNode origin, CWStageNode target)
	{
		LinkState = NodeLinkState.OneDirection;
		Origin = origin;
		Target = target;
	}

	/// <summary>
	/// 병력을 보낸다.
	/// </summary>
	/// <param name="armyPrefab"></param>
	public void SendColor(GameObject armyObject)
	{
		if (armyObject == null)
		{
			return;
		}

		var cwStageColorArmy = armyObject.GetComponent(typeof (CWStageColorArmy)) as CWStageColorArmy;
		if (cwStageColorArmy == null)
		{
			return;
		}

		if (LinkState == NodeLinkState.OneDirection)
		{
			if (Target != null)
			{
				cwStageColorArmy.Init(Target, Origin.Position, Target.Position);
			}
		}
		else
		{
			if (_dispersionIndex >= Origin.LinkedStageNode.Count)
				_dispersionIndex = 0;

			Target = Origin.LinkedStageNode[_dispersionIndex++];
			cwStageColorArmy.Init(Target, Origin.Position, Target.Position);
		}
	}
}
