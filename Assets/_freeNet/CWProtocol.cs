using UnityEngine;
using System.Collections;

public enum PROTOCOL : short
{
	BEGIN = 0,

	LOGIN,
	LOGIN_SUCCESS,
	LOGIN_FAIL,

	CHAT_MSG_REQ,
	CHAT_MSG_ACK,

	END
}
