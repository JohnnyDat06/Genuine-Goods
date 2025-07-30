using UnityEngine;

public class DamageZoneForwarder : MonoBehaviour
{
    // Kéo object Crusher cha vào đây trong Inspector
    [SerializeField] private CrusherTrap mainTrap;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        mainTrap.AddPlayerToList(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        mainTrap.RemovePlayerFromList(collision.gameObject);
    }
}