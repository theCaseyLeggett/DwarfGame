using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectObject : MonoBehaviour
{
    public float effectDuration;
    public bool fadesAway;
    public float fadeSpeed;
    public GameObject physicsObject;
    public AudioClip sound;

    private void Awake()
    {
        if (physicsObject != null)
        {
            Instantiate(physicsObject, transform.position, Quaternion.identity);
        }
        
        if(sound != null)
        {
            SoundManager.Instance.PlaySound(sound.name);
        }

        Destroy(gameObject, effectDuration);
    }

    private void Update()
    {
        if (fadesAway)
        {
            Material material = GetComponent<SpriteRenderer>().material;
            Color color = material.color;
            material.color = new Color(color.r, color.g, color.b, color.a - (fadeSpeed * Time.deltaTime));
        }
    }
}
