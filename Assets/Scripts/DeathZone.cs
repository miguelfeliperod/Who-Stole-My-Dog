using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.gameObject.GetComponentInParent<PlayerController>();
        if(player != null)
        {
            StartCoroutine(player.Die());
        }
    }
}
