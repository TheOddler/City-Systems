using System.Linq;
using cakeslice;
using UnityEngine;
using UnityEngine.EventSystems;

public class Blueprint : MonoBehaviour, IDragHandler
{
	const int BAD_PLACE_COLOR = 0;
	const int GOOD_PLACE_COLOR = 1;

	[SerializeField]
	LayerMask _placeMask = 256; //LayerMask.GetMask( new String[] {"Terrain"} );

    public void OnDrag(PointerEventData eventData)
    {
		Ray ray = Camera.main.ScreenPointToRay(eventData.position);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, Camera.main.farClipPlane, _placeMask))
		{
			transform.position = hit.point;
		}
    }

	void Update()
	{
		var glowers = GetComponentsInChildren<Outline>();
		var placementNodes = GetComponentsInChildren<PlacementNode>();

		if (placementNodes.All(n => n.IsHappy()))
		{
			foreach(var glower in glowers)
			{
				glower.color = GOOD_PLACE_COLOR;
			}
		}
		else 
		{
			foreach(var glower in glowers)
			{
				glower.color = BAD_PLACE_COLOR;
			}
		}
	}
}
