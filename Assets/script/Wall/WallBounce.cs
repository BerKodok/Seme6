using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class WallBounce : MonoBehaviour
{
    public float bounceForce = 10f;
    public float bounceDamping = 0.8f; // biar gak terlalu liar

    private CharacterController controller;
    private Vector3 lastMoveDirection;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    // dipanggil dari script movement kamu
    public void SetMoveDirection(Vector3 moveDir)
    {
        lastMoveDirection = moveDir;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // cek apakah nabrak "dinding"
        if (hit.gameObject.CompareTag("Dinding"))
        {
            Vector3 normal = hit.normal;

            // hitung arah pantulan (reflection)
            Vector3 bounceDir = normal + (lastMoveDirection * -0.5f);
            bounceDir.Normalize();

            // kasih sedikit kontrol biar gak terlalu random
            bounceDir.y = 0f;

            // apply pantulan
            controller.Move(bounceDir * bounceForce * bounceDamping * Time.deltaTime);
        }
    }
}