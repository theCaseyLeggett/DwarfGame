using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatTextManager : MonoBehaviour
{
    public GameObject dmgTextPrefab;
    public GameObject healTextPrefab;
    public GameObject kuldoTextPrefab;
    public GameObject goldTextPrefab;
    public GameObject normalTextPrefab;

    public RectTransform canvasTransform;

    private static CombatTextManager instance;

    public static CombatTextManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CombatTextManager>();
            }
            return instance;
        }
    }

    public void DisplayDamageText(Vector2 position, int damageAmount)
    {
        GameObject dmgText = Instantiate(dmgTextPrefab, position, Quaternion.identity);
        dmgText.GetComponent<Text>().text = damageAmount.ToString();
        //Make the text a child of our dmg text canvas so it renders
        dmgText.transform.SetParent(canvasTransform);
        // Make the text the proper scale because Unity and reasons make it absurdly large
        dmgText.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
    }

    public void DisplayHealText(Vector2 position, int healAmount)
    {
        GameObject dmgText = Instantiate(healTextPrefab, position, Quaternion.identity);
        dmgText.GetComponent<Text>().text = "+" + healAmount.ToString();
        dmgText.transform.SetParent(canvasTransform);
        dmgText.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
    }

    public void DisplayGoldText(Vector2 position, int goldAmount)
    {
        GameObject dmgText = Instantiate(goldTextPrefab, position, Quaternion.identity);
        dmgText.GetComponent<Text>().text = "+" + goldAmount.ToString();
        dmgText.transform.SetParent(canvasTransform);
        dmgText.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
    }

    public void DisplayKuldoPickup(Vector2 position, int crystalCount)
    {
        GameObject dmgText = Instantiate(kuldoTextPrefab, position, Quaternion.identity);
        dmgText.GetComponent<Text>().text = "+" + crystalCount.ToString() + " Kuldo";
        dmgText.transform.SetParent(canvasTransform);
        dmgText.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
    }

    public void DisplayText(Vector2 position, string textToDisplay)
    {
        GameObject text = Instantiate(normalTextPrefab, position, Quaternion.identity);
        text.GetComponent<Text>().text = textToDisplay;
        text.transform.SetParent(canvasTransform);
        text.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
    }


}
