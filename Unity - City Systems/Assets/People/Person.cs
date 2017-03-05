using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Person : MonoBehaviour
{
	void Update ()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit))
			{
				var agent = GetComponent<NavMeshAgent>();
				agent.SetDestination(hit.point);
			}
		}
	}
}
