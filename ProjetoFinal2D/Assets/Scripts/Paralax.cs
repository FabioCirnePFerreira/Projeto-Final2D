using UnityEngine;


[System.Serializable]
public class ParalaxLayer
{
    public float paralaxForce;
    public Transform paralaxTransform;
}

public class Paralax : MonoBehaviour
{
    [SerializeField] private ParalaxLayer[] layers;
    [SerializeField] private Transform target;


    private Vector3 lastPositionTarget;
    private void Update()
    {
        if (target.gameObject.activeSelf) Paralax_();
    }

    void Paralax_()
    {
        Vector3 movement = target.position - lastPositionTarget;

        foreach (ParalaxLayer layer in layers)
        {
            layer.paralaxTransform.position -= Vector3.right*movement.x * layer.paralaxForce;
        }

        lastPositionTarget = target.position;
    }
}
