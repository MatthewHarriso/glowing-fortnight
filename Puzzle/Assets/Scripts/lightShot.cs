using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lightShot : MonoBehaviour
{
	const bool OnRaycastExitMessage = false;
	const bool OnRaycastEnterMessage = true;

	GameObject previous;

	Ray lightDir;
	Vector3 forward;
	LineRenderer laser;

	// Use this for initialization
	void Start ()
	{
		forward = this.transform.forward;
		lightDir = new Ray(this.transform.position, forward);
		laser = gameObject.GetComponent<LineRenderer>();
		laser.enabled = true;
	}

	// Update is called once per frame
	void Update()
	{
		LaserChecking();
	}

	void LaserChecking()
	{
		RaycastHit[] hits;
		hits = new RaycastHit[10];

		Physics.Raycast(lightDir, out hits[0]);

		if (laser.positionCount < 1)
		{
			laser.positionCount = 1;
		}

		laser.SetPosition(0, lightDir.origin);

		for (int i = 0; i < hits.Length; i++)
		{
			if (laser.positionCount != i + 2)
			{
				laser.positionCount = i + 2;
			}

			laser.SetPosition(i + 1, hits[i].point);

			switch (hits[i].transform.tag)
			{
				case "Wall":
					if (previous != null)
					{
						SendMessageTo(previous, OnRaycastExitMessage);

						previous = null;
					}

					i = hits.Length + 1;
					break;

				case "Bouncer":
					if (previous != null)
					{
						SendMessageTo(previous, OnRaycastExitMessage);

						previous = null;
					}

					Physics.Raycast(hits[i].transform.position, hits[i].transform.forward, out hits[i + 1]);

					for (int j = 0; j < i; j++)
					{
						if (hits[j].transform == hits[i + 1].transform)
						{
							Physics.Raycast(hits[i + 1].transform.position, hits[i + 1].transform.forward, out hits[i + 2]);

							laser.positionCount = i + 3;

							laser.SetPosition(i + 2, hits[i + 1].point);

							i = hits.Length + 1;
							j = i + 1;
						}
					}
					break;

				case "Reciever":
					GameObject current = hits[i].collider.gameObject;

					if (previous != current)
					{
						SendMessageTo(previous, OnRaycastExitMessage);
						SendMessageTo(current, OnRaycastEnterMessage);

						previous = current;
					}

					i = hits.Length + 1;
					break;

				default:
					if (previous != null)
					{
						SendMessageTo(previous, OnRaycastExitMessage);

						previous = null;
					}

					i = hits.Length + 1;
					break;
			}
		}
	}

	void SendMessageTo (GameObject target, bool l_state)
	{
		if (target)
		{
			//target.SendMessage(message, target, SendMessageOptions.DontRequireReceiver);
			target.GetComponent<LightTake>().LightHit(l_state);
		}
	}
}
