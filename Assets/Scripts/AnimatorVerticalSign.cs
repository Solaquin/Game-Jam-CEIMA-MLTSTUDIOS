using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Rigidbody))]
public class AnimatorVerticalSign : MonoBehaviour
{
    Animator anim; Rigidbody rb;
    void Awake() { anim = GetComponent<Animator>(); rb = GetComponent<Rigidbody>(); }
    void Update()
    {
        // -1 abajo, +1 arriba, 0 quieto
        float sign = Mathf.Sign(rb.linearVelocity.y);
        anim.SetFloat("VerticalSign", sign);
    }
}
