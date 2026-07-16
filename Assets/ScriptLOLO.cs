using UnityEngine;
using TMPro;

public class ScriptLOLO : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text popupText;

    void Start()
    {
        if (popupText != null)
        {
            popupText.gameObject.SetActive(true);
            popupText.text = "Hello LOLO";
        }
    }

    void Update()
    {
        
    }
}