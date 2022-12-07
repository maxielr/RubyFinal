using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoCollect : MonoBehaviour
{
    public AudioClip collectclip;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();
        if (controller != null)
        {
            if(controller.ammo <= controller.cogs)
            {
                controller.AmmoCount(1); 
                controller.AmmoText(); 
                Destroy(gameObject);

                controller.PlaySound(collectclip);
            }
        }
    }
}
