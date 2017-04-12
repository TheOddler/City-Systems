using System;
using System.Linq;
using cakeslice;
using UnityEngine;
using UnityEngine.EventSystems;

public class Blueprint : MonoBehaviour, IDragHandler, IPointerClickHandler
{
	const int BAD_PLACE_COLOR = 0;
	const int GOOD_PLACE_COLOR = 1;

	[SerializeField]
	LayerMask _placeMask = 256; //256 = LayerMask.GetMask( new String[] {"Terrain"} );

    public void OnDrag(PointerEventData eventData)
    {
		Ray ray = Camera.main.ScreenPointToRay(eventData.position);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, Camera.main.farClipPlane, _placeMask))
		{
			var xAngle = Mathf.Min( Vector3.Angle(Vector3.right, hit.normal),	Vector3.Angle(Vector3.left, hit.normal) );
			var yAngle = Mathf.Min( Vector3.Angle(Vector3.up, hit.normal),		Vector3.Angle(Vector3.down, hit.normal) );
			var zAngle = Mathf.Min( Vector3.Angle(Vector3.forward, hit.normal),	Vector3.Angle(Vector3.back, hit.normal) );

			var x = yAngle <= 45 || zAngle <= 45 ? Mathf.Round(hit.point.x) : hit.point.x;
			var y = xAngle <= 45 || zAngle <= 45 ? Mathf.Round(hit.point.y) : hit.point.y;
			var z = xAngle <= 45 || yAngle <= 45 ? Mathf.Round(hit.point.z) : hit.point.z;
			transform.position = new Vector3(x, y, z);
		}
    }

    public void OnPointerClick(PointerEventData eventData)
    {
		if (eventData.clickCount == 2)
		{
			// BUILD
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
