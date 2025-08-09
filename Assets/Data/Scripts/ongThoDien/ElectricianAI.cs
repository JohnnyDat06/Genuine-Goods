// ElectricianAI.cs
using UnityEngine;

public class ElectricianAI : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        // Đảm bảo Animator Controller của ông thợ điện có 1 parameter dạng bool tên là "IsScared"
        // và state "Scared" được set để loop.
        if (anim != null)
        {
            anim.SetBool("IsScared", true);
        }
    }
}