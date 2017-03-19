using System.Linq;
using UnityEngine;

public class PlacementCastGridNode : MonoBehaviour, PlacementNode
{
    [Header("Grid")]
    [SerializeField] Vector2 _size = new Vector2(4, 4);
    [SerializeField] int _xCount = 4;
    [SerializeField] int _yCount = 4;
    [SerializeField] Vector3 _offset = Vector3.zero;

    enum CastType
    {
        Ray,
        Box,
        Capsule,
        Sphere
    }
    [Header("Casting")]
    [SerializeField] CastType _type = CastType.Ray;
    [SerializeField] Vector3 _ray = Vector3.down;

    [Header("Happiness")]
	[SerializeField] LayerMask _good;
	[SerializeField] LayerMask _bad;

	public bool IsHappy() 
	{
        int goodCount = 0;

        for (int x = 0; x < _xCount; ++x)
        {
            for (int y = 0; y < _yCount; ++y)
            {
                Vector2 stepSize = new Vector2(_size.x / (_xCount-1), _size.y / (_yCount-1));
                Vector3 origin = transform.TransformPoint(
                    _offset.x - _size.x/2 + stepSize.x * x, 
                    _offset.y, 
                    _offset.z - _size.y/2 + stepSize.y * y );
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
        // Mirrored of IsHappy()
        for (int x = 0; x < _xCount; ++x)
        {
            for (int y = 0; y < _yCount; ++y)
            {
                Vector2 stepSize = new Vector2(_size.x / (_xCount-1), _size.y / (_yCount-1));
                Vector3 origin = transform.TransformPoint(
                    _offset.x - _size.x/2 + stepSize.x * x, 
                    _offset.y, 
                    _offset.z - _size.y/2 + stepSize.y * y );
                Vector3 direction = transform.TransformDirection(_ray);

                // Default = good (happens when not hitting anything bad, and there is nothing good)
                var col = Color.green;
                col.a = .3f;
                
                // Raycast
                var normal = Physics.RaycastAll(origin, direction, direction.magnitude).Where(h => !h.transform.IsChildOf(transform));
                var reverse = Physics.RaycastAll(origin + direction, -direction, direction.magnitude).Where(h => !h.transform.IsChildOf(transform));
                var hits = normal.Concat(reverse);
                
                // Any good?
                if (hits.Any(h => _good.Contains(h.transform.gameObject.layer)))
                {
                    // Still default green
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
                
                Gizmos.color = col;
                Gizmos.DrawLine(origin, origin + direction);
            }
        }
    }

}
