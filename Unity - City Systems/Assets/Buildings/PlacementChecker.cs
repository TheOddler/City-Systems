using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Collider))]
public class PlacementChecker : MonoBehaviour
{
	[SerializeField]
	LayerMask _good;

	[SerializeField]
	LayerMask _bad;

	int _goodCount = 0;
    int _badCount = 0;

	void Start() 
	{
        Assert.IsTrue(GetComponent<Collider>().isTrigger, "Placement checker: collider has to be a trigger.");
        Assert.IsNotNull(GetComponentInParent<Rigidbody>(), "Placement checker: needs a rigidbody in some parent to work.");
	}

	public bool IsHappy() 
	{
		return _badCount == 0 && (_good.IsEmpty() || _goodCount > 0);
	}

	private bool IsGood(Collider col)
    {
        return _good.Contains(col.gameObject.layer);
    }

    private bool IsBad(Collider col)
    {
        return _bad.Contains(col.gameObject.layer);
    }

	void OnTriggerEnter(Collider other) //mss void OnCollisionEnter(Collision collision)?
	{
		if (IsGood(other)) ++_goodCount;
		if (IsBad(other)) ++_badCount;
		Debug.Log("in");
	}

	void OnTriggerExit(Collider other)
	{
		if (IsGood(other)) --_goodCount;
		if (IsBad(other)) --_badCount;
		Debug.Log("out");
	}

	void OnDrawGizmos()
    {
        if (IsHappy())
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }
        var col = Gizmos.color;
        col.a = .3f;
        Gizmos.color = col;

        Gizmos.matrix = transform.localToWorldMatrix;

        var box = GetComponent<BoxCollider>();
        if (box != null)
        {
            Gizmos.DrawCube(box.center, box.size);
            return;
        }

        var sphere = GetComponent<SphereCollider>();
        if (sphere != null)
        {
            Gizmos.DrawSphere(sphere.center, sphere.radius);
            return;
        }
        
        var mesh = GetComponent<MeshCollider>();
        if (mesh != null)
        {
            Gizmos.DrawMesh(mesh.sharedMesh);
            return;
        }

        var collider = GetComponent<Collider>();
        if (mesh != null)
        {
            Gizmos.DrawCube(collider.bounds.center, collider.bounds.size);
            return;
        }
    }

}
