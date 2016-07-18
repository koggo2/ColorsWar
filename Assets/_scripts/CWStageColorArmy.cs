using UnityEngine;
using System.Collections;

public class CWStageColorArmy : MonoBehaviour
{
	public float Speed = 1f;

	private CWStageNode _targetNode = null;
	private Vector3 _originPos = Vector3.zero;
	private Vector3 _destinationPos = Vector3.zero;

	public void Init(CWStageNode targetNode, Vector3 originPos, Vector3 destinationPos)
	{
		_targetNode = targetNode;
		_originPos = originPos;
		_destinationPos = destinationPos;
		transform.localPosition = originPos;

		StartCoroutine(TweenPosition());
	}

	private IEnumerator TweenPosition()
	{
		float time = 0f;
		while (transform.localPosition != _destinationPos)
		{
			transform.localPosition = Vector3.Lerp(_originPos, _destinationPos, time / 1f);
			time += Time.deltaTime;

			yield return new WaitForEndOfFrame();
		}

		if (_targetNode != null)
		{
			_targetNode.IncreaseChargeValue();
		}

		Destroy(gameObject);
	}
}
