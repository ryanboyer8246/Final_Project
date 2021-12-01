using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvincibilityPowerup : MonoBehaviour
{
    public float duration = 4.0f;
    public AudioClip invincibleSound;

    void OnTriggerEnter2D (Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();

        if (other.CompareTag("RubyController"))
        {
            StartCoroutine( Pickup(other) );
            controller.PlaySound(invincibleSound);
        }
    }

    IEnumerator Pickup (Collider2D RubyController)
    {
        RubyController controller = RubyController.GetComponent<RubyController>();


        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

        yield return new WaitForSeconds(duration);

        

        Destroy(gameObject);
    }
}
