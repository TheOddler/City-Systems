using System.Linq;
using UnityEngine;

public class PlacementRaycastGridNode : MonoBehaviour, PlacementNode
{
    [Header("Grid")]
    [SerializeField] Vector2 _gridSize = new Vector2(4, 4);
    [SerializeField] int _xCount = 4;
    [SerializeField] int _yCount = 4;

    [Space, Header("Rays")]
    [SerializeField] Vector3 _origin = Vector3.zero;
    [SerializeField] Vector3 _ray = Vector3.down;

    [Space, Header("Happiness")]
	[SerializeField] LayerMask _good;
	[SerializeField] LayerMask _bad;

	public bool IsHappy() 
	{
        int goodCount = 0;

        for (int x = 0; x < _xCount; ++x)
        {
            for (int y = 0; y < _yCount; ++y)
            {
                Vector2 stepSize = new Vector2(_gridSize.x / _xCount, _gridSize.y / _yCount);
                Vector3 origin = transform.TransformPoint(
                    _origin.x - _gridSize.x/2 + stepSize.x/2 + stepSize.x * x, 
                    _origin.y, 
                    _origin.z - _gridSize.y/2 + stepSize.y/2 + stepSize.y * y );
                Vector3 direction = transform.TransformDirection(_ray);
                
                // Raycast
                var normal = Physics.RaycastAll(origin, direction, direction.magnitude).Where(h => !h.transform.IsChildOf(transform));
                var reverse = Physics.RaycastAll(origin + direction, -direction, direction.magnitude).Where(h => !h.transform.IsChildOf(transform));
                var hits = normal.Concat(reverse);

                // Any good?
                if (hits.Any(h => _good.Contains(h.transform.gameObject.layer)))
                {
                    goodCount += 1;
                }
                // No good, but there should be, so fail
                else if (!_good.IsEmpty())
                {
                    return false;
                }
               
                // Any bad?
                if (hits.Any(h => _bad.Contains(h.transform.gameObject.layer)))
                {
                    return false;
                }
            }
        }

		return _good.IsEmpty() || goodCount > 0;
	}

	void OnDrawGizmos()
    {
        for (int x = 0; x < _xCount; ++x)
        {
            for (int y = 0; y < _yCount; ++y)
            {
                Vector2 stepSize = new Vector2(_gridSize.x / _xCount, _gridSize.y / _yCount);
                Vector3 origin = transform.TransformPoint(
                    _origin.x - _gridSize.x/2 + stepSize.x/2 + stepSize.x * x, 
                    _origin.y, 
                    _origin.z - _gridSize.y/2 + stepSize.y/2 + stepSize.y * y );
                Vector3 direction = transform.TransformDirection(_ray);

                // Default = good (happens when not hitting anything bad, and there is nothing good)
                var col = Color.green;
                
                // Raycast
                var normal = Physics.RaycastAll(origin, direction, direction.magnitude).Where(h => !h.transform.IsChildOf(transform));
                var reverse = Physics.RaycastAll(origin + direction, -direction, direction.magnitude).Where(h => !h.transform.IsChildOf(transform));
                var hits = normal.Concat(reverse);
                
                // Any good?
                if (hits.Any(h => _good.Contains(h.transform.gameObject.layer)))
                {
                    col = Color.green;
                }
                // No good, but there should be, so fail
                else if (!_good.IsEmpty())
                {
                    col = Color.red;
                }
               
                // Any bad?
                if (hits.Any(h => _bad.Contains(h.transform.gameObject.layer)))
                {
                    col = Color.red;
                }
                
                col.a = .5f;
                Gizmos.color = col;
                Gizmos.DrawLine(origin, origin + direction);
            }
        }
    }

}
