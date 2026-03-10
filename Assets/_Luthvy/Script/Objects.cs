using Mirror;
using UnityEngine;

public class Objects : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnStartClient()
    {
        if (!isServer) rb.isKinematic = true;
    }
}
