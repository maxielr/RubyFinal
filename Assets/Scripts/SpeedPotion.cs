using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPotion : MonoBehaviour
{

   public ParticleSystem potionParticle;
   public AudioClip swoosh;

   void OnTriggerEnter2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();
        {
         potionParticle = Instantiate(potionParticle, transform.position, Quaternion.identity);
         Destroy(gameObject); 
         controller.PlaySound(swoosh); 
        }
    }
}
