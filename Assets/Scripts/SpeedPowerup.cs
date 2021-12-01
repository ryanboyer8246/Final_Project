using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPowerup : MonoBehaviour
{

    public float multiplier = 1.5f;
    public float duration = 4.0f;
    public AudioClip speedSound;

    void OnTriggerEnter2D (Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();

        if (other.CompareTag("RubyController"))
        {
            StartCoroutine( Pickup(other) );
            controller.PlaySound(speedSound);
        }
    }

    IEnumerator Pickup (Collider2D RubyController)
    {
        RubyController controller = RubyController.GetComponent<RubyController>();

        controller.speed *= multiplier;

        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

        yield return new WaitForSeconds(duration);

        controller.speed /= multiplier;

        Destroy(gameObject);
    }
} 
