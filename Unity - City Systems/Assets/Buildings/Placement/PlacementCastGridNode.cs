using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlacementCastGridNode : MonoBehaviour, PlacementNode
{
	// Constants
	static readonly Color BAD = new Color(1, 0, 0, .8f);
	static readonly Color GOOD = new Color(0, 1, 0, .3f);

	[Header("Type")]
	[SerializeField] CastType _type = CastType.Ray;
	enum CastType
	{
		Ray,
		Box,
		Capsule
	}

	// Settings
	[Header("Grid")]
	[SerializeField] Vector2 _size = new Vector2(4, 4);
	[SerializeField] float _height = 4;
	[SerializeField] int _xCount = 4;
	[SerializeField] int _yCount = 4;
	[SerializeField] Vector3 _offset = Vector3.zero;

	[Header("Happiness")]
	[SerializeField] LayerMask _good;
	[SerializeField] LayerMask _bad;

	// Cache
	static int HitsCacheUsed = 0;
	static RaycastHit[] HitsCache = new RaycastHit[8];

	public bool IsHappy() 
	{
		int goodCount = 0;

		for (int x = 0; x < _xCount; ++x)
		{
			for (int y = 0; y < _yCount; ++y)
			{
				// Raycast
				var hits = DoCast(x, y);

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

	void CalcCastParameters(int x, int y, out Vector2 stepSize, out Vector3 origin, out Vector3 direction, out float distance)
	{
		stepSize = new Vector2(_size.x / _xCount, _size.y / _yCount);
		origin = transform.TransformPoint(
			_offset.x + stepSize.x/2 - _size.x/2 + stepSize.x * x, 
			_offset.y, 
			_offset.z + stepSize.y/2 - _size.y/2 + stepSize.y * y );
		direction = Mathf.Sign(_height) * transform.up; //must be normalized
		distance = Mathf.Abs(_height);
	}

	IEnumerable<RaycastHit> DoCast(int x, int y)
	{
		Vector2 stepSize; Vector3 origin, direction; float distance;
		CalcCastParameters(x, y, out stepSize, out origin, out direction, out distance);

		switch(_type)
		{
			case CastType.Ray:
			{
				float radius = .01f;
				HitsCacheUsed = Physics.SphereCastNonAlloc(origin + direction * radius, radius, direction, HitsCache, Mathf.Max(distance - radius * 2, 0));
			} break;
			case CastType.Box:
			{
				float extend = Mathf.Max(stepSize.x, stepSize.y) / 2;
				HitsCacheUsed = Physics.BoxCastNonAlloc(origin + direction * extend/2, new Vector3(stepSize.x/2, extend/2, stepSize.y/2), direction, HitsCache, transform.rotation, Mathf.Max(distance - extend, 0));
			} break;
			case CastType.Capsule:
			{
				float radius = Mathf.Max(stepSize.x, stepSize.y) / 2;
				HitsCacheUsed = Physics.SphereCastNonAlloc(origin + direction * radius, radius, direction, HitsCache, Mathf.Max(distance - radius * 2, 0));
			} break;
			default: throw new UnityException("Unknown cast type used.");
		}

		if (HitsCache.Length == HitsCacheUsed)
		{
			Array.Resize(ref HitsCache, HitsCache.Length * 2);
			Debug.Log("Increased hit cache size to: " + HitsCache.Length);
		}

		return HitsCache.Take(HitsCacheUsed).Where(h => !h.transform.IsChildOf(transform));
	}

	void DrawCast(int x, int y)
	{
		Vector2 stepSize; Vector3 origin, direction; float distance;
		CalcCastParameters(x, y, out stepSize, out origin, out direction, out distance);

		switch(_type)
		{
			case CastType.Ray:
			{
				Gizmos.DrawLine(origin, origin + direction * distance);
			} break;
			case CastType.Box:
			{
				Gizmos.matrix = Matrix4x4.TRS(origin + direction * distance / 2, transform.rotation, Vector3.one);
				Gizmos.DrawWireCube(Vector3.zero, new Vector3(stepSize.x, distance, stepSize.y));
			} break;
			case CastType.Capsule:
			{
				//return Physics.CapsuleCastAll(origin + direction * radius, origin + direction * distance - direction * radius, radius, direction, 0);
				float radius = Mathf.Max(stepSize.x, stepSize.y) / 2;
				//Gizmos.matrix = Matrix4x4.TRS(origin + direction * _height / 2, transform.rotation, Vector3.one);
				DebugExtension.DrawCapsule(origin, origin + direction * distance, Gizmos.color, radius);
			} break;
			default: throw new UnityException("Unknown cast type used.");
		}
	}

	void OnDrawGizmos()
	{
		// Mirrored of IsHappy()
		for (int x = 0; x < _xCount; ++x)
		{
			for (int y = 0; y < _yCount; ++y)
			{
				// Default = good (happens when not hitting anything bad, and there is nothing good)
				var col = GOOD;
				
				// Raycast
				var hits = DoCast(x, y);
				
				// Any good?
				if (hits.Any(h => _good.Contains(h.transform.gameObject.layer)))
				{
					col = GOOD;
				}
				// No good, but there should be, so fail
				else if (!_good.IsEmpty())
				{
					col = BAD;
				}
			   
				// Any bad?
				if (hits.Any(h => _bad.Contains(h.transform.gameObject.layer)))
				{
					col = BAD;
				}
				
				Gizmos.color = col;
				DrawCast(x, y);
			}
		}
	}

}
