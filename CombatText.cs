using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatText : MonoBehaviour
{
    public float effectDuration;
    public float fadeSpeed;
    public float fadeWaitTime;
    public float scrollSpeed;
    public float offsetX, offsetY;
    public float scrollHeight;
    

    private void Awake()
    {
        Destroy(gameObject, effectDuration);
    }

    private void Update()
    {
        transform.Translate(new Vector2(0, scrollHeight * fadeSpeed * Time.deltaTime));
        StartCoroutine("FadeOut");
    }

    private IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(fadeWaitTime);
        float startAlpha = GetComponent<Text>().color.a;
        float progress = 0f;

        while (progress < 1)
        {
            Color temp = GetComponent<Text>().color;
            GetComponent<Text>().color = new Color(temp.r, temp.g, temp.b, Mathf.Lerp(startAlpha, 0, progress));

            progress += fadeSpeed * Time.deltaTime;

            yield return null;
        }
    }
}
